using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Timers;
using System.Text;

namespace GameScoresApp
{
    public class BigBallRequest
    {
        private static Logger _logRequest = LogManager.GetLogger("Request_Log");
        private static Logger _logChange = LogManager.GetLogger("Change_Log");

        private static bool _IsEnableBigballRequest = false;

        public static void IsEnableBigballRequest()
        {
            bool state = false;
            try
            {
                string isEnable = ConfigurationManager.AppSettings["IsEnableBigballRequest"];
                if (string.IsNullOrEmpty(isEnable) || Boolean.Parse(isEnable))
                    //return true;
                    state = true;
            }
            catch { }
            //return false;
            _IsEnableBigballRequest = state;
        }

        public static void BigBallAnalysis(string gameType, DateTime date)
        {
            string cacheName = gameType + "_" + date.ToString("yyyyMMdd");
            if (!SqlDependencyCache.DependencyDataTable.ContainsKey(cacheName))
                return;

            DataTable dt = SqlDependencyCache.DependencyDataTable[cacheName];
            if (dt.Rows.Count == 0)
                return;

            if (_IsEnableBigballRequest)
            {
                try
                {
                    //if (gameType == "bkos" || gameType == "bkbf")
                    //{
                    //    //无用了
                    //    DataTable bkos = ProcessBkos(dt, gameType);
                    //    if(bkos != null && bkos.Rows.Count >0)
                    //        AnalysisDataChange(cacheName, ref bkos);//分析資料變動
                    //}
                    //else
                    AnalysisDataChange(cacheName, ref dt);//分析資料變動
                }
                catch (Exception ex)
                {
                    _logRequest.Error("CacheName: {0} \n Message: {1} \n StackTrace:\n\r{2}",
                        cacheName, ex.Message, ex.StackTrace);
                }
            }
        }
        //增加gameType参数。用来区别奥讯篮球和BF篮球 add by ha
        //舍弃版本
        private static DataTable ProcessBkos(DataTable dt, string gameType)
        {
            DataTable bkos = new DataTable();
            // 資料表
            bkos.Columns.Add(new DataColumn("GID"));
            bkos.Columns.Add(new DataColumn("GameType"));
            bkos.Columns.Add(new DataColumn("GameDate"));
            bkos.Columns.Add(new DataColumn("GameTime"));
            bkos.Columns.Add(new DataColumn("StatusText"));
            bkos.Columns.Add(new DataColumn("Record"));
            bkos.Columns.Add(new DataColumn("GameStates"));
            bkos.Columns.Add(new DataColumn("NA"));
            bkos.Columns.Add(new DataColumn("NB"));
            bkos.Columns.Add(new DataColumn("RunsA"));
            bkos.Columns.Add(new DataColumn("RunsB"));
            bkos.Columns.Add(new DataColumn("ChangeCount"));
            bkos.Columns.Add(new DataColumn("SourceType"));
            foreach (DataRow row in dt.Rows)
            {
                int changeCount = int.Parse(row["ChangeCount"].ToString());
                foreach (KeyValuePair<string, JToken> games in JObject.Parse(row["Info"].ToString()))
                {
                    foreach (KeyValuePair<string, JToken> game in (games.Value as JObject))
                    {
                        JObject data = game.Value as JObject;

                        //BK資料欄位
                        //[GID],[GameType],[Number],[GameStates],[GameDate],[GameTime],[TeamAID],[TeamBID],[RunsA],[RunsB],[StatusText],[TrackerText],[ChangeCount]
                        DataRow dr = bkos.NewRow();
                        dr["GID"] = game.Key;
                        dr["GameType"] = games.Key;
                        dr["NA"] = data["TeamA"];
                        dr["NB"] = data["TeamB"];
                        dr["GameDate"] = data["GameDate"];
                        dr["GameTime"] = TimeSpan.Parse(data["GameTime"].ToString());
                        dr["GameStates"] = data["GameStates"];
                        dr["StatusText"] = data["StatusText"];
                        dr["Record"] = data["Record"];//該場比賽是否為待定 沒有比賽過程
                        dr["RunsA"] = data["RunsA"];
                        dr["RunsB"] = data["RunsB"];
                        dr["ChangeCount"] = changeCount;
                        dr["SourceType"] = gameType;
                        //替换手动改过的时间 ，目前只替换时间，其他部分后续有用再加
                        string FBViewScoreData = SqlDependencyCache.GetFBViewScoreData(game.Key);
                        if (FBViewScoreData != string.Empty && FBViewScoreData != "N" && FBViewScoreData.IndexOf("GameTime") != -1)
                        {
                            foreach (KeyValuePair<string, JToken> item in JObject.Parse(FBViewScoreData))
                            {
                                if (item.Key == "GameTime")
                                {
                                    dr["GameTime"] = TimeSpan.Parse(item.Value.ToString());
                                    break;
                                }
                            }
                        }

                        bkos.Rows.Add(dr);
                    }
                }
            }

            return bkos;
        }

