using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;


namespace GameScoresApp
{
    public class SqlDependencyCache
    {
        private static Logger _logDependency = LogManager.GetLogger("Dependency_Log");
        private static string connectionString = ConfigurationManager.ConnectionStrings["sport"].ConnectionString;
        private static MainForm mainForm;

        // 查詢通知事件處理
        public static ConcurrentDictionary<string, DependencySetting> DependencyObject = new ConcurrentDictionary<string, DependencySetting>();
        public static Dictionary<string, DataTable> DependencyDataTable = new Dictionary<string, DataTable>();
        public static Dictionary<string, DataTable> DependencyTeamData = new Dictionary<string, DataTable>();

        //全局变量 联盟名称 奥逊 + bf  key:os_id/bf_id  val:联盟名
        private static Dictionary<string, string> AllianceData = new Dictionary<string, string>();

        public SqlDependencyCache(MainForm mForm)
        {
            mainForm = mForm;
        }

        // 清除緩存
        public void ClearCache()
        {
            AddMessage("Reset SqlDependency!!");
            ResetDependency();
        }

        private static void ResetDependency()
        {
            DependencyClear();// 清除全部通知事件
            DependencyToAll();// 加入全部通知事件
        }

        // 清除全部通知事件
        public static void DependencyClear()
        {
            try
            {
                foreach (KeyValuePair<string, DependencySetting> item in DependencyObject)
                {
                    SqlDependency sd = item.Value.dep;
                    if (sd != null)
                    {
                        sd.OnChange -= dependency_OnChange;//解除委派
                    }
                }
                // 清除
                DependencyObject.Clear();
            }
            catch (Exception ex)
            {
                _logDependency.Error("DependencyClear Error! \r\n Message:\r\n{0}, \r\n StackTrace:\r\n{1}",
                    ex.Message, ex.StackTrace);
            }
        }

        #region 取隊伍

        // 清除全部通知事件
        public static void DependencyTeamClear()
        {
            try
            {
                foreach (KeyValuePair<string, DependencySetting> item in DependencyTeamObject)
                {
                    SqlDependency sd = item.Value.dep;
                    if (sd != null)
                    {
                        sd.OnChange -= dependencyTeam_OnChange;//解除委派
                    }
                }
                // 清除
                DependencyTeamObject.Clear();
            }
            catch (Exception ex)
            {
                _logDependency.Error("DependencyTeamClear Error! \r\n Message:\r\n{0}, \r\n StackTrace:\r\n{1}",
                    ex.Message, ex.StackTrace);
            }
        }

        public void ReadAllTeamToCache()
        {
            AddMessage("ReadAllTeamToCache!!");
            ResetTeamDependecy();
        }

        private static void ResetTeamDependecy()
        {
            DependencyTeamClear();

            ReadTeamToCache(connectionString, "SELECT [TeamID], [GameType], [ShowName] FROM [dbo].[BaseballTeam]　With(NOLOCK) ", "BaseballTeam");
            ReadTeamToCache(connectionString, "SELECT [TeamID], [GameType], [ShowName] FROM [dbo].[BasketballTeam]　With(NOLOCK) ", "BasketballTeam");
            ReadTeamToCache(connectionString, "SELECT [TeamID], [GameType], [ShowName] FROM [dbo].[AFBTeam]　With(NOLOCK) ", "AFBTeam");
            ReadTeamToCache(connectionString, "SELECT [TeamID], [GameType], [ShowName] FROM [dbo].[IceHockeyTeam]　With(NOLOCK) ", "IceHockeyTeam");
            ReadTeamToCache(connectionString, "SELECT [ID],[ChangeText], [SourceText] FROM [dbo].[NameControl] 　With(NOLOCK) WHERE [GTLangx] = 'tn' AND [GameType] = 'name'", "TennisTeam");

        }

