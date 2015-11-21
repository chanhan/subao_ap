using HtmlAgilityPack;
using Schedules.SourceModel.Baseball;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Windows.Forms;
using System.Linq;

namespace Schedules
{
    public partial class FrmMain
    {
        #region 美棒 MLB - CBS

        private Dictionary<string, GameInfo> GetSchedulesByMLB(int allianceID, string gameType, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webMLB.Document == null ||
                this.webMLB.Document.Body == null)
            {
                return null;
            }

            HtmlWindow calendar = null;
            try
            {
                calendar = this.webMLB.Document.Window.Frames["arenaCalendar"];
            }
            catch { }

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();

            if (calendar != null)
            {
                return GetScheduleOnRegular(allianceID, gameType, acH, calendar, schedules);
            }
            else
            {
                string url = this.webMLB.Url.ToString().ToLower();
                if (url.Contains("playoffs"))
                {
                    return GetScheduleOnPlayOffs(allianceID, gameType, acH, schedules);
                }
                else
                {
                    return null;
                }
            }
        }


        #region 賽事處理

        /// <summary>
        /// 季後賽賽事資料處理
        /// </summary>
        /// <param name="allianceID">聯盟編號</param>
        /// <param name="gameType">賽事種類</param>
        /// <param name="acH">是否主客互換</param>
        /// <param name="schedules">賽程資料</param>
        /// <returns>賽程資料</returns>
        private Dictionary<string, GameInfo> GetScheduleOnPlayOffs(int allianceID, string gameType, bool acH, Dictionary<string, GameInfo> schedules)
        {
            DateTime gameDate = DateTime.Now.AddDays(-100);//初始化
            DateTime gameDate_old = DateTime.Now.AddDays(-100);//初始化
            DateTime gameTime = DateTime.Now;

            WebClient client = new WebClient();
            SortedList<DateTime, List<PlayOff>> gameList = new SortedList<DateTime, List<PlayOff>>();
            HtmlElementCollection dataDoc = this.webMLB.Document.GetElementsByTagName("table");

            // 特殊日期格式處理
            CultureInfo info = CultureInfo.CreateSpecificCulture("en-US");
            info.DateTimeFormat.AbbreviatedMonthNames = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec", "" };

            // 資料
            foreach (HtmlElement table in dataDoc)
            {
                string className = table.GetAttribute("className");
                if (!className.Contains("data")) { continue; }

                HtmlElementCollection trDoc = table.GetElementsByTagName("tr");
                // 沒有資料就往下處理
                if (trDoc.Count <= 2)
                    continue;

                foreach (HtmlElement tr in trDoc)
                {
                    if (!tr.GetAttribute("className").Contains("row")) { continue; }

                    // 沒有資料就往下處理
                    if (string.IsNullOrEmpty(tr.InnerText))
                        continue;

                    HtmlElementCollection td = tr.GetElementsByTagName("td");

                    if (td.Count < 3) { continue; }

                    #region 取得日期/時間

                    string date = td[0].InnerText.Trim().Replace(".", String.Empty).Replace("*", String.Empty).Replace("Thurs", "Thu");
                    string time = td[1].InnerText.Trim().Replace(".", String.Empty);
                    // 待定賽事, 不處理
                    if ("TBD".Equals(time.ToUpper())) { continue; }

                    // 轉成日期失敗就往下處理
                    if (!DateTime.TryParse(date, info, DateTimeStyles.None, out gameDate))
                    {
                        continue;
                    }
                   
                    // 如果時間格式不包含 am/pm, 預設補上 pm
                    time = time.ToLower();
                    if (!time.Contains("am") && !time.Contains("pm"))
                    {
                        time = String.Format("{0} pm", time);
                    }

                    string dateTime = String.Format("{0:yyyy-MM-dd} {1}", gameDate, time);
                    // 轉換時間失敗就往下處理
                    if (!DateTime.TryParse(dateTime, out gameTime)) { continue; }

                    #endregion

                    // 取得隊名 ( ex: Oakland at Kansas City ) 
                    // 2014.10.02 季後賽隊名的欄位有可能是顯示開賽場地 ( 欄位名為 MATCHUP=隊伍, SITE=場地 )                   
                    string away = String.Empty;
                    string home = String.Empty;
                    string[] temp = td[2].InnerText.Split(new string[] { "at" }, StringSplitOptions.RemoveEmptyEntries);
                    if (temp.Length >= 2)
                    {
                        away = temp[0].Trim();
                        home = temp[1].Trim();
                    }

                    // 依開賽日期加入賽事資料
                    List<PlayOff> playoffList = (gameList.ContainsKey(gameDate)) ? gameList[gameDate] : new List<PlayOff>();
                    PlayOff playoff = new PlayOff(gameTime, away, home)
                    {
                        OriginalGameDate = date,
                        OriginalGameTime = time
                    };

                    playoffList.Add(playoff);
                    gameList[gameDate] = playoffList;
                }
            }

            foreach (KeyValuePair<DateTime, List<PlayOff>> pair in gameList)
            {
                // HtmlDocument
                HtmlAgilityPack.HtmlDocument doc = null;
                gameDate = pair.Key;
                List<PlayOff> playoffList = pair.Value;
                // 按開賽時間排序
                playoffList.Sort((x, y) => { return Comparer<DateTime>.Default.Compare(x.GameTime, y.GameTime); });

                if (gameDate != gameDate_old)
                {
                    gameDate_old = gameDate;

                    string html = GetScoreBoardWeb(gameDate, client);
                    if (html != null && html.IndexOf("No games scheduled.") != -1)
                    {
                        //没有赛事 尝试去热身赛
                        html = GetScoreBoardWeb2(gameDate, client);
                    }
                    if (String.IsNullOrEmpty(html)) { continue; }

                    doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                }

                foreach (PlayOff playoff in playoffList)
                {
                    bool isReschedule = false;
                    string away = playoff.Away;
                    string home = playoff.Home;

                    #region 跟盤 ID

                    // 取得跟盤編號
                    string webId = FindScheduleWebID(schedules, doc, ref away, ref home, playoff.OriginalGameTime);

                    // 沒有編號就往下處理
                    if (string.IsNullOrEmpty(webId))
                        continue;
                    #endregion

                    GameInfo schedule = new GameInfo(allianceID, gameType, playoff.GameTime, webId, isReschedule)
                    {
                        AcH = acH,
                        Away = away,
                        Home = home
                    };

                    // 加入比賽資料
                    schedules[schedule.WebID] = schedule;
                }
            }

            // 傳回
            return schedules;
        }

