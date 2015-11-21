using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Monitor
{
    class Schedules
    {
        public DataTable Data { get; private set; }
        private Dictionary<string, Record> ScheduleRecord;

        public Schedules()
        {
            Data = new DataTable();
            ScheduleRecord = new Dictionary<string, Record>();

            // 資料表
            this.Data.Columns.Add(new DataColumn("GID"));
            this.Data.Columns.Add(new DataColumn("Type"));
            this.Data.Columns.Add(new DataColumn("TypeName"));
            this.Data.Columns.Add(new DataColumn("Date"));
            this.Data.Columns.Add(new DataColumn("Time"));
            this.Data.Columns.Add(new DataColumn("DateTime", System.Type.GetType("System.DateTime")));
            this.Data.Columns.Add(new DataColumn("Away"));
            this.Data.Columns.Add(new DataColumn("Home"));
            this.Data.Columns.Add(new DataColumn("Status"));
            this.Data.Columns.Add(new DataColumn("CtrlStates"));
            this.Data.Columns.Add(new DataColumn("StatusText"));
            this.Data.Columns.Add(new DataColumn("WebId"));
            this.Data.Columns.Add(new DataColumn("ChangeCount"));
            this.Data.Columns.Add(new DataColumn("ChangeTime"));
        }

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
                       + "   FROM [BaseballSchedules] AS S INNER JOIN"
                       + "        [BaseballTeam] AS TA ON (TA.[TeamID] = S.[TeamAID]) INNER JOIN"
                       + "        [BaseballTeam] AS TB ON (TB.[TeamID] = S.[TeamBID])"
                       + "  WHERE (S.[IsDeleted]=0) {0}" + "\r\n"
                       + "" // 籃球
                       + " INSERT INTO @Games"
                       + " SELECT S.[GID],S.[GameType],S.[GameDate],S.[GameTime],S.[GameStates],S.[CtrlStates],TA.[ShowName] AS [Home],TB.[ShowName] AS Away,S.[WebID],S.[ChangeCount]"
                       + "   FROM [BasketballSchedules] AS S INNER JOIN"
                       + "        [BasketballTeam] AS TA ON (TA.[TeamID] = S.[TeamAID]) INNER JOIN"
                       + "        [BasketballTeam] AS TB ON (TB.[TeamID] = S.[TeamBID])"
                       + "  WHERE (S.[IsDeleted]=0) {0}" + "\r\n"
                       + "" // 冰球
                       + " INSERT INTO @Games"
                       + " SELECT S.[GID],S.[GameType],S.[GameDate],S.[GameTime],S.[GameStates],S.[CtrlStates],TA.[ShowName] AS [Home],TB.[ShowName] AS Away,S.[WebID],S.[ChangeCount]"
                       + "   FROM [IceHockeySchedules] AS S INNER JOIN"
                       + "        [IceHockeyTeam] AS TA ON (TA.[TeamID] = S.[TeamAID]) INNER JOIN"
                       + "        [IceHockeyTeam] AS TB ON (TB.[TeamID] = S.[TeamBID])"
                       + "  WHERE (S.[IsDeleted]=0) {0}" + "\r\n"
                       + "" // 美足
                       + " INSERT INTO @Games"
                       + " SELECT S.[GID],S.[GameType],S.[GameDate],S.[GameTime],S.[GameStates],S.[CtrlStates],TA.[ShowName] AS [Home],TB.[ShowName] AS Away,S.[WebID],S.[ChangeCount]"
                       + "   FROM [AFBSchedules] AS S INNER JOIN"
                       + "        [AFBTeam] AS TA ON (TA.[TeamID] = S.[TeamAID]) INNER JOIN"
                       + "        [AFBTeam] AS TB ON (TB.[TeamID] = S.[TeamBID])"
                       + "  WHERE (S.[IsDeleted]=0) {0}" + "\r\n"
                       + "" // 足球
                       + " INSERT INTO @Games"
                       + " SELECT [id], [GameType], [GameDate], '00:00' AS [GameTime],'S' AS [GameStates], '2' AS [CtrlStates], N'足' AS [Home], N'球' AS [Away], NULL AS [WebID], [ChangeCount]"
                       + "   FROM [FBView]"
                       + "   WHERE (GameType=N'SB') AND ([GameDate] = (SELECT [val] FROM [SetTypeVal] WHERE [type] = 'FootballDate'))" + "\r\n"
                       + "" // 網球
                       + " INSERT INTO @Games"
                       + " SELECT [id], [GameType], [GameDate], '00:00' AS [GameTime],'S' AS [GameStates], '2' AS [CtrlStates], N'網' AS [Home], N'球' AS [Away], NULL AS [WebID], [ChangeCount]"
                       + "   FROM [FBView]"
                       + "   WHERE (GameType=N'TN') AND ([GameDate]='" + DateTime.Now.ToString("yyyy-MM-dd") + "')" + "\r\n"
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
                    DataTable dt = this.Data.Copy();
                    this.Data.Rows.Clear();
                    this.Data.AcceptChanges();
                    // 資料
                    foreach (DataRow row in ds.Tables["Games"].Rows)
                    {
                        DataRow now = this.Data.NewRow();
                        DateTime gameTime = DateTime.Parse(((DateTime)row["GameDate"]).ToString("yyyy-MM-dd") + " " + row["GameTime"].ToString());
                        string gid = row["GameType"].ToString() + row["GID"].ToString();
                        int changeCount = (int)row["ChangeCount"];
                        // 如果是俄冰，就把時間加 4 個小時
                        //if (row["GameType"].ToString() == "IHRU")
                        //{
                        //    gameTime = gameTime.AddHours(4);
                        //}
                        // 如果是足球或網球，就把日期設為今天
                        if (row["GameType"].ToString() == "SB" ||
                            row["GameType"].ToString() == "TN")
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
                        this.Data.Rows.Add(now);
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
                    this.Data.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
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
                       + " INSERT INTO @Games SELECT [GameType],MAX([GameDate]) FROM [BaseballSchedules] WHERE ([IsDeleted]=0) GROUP BY [GameType]" + "\r\n"
                       + "" // 籃球
                       + " INSERT INTO @Games SELECT [GameType],MAX([GameDate]) FROM [BasketballSchedules] WHERE ([IsDeleted]=0) GROUP BY [GameType]" + "\r\n"
                       + "" // 冰球
                       + " INSERT INTO @Games SELECT [GameType],MAX([GameDate]) FROM [IceHockeySchedules] WHERE ([IsDeleted]=0) GROUP BY [GameType]" + "\r\n"
                       + "" // 足球
                       + " INSERT INTO @Games SELECT [GameType],MAX([GameDate]) FROM [AFBSchedules] WHERE ([IsDeleted]=0) GROUP BY [GameType]" + "\r\n"
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
                    type = "(Football)"; break;
                case "TN":
                    type = "(Tennis)"; break;
            }
            // 傳回
            return type;
        }

        //需要關閉的跟盤
        private void CheckGameClose()
        {
            Dictionary<string, string> kill = new Dictionary<string, string>();
            DataView dv = null;

            #region 需要關閉的跟盤
            // 找出所有比賽結束的賽程
            dv = new DataView(this.Data, "Status='E' OR Status='P'", "Date,Time", DataViewRowState.CurrentRows);
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
                    // 初始資料
                    if (!kill.ContainsKey(type))
                    {
                        kill[type] = "";
                    }
                    // 判斷時間 (6 小時)
                    if (spanSeconds >= 0)
                    {
                        DataView dv2 = new DataView(this.Data, "Type='" + type + "' AND (Status='S' OR (Status='X' AND DateTime<'" + DateTime.Now.AddHours(4).ToString("yyyy-MM-dd HH:mm") + "'))", "", DataViewRowState.CurrentRows);
                        if (dv2.Count == 0)// 判斷是否有未完成的比賽
                        {
                            // 可以關閉
                            if (kill[type] == "")
                            {
                                //Console.WriteLine("有比賽需要關閉");
                                kill[type] = "1";

                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("Need Kill!!");
                                sb.AppendLine("Type:" + type);
                                sb.AppendLine("spanSeconds:" + Math.Floor(spanSeconds));

                                apLog.Info(sb.ToString());
                            }
                        }
                    }
                    else
                    {
                        // 不需要關閉
                        kill[type] = "0";
                    }
                }
            }
            #endregion

            #region 關閉跟盤程式
            if (kill.Count > 0)
            {
                string cmd = "<XML><Command>kill</Command><Process>{0}</Process><Note>Monitor</Note></XML>";
                SendCommand(kill, cmd, true);
            }
            #endregion
        }

        //重啟跟盤程式
        private void CheckGameRestart()
        {
            Dictionary<string, string> open = new Dictionary<string, string>();
            DataView dv = null;

            #region 需要重啟的跟盤
            // 找出所有比賽中的賽程
            dv = new DataView(this.Data, "Status='S'", "Date,Time", DataViewRowState.CurrentRows);
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

                    _MonitorWeb.DataAnalysis(type, dv[i]["Date"].ToString(), changeTime);

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
                    if (GameChangeMinSec.ContainsKey(key))
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
                                open[type] = dv[i]["DateTime"].ToString();

                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("Need Restart!!");
                                sb.AppendLine("Type:" + type);
                                sb.AppendLine("DateTime:" + dv[i]["DateTime"].ToString());

                                apLog.Info(sb.ToString());
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
            #region 重啟跟盤程式
            if (open.Count > 0)
            {
                string cmd = "<XML><Command>open</Command><Process>{0}</Process><Date>{1}</Date><Note>Monitor</Note></XML>";
                SendCommand(open, cmd);
            }
            #endregion
        }

        //需要開啟的跟盤
        private void CheckGameStart()
        {
            Dictionary<string, string> run = new Dictionary<string, string>();
            DataView dv = null;
            string message = null;

            #region 需要開啟的跟盤
            // 找出所有還沒比賽的賽程
            dv = new DataView(this.Data, "Status='X'", "Date,Time", DataViewRowState.CurrentRows);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    int CtrlStates = 0;
                    if (int.TryParse(dv[i]["CtrlStates"].ToString(), out CtrlStates) && CtrlStates <= 1)//非自動跟盤
                        continue;

                    string type = dv[i]["Type"].ToString();
                    DataView dv2 = new DataView(this.Data, "Type='" + type + "' AND Status='S'", "", DataViewRowState.CurrentRows);
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
                        run[type] = dv[i]["DateTime"].ToString();
                        // 訊息
                        message += this.GetScheduleTypeName(type).PadRight(20, ' ') + "\t" + gameTime.ToString("HH:mm") + "\r\n";

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Need Run!!");
                        sb.AppendLine("Type:" + type);
                        sb.AppendLine("DateTime:" + dv[i]["DateTime"].ToString());

                        apLog.Info(sb.ToString());
                    }
                }
            }
            #endregion

            #region 執行跟盤程式
            if (run.Count > 0)
            {
                string cmd = "<XML><Command>run</Command><Process>{0}</Process><Date>{1}</Date><Note>Monitor</Note></XML>";
                SendCommand(run, cmd);
            }
            #endregion

        }
    }

    public class Record
    {
        public int Count { get; set; }
        public DateTime Timer { get; set; }
    }
}
