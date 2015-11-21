//using SHGG.SocketService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Monitor
{
    public partial class frmMain : Form
    {
        private Dictionary<string, DateTime> StartAPTime = new Dictionary<string, DateTime>();

        private static Logger apLog = LogManager.GetLogger("MonitorAP_Log");
        private int iRestartAPTime = 0;//重啟跟分程式間隔(秒) 最低60秒

        private static SocketClient Client;

        #region Form
        public frmMain()
        {
            InitializeComponent();

            // 版本
            this.Text += "  v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // 設定
            this.Icon = Properties.Resources.Monitor;
            this.nfiMain.Text = this.Text;
            this.nfiMain.Icon = this.Icon;

            // 計時器
            int iMonitorTime = 30;
            int.TryParse(ConfigurationManager.AppSettings["MonitorTime"], out iMonitorTime);
            if (iMonitorTime < 30)//逾設值30秒處理一次
                iMonitorTime = 30;

            this.timerLoad.Interval = 1000 * iMonitorTime;//每3秒送一筆封包給server

            // 資料表
            this.Schedule.Columns.Add(new DataColumn("GID"));
            this.Schedule.Columns.Add(new DataColumn("Type"));
            this.Schedule.Columns.Add(new DataColumn("TypeName"));
            this.Schedule.Columns.Add(new DataColumn("Date"));
            this.Schedule.Columns.Add(new DataColumn("Time"));
            this.Schedule.Columns.Add(new DataColumn("DateTime", System.Type.GetType("System.DateTime")));
            this.Schedule.Columns.Add(new DataColumn("Away"));
            this.Schedule.Columns.Add(new DataColumn("Home"));
            this.Schedule.Columns.Add(new DataColumn("Status"));
            this.Schedule.Columns.Add(new DataColumn("CtrlStates"));
            this.Schedule.Columns.Add(new DataColumn("StatusText"));
            this.Schedule.Columns.Add(new DataColumn("WebId"));
            this.Schedule.Columns.Add(new DataColumn("ChangeCount"));
            this.Schedule.Columns.Add(new DataColumn("ChangeTime"));
            
            //重啟跟分程式間隔(秒) 最低60秒
            int.TryParse(ConfigurationManager.AppSettings["RestartAP"], out iRestartAPTime);
            if (iRestartAPTime == 0)
                iRestartAPTime = 120;//預設120秒
            else if (iRestartAPTime < 60)
                iRestartAPTime = 60;

            //比賽資料變動最低時間(秒)
            this.InitGameChangeTime();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                //  測試連線資料庫
                if (!this.TestConnection())
                {
                    MessageBox.Show("無法連接資料庫，應用程式無法執行。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.CanExit = true;
                    this.Close();
                }

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                dbSourcelb.Text += string.Format("{0}:{1}", builder.DataSource, builder.InitialCatalog);//設定資料來源

                weblb.Text += ConfigurationManager.AppSettings["WebUrl"];

                lbServerInfo.Text = string.Format("{0}:{1}", ServerIp, ServerPort);

                Action<bool> action = (s => SetServerStatus(s));
                Client = new SocketClient(ServerIp, ServerPort, action);
                Task.Factory.StartNew(() => Client.Start(), TaskCreationOptions.LongRunning);

                //Client.CheckConnect();
                //Task.Run(() => Client.CheckConnect());//檢查連線

                // 計時器啟動
                this.timerLoad.Start();
                this.timerLoad_Tick(this.timerLoad, new EventArgs());
            }
            catch(Exception ex) 
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Message: " + ex.Message);
                sb.AppendLine("StackTrace:");
                sb.AppendLine(ex.StackTrace);

                apLog.Error(sb.ToString());

                MessageBox.Show("程式載入錯誤", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.CanExit)
            {
                e.Cancel = true;
                // 隱藏
                this.Hide();
            }
        }

        private bool CanExit = false;
        private void nfiMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 顯示
            this.Show();
            this.Activate();
        }
        private void nfiMain_BalloonTipClicked(object sender, EventArgs e)
        {
            // 顯示
            this.Show();
            this.Activate();
        }
        private void dgvData_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.ScheduleChecked();
        }
        #endregion
        #region Tools
        private void tspbtnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "您確定要結束程式？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                //關閉連線
                Client.Stop();

                // 結束
                this.CanExit = true;
                this.Close();
            }
        }
        private void tspbtnReload_Click(object sender, EventArgs e)
        {
            this.LoadData();
        }
        private void tspbtnLastDate_Click(object sender, EventArgs e)
        {
            this.LoadLastDate();
        }
        private void tspbtnNeedFollow_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> lst = new Dictionary<string, string>();
            string txt = null;
            // 資料
            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                string type = row.Cells[this.colType.Name].Value.ToString();
                string gid = row.Cells[this.colGID.Name].Value.ToString();
                string value = row.Cells[this.colStatusText.Name].Value.ToString();
                // 判斷
                if (value != "結束" &&
                    !lst.ContainsKey(type))
                {
                    if (value != "延後")
                    {
                        lst.Add(type, gid);
                        txt += row.Cells[this.colTypeName.Name].Value + "\r\n";
                    }
                }
            }
            // 顯示
            if (txt != null)
            {
                MessageBox.Show(this, txt, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion
        #region ConnectionString
        //  測試連線資料庫
        private bool TestConnection()
        {
            bool result = false;
            SqlConnection conn = null;
            // 錯誤處理
            try
            {
                conn = new SqlConnection(ConnectionString);
                // 開啟
                conn.Open();
                // 關閉
                conn.Close();
                // 完成
                result = true;
            }
            catch { }
            conn = null;
            // 傳回
            return result;
        }
        // 連接字串
        public static string ConnectionString
        {
            get
            {
                return string.Format("Data Source={0};Initial Catalog={1};UID={2};PWD={3};Integrated Security=false;", new string[] { SqlServer, SqlDB, SqlUID, SqlPWD });
            }
        }
        public static string SqlServer { get; set; }
        public static string SqlDB { get; set; }
        public static string SqlUID { get; set; }
        public static string SqlPWD { get; set; }
        public static string ServerIp { get; set; }
        public static string ServerPort { get; set; }
        #endregion
        #region Database
        private void LoadData()
        {
            #region Sql
            string sql = " DECLARE @Games TABLE("
                       + "     GID INT"
                       + "    ,GameType NVARCHAR(10)"
                       + "    ,GameDate DATE"
                       + "    ,GameTime TIME(7)"
                       + "    ,GameStates NVARCHAR(50)"
                       + "    ,CtrlStates INT"
                       + "    ,Home NVARCHAR(100)"
                       + "    ,Away NVARCHAR(100)"
                       + "    ,WebID NVARCHAR(50)" + "\r\n"
                       + "    ,ChangeCount INT)" + "\r\n"
                       + "" // 棒球
                       + " INSERT INTO @Games"
                       + " SELECT S.[GID],S.[GameType],S.[GameDate],S.[GameTime],S.[GameStates],S.[CtrlStates],TA.[ShowName] AS [Home],TB.[ShowName] AS Away,S.[WebID],S.[ChangeCount]"
                       + "   FROM [BaseballSchedules] AS S 　With(NOLOCK)  INNER JOIN"
                       + "        [BaseballTeam] AS TA 　With(NOLOCK)  ON (TA.[TeamID] = S.[TeamAID]) INNER JOIN"
                       + "        [BaseballTeam] AS TB 　With(NOLOCK) ON (TB.[TeamID] = S.[TeamBID])"
                       + "  WHERE (S.[IsDeleted]=0) {0}" + "\r\n"
                       + "" // 籃球
                       + " INSERT INTO @Games"
                       + " SELECT S.[GID],S.[GameType],S.[GameDate],S.[GameTime],S.[GameStates],S.[CtrlStates],TA.[ShowName] AS [Home],TB.[ShowName] AS Away,S.[WebID],S.[ChangeCount]"
                       + "   FROM [BasketballSchedules] AS S 　With(NOLOCK) INNER JOIN"
                       + "        [BasketballTeam] AS TA 　With(NOLOCK)  ON (TA.[TeamID] = S.[TeamAID]) INNER JOIN"
                       + "        [BasketballTeam] AS TB　With(NOLOCK)  ON (TB.[TeamID] = S.[TeamBID])"
                       + "  WHERE (S.[IsDeleted]=0) {0} AND S.[GameType] not in ('bkos','bkbf')" + "\r\n"
                       + "" // 冰球
                       + " INSERT INTO @Games"
                       + " SELECT S.[GID],S.[GameType],S.[GameDate],S.[GameTime],S.[GameStates],S.[CtrlStates],TA.[ShowName] AS [Home],TB.[ShowName] AS Away,S.[WebID],S.[ChangeCount]"
                       + "   FROM [IceHockeySchedules] AS S 　With(NOLOCK)  INNER JOIN"
                       + "        [IceHockeyTeam] AS TA　With(NOLOCK)  ON (TA.[TeamID] = S.[TeamAID]) INNER JOIN"
                       + "        [IceHockeyTeam] AS TB　With(NOLOCK)  ON (TB.[TeamID] = S.[TeamBID])"
                       + "  WHERE (S.[Display]=1) {0} AND S.[GameType]  <> 'IHBF' " + "\r\n"
                       + "" // 美足
                       + " INSERT INTO @Games"
                       + " SELECT S.[GID],S.[GameType],S.[GameDate],S.[GameTime],S.[GameStates],S.[CtrlStates],TA.[ShowName] AS [Home],TB.[ShowName] AS Away,S.[WebID],S.[ChangeCount]"
                       + "   FROM [AFBSchedules] AS S 　With(NOLOCK) INNER JOIN"
                       + "        [AFBTeam] AS TA　With(NOLOCK)  ON (TA.[TeamID] = S.[TeamAID]) INNER JOIN"
                       + "        [AFBTeam] AS TB　With(NOLOCK)  ON (TB.[TeamID] = S.[TeamBID])"
                       + "  WHERE (S.[IsDeleted]=0) {0}" + "\r\n"
                       + "" // 足球
                       + " INSERT INTO @Games"
                       + " SELECT  0 AS [id], [GameType], [GameDate], '00:00' AS [GameTime],'S' AS [GameStates], '2' AS [CtrlStates], N'足' AS [Home], N'球' AS [Away], NULL AS [WebID],SUM([C]) AS  [ChangeCount]"
                       + "   FROM [FootballSchedules]　With(NOLOCK) "
                       + "   WHERE (GameType=N'SB') AND ([GameDate] = (SELECT [val] FROM [SetTypeVal]　With(NOLOCK)  WHERE [type] = 'FootballDate')) AND C <> -1 GROUP BY [GameType],[GameDate]" + "\r\n"
                       + "" // 網球
                       + " INSERT INTO @Games"
                       + " SELECT 0 AS [id], N'TN' AS [GameType], [GameDate],'00:00' AS [GameTime],'S' AS [GameStates], '2' AS [CtrlStates], N'網' AS [Home], N'球' AS [Away],NULL　AS　[WebID],SUM([ChangeCount]) AS [ChangeCount]"
                       + "   FROM [TennisSchedules]　With(NOLOCK) "
                       + "   WHERE [IsDeleted]=0 AND ([GameDate]='" + DateTime.Now.ToString("yyyy-MM-dd") + "')" + "　  GROUP BY [GameDate] \r\n"
                       + "" // 奧訊
                       + " INSERT INTO @Games"
                       + " SELECT  0 AS [id], [GameType],[GameDate], '00:00' AS [GameTime],'S' AS [GameStates], '2' AS [CtrlStates], N'籃球' AS [Home], N'(奧訊)' AS [Away], NULL AS [WebID],SUM([ChangeCount]) AS [ChangeCount]"
                       + "   FROM [BasketballSchedules]　With(NOLOCK) "
                       + "   WHERE (GameType=N'BKOS') AND ([GameDate]='" + DateTime.Now.ToString("yyyy-MM-dd") + "')" + "   GROUP BY [GameType],[GameDate] \r\n"
                        + "" // BF
                       + " INSERT INTO @Games"
                       + " SELECT  0 AS [id], [GameType], [GameDate], '00:00' AS [GameTime],'S' AS [GameStates], '2' AS [CtrlStates], N'籃球' AS [Home], N'(BF)' AS [Away], NULL AS [WebID],SUM([ChangeCount]) AS  [ChangeCount]"
                       + "   FROM [BasketballSchedules]　With(NOLOCK) "
                       + "   WHERE (GameType=N'BKBF') AND ([GameDate]='" + DateTime.Now.ToString("yyyy-MM-dd") + "')" + "   GROUP BY [GameType],[GameDate] \r\n"

                       + "" // IHBF
                       + " INSERT INTO @Games"
                       + " SELECT  0 AS [id], [GameType], [GameDate], '00:00' AS [GameTime],'S' AS [GameStates], '2' AS [CtrlStates], N'冰球' AS [Home], N'(BF)' AS [Away], NULL AS [WebID],SUM([ChangeCount]) AS  [ChangeCount]"
                       + "   FROM [IceHockeySchedules]　With(NOLOCK) "
                       + "   WHERE (GameType=N'IHBF') AND ([GameDate]='" + DateTime.Now.ToString("yyyy-MM-dd") + "')" + "   GROUP BY [GameType],[GameDate] \r\n"
                       

                       + ""
                       + "SELECT * FROM @Games"
                       + " ORDER BY [GameType], [GameDate],[GameTime]"
                       + ""
                       + "\r\n";
            #endregion
            #region 篩選條件

            //篩選時間 當前時間 +-6小時
            DateTime selectTimeBegin = DateTime.Now.AddHours(-6);
            DateTime selectTimeEnd = DateTime.Now.AddHours(6);

            string where = "";
            where += " AND ((S.[GameDate] ='" + selectTimeBegin.ToString("yyyy-MM-dd") + "' AND DATEPART(hh, S.[GameTime]) >= " + selectTimeBegin.Hour + ")";
            where += "      OR (S.[GameDate] ='" + selectTimeEnd.ToString("yyyy-MM-dd") + "' AND DATEPART(hh, S.[GameTime]) <= " + selectTimeEnd.Hour + "))";
            #endregion

            SqlConnection conn = null;
            SqlDataAdapter da = null;
            DataSet ds = new DataSet();
            // 錯誤處理
            try
            {
                conn = new SqlConnection(ConnectionString);
                da = new SqlDataAdapter(string.Format(sql, where), conn);
                // 開啟
                conn.Open();
                // 讀取
                da.Fill(ds, "Games");
                // 判斷資料
                if (ds.Tables.Contains("Games") &&
                    ds.Tables["Games"].Rows.Count > 0)
                {
                    DataTable dt = this.Schedule.Copy();
                    this.Schedule.Rows.Clear();
                    this.Schedule.AcceptChanges();
                    // 資料
                    foreach (DataRow row in ds.Tables["Games"].Rows)
                    {
                        DataRow now = this.Schedule.NewRow();
                        DateTime gameTime = DateTime.Parse(((DateTime)row["GameDate"]).ToString("yyyy-MM-dd") + " " + row["GameTime"].ToString());
                        string gid = row["GameType"].ToString() + row["GID"].ToString();
                        int changeCount = (int)row["ChangeCount"];
                        // 如果是俄冰，就把時間加 4 個小時
                        //if (row["GameType"].ToString() == "IHRU")
                        //{
                        //    gameTime = gameTime.AddHours(4);
                        //}
                        // 如果是足球/網球/奧訊，就把日期設為今天
                        if (row["GameType"].ToString() == "SB" ||
                            row["GameType"].ToString() == "TN" || 
                            row["GameType"].ToString() == "BKOS") 
                        {
                            gameTime = DateTime.Now.Date;
                        }
                        // 設定
                        now["GID"] = row["GID"].ToString();
                        now["Type"] = row["GameType"].ToString();
                        now["TypeName"] = this.GetScheduleTypeName(now["Type"].ToString());
                        now["Status"] = row["GameStates"].ToString();
                        now["CtrlStates"] = row["CtrlStates"].ToString();
                        now["StatusText"] = this.GetScheduleStatus(now["Status"].ToString());
                        now["Away"] = row["Home"].ToString();
                        now["Home"] = row["Away"].ToString();
                        now["Date"] = gameTime.ToString("yyyy-MM-dd");
                        now["Time"] = gameTime.ToString("HH:mm");
                        now["DateTime"] = gameTime;
                        now["WebId"] = row["WebId"].ToString();
                        now["ChangeCount"] = row["ChangeCount"].ToString();
                        now["ChangeTime"] = DateTime.Now.ToString();
                        // 加入
                        this.Schedule.Rows.Add(now);
                        // 判斷
                        if (!this.ScheduleRecord.ContainsKey(gid))
                        {
                            // 設定初始
                            this.ScheduleRecord[gid] = new Record();
                            this.ScheduleRecord[gid].Count = changeCount;
                            this.ScheduleRecord[gid].Timer = DateTime.Now;
                        }
                        else
                        {
                            // 與記錄的次數不相同就重設時間
                            if (this.ScheduleRecord[gid].Count != changeCount)
                            {
                                this.ScheduleRecord[gid].Count = changeCount;
                                this.ScheduleRecord[gid].Timer = DateTime.Now;
                            }
                            else
                            {
                                // 與記錄相同設定成記錄時間
                                now["ChangeTime"] = this.ScheduleRecord[gid].Timer.ToString();
                            }
                        }
                    }
                    this.Schedule.AcceptChanges();

                    BindingSource bs = new BindingSource();
                    // 設定
                    bs.DataSource = this.Schedule;
                    //bs.Filter = string.Format("Date='{0}'", DateTime.Now.ToString("yyyy-MM-dd"));
                    bs.Sort = "Date DESC,Time";
                    // 資料
                    this.dgvData.DataSource = bs;
                }
            }
            catch(Exception ex) {
                apLog.Error("LoadData Error Message:{0},\r\nStackTrace:{1}\r\n Sql:{2}", ex.Message, ex.StackTrace, string.Format(sql, where));
            };
        }
        private void LoadLastDate()
        {
            #region Sql
            string sql = " DECLARE @Games TABLE("
                       + "    GameType NVARCHAR(10)"
                       + "   ,GameDate DATE"
                       + "    )" + "\r\n"
                       + "" // 棒球
                       + " INSERT INTO @Games SELECT [GameType],MAX([GameDate]) FROM [BaseballSchedules] 　With(NOLOCK) WHERE ([IsDeleted]=0) GROUP BY [GameType]" + "\r\n"
                       + "" // 籃球
                       + " INSERT INTO @Games SELECT [GameType],MAX([GameDate]) FROM [BasketballSchedules] 　With(NOLOCK) WHERE ([IsDeleted]=0) GROUP BY [GameType]" + "\r\n"
                       + "" // 冰球
                       + " INSERT INTO @Games SELECT [GameType],MAX([GameDate]) FROM [IceHockeySchedules] 　With(NOLOCK) WHERE ([IsDeleted]=0) GROUP BY [GameType]" + "\r\n"
                       + "" // 足球
                       + " INSERT INTO @Games SELECT [GameType],MAX([GameDate]) FROM [AFBSchedules]　With(NOLOCK)  WHERE ([IsDeleted]=0) GROUP BY [GameType]" + "\r\n"
                       + ""
                       + "SELECT * FROM @Games ORDER BY [GameDate] DESC"
                       + ""
                       + "\r\n";
            #endregion
            SqlConnection conn = null;
            SqlDataAdapter da = null;
            DataSet ds = new DataSet();
            // 錯誤處理
            try
            {
                conn = new SqlConnection(ConnectionString);
                da = new SqlDataAdapter(sql, conn);
                // 開啟
                conn.Open();
                // 讀取
                da.Fill(ds, "Games");
                // 判斷資料
                if (ds.Tables.Contains("Games") && ds.Tables["Games"].Rows.Count > 0)
                {
                    string msg = "";
                    // 比賽資料
                    foreach (DataRow row in ds.Tables["Games"].Rows)
                        msg += this.GetScheduleTypeName(row["GameType"].ToString()).PadRight(20, ' ') + "\t" + ((DateTime)row["GameDate"]).ToString("yyyy-MM-dd") + "\r\n";
                    // 顯示訊息
                    if (!string.IsNullOrEmpty(msg))
                        MessageBox.Show(this, msg, "最後建立的賽程日期");
                }
                // 關閉
                conn.Close();
            }
            catch { }
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
            }
            da = null;
            ds = null;
        }

        private string GetScheduleTypeName(string type)
        {
            switch (type)
            {
                case "AFUS":
                    type = "國家美式足球聯盟 (NFL)"; break;
                case "BBTW":
                    type = "中華職棒 (CPBL)"; break;
                case "BBTW7":
                    type = "爆米花夏季棒球聯盟 (PL)"; break;
                case "BBJP":
                    type = "日本職棒 (NPB)"; break;
                case "BBKR":
                    type = "韓國職棒 (KBO)"; break;
                case "BBUS":
                    type = "美國職棒 (MLB)"; break;
                case "BB3AIL":
                    type = "美國職棒 3A 國際聯盟(IL)"; break;
                case "BB3APCL":
                    type = "美國職棒 3A 太平洋岸聯盟(PCL)"; break;
                case "BBMX":
                    type = "墨西哥冬季聯盟 (LMP)"; break;
                case "BBMX2":
                    type = "墨西哥夏季聯盟 (LMB)"; break;
                case "BBAU":
                    type = "澳洲棒球 (ABL)"; break;
                case "BBNL":
                    type = "荷兰棒球 (HB)"; break;
                case "BKCN":
                    type = "中國職籃 (CBA)"; break;
                case "BKJP":
                    type = "日本職籃 (BJ)"; break;
                case "BKKR":
                    type = "韓國職籃 - 男子 (KBL)"; break;
                case "BKKRW":
                    type = "韓國職籃 - 女子 (WKBL)"; break;
                case "BKUS":
                    type = "美國職籃 (NBA)"; break;
                case "BKUSW":
                    type = "美國女子職籃 (WNBA)"; break;
                case "BKEL":
                    type = "歐洲職籃 (Euroleague)"; break;
                case "BKEL2":
                    type = "歐洲籃球聯盟盃 (Eurocup)"; break;
                case "BKVTB":
                    type = "籃球聯賽 (VTB)"; break;
                case "BKAU":
                    type = "澳洲職籃 (NBL)"; break;
                case "BKFIBA":
                    type = "亞洲籃球錦標賽 (FIBA)"; break;
                case "BKEBT":
                    type = "歐洲籃球錦標賽 (FIBA)"; break;
                case "BKACB":
                    type = "西班牙籃球 (ACB)"; break;
                case "BKBBL":
                    type = "德國籃球甲級聯賽 (BBL)"; break;
                case "BKNCAA":
                    type = "美國大學男子籃球 (NCAA)"; break;
                case "BKNBL":
                    type = "中國男子籃球甲級聯賽 (NBL)"; break;
                case "IHUS":
                    type = "國家冰球 (NHL)"; break;
                case "IHUS2":
                    type = "美國冰球 (AHL)"; break;
                case "IHRU":
                    type = "俄羅斯冰球 (KHL)"; break;
                case "SB":
                    type = "足球 (Football)"; break;
                case "TN":
                    type = "網球 (Tennis)"; break;
                case "BKOS":
                    type = "籃球 (奧訊)"; break;
                case "BKBF":
                    type = "籃球 (BF)"; break;
                case "IHBF":
                    type = "冰球 (BF)"; break;
            }
            // 傳回
            return type;
        }
        private string GetScheduleTypeProcess(string type)
        {
            switch (type)
            {
                case "AFUS":
                    type = "(NFL)"; break;
                case "BBTW":
                    type = "(CPBL)"; break;
                case "BBTW7":
                    type = "(PL)"; break;
                case "BBJP":
                    type = "(NPB)"; break;
                case "BBKR":
                    type = "(KBO)"; break;
                case "BBUS":
                    type = "(MLB)"; break;
                case "BB3AIL":
                    type = "(IL)"; break;
                case "BB3APCL":
                    type = "(PCL)"; break;
                case "BBMX":
                    type = "(LMP)"; break;
                case "BBMX2":
                    type = "(LMB)"; break;
                case "BBAU":
                    type = "(ABL)"; break;
                case "BBNL":
                    type = "(HB)"; break;
                case "BKCN":
                    type = "(CBA)"; break;
                case "BKJP":
                    type = "(BJ)"; break;
                case "BKKR":
                    type = "(KBL)"; break;
                case "BKKRW":
                    type = "(WKBL)"; break;
                case "BKUS":
                    type = "(NBA)"; break;
                case "BKUSW":
                    type = "(WNBA)"; break;
                case "BKEL":
                    type = "(Euroleague)"; break;
                case "BKEL2":
                    type = "(Eurocup)"; break;
                case "BKVTB":
                    type = "(VTB)"; break;
                case "BKAU":
                    type = "(NBL)"; break;
                case "BKFIBA":
                    type = "(FIBA)"; break;
                case "BKEBT":
                    type = "(EBT)"; break;
                case "BKACB":
                    type = "(ACB)"; break;
                case "BKBBL":
                    type = "(BBL)"; break;
                case "BKNCAA":
                    type = "(NCAA)"; break;
                case "BKNBL":
                    type = "(CNBL)"; break;
                case "IHUS":
                    type = "(NHL)"; break;
                case "IHUS2":
                    type = "(AHL)"; break;
                case "IHRU":
                    type = "(KHL)"; break;
                case "SB":
                    type = "(Football)";break;
                case "TN":
                    type = "(Tennis)"; break;
                case "BKOS":
                    type = "(BKOS)"; break;
                case "BKBF":
                    type = "(BKBF)"; break;
                case "IHBF":
                    type = "(IHBF)"; break;
            }
            // 傳回
            return type;
        }
        private string GetScheduleStatus(string status)
        {
            switch (status)
            {
                case "X": status = "未開始"; break;
                case "S": status = "比賽中"; break;
                case "E": status = "結束"; break;
                case "P": status = "延後"; break;
            }
            // 傳回
            return status;
        }
        private void dgvData_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, this.dgvData.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.dgvData.RowHeadersDefaultCellStyle.Font, rectangle, this.dgvData.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion

        #region Timer

        //比賽資料變動最低時間(秒)
        private int defaultMinSec = 180;
        private Dictionary<string, int> GameChangeMinSec = new Dictionary<string, int>();
        private void InitGameChangeTime()
        {
            //預設值
            List<string> tmp = new List<string>() { "bb", "bk", "sb", "tn", "ih", "af","fb"};

            foreach (var key in tmp)
            {
                int sec = 0;
                int.TryParse(ConfigurationManager.AppSettings[key + "ChgMinTime"], out sec);

                if (sec == 0)
                    sec = defaultMinSec;
                else if (sec < 30)//最小30秒
                    sec = 30;
                
                GameChangeMinSec.Add(key, sec);
            }

        }

        private void timerLoad_Tick(object sender, EventArgs e)
        {
            // 讀取資料
            this.LoadData();

            try
            {
                //Task.Run(()=>Client.CheckConnect());//檢查連線
                //Client.CheckConnect();

                //需要關閉的跟盤
                CheckGameClose();

                //重啟跟盤程式
                CheckGameRestart();

                //需要開啟的跟盤
                CheckGameStart();
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Message:" + ex.Message);
                sb.AppendLine("StackTrace:" + ex.StackTrace);

                apLog.Error(sb.ToString());
            }
        }

        private void SetServerStatus(bool isLink)
        {
            this.InvokeIfRequired(() =>
            {
                if (isLink)
                {
                    lbServerStatus.Text = "連線中";
                    lbServerStatus.ForeColor = Color.Green;
                }
                else
                {
                    lbServerStatus.Text = "未連線";
                    lbServerStatus.ForeColor = Color.Red;
                }
            });
        }

        //需要關閉的跟盤
        private void CheckGameClose()
        {
            Dictionary<string, string> kill = new Dictionary<string, string>();
            DataView dv = null;

            #region 需要關閉的跟盤
            // 找出所有比賽結束的賽程
            dv = new DataView(this.Schedule, "( Status='E' OR Status='P' ) AND CtrlStates ='2'", "Date,Time", DataViewRowState.CurrentRows);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    int CtrlStates = 0;
                    if (int.TryParse(dv[i]["CtrlStates"].ToString(), out CtrlStates) && CtrlStates <= 1)//非自動跟盤
                        continue;

                    string webId = dv[i]["WebId"].ToString();
                    string type = dv[i]["Type"].ToString();
                    DateTime gameTime = DateTime.Parse(dv[i]["Date"] + " " + dv[i]["Time"].ToString());
                    TimeSpan spanTime = DateTime.Now.Subtract(gameTime);
                    double spanSeconds = Math.Abs(spanTime.TotalSeconds);

                    // 判斷時間 (6 小時)
                    if (spanSeconds >= 60 * 60 * 6)
                    {
                        DataView dv2 = new DataView(this.Schedule, "Type='" + type + "' AND CtrlStates ='2' AND (Status='S' OR (Status='X' AND DateTime<'" + DateTime.Now.AddHours(4).ToString("yyyy-MM-dd HH:mm") + "'))", "", DataViewRowState.CurrentRows);
                        if (dv2.Count == 0)// 判斷是否有未完成的比賽
                        {
                            // 可以關閉
                            if (!kill.ContainsKey(type))
                            {
                                //Console.WriteLine("有比賽需要關閉");
                                kill[type] = "1";

                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("Need Kill!!");
                                sb.AppendLine("Type:" + type);
                                sb.AppendLine("spanSeconds:" + Math.Floor(spanSeconds));

                                apLog.Info(sb.ToString());

                                if (StartAPTime.ContainsKey(type))//資料存在移除
                                    StartAPTime.Remove(type);

                                string cmd = "<XML><Command>kill</Command><Process>{0}</Process><Note>Monitor</Note></XML>";
                                string data = string.Format(cmd, this.GetScheduleTypeProcess(type));
                                if (!string.IsNullOrEmpty(data))//有資料需要送
                                    Client.Send(data, type);
                            }
                        }
                    }
                }
            }
            #endregion

            //#region 關閉跟盤程式
            //if (kill.Count > 0)
            //{
            //    string cmd = "<XML><Command>kill</Command><Process>{0}</Process><Note>Monitor</Note></XML>";
            //    SendCommand(kill, cmd, true);
            //}
            //#endregion
        }

        //重啟跟盤程式
        private void CheckGameRestart()
        {
            Dictionary<string, string> open = new Dictionary<string, string>();
            DataView dv = null;

            #region 需要重啟的跟盤
            // 找出所有比賽中的賽程
            dv = new DataView(this.Schedule, "Status='S' AND CtrlStates ='2'", "Date,Time", DataViewRowState.CurrentRows);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    string gid = dv[i]["Type"].ToString() + dv[i]["GID"].ToString();
                    string webId = dv[i]["WebId"].ToString();
                    string type = dv[i]["Type"].ToString();
                    int changeCount = int.Parse(dv[i]["ChangeCount"].ToString());
                    DateTime changeTime = DateTime.Parse(dv[i]["ChangeTime"].ToString());
                    TimeSpan spanTime = DateTime.Now.Subtract(changeTime);
                    double spanSeconds = Math.Abs(spanTime.TotalSeconds);

                    int CtrlStates = 0;
                    if (int.TryParse(dv[i]["CtrlStates"].ToString(), out CtrlStates) && CtrlStates <= 1)//非自動跟盤
                        continue;

                    // 初始資料
                    if (!open.ContainsKey(type))
                    {
                        open[type] = "";
                    }

                    // 判斷時間
                    int diffMinSec = this.defaultMinSec;
                    string key = type.ToLower().Substring(0, 2);
                    if(type.ToLower() == "bkos" || key == "tn" || key == "sb")//fbview 資料格式統一取fb重啟時間
                        diffMinSec = GameChangeMinSec["fb"];
                    else if (GameChangeMinSec.ContainsKey(key))
                        diffMinSec = GameChangeMinSec[key];

                    if (spanSeconds >= diffMinSec)
                    {
                        // 變更次數相同，表示資料很久沒有更新，需要重啟跟盤
                        if (this.ScheduleRecord[gid].Count == changeCount)
                        {
                            if (!StartAPTime.ContainsKey(type))
                                StartAPTime.Add(type, DateTime.Now);//資料不存在初始化
                            else if (new TimeSpan(DateTime.Now.Ticks - StartAPTime[type].Ticks).TotalSeconds < this.iRestartAPTime)
                                continue;

                            // 可以重啟
                            if (open[type] == "")
                            {
                                //重啟時間紀錄
                                StartAPTime[type] = DateTime.Now;

                                //Console.WriteLine("有比賽需要重啟");
                                DateTime time = DateTime.Now;
                                DateTime.TryParse(dv[i]["DateTime"].ToString(), out time);

                                open[type] = time.ToString("yyyy/MM/dd HH:mm");

                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("Need Restart!!");
                                sb.AppendLine("Type:" + type);
                                sb.AppendLine("DateTime:" + open[type]);

                                apLog.Info(sb.ToString());

                                string cmd = "<XML><Command>open</Command><Process>{0}</Process><Date>{1}</Date><Note>Monitor</Note></XML>";
                                string data = string.Format(cmd, this.GetScheduleTypeProcess(type), open[type]);
                                if (!string.IsNullOrEmpty(data))//有資料需要送
                                    Client.Send(data, type);
                            }
                        }
                    }

                    // 變更次數不同，表示資料有在更新，重設次數與時間
                    if (this.ScheduleRecord[gid].Count != changeCount)
                    {
                        this.ScheduleRecord[gid].Count = changeCount;
                        this.ScheduleRecord[gid].Timer = changeTime;
                    }
                }
            }
            #endregion
            //#region 重啟跟盤程式
            //if (open.Count > 0)
            //{
            //    string cmd = "<XML><Command>open</Command><Process>{0}</Process><Date>{1}</Date><Note>Monitor</Note></XML>";
            //    SendCommand(open, cmd);
            //}
            //#endregion
        }

        //需要開啟的跟盤
        private void CheckGameStart()
        {
            Dictionary<string, string> run = new Dictionary<string, string>();
            DataView dv = null;
            string message = null;

            #region 需要開啟的跟盤
            // 找出所有還沒比賽的賽程
            dv = new DataView(this.Schedule, "Status='X' AND CtrlStates ='2' ", "Date,Time", DataViewRowState.CurrentRows);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    int CtrlStates = 0;
                    if (int.TryParse(dv[i]["CtrlStates"].ToString(), out CtrlStates) && CtrlStates <= 1)//非自動跟盤
                        continue;

                    string type = dv[i]["Type"].ToString();
                    DataView dv2 = new DataView(this.Schedule, "Type='" + type + "' AND Status='S' AND CtrlStates ='2'", "", DataViewRowState.CurrentRows);
                    if (dv2.Count > 0)// 判斷是否有進行中的比賽
                        continue;                   

                    DateTime gameTime = DateTime.Parse(dv[i]["Date"] + " " + dv[i]["Time"].ToString());
                    TimeSpan spanTime = DateTime.Now.Subtract(gameTime);
                    //double spanSeconds = Math.Abs(spanTime.TotalSeconds);

                    if (spanTime.TotalSeconds > -60 * 10 && // 開賽前時間 (10 分鐘)
                        spanTime.TotalSeconds < 60 * 60 * 2)      //開賽後2小時內
                    {
                        if (!StartAPTime.ContainsKey(type))
                            StartAPTime.Add(type, DateTime.Now);//資料不存在初始化
                        else if (new TimeSpan(DateTime.Now.Ticks - StartAPTime[type].Ticks).TotalSeconds < this.iRestartAPTime)
                            continue;

                        //重啟時間紀錄
                        StartAPTime[type] = DateTime.Now;

                        // 需要執行
                        run[type] = gameTime.ToString("yyyy/MM/dd HH:mm");
                        // 訊息
                        message += this.GetScheduleTypeName(type).PadRight(20, ' ') + "\t" + gameTime.ToString("HH:mm") + "\r\n";

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Need Run!!");
                        sb.AppendLine("Type:" + type);
                        sb.AppendLine("DateTime:" + run[type]);

                        apLog.Info(sb.ToString());

                        string cmd = "<XML><Command>run</Command><Process>{0}</Process><Date>{1}</Date><Note>Monitor</Note></XML>";
                        string data = string.Format(cmd, this.GetScheduleTypeProcess(type), run[type]);
                        if (!string.IsNullOrEmpty(data))//有資料需要送
                            Client.Send(data, type);
                    }
                    
                }
            }
            #endregion

            //#region 執行跟盤程式
            //if (run.Count > 0)
            //{
            //    string cmd = "<XML><Command>run</Command><Process>{0}</Process><Date>{1}</Date><Note>Monitor</Note></XML>";
            //    SendCommand(run, cmd);
            //}
            //#endregion

            #region 顯示訊息
            if (!string.IsNullOrEmpty(message))
            {
                this.nfiMain.ShowBalloonTip(1000 * 30, this.Text, message, ToolTipIcon.Error);
            }
            else
            {
                this.nfiMain.BalloonTipText = "";
                this.nfiMain.BalloonTipTitle = "";
            }
            #endregion

            this.ScheduleChecked();
        }

        private void SendCommand(Dictionary<string, string> obj, string cmd, bool isKill = false)
        {
            foreach (KeyValuePair<string, string> pro in obj)
            {
                string proName = this.GetScheduleTypeProcess(pro.Key);
                string data = null;
                

                if (isKill && pro.Value == "1")//需要被關閉
                {
                    if (StartAPTime.ContainsKey(pro.Key))//資料存在移除
                        StartAPTime.Remove(pro.Key);

                    data = string.Format(cmd, proName);
                }
                else if (!string.IsNullOrEmpty(pro.Value))//需要重啟/開啟
                {
                    DateTime time = DateTime.Now;
                    DateTime.TryParse(pro.Value, out time);

                    data = string.Format(cmd, proName, time.ToString("yyyy/MM/dd HH:mm"));
                }

                if (!string.IsNullOrEmpty(data))//有資料需要送
                    Client.Send(data, proName);
            }
        }

        private DataTable Schedule = new DataTable();
        private Dictionary<string, Record> ScheduleRecord = new Dictionary<string, Record>();
        private void ScheduleChecked()
        {
            foreach (DataGridViewRow drow in this.dgvData.Rows)
            {
                switch (drow.Cells["colType"].Value.ToString().Substring(0, 2))
                {
                    case "BB":  // 棒球
                        drow.Cells["colTypeName"].Style.BackColor = Color.FromArgb(255, 108, 0);
                        drow.Cells["colTypeName"].Style.ForeColor = Color.White;
                        break;
                    case "BK":  // 籃球
                        drow.Cells["colTypeName"].Style.BackColor = Color.FromArgb(179, 0, 0);
                        drow.Cells["colTypeName"].Style.ForeColor = Color.White;
                        break;
                    case "IH":  // 冰球
                        drow.Cells["colTypeName"].Style.BackColor = Color.FromArgb(0, 67, 179);
                        drow.Cells["colTypeName"].Style.ForeColor = Color.White;
                        break;
                    case "AF":  // 足球
                        drow.Cells["colTypeName"].Style.BackColor = Color.FromArgb(0, 140, 20);
                        drow.Cells["colTypeName"].Style.ForeColor = Color.White;
                        break;
                }
                switch (drow.Cells["colStatus"].Value.ToString())
                {
                    case "S":
                        drow.Cells["colStatusText"].Style.BackColor = Color.Blue;
                        drow.Cells["colStatusText"].Style.ForeColor = Color.White;
                        break;
                    case "X":
                        DateTime gameTime = DateTime.Parse(drow.Cells["colDate"].Value.ToString() + " " + drow.Cells["colTime"].Value.ToString());
                        // 超過比賽時間
                        if (DateTime.Now > gameTime)
                        {
                            drow.Cells["colStatusText"].Style.BackColor = Color.Yellow;
                            drow.Cells["colStatusText"].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            drow.Cells["colStatusText"].Style.BackColor = Color.White;
                            drow.Cells["colStatusText"].Style.ForeColor = Color.Black;
                        }
                        break;
                    default:
                        drow.Cells["colStatusText"].Style.BackColor = Color.White;
                        drow.Cells["colStatusText"].Style.ForeColor = Color.Black;
                        break;
                }
            }
        }
        #endregion
    }

    public class Record
    {
        public int Count { get; set; }
        public DateTime Timer { get; set; }
    }

        //扩展方法必须在非泛型静态类中定义
    public static class Extension
    {
        //非同步委派更新UI
        public static void InvokeIfRequired(this Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)//在非當前執行緒內 使用委派
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