        /// <summary>
        /// 正規賽賽事資料處理
        /// </summary>
        /// <param name="allianceID">聯盟編號</param>
        /// <param name="gameType">賽事種類</param>
        /// <param name="acH">是否主客互換</param>
        /// <param name="calendar">arenaCalendar HtmlWindow 物件</param>
        /// <param name="schedules">賽程資料</param>
        /// <returns>賽程資料</returns>
        private Dictionary<string, GameInfo> GetScheduleOnRegular(int allianceID, string gameType, bool acH, HtmlWindow calendar, Dictionary<string, GameInfo> schedules)
        {
            DateTime gameDate = DateTime.Now.AddDays(-100);//初始化
            DateTime gameDate_old = DateTime.Now.AddDays(-100);//初始化
            DateTime gameTime = DateTime.Now;

            WebClient client = new WebClient();
            HtmlElementCollection dataDoc = calendar.Document.GetElementsByTagName("table");

            // 資料
            foreach (HtmlElement table in dataDoc)
            {
                HtmlElementCollection trDoc = table.GetElementsByTagName("tr");
                
                // 沒有資料就往下處理
                if (trDoc.Count <= 2)
                    continue;
                if (string.IsNullOrEmpty(trDoc[0].InnerText) || !DateTime.TryParse(trDoc[0].InnerText, out gameDate))
                    continue;

                string html = String.Empty;
                foreach (HtmlElement tr in trDoc)
                {
                    // 沒有資料就往下處理
                    if (string.IsNullOrEmpty(tr.InnerText))
                        continue;

                    HtmlElementCollection td = tr.GetElementsByTagName("td");
                    string webId = null;
                    bool isReschedule = false;                    

                    #region 取得日期
                    if (td.Count == 1)
                    {
                        // 轉成日期失敗就往下處理
                        if (!DateTime.TryParse(tr.InnerText, out gameDate))
                        {
                            continue;
                        }
                        else if (gameDate != gameDate_old)
                        {
                            gameDate_old = gameDate;
                            html = GetScoreBoardWeb(gameDate, client);
                            if (html != null && html.IndexOf("No games scheduled.") != -1)
                            {
                                //没有赛事 尝试去热身赛
                                html = GetScoreBoardWeb2(gameDate, client);
                            }
                            if (String.IsNullOrEmpty(html)) { continue; }
                        }
                    }
                    #endregion
                    #region 取得時間
                    if (td.Count < 6)
                        continue;
                    // 轉成時間失敗就往下處理
                    string dateTime = String.Format("{0:yyyy-MM-dd} {1}", gameDate, td[2].InnerText);
                    if (!DateTime.TryParse(dateTime, out gameTime)) { continue; }
                    
                    #endregion
                    #region 跟盤 ID

                    // HtmlDocument
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    // 取得跟盤編號
                    string away = td[0].InnerText.Trim();
                    string home = td[1].InnerText.Trim();
                    webId = FindScheduleWebID(schedules, doc, ref away, ref home);

                    // 沒有編號就往下處理
                    if (string.IsNullOrEmpty(webId))
                        continue;
                    #endregion

                    GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId, isReschedule)
                    {
                        AcH = acH,
                        Away = away,
                        Home = home
                    };

                    // 加入比賽資料
                    schedules[schedule.WebID] = schedule;
                }
            }
            // 傳回
            return schedules;
        }

        #endregion

        #region 資料處理


        /// <summary>
        /// 取得跟盤編號
        /// </summary>
        /// <param name="schedules">賽程資料</param>
        /// <param name="document">scoreboard 頁面</param>
        /// <param name="away">客隊</param>
        /// <param name="home">主隊</param>
        /// <param name="time">開賽時間</param>
        /// <returns>跟盤編號</returns>
        private string FindScheduleWebID(Dictionary<string, GameInfo> schedules, HtmlAgilityPack.HtmlDocument document, ref string away, ref string home, string time = null)
        {
            string webId = String.Empty;

            if (document == null) { return webId; }

            var spanDoc = document.DocumentNode.Descendants("span");

            foreach (HtmlNode span in spanDoc)
            {
                // 判斷跟盤 ID
                if (span.Id != null && span.Id.ToLower().IndexOf("board") != -1)
                {
                    string id = span.Id.ToLower().Replace("board", "").Replace(" ", "");
                    var spanTr = span.Descendants("tr");
                    int nodeCount = spanTr.Count();

                    HtmlNode gameStatus = span.SelectSingleNode("./table[1]/tr[1]/td[2]");
                    if (gameStatus != null && gameStatus.InnerText.ToUpper().Contains("FINAL"))
                    {
                        return webId;
                    }

                    // 判斷是否為正確的格式
                    if (nodeCount >= 3)
                    {
                        // 有傳入時間時, 需比較開賽時間
                        if (!String.IsNullOrEmpty(time))
                        {
                            string gameTime = spanTr.ElementAt(0).Descendants("td").ElementAt(0).InnerText;
                            if (String.IsNullOrEmpty(gameTime)) { continue; }

                            gameTime = gameTime.Replace("EDT", String.Empty).ToLower().Trim();

                            // 時間差 30 分內的賽事
                            DateTime dateTime1, dateTime2;
                            if (DateTime.TryParse(gameTime, out dateTime1) && DateTime.TryParse(time, out dateTime2))
                            {
                                double min = (dateTime1 - dateTime2).TotalMinutes;
                                if (Math.Abs(min) >= 30) { continue; }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        //取得主客隊的資料位置
                        int idxTeam = (nodeCount > 3) ? 2 : 1;

                        //取得隊名/LOGO
                        string teamA = spanTr.ElementAt(idxTeam).Descendants("td").ElementAt(0).InnerText.Trim();
                        string teamAHtml = spanTr.ElementAt(idxTeam).Descendants("td").ElementAt(0).InnerHtml;

                        string teamB = spanTr.ElementAt(idxTeam + 1).Descendants("td").ElementAt(0).InnerText.Trim();
                        string teamBHtml = spanTr.ElementAt(idxTeam + 1).Descendants("td").ElementAt(0).InnerHtml;

                        // 去掉隊名中的排名 (Rank)
                        teamA = TrimRank(teamA);
                        teamB = TrimRank(teamB);

                        // 相同的隊伍 scoreboard主客隊
                        if (MLB_TeamCheck(teamA, away, teamAHtml) &&
                            MLB_TeamCheck(teamB, home, teamBHtml))
                        {
                            if (String.IsNullOrEmpty(away)) { away = teamA; }
                            if (String.IsNullOrEmpty(home)) { home = teamB; }

                            if (!schedules.ContainsKey(id))
                            {
                                //if (gameCount > 1)//補賽
                                //    isReschedule = true;
                                webId = id;
                                break;
                            }
                        }
                    }
                }
            }

            return webId;
        }

        // 去掉隊名中的排名/投手輸贏成績 (ex. Detroit #3 )
        private string TrimRank(string teamName)
        {
            int findInx = teamName.IndexOf("(");
            if (findInx != -1)
            {
                teamName = teamName.Substring(0, findInx);
            }

            findInx = teamName.IndexOf("#");
            if (findInx != -1)
            {
                teamName = teamName.Substring(0, findInx).Trim();
            }
            return teamName;
        }

        #endregion

        /// <summary>
        /// 取得 scoreboard 網頁
        /// </summary>
        /// <param name="gameDate">開賽日期</param>
        /// <param name="client">WebClient</param>
        /// <returns>網頁 html 內容</returns>
        private string GetScoreBoardWeb(DateTime gameDate, WebClient client)
        {
            string html = String.Empty;
            // 開啟比賽編號網站
            string url = String.Format("http://www.cbssports.com/mlb/scoreboard/{0}", gameDate.ToString("yyyyMMdd"));

            try
            {
                
                html = client.DownloadString(url);
                if (client.IsBusy)
                {
                    
                }
            }
            catch { }

            return html;
        }

        private string GetScoreBoardWeb2(DateTime gameDate, WebClient client)
        {
            string html = String.Empty;
            // 開啟比賽編號網站
            string url = String.Format("http://www.cbssports.com/mlb/scoreboard/{0}/spring", gameDate.ToString("yyyyMMdd"));

            try
            {

                html = client.DownloadString(url);
                if (client.IsBusy)
                {

                }
            }
            catch { }

            return html;
        }

        //MLB 來源與跟盤編號 不同名稱但同隊伍的查詢表
        private Dictionary<string, string> MLB_Logo
        {
            get
            {
                Dictionary<string, string> team = new Dictionary<string, string>();
                team["CHC"] = "Chi. Cubs";
                team["CHW"] = "Chi. White Sox";
                team["chicago-cubs"] = "Chi. Cubs";
                team["chicago-white-sox"] = "Chi. White Sox";

                team["NYY"] = "N.Y. Yankees";
                team["NYM"] = "N.Y. Mets";
                team["new-york-mets"] = "N.Y. Mets";
                team["new-york-yankees"] = "N.Y. Yankees";

                team["LAA"] = "L.A. Angels";
                team["LAD"] = "L.A. Dodgers";
                team["los-angeles-angels"] = "L.A. Angels";
                team["los-angeles-dodgers"] = "L.A. Dodgers";

                return team;
            }
        }

        private bool MLB_TeamCheck(string source, string compare, string html)
        {
            int findInx = source.IndexOf("(");
            if (findInx != -1)
                source = source.Substring(0, findInx);

            if (source.Trim() == compare.Trim())
                return true;
            else
            {
                try
                {
                    var arr = html.Split(new string[] { "<", ">", "\"" }, StringSplitOptions.RemoveEmptyEntries);
                    arr = arr[1].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    string basicName = arr[arr.Length - 1];//取得完整名稱

                    //如果有對應名稱 就回傳 
                    if (MLB_Logo.ContainsKey(basicName))
                    {
                        // 如果傳進來比較的隊名為空, 直接回傳不比較 ( 季後賽賽事會有沒給隊名的情況 )
                        if (!String.IsNullOrEmpty(compare))
                        {
                            return (MLB_Logo[basicName] == compare);
                        }
                        else { return true; }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(compare)) { return true; }
                    }
                }
                catch { }

                return false;
            }
        }

        #endregion
    }
}