        public static ConcurrentDictionary<string, DependencySetting> DependencyTeamObject = new ConcurrentDictionary<string, DependencySetting>();
        private static void ReadTeamToCache(string connectionString, string selectSql, string cacheName)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(selectSql, con))
                {
                    cmd.Notification = null;
                    // 查詢通知使用的項目 (必須放在讀取資料之前，否則無效)
                    SqlDependency sd = new SqlDependency(cmd);
                    sd.OnChange += dependencyTeam_OnChange;// 事件

                    // 設定
                    DependencySetting ds = new DependencySetting();
                    ds.SqlCommand = selectSql;
                    ds.Date = DateTime.Now; // 讀取資料的時間
                    ds.GameType = cacheName;
                    ds.dep = sd;

                    // 加入
                    DependencyTeamObject.AddOrUpdate(sd.Id, ds, (k, v) => ds);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        // 主鍵
                        if (!dt.Columns.Contains("TeamID"))//网球
                        {
                            dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
                        }
                        else
                        {
                            dt.PrimaryKey = new DataColumn[] { dt.Columns["TeamID"] };
                        }
                        // 記錄
                        DependencyTeamData[cacheName] = dt;
                    }
                }
            }
        }

        private static void dependencyTeam_OnChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency sd = (SqlDependency)sender;
            sd.OnChange -= dependencyTeam_OnChange;//解除委派

            // 判斷是否在事件中
            if (DependencyTeamObject.ContainsKey(sd.Id))
            {
                try
                {
                    DependencySetting ds = DependencyTeamObject[sd.Id];
                    // 讀取資料 非同步
                    ReadTeamToCache(connectionString, ds.SqlCommand, ds.GameType);
                    // 移除資料
                    DependencyTeamObject.TryRemove(sd.Id, out ds);

                    _logDependency.Info("DependencyTeam OnChange: " + ds.GameType);
                }
                catch (Exception ex)
                {
                    _logDependency.Error("DependencyToTeam Error! \r\n Message:\r\n{0}, \r\n StackTrace:\r\n{1}",
                        ex.Message, ex.StackTrace);
                }
            }
        }

        #endregion

        #region 取联盟 奥逊 bf
        private static void AddAllianceData()
        {
            //没有的时候才去数据库里取
            if (AllianceData.Count == 0)
            {
                string sDate = string.Empty;
                string sSql = " select N'bkos' t,[AllianceID],[ShowName] from  [dbo].[OSAlliance] where [ShowName] like '%ncaa%' " +
                              "  UNION ALL " +
                              "  select N'bkbf' t,[AllianceID],[ShowName] from  [dbo].[BFAlliance] where [ShowName] like '%ncaa%' ";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = sSql;
                        try
                        {
                            cmd.Connection.Open();
                            SqlDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                AllianceData.Add(string.Format("{0}_{1}", dr["t"], dr["AllianceID"]), dr["ShowName"].ToString());
                            }
                        }
                        catch (Exception exc)
                        {
                            AddMessage("AddAllianceData Error!!" + exc.Message + "////", 2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取联盟名称
        /// </summary>
        /// <param name="type">gameType</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public static string GetAllianceName(string type, string id)
        {
            if (AllianceData.Count == 0)
            {
                AddAllianceData();
                AddMessage("ReadAllianceData!!");
            }
            if (AllianceData.ContainsKey(string.Concat(type, "_", id)))
            {
                return AllianceData[string.Concat(type, "_", id)].ToLower().Trim();
            }
            else
            {
                return "";
            }

        }
        #endregion

        public void StartDependency()
        {
            SqlDependency.Start(connectionString);
            AddMessage("SqlDependency Start");

            ReadAllTeamToCache();//重讀隊伍名稱

            AddAllianceData();//添加联盟名称
            AddMessage("AddAllianceData!!");

            ClearCache();//重建資料


        }

        public void StopDependency()
        {
            SqlDependency.Stop(connectionString);
            AddMessage("SqlDependency Stop");
        }

        private static void DependencyToAll()
        {
            DateTime dateNow = DateTime.Now;
            string Sql = null;

            // 棒球
            Sql = "SELECT [GID],[GameType],[GameStates],[GameDate],[GameTime],[TeamAID],[TeamBID],[RunsA],[RunsB],[StatusText],[TrackerText],[ChangeCount],[RA],[HA],[EA],[RB],[HB],[EB],[B],[S],[O],[Bases],[Record]"
                + "  FROM [dbo].[BaseballSchedules]　 "
                + " WHERE ([IsDeleted]=@IsDeleted)"
                + "   AND (CONVERT(VARCHAR(8),[GameDate],112) = @Date)"
                + " ORDER BY [GameTime],[GameType]";
            DependencyToTable(Sql, dateNow, "bb");
            DependencyToTable(Sql, dateNow.AddDays(-1), "bb");


            // 籃球
            Sql = "SELECT [GID],[GameType],[AllianceID],[GameStates],[GameDate],[GameTime],[TeamAID],[TeamBID],[RunsA],[RunsB],[Record],[StatusText],[TrackerText],[ChangeCount]"
                + "  FROM [dbo].[BasketballSchedules]　 "
                + " WHERE ([IsDeleted] = @IsDeleted)"
                + "   AND (CONVERT(VARCHAR(8),[GameDate],112) = @Date)"
                + " ORDER BY [GameTime],[GameType]";
            DependencyToTable(Sql, dateNow, "bk");
            DependencyToTable(Sql, dateNow.AddDays(-1), "bk");


            // 美足
            Sql = "SELECT [GID],[GameType],[GameStates],[GameDate],[GameTime],[TeamAID],[TeamBID],[RunsA],[RunsB],[StatusText],[TrackerText],[ChangeCount]"
                + "  FROM [dbo].[AFBSchedules]　 "
                + " WHERE ([IsDeleted] = @IsDeleted)"
                + "   AND (CONVERT(VARCHAR(8),[GameDate],112) = @Date)"
                + " ORDER BY [GameTime],[GameType]";
            DependencyToTable(Sql, dateNow, "af");
            DependencyToTable(Sql, dateNow.AddDays(-1), "af");

            // 冰球
            Sql = "SELECT [GID],[GameType],[GameStates],[GameDate],[GameTime],[TeamAID],[TeamBID],[RunsA],[RunsB],[StatusText],[TrackerText],[ChangeCount]"
                + "  FROM [dbo].[IceHockeySchedules]　 "
                + " WHERE ([Display] = @IsDeleted)"
                + "   AND (CONVERT(VARCHAR(8),[GameDate],112) = @Date)"
                + " ORDER BY [GameTime],[GameType]";
            DependencyToTable(Sql, dateNow, "ih");
            DependencyToTable(Sql, dateNow.AddDays(-1), "ih");


            // 網球
            Sql = @"SELECT [GID],[AllianceID],[GameDate],[GameTime],[GameStates],[TeamAID],[TeamBID],[RunsA],[RunsB],[RA],[RB],[WN],[PR],[WebID],[TrackerText],[IsDeleted],[ChangeCount] FROM [dbo].[TennisSchedules] WHERE (CONVERT(VARCHAR(8),[GameDate],112) = @Date)";
            DependencyToTable(Sql, dateNow, "tn");
            DependencyToTable(Sql, dateNow.AddDays(-1), "tn");

            ////奧迅籃球
            //Sql = "SELECT [id],[GameDate],[Info],[Changed],[ChangeCount],[GameCount]"
            //    + "  FROM [dbo].[FBView] "
            //    + " WHERE ([GameType]='bkos')"
            //    + "   AND (CONVERT(VARCHAR(8),[GameDate],112) = @Date)";
            //DependencyToTable(Sql, dateNow, "bkos");
            //DependencyToTable(Sql, dateNow.AddDays(-1), "bkos");

            ////BF篮球
            //Sql = "SELECT [id],[GameDate],[Info],[Changed],[ChangeCount],[GameCount]"
            //   + "  FROM [dbo].[FBView]"
            //   + " WHERE ([GameType]='bkbf')"
            //   + "   AND (CONVERT(VARCHAR(8),[GameDate],112) = @Date)";
            //DependencyToTable(Sql, dateNow, "bkbf");
            //DependencyToTable(Sql, dateNow.AddDays(-1), "bkbf");

            ////新版奧遜籃球
            //Sql = "SELECT [id],[GameDate],[Info],[Changed],[ChangeCount],[GameCount]"
            //    + "  FROM [dbo].[FBView] "
            //    + " WHERE ([GameType]='bkos')"
            //    + "   AND (CONVERT(VARCHAR(8),[GameDate],112) = @Date)";
            //DependencyToTable(Sql, dateNow, "bkos");
            //DependencyToTable(Sql, dateNow.AddDays(-1), "bkos");

        }


        public static void DependencyToTable(string Sql, DateTime date, string gameType)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sql, con))
                {
                    string strDate = date.ToString("yyyyMMdd");
                    cmd.Parameters.Add(new SqlParameter("@IsDeleted", SqlDbType.Bit) { Value = (gameType == "ih" ? true : false) });
                    cmd.Parameters.Add(new SqlParameter("@Date", strDate));
                    cmd.Notification = null;

                    // 查詢通知使用的項目 (必須放在讀取資料之前，否則無效)
                    SqlDependency sd = new SqlDependency(cmd);//timeout = ? sec
                    sd.OnChange += dependency_OnChange;

                    // 設定
                    DependencySetting ds = new DependencySetting();
                    ds.SqlCommand = Sql;
                    ds.Date = date; // 讀取的比賽日期
                    ds.GameType = gameType;
                    ds.dep = sd;

                    // 加入                    
                    DependencyObject.AddOrUpdate(sd.Id, ds, (k, v) => ds);

                    // 讀取資料
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            // 加入主鍵
                            dt.PrimaryKey = new DataColumn[] { dt.Columns["GID"] };
                            // 放入緩存
                            DependencyDataTable[gameType + "_" + strDate] = dt;

                            // 移除2天前的緩存
                            if (DependencyDataTable.ContainsKey(gameType + "_" + date.AddDays(-2).ToString("yyyyMMdd")))
                                DependencyDataTable.Remove(gameType + "_" + date.AddDays(-2).ToString("yyyyMMdd"));
                        }
                        catch (Exception ex)
                        {
                            string msg = string.Format("SqlDataAdapter Error!\r\n Sql:{0} \r\n cacheName:{1} \r\n \r\n Message:{2} \r\n StackTrace:{3}",
                                                        Sql, (gameType + "_" + date.ToString("yyyyMMdd")), ex.Message, ex.StackTrace);
                            AddMessage(msg, 2);

                            // 重新註冊 sqldependency
                            ResetDependency();
                        }

                    }

                    //資料分析
                    BigBallRequest.BigBallAnalysis(gameType, date);
                }
            }
        }

        private static void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            try
            {
                SqlDependency sd = (SqlDependency)sender;
                sd.OnChange -= dependency_OnChange;//解除委派

                // 判斷是否在事件中
                DependencySetting ds;
                if (DependencyObject.TryGetValue(sd.Id, out ds))
                {
                    // 移除資料
                    DependencySetting tmpDs;
                    DependencyObject.TryRemove(sd.Id, out tmpDs);

                    if (e.Type == SqlNotificationType.Change)//查詢通知成功
                    {
                        DependencyDateChange(ds);
                    }
                    else//查詢通知失效
                    {
                        string msg = string.Format("DependencyRegister Error! \r\n CacheName: {0} ,\r\n SqlCommand:\r\n{1}",
                                                    ds.GameType + "_" + ds.Date.ToString("yyyyMMdd"), ds.SqlCommand);
                        AddMessage(msg, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                _logDependency.Error("dependency_OnChange Error! \r\n Message:\r\n{0}, \r\n StackTrace:\r\n{1}",
                    ex.Message, ex.StackTrace);
            }
        }

        private static void DependencyDateChange(DependencySetting ds)
        {
            string info = string.Format("{0}_{1}", ds.GameType, ds.Date.ToString("yyyyMMdd"));

            try
            {
                Thread.Sleep(1000);

                // 讀取資料
                DependencyToTable(ds.SqlCommand, ds.Date, ds.GameType);

                AddMessage(info);
            }
            catch (Exception ex)
            {
                string msg = string.Format("DependencyToTable Error! \r\n CacheName: {0} ,\r\n Message:\r\n{1}, \r\n StackTrace:\r\n{2}",
                                        ds.GameType, ex.Message, ex.StackTrace);
                AddMessage(msg, 2);
            }
        }

        public static void AddMessage(string msg, int writeLogState = 1)//增加介面訊息
        {
            if (writeLogState > 0)
            {
                if (writeLogState == 1)
                    _logDependency.Info(msg);
                else
                    _logDependency.Error(msg);
            }

            if (mainForm != null)
                mainForm.InvokeIfRequired(() => mainForm.AddInfo(msg));
        }

        public class DependencySetting
        {
            //public string ConnectionString { get; set; }
            public string SqlCommand { get; set; }
            public string GameType { get; set; }
            public DateTime Date { get; set; }
            public SqlDependency dep { get; set; }
        }

        /// <summary>
        /// 奥讯篮球获取修改过的时间数据
        /// ale add 2014/11/12
        /// </summary>
        /// <param name="sWid">wid</param>
        /// <returns>josn字符串</returns>
        public static string GetFBViewScoreData(string sWid)
        {
            string sDate = string.Empty;
            string sSql = " IF EXISTS(SELECT gid FROM dbo.FBViewScore 　With(NOLOCK) WHERE IsDeleted=0 AND WebID=@wid) " +
                          " BEGIN " +
                          "    SELECT Data FROM dbo.FBViewScore　With(NOLOCK)  WHERE IsDeleted=0 AND WebID=@wid  " +
                          " END  " +
                          " ELSE " +
                          "    SELECT 'N'";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = sSql;
                    cmd.Parameters.Add(new SqlParameter("@wid", sWid.Trim()));
                    try
                    {
                        cmd.Connection.Open();
                        sDate = cmd.ExecuteScalar().ToString();
                    }
                    catch (Exception exc)
                    {
                        AddMessage("GetFBViewScoreData Error!!" + exc.Message + "////", 2);
                    }
                    return sDate;
                }
            }

        }


        /// <summary>
        /// 奥讯篮球获取隊伍名稱
        /// ale add 2015/1/13
        /// </summary>
        /// <param name="sWid">隊伍id</param>
        /// <returns>josn字符串</returns>
        public static string[] GetOSBFTeamName(string sTidA, string sTidB, string type)
        {
            string[] sDate = { "", "" };
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    if (type.IndexOf("bkos") != -1)
                    {
                        cmd.CommandText = " select [ShowName] from [dbo].[OSTeam] where [TeamID] =@tidA " +
                                  " select [ShowName] from [dbo].[OSTeam] where [TeamID] =@tidB  ";
                    }
                    else
                    {
                        cmd.CommandText = " select [ShowName] from [dbo].[BFTeam] where [TeamID] =@tidA " +
                               " select [ShowName] from [dbo].[BFTeam] where [TeamID] =@tidB ";
                    }
                    cmd.Parameters.Add(new SqlParameter("@tidA", sTidA));
                    cmd.Parameters.Add(new SqlParameter("@tidB", sTidB));
                    try
                    {
                        cmd.Connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            sDate[0] = dr["ShowName"].ToString();
                        }
                        dr.NextResult();
                        if (dr.Read())
                        {
                            sDate[1] = dr["ShowName"].ToString();
                        }
                    }
                    catch (Exception exc)
                    {
                        AddMessage("GetOSBFTeamName Error!!" + exc.Message + "////", 2);
                    }
                    return sDate;
                }
            }

        }
    }
}