        public static Dictionary<string, JObject> ChangeCache = new Dictionary<string, JObject>();
        public static void ClearChangeCache()//清除逾期的緩存
        {
            var clearDate = DateTime.Now.Date.AddDays(-2);//前天
            var removeArray = new List<string>();

            foreach (var data in ChangeCache)
            {
                try
                {
                    var tmp = data.Key.ToString().Split('_')[1];
                    var sDate = string.Format("{0}-{1}-{2}", tmp.Substring(0, 4), tmp.Substring(4, 2), tmp.Substring(6, 2));
                    DateTime date;
                    if (DateTime.TryParse(sDate, out date) && date <= clearDate)//資料日期比對
                        removeArray.Add(data.Key);
                }
                catch { }
            }

            //資料移除
            foreach (var key in removeArray)
                ChangeCache.Remove(key);
        }

        private static void AnalysisDataChange(string cacheName, ref DataTable dt)
        {
            bool isTn = false;
            cacheName = cacheName.ToLower();

            if (cacheName.IndexOf("sb") > -1)//若為sb
                return;
            else if (cacheName.IndexOf("tn") > -1)
                isTn = true;

            if (!ChangeCache.ContainsKey(cacheName))//暫存檢查
                ChangeCache.Add(cacheName, new JObject());//不存在則建立

            DateTime GameDate;
            int changeCount = 0;
            foreach (DataRow row in dt.Rows)
            {
                string GID = row["GID"].ToString();

                if (row["ChangeCount"] != null && Int32.TryParse(row["ChangeCount"].ToString(), out changeCount))
                {
                    if (!DateTime.TryParse(row["GameDate"].ToString(), out GameDate)) //取得日期
                        continue; //取不到資料

                    if (ChangeCache[cacheName][GID] == null)//緩存不存在 需要建立
                    {
                        if (isTn == false && row["GameStates"] != null)//非網球類比賽，有開賽狀態
                            if (row["GameStates"].ToString() == "E")//緩存初始化不處已完賽資料，避免程式重啟，又重覆送出已結束的比分
                                continue;

                        JObject tmp = new JObject(new JProperty("changeCount", 0));//DB改變次數
                        if (isTn == false)//非網球
                        {
                            tmp.Add("status", new JObject());      //比賽狀態 所有狀態紀錄
                            if (row["GameType"].ToString().ToLower().IndexOf("af") == -1)//除了美足 要計算尾分
                            {
                                tmp.Add("qwfRuns", "");            //尾分狀態紀錄
                                tmp.Add("curRuns", "");            //當前紀錄
                                tmp.Add("curGamePlay", "");        //當前正在打第幾節(局)紀錄  棒球用
                            }
                        }

                        ChangeCache[cacheName][GID] = tmp;
                    }

                    if (changeCount > (int)ChangeCache[cacheName][GID]["changeCount"])//資料有異動
                    {
                        //更新變動次數
                        ChangeCache[cacheName][GID]["changeCount"] = changeCount;
                        _logChange.Info(string.Format("cacheName = {0} , GID = {1}, ChangeCount = {2}", cacheName, GID, changeCount));

                        #region 設定非tn 狀態/比分/名稱
                        if (isTn == false)
                        {
                            WebRequestData wData = null;
                            if (row["GameType"].ToString().ToLower() == "bkos" || row["GameType"].ToString().ToLower() == "bkbf")//判断是奥讯篮球和BF篮球
                            {
                                //序列化格式
                                wData = new WebRequestData(
                                   row["GameType"].ToString().ToLower(),   //比賽類型
                                   row["GameStates"].ToString().Trim(),    //比賽狀態
                                   GID, row["AllianceID"].ToString());
                            }
                            else
                            {
                                //序列化格式
                                wData = new WebRequestData(
                                   row["GameType"].ToString().ToLower(),   //比賽類型
                                   row["GameStates"].ToString().Trim(),    //比賽狀態
                                   GID);
                            }

                            if (wData.iGameType == 0)
                                continue; //取不到資料

                            if (wData.gameType.IndexOf("bkos") != -1 || wData.gameType.IndexOf("bkbf") != -1)//奧迅籃球 BF篮球
                            {
                                //先通過id查找 showname
                                string[] name = SqlDependencyCache.GetOSBFTeamName(row["TeamAID"].ToString(), row["TeamBID"].ToString(), wData.gameType);

                                wData.NA = ReplaceTeamName(name[0], false);
                                wData.NB = ReplaceTeamName(name[1], false);
                            }
                            else
                            {
                                string[] name = { row["TeamAID"].ToString(), row["TeamBID"].ToString() };
                                if (!string.IsNullOrEmpty(wData.GameTeam))
                                    GetTeamName(ref wData, name);//透過TeamID取得名稱
                                else
                                    continue;//沒有取到資料 
                            }

                            try
                            {
                                var StatusText = row["StatusText"].ToString().Trim();
                                var RunsA = row["RunsA"].ToString();
                                var RunsB = row["RunsB"].ToString();

                                //寫入變更資料
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(string.Format("NA: {0}", wData.NA));
                                sb.AppendLine(string.Format("NB: {0}", wData.NB));
                                sb.AppendLine(string.Format("gameStatus: {0}", wData.gameStatus));

                                if (string.IsNullOrEmpty(StatusText) == false)
                                    sb.AppendLine(string.Format("StatusText: {0}", StatusText));

                                if (string.IsNullOrEmpty(RunsA) == false)
                                    sb.AppendLine(string.Format("RunsA: {0}", RunsA));

                                if (string.IsNullOrEmpty(RunsB) == false)
                                    sb.AppendLine(string.Format("RunsB: {0}", RunsB));

                                if (wData.gameType == "bkos" || wData.gameType == "bkbf")
                                    UpdateLog.Info(wData.gameType, wData.GID, wData.alliance, sb.ToString());
                                else
                                    UpdateLog.Info(wData.gameType, wData.GID, sb.ToString());

                                //X:未開賽 S:開賽 E:結束 C:取消
                                if (wData.gameStatus != "" &&
                                    wData.gameStatus != "X" &&
                                    wData.gameStatus != "C")
                                {
                                    string Record = null;//該場比賽是否為待定 沒有比賽過程
                                    if (row.Table.Columns.Contains("Record") && row["Record"] != null)
                                        Record = row["Record"].ToString().Trim();

                                    //設定比分 比賽狀態
                                    string[] tmpRuns = { RunsA, RunsB };
                                    if (GetRuns(tmpRuns, null, ref wData, cacheName, StatusText, Record) == 0)
                                        continue;//存取非必要
                                }
                                else
                                    continue;//不需要處理的資料

                                if (wData.gameType == "bkos" || wData.gameType == "bkbf")
                                    RunsLog.Info(wData.gameType, wData.GID, wData.alliance, sb.ToString());
                                else
                                    RunsLog.Info(wData.gameType, wData.GID, sb.ToString());

                                //設定開賽時間
                                wData.gameDate = GameDate.ToString("yyyy-MM-dd") + " " + row["GameTime"].ToString();

                                //資料處理 並送出 webRequest
                                DataProcessing(cacheName, ref wData);
                            }
                            catch (Exception ex)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("DataProcessing Error!!");
                                sb.AppendLine(string.Format("GID = {0} ,gameType = {1}", wData.GID, wData.gameType));
                                sb.AppendLine("Message: " + ex.Message);
                                sb.AppendLine("StackTrace:");
                                sb.AppendLine(ex.StackTrace);

                                _logRequest.Error(sb.ToString());
                            }
                            continue;
                        }
                        #endregion

                        #region 設定tn 狀態/比分/名稱
                        else if (isTn)
                        {

                            //序列化格式
                            WebRequestData wData = new WebRequestData(
                                "tn",                                   //比賽類型
                                row["GameStates"].ToString().ToLower().Trim(),  //比賽狀態
                                row["WebID"].ToString());

                            if (wData.iGameType == 0)
                                continue; //取不到資料

                            if (ChangeCache[cacheName][wData.GID] == null)
                            {
                                if (wData.gameStatus.IndexOf("結束") != -1)//該比賽已結束 且是舊資料故不處理
                                    continue;

                                ChangeCache[cacheName][wData.GID] = new JObject(new JProperty("status", new JObject()));
                            }

                            //設定名稱
                            string[] name = { row["TeamAID"].ToString(), row["TeamBID"].ToString() };
                            if (!string.IsNullOrEmpty(wData.GameTeam))
                                GetTeamName(ref wData, name);//透過TeamID取得名稱
                            else
                                continue;//沒有取到資料 

                            try
                            {
                                var RunsA = row["RunsA"].ToString();
                                var RunsB = row["RunsB"].ToString();
                                var OA = row["RA"].ToString();
                                var OB = row["RB"].ToString();

                                //寫入變更資料
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(string.Format("NA: {0}", wData.NA));
                                sb.AppendLine(string.Format("NB: {0}", wData.NB));
                                sb.AppendLine(string.Format("gameStatus: {0}", wData.gameStatus));

                                if (string.IsNullOrEmpty(RunsA.Replace(",", "")) == false)
                                    sb.AppendLine(string.Format("RA: {0}", RunsA));

                                if (string.IsNullOrEmpty(RunsB.Replace(",", "")) == false)
                                    sb.AppendLine(string.Format("RB: {0}", RunsB));

                                if (string.IsNullOrEmpty(OA.Replace("-", "")) == false)
                                    sb.AppendLine(string.Format("OA: {0}", OA));

                                if (string.IsNullOrEmpty(OB.Replace("-", "")) == false)
                                    sb.AppendLine(string.Format("OB: {0}", OB));

                                UpdateLog.Info(wData.gameType, wData.GID, sb.ToString());

                                //設定比分 比賽狀態
                                string[] tmpRuns = { RunsA, RunsB };
                                string[] tmpR = { OA, OB };//盤數
                                if (GetRuns(tmpRuns, tmpR, ref wData, cacheName, wData.gameStatus) == 0)
                                    continue;//存取非必要

                                RunsLog.Info(wData.gameType, wData.GID, sb.ToString());

                                //設定開賽時間
                                wData.gameDate = GameDate.ToString("yyyy-MM-dd");

                                //資料處理 並送出 webRequest
                                DataProcessing(cacheName, ref wData);
                            }
                            catch (Exception ex)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("DataProcessing Error!!");
                                sb.AppendLine(string.Format("GID = {0} ,gameType = {1}", wData.GID, wData.gameType));
                                sb.AppendLine("Message: " + ex.Message);
                                sb.AppendLine("StackTrace:");
                                sb.AppendLine(ex.StackTrace);

                                _logRequest.Error(sb.ToString());
                            }
                        #endregion
                        }
                    }
                }

            }
        }

        public static string ReplaceTeamName(string name, bool isTn)
        {
            int findIndex;
            if (isTn == false)
            {
                name = name.Trim();
                findIndex = name.IndexOf("-");
                if (findIndex != -1)
                {
                    findIndex++;
                    name = name.Substring(findIndex, name.Length - findIndex);
                }
            }

            name = name.Trim();
            findIndex = name.IndexOf("(");
            if (findIndex != -1)
                name = name.Substring(0, findIndex);

            return name.Trim();
        }

        static void DataProcessing(string cache, ref WebRequestData wData)
        {
            int sendCount = 0;//計算封包處理次數，並增加送出間隔
            //首分
            if (wData.gameStatusQsf)
            {
                sendCount++;
                ProcessRequest.SendRequest(cache, ref wData, wData.RunsQsf, "qsf", sendCount);
            }

            //單節(局)
            if (wData.Runs != null)
            {
                string status = null;
                for (int i = 0; i < wData.Runs.Length; i++)
                {
                    if (wData.Runs[i] != null)
                    {
                        status = GetGameStatus(ref wData, i + 1);
                        if (status != "ot" || status == "ot" && wData.gameStatusR)//OT比分須比賽結束
                        {
                            sendCount++;
                            ProcessRequest.SendRequest(cache, ref wData, wData.Runs[i], status, sendCount);
                        }
                    }
                }
            }

            //上下半場
            if (wData.RunsHalf != null)
            {
                for (int i = 0; i < wData.RunsHalf.Length; i++)
                {
                    if (wData.RunsHalf[i] != null)
                    {
                        sendCount++;
                        ProcessRequest.SendRequest(cache, ref wData, wData.RunsHalf[i], (i == 0) ? "up" : "down", sendCount);
                    }
                }
            }

            //冰球不包括 首分
            if (wData.gameStatusQsf2)
            {
                sendCount++;
                ProcessRequest.SendRequest(cache, ref wData, wData.RunsQsf2, "qsfn", sendCount);
            }

            //冰球不包括 尾分
            if (wData.gameStatusQwf2)
            {
                sendCount++;
                ProcessRequest.SendRequest(cache, ref wData, wData.RunsQwf, "qwfn", sendCount);
            }

            //全場
            if (wData.gameStatusR)
            {
                sendCount++;
                ProcessRequest.SendRequest(cache, ref wData, wData.RunsQwf, "qwf", sendCount);

                sendCount++;
                ProcessRequest.SendRequest(cache, ref wData, wData.RunsR, "R", sendCount);
            }
        }



        private static void GetTeamName(ref WebRequestData wData, string[] name)//取得隊伍名稱
        {
            if (SqlDependencyCache.DependencyTeamData.ContainsKey(wData.GameTeam))
            {
                DataTable dtTeam = SqlDependencyCache.DependencyTeamData[wData.GameTeam];
                if (dtTeam != null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        try
                        {

                            DataRow dtTeamRow = dtTeam.Rows.Find(name[i]);
                            if (dtTeamRow != null)
                            {
                                if (wData.GameTeam != "TennisTeam")
                                {
                                    name[i] = dtTeamRow["ShowName"].ToString();
                                }
                                else
                                {
                                    name[i] = dtTeamRow["ChangeText"].ToString();
                                }
                            }
                        }
                        catch
                        {
                            if (wData.GameTeam != "TennisTeam")
                                return;
                        }
                    }

                    bool isTn = (wData.GameTeam == "TennisTeam");
                    wData.NA = ReplaceTeamName(name[0], isTn);
                    wData.NB = ReplaceTeamName(name[1], isTn);
                }
            }
        }

        private static int GetRuns(string[] Runs, string[] RunsR, ref WebRequestData wData, string cache, string StatusText = null, string Record = null)//計算分數
        {
            //GameType=1  美棒    //GameType=2  日棒            //GameType=3  台棒    //GameType=4  韩棒
            //GameType=5  冰球    //GameType=6  篮球(NBA,WNBA)  //GameType=7  彩球    //GameType=8  美足
            //GameType=9  网球    //GameType=13 篮球(其他)      //GameType=14 棒球(其他)

            List<string[]> runsTxt = new List<string[]>();
            runsTxt.Add(Runs[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            runsTxt.Add(Runs[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

            int runsCount = 0;
            int tryParseInt;
            for (int i = 0; i < runsTxt.Count; i++)
            {
                if (wData.iGameType == 9 && Runs[i].LastIndexOf(",") != Runs[i].Length - 1)//網球 若有該盤分數 ex:6,6,6,,,15 則必須去尾數
                {
                    string[] tmp = new string[runsTxt[i].Length - 1];
                    Array.Copy(runsTxt[i], tmp, runsTxt[i].Length - 1);
                    runsTxt[i] = tmp;
                }

                for (int j = 0; j < runsTxt[i].Length; j++)
                {
                    int findIndex = runsTxt[i][j].IndexOf("<sup>");
                    if (findIndex > -1)//分數修正去除<sup> </sup>
                        runsTxt[i][j] = runsTxt[i][j].Substring(0, findIndex);

                    if (runsTxt[i][j].Length > 1)
                    {
                        findIndex = runsTxt[i][j].IndexOf("X");
                        if (findIndex > -1)//完賽 棒球會有此情況
                            runsTxt[i][j] = runsTxt[i][j].Substring(0, findIndex);
                    }
                    else if (runsTxt[i][j].ToLower() == "x")
                        runsTxt[i][j] = "0";

                    if (runsTxt[i][j] != null && Int32.TryParse(runsTxt[i][j], out tryParseInt))//合法分數檢查
                        runsCount++;
                }
            }
            if (runsCount == 0)
                return 0;

            bool isBk = (wData.gameType.IndexOf("bk") == 0);//籃球判斷
            bool isBB = (wData.gameType.IndexOf("bb") == 0);//棒球判斷
            int processCount = 0;//分數操作次數

            if (isBB || isBk)//棒球 or 籃球
            {
                if (Record != "只顯示最終比分")
                {
                    processCount += ProcessQsfRuns(runsCount, ref wData, cache, runsTxt);//首分處理

                    if (isBk)
                        ProcessQwfRuns(ref wData, cache, runsTxt);//尾分處理
                }

                int offset = runsTxt[0].Length - 1;
                if (CheckState(StatusText, ref wData, runsTxt))//該節(局)結束 則也傳出該局比分
                    offset++;

                //上半場 处理奥讯ncaa联盟 add by le
                if ((isBB && offset >= 5) ||
                    (isBk && offset >= 2 && (wData.gameType != "bkncaa" && SqlDependencyCache.GetAllianceName(wData.gameType, wData.alliance).IndexOf("ncaa") == -1)) ||
                    (isBk && offset >= 1 && (wData.gameType == "bkncaa" || SqlDependencyCache.GetAllianceName(wData.gameType, wData.alliance).IndexOf("ncaa") != -1)))
                    processCount += ProcessHalfRuns("up", ref wData, cache, runsTxt);

                if (wData.gameType != "bkncaa" && SqlDependencyCache.GetAllianceName(wData.gameType, wData.alliance).IndexOf("ncaa") == -1)//NCAA 不處理單節比分
                {
                    for (int i = 0; i < offset; i++)
                        processCount += ProcessRuns(i, ref wData, cache, runsTxt);//各節(局)比分 
                }

                if (wData.gameStatus == "E")
                {
                    if (isBk)//處理下半場結算
                        processCount += ProcessHalfRuns("down", ref wData, cache, runsTxt);

                    if (isBB)//棒球尾分判斷
                        ProcessQwfRunsByBB(ref wData, cache, runsTxt);//尾分處理

                    processCount += ProcessR_Runs(ref wData, cache, runsTxt);//全場
                }
            }
            else if (wData.iGameType == 8)//美足
            {
                if (wData.gameStatus == "S" && runsCount > 4)//未完局送上半場結果
                {
                    processCount += ProcessHalfRuns("up", ref wData, cache, runsTxt);//上半場 1+2節
                }
                else if (wData.gameStatus == "E")
                {
                    processCount += ProcessHalfRuns("down", ref wData, cache, runsTxt);//下半場 3+4+ot
                }
            }
            else if (wData.iGameType == 5)//冰球
            {

                if (Record != "只顯示最終比分")
                {
                    processCount += ProcessQsfRuns(runsCount, ref wData, cache, runsTxt);//首分處理
                }

                int offset = runsTxt[0].Length - 1;
                if (StatusText.Trim() == "0" || StatusText == "結束" || wData.gameStatus == "E")//該節(局)結束 則也傳出該局比分
                {
                    offset++;
                }

                //第三节结束之后 没有（不含加时）抢首分  为 -1:-1
                if (offset == 3 && !wData.gameStatusQsf2 && ChangeCache[cache][wData.GID]["status"]["qsf2"] == null)
                {
                    wData.RunsQsf2 = new string[] { "-1", "-1" };
                    wData.gameStatusQsf2 = true;
                    (ChangeCache[cache][wData.GID]["status"] as JObject).Add("qsf2", "");
                    processCount++;
                }

                for (int i = 0; i < offset; i++)
                {
                    processCount += ProcessRuns(i, ref wData, cache, runsTxt);//各節(局)比分 
                }

                ProcessQwfRuns(ref wData, cache, runsTxt);//尾分處理
                //第三节结束 准备送（不含加时）抢尾
                if (offset == 3 && ChangeCache[cache][wData.GID]["status"]["qwf2"] == null)
                {
                    wData.gameStatusQwf2 = true;
                    (ChangeCache[cache][wData.GID]["status"] as JObject).Add("qwf2", "");
                    processCount++;
                }

                //全场
                if (wData.gameStatus == "E")
                {
                    processCount += ProcessR_Runs(ref wData, cache, runsTxt);
                }
            }
            else if (wData.iGameType == 9)//網球
            {
                int offset = runsTxt[0].Length - 1;
                if (CheckState(StatusText, ref wData, runsTxt))//該節(局)結束 則也傳出該局比分
                    offset++;
                for (int i = 0; i < offset; i++)
                    processCount += ProcessRuns(i, ref wData, cache, runsTxt);//各節(局)比分                 

                if (wData.gameStatus == "結束")//正常結束
                {
                    //R = 第一盘|第二盘|第三盘|第四盘|第五盘|盘数|加总
                    for (int i = 0; i < 2; i++)
                    {
                        int sum = 0, tryIntParse;
                        wData.RunsR[i] = "";

                        for (int j = 0; j < 5; j++)//1-5盤
                        {
                            if (runsTxt[i].Length > j && int.TryParse(runsTxt[i][j], out tryIntParse))
                            {
                                sum += tryIntParse;
                                wData.RunsR[i] += (runsTxt[i][j] + "|");
                            }
                            else
                                wData.RunsR[i] += ("0|");
                        }

                        //盤數
                        wData.RunsR[i] += (RunsR[i] + "|");

                        //局數
                        wData.RunsR[i] += sum;
                    }

                    wData.gameStatusR = true;
                    if (ChangeCache[cache][wData.GID]["status"]["R"] == null)
                        (ChangeCache[cache][wData.GID]["status"] as JObject).Add("R", "");

                    processCount++;
                }
            }

            return processCount;
        }

        static bool CheckState(string StatusText, ref WebRequestData wData, List<string[]> runsTxt = null)
        {
            //冰球中场休息不送分
            if ((StatusText == "結束" || (wData.iGameType != 5 && StatusText == "中場休息") || wData.gameStatus == "E"))
            {
                if (wData.gameType.IndexOf("bk") == 0 && wData.gameStatus != "E")//籃球判斷
                {
                    int tryParseIntA, tryParseIntB;
                    int length = runsTxt[0].Length;
                    if (length > runsTxt[0].Length || !Int32.TryParse(runsTxt[0][length - 1], out tryParseIntA))
                        tryParseIntA = 0;

                    if (length > runsTxt[1].Length || !Int32.TryParse(runsTxt[1][length - 1], out tryParseIntB))
                        tryParseIntB = 0;

                    //OT比完 比分不同  視為比賽結束
                    if (GetGameStatus(ref wData, length) == "ot" && tryParseIntA != tryParseIntB)
                        wData.gameStatus = "E";
                    else if (tryParseIntA == 0 && tryParseIntB == 0)//籃球比賽未結束 不會發生比分 0:0
                        return false;
                }
                return true;
            }
            return false;
        }

        private static int ProcessQsfRuns(int runsCount, ref WebRequestData wData, string cache, List<string[]> runsTxt)
        {
            int returnInt = 0;
            if (runsCount >= 1 && ChangeCache[cache][wData.GID]["status"]["qsf"] == null)
            {
                int tryParseIntA, tryParseIntB;
                //取最高的節數
                int offset = runsTxt[0].Length > runsTxt[1].Length ? runsTxt[0].Length : runsTxt[1].Length;
                for (int i = 0; i < offset; i++)
                {
                    if (runsTxt[0].Length <= i || !Int32.TryParse(runsTxt[0][i], out tryParseIntA))
                        tryParseIntA = 0;

                    if (runsTxt[1].Length <= i || !Int32.TryParse(runsTxt[1][i], out tryParseIntB))
                        tryParseIntB = 0;

                    if (tryParseIntA != tryParseIntB)//不同分才傳
                    {
                        if (tryParseIntA > tryParseIntB)
                            wData.RunsQsf = new string[] { "1", "0" };
                        else
                            wData.RunsQsf = new string[] { "0", "1" };

                        wData.gameStatusQsf = true;
                        (ChangeCache[cache][wData.GID]["status"] as JObject).Add("qsf", "");
                        returnInt = 1;
                        break;
                    }
                }
            }
            //冰球的第二种模式 不包含加时赛 的抢首
            if (wData.gameType.ToLower().IndexOf("ih") != -1 && runsCount >= 1 && ChangeCache[cache][wData.GID]["status"]["qsf2"] == null)
            {
                int tryParseIntA, tryParseIntB;
                for (int i = 0; i < 3; i++)
                {
                    if (runsTxt[0].Length <= i || !Int32.TryParse(runsTxt[0][i], out tryParseIntA))
                        tryParseIntA = 0;

                    if (runsTxt[1].Length <= i || !Int32.TryParse(runsTxt[1][i], out tryParseIntB))
                        tryParseIntB = 0;

                    if (tryParseIntA != tryParseIntB)//不同分才傳
                    {
                        wData.RunsQsf2 = tryParseIntA > tryParseIntB ? new string[] { "1", "0" } : new string[] { "0", "1" };

                        wData.gameStatusQsf2 = true;
                        (ChangeCache[cache][wData.GID]["status"] as JObject).Add("qsf2", "");
                        returnInt = 1;
                        break;
                    }
                }
            }

            return returnInt;
        }

        //取得指定的比賽狀態
        private static string GetGameStatus(ref WebRequestData wData, int offset)
        {
            bool isBB = (wData.gameType.IndexOf("bb") == 0);
            bool isBK = (wData.gameType.IndexOf("bk") == 0);
            bool isIH = (wData.gameType.IndexOf("ih") == 0);
            if ((isBB && offset > 9) ||
                (isBK && offset > 4) ||
                (isIH && offset > 3) ||
                ((wData.gameType == "bkncaa" || SqlDependencyCache.GetAllianceName(wData.gameType, wData.alliance).IndexOf("ncaa") != -1) && offset > 2))
                return "ot";

            return offset.ToString();
        }

        private static int ProcessRuns(int offset, ref WebRequestData wData, string cache, List<string[]> runsTxt)
        {
            if (offset >= wData.Runs.Length)
                return 0;

            if (wData.Runs[offset] == null)
                wData.Runs[offset] = new string[2];

            int tryIntParse = 0;
            if (GetGameStatus(ref wData, offset + 1) == "ot")
            {
                bool isIH = (wData.gameType.IndexOf("ih") >= 0);

                int begin = 0, end = 0, sum = 0, so1 = 0, so2 = 0;

                if (wData.gameType.IndexOf("bb") == 0)
                    begin = 9;
                else if (wData.gameType.IndexOf("bk") == 0)
                    begin = 4;
                else if (isIH)
                    begin = 3;

                for (int i = 0; i < runsTxt.Count; i++)
                {
                    sum = 0;
                    end = runsTxt[i].Length;
                    if (isIH && end > 4)//冰球先只计算OT，SO后面算 
                    {
                        end = 4;
                    }
                    for (int j = begin; j < end; j++)
                    {
                        if (end > j && int.TryParse(runsTxt[i][j], out tryIntParse))
                            sum += tryIntParse;
                    }

                    //计算冰球SO
                    if (isIH && runsTxt[0].Length == 5)
                    {
                        so1 = 0; so2 = 0;
                        Int32.TryParse(runsTxt[0][4], out so1);
                        Int32.TryParse(runsTxt[1][4], out so2);
                        if (so1 > so2 && i == 0)
                            sum++;
                        else if (so1 < so2 && i == 1)
                            sum++;

                    }
                    wData.Runs[offset][i] = sum.ToString();
                }

            }
            else
            {
                for (int i = 0; i < runsTxt.Count; i++)
                {
                    if (runsTxt[i].Length > offset && int.TryParse(runsTxt[i][offset], out tryIntParse))
                        wData.Runs[offset][i] = runsTxt[i][offset];//資料合法
                    else
                        wData.Runs[offset][i] = "0";
                }
            }

            return 1;
        }

        private static int ProcessHalfRuns(string HalfStatus, ref WebRequestData wData, string cache, List<string[]> runsTxt)
        {
            if (HalfStatus == "up" || HalfStatus == "down")
            {
                bool isBB = (wData.gameType.IndexOf("bb") == 0);
                bool isBK = (wData.gameType.IndexOf("bk") == 0);
                bool isAF = (wData.gameType.IndexOf("af") == 0);
                bool isTN = (wData.gameType.IndexOf("tn") == 0);
                int tryParseInt = 0;
                int offset = 0;

                for (int i = 0; i < 2; i++)
                {
                    int begin = 0;
                    int end = 0;
                    if (HalfStatus == "up")
                    {
                        if (isBB)
                            end = 5;
                        else if (isTN || wData.gameType == "bkncaa" || SqlDependencyCache.GetAllianceName(wData.gameType, wData.alliance).IndexOf("ncaa") != -1)//NCAA 上半=1
                            end = 1;
                        else if (isAF || isBK)
                            end = 2;
                    }
                    else if (HalfStatus == "down")
                    {
                        offset = 1;
                        if (wData.gameType == "bkncaa" || SqlDependencyCache.GetAllianceName(wData.gameType, wData.alliance).IndexOf("ncaa") != -1)//NCAA 下半=2,ot
                            begin = 1;
                        else
                            begin = 2;
                        end = runsTxt[i].Length;
                    }

                    int sum = 0;
                    for (int j = begin; j < end; j++)
                    {
                        if (runsTxt[i].Length <= j || !Int32.TryParse(runsTxt[i][j], out tryParseInt))
                            tryParseInt = 0;

                        sum += tryParseInt;
                    }

                    if (wData.RunsHalf[offset] == null)
                        wData.RunsHalf[offset] = new string[2];

                    wData.RunsHalf[offset][i] = sum.ToString();
                }

                return 1;
            }
            return 0;
        }

        //棒球尾分檢查
        private static void ProcessQwfRunsByBB(ref WebRequestData wData, string cache, List<string[]> runsTxt)
        {
            int tmp;
            List<int[]> runs = new List<int[]>();//當前比分
            runs.Add(new int[runsTxt[0].Length]);
            runs.Add(new int[runsTxt[0].Length]);

            for (int i = 0; i < runsTxt.Count; i++)
            {
                for (int j = 0; j < runsTxt[0].Length; j++)
                {
                    //只有当最后一局的时候判断
                    if ((j == runsTxt[0].Length - 1 && runsTxt[i].Length < runsTxt[0].Length) || runsTxt[i][j] == null || !int.TryParse(runsTxt[i][j], out tmp))
                        runs[i][j] = 0;
                    else
                        runs[i][j] = tmp;
                }
            }

            string qwf = "0:0";
            for (int inning = runs[0].Length - 1; inning >= 0; inning--)//從最後一局開始檢查尾分
            {
                if (runs[1][inning] != 0)//下
                    qwf = "0:1";
                else if (runs[0][inning] != 0)//上
                    qwf = "1:0";

                if (qwf != "0:0")//有尾分
                    break;
            }

            wData.RunsQwf = qwf.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void ProcessQwfRuns(ref WebRequestData wData, string cache, List<string[]> runsTxt)
        {
            //取最高的節數
            int offset = runsTxt[0].Length > runsTxt[1].Length ? runsTxt[0].Length : runsTxt[1].Length;
            int[] newRuns = new int[2];//當前比分

            for (int i = 0; i < runsTxt.Count; i++)
            {
                if (runsTxt[i].Length < offset || !int.TryParse(runsTxt[i][offset - 1], out newRuns[i]))
                    newRuns[i] = 0;
            }

            string qwfRuns = null;
            //if (wData.gameType.IndexOf("ih") == 0 && offset == 5)//冰球SO 採獨立計算
            //{
            //    if (newRuns[0] > newRuns[1])//射門數較高隊伍得分                  
            //        qwfRuns = "1:0";
            //    else
            //        qwfRuns = "0:1";
            //}
            //else
            //{
            ////緩存比分記錄
            //if(wData.gameType.IndexOf("bb") == 0)//棒球
            //{
            //    string curGamePlay = ChangeCache[cache][wData.GID]["curGamePlay"].ToString();
            //    if(offset.ToString() != curGamePlay)
            //    {
            //        ChangeCache[cache][wData.GID]["curGamePlay"] = offset.ToString();
            //        ChangeCache[cache][wData.GID]["curRuns"] = "0:0";
            //    }
            //}

            string[] compareTxt = (ChangeCache[cache][wData.GID]["curRuns"].ToString() == "" ? new string[] { "0", "0" } : ChangeCache[cache][wData.GID]["curRuns"].ToString().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries));
            int[] oldRuns = new int[2];

            for (int i = 0; i < compareTxt.Length; i++)
            {
                if (!int.TryParse(compareTxt[i], out oldRuns[i]))
                    oldRuns[i] = 0;

                if (newRuns[i] > oldRuns[i])//分數有變動
                {
                    if (i == 0)
                        qwfRuns = "1:0";
                    else
                        qwfRuns = "0:1";
                }
            }
            ChangeCache[cache][wData.GID]["curRuns"] = string.Format("{0}:{1}", newRuns[0], newRuns[1]);//寫入比分記錄
            //}

            if (string.IsNullOrEmpty(qwfRuns))//檢查目前尾分狀態
            {
                string oldQwfRuns = ChangeCache[cache][wData.GID]["qwfRuns"].ToString();
                if (string.IsNullOrEmpty(oldQwfRuns))
                    qwfRuns = wData.gameType.IndexOf("ih") == 0 ? "-1:-1" : "0:0"; //冰球没有抢尾时 使用-1：-1
                else
                    qwfRuns = ChangeCache[cache][wData.GID]["qwfRuns"].ToString();
            }
            else
                ChangeCache[cache][wData.GID]["qwfRuns"] = qwfRuns;

            wData.RunsQwf = qwfRuns.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static int ProcessR_Runs(ref WebRequestData wData, string cache, List<string[]> runsTxt)
        {

            //完局結算
            int numValue = 0;
            bool isIH = (wData.gameType.IndexOf("ih") >= 0);
            for (int i = 0; i < runsTxt.Count; i++)
            {
                int sum = 0;
                for (int j = 0; j < runsTxt[i].Length; j++)
                {
                    if (!isIH || //不是冰球類
                        (isIH && j < 4)  //冰球NHL不計算SO计算OT  ///更新规则 2015.4.20 全部冰球类计算加时赛OT
                       )    //其它冰球 只計算到第三節)  (isIH && wData.gameType != "ihus" && j < 3)
                    {
                        if (runsTxt[i].Length > j && Int32.TryParse(runsTxt[i][j], out numValue))
                            sum += numValue;
                    }
                }

                if (isIH && runsTxt[0].Length == 5)//冰球NHL計算SO  ///更新规则 2015.4.20 全部冰球类计算SO
                {
                    int so1 = 0, so2 = 0;
                    Int32.TryParse(runsTxt[0][4], out so1);
                    Int32.TryParse(runsTxt[1][4], out so2);
                    if (so1 > so2 && i == 0)
                        sum++;
                    else if (so1 < so2 && i == 1)
                        sum++;
                }

                wData.RunsR[i] = sum.ToString();
            }


            wData.gameStatusR = true;

            if (ChangeCache[cache][wData.GID]["status"]["R"] == null)
                (ChangeCache[cache][wData.GID]["status"] as JObject).Add("R", "");

            return 1;

            //return 0;
        }
    }
}
