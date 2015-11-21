using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region BKUS - 美國職籃 (NBA)

        #region NBA - ESPN (新版改于2015.4.1)

        private Dictionary<string, GameInfo>
                  GetSchedulesByNBA(int allianceID, string gameType, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webNBA.Document == null ||
                this.webNBA.Document.Body == null)
                return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;

            #region 取得日期(新版)
            HtmlElement spanDate = this.webNBA.Document.GetElementById("sbpDate");
            if (spanDate.InnerText == null || !DateTime.TryParse(spanDate.InnerText.Replace("Scores for", "").Trim(), out gameDate))
            {
                return null;
            }
            #endregion 取得日期

            #region 处理数据(新版)
            string webId = String.Empty;
            foreach (HtmlElement game in this.webNBA.Document.GetElementsByTagName("article"))
            {
                if (game.Id != null && game.GetAttribute("className").ToLower().IndexOf("js-show") != -1)
                {
                    //网页ID
                    webId = game.Id.Trim();
                    HtmlElementCollection timeSpan = game.GetElementsByTagName("span");

                    //开赛时间
                    if (!DateTime.TryParse(gameDate.ToString("yyyy-MM-dd") + " " + timeSpan[0].InnerText.ToUpper().Replace("AM", "").Replace("PM", "").Replace("ET", "").Replace("CT", ""), out gameTime))
                        continue;

                    //页面的时间，时间+24小时，如4月1日ESPN的赛事必然为隔日4月2号的赛程
                    //不判定AM  PM字样 4/2 
                    gameTime = gameTime.AddHours(12);

                    // 队伍
                    HtmlElementCollection teams = game.GetElementsByTagName("tbody");
                    if (teams == null || teams.Count == 0 || teams[0].Id != "teams")
                    {
                        continue;
                    }
                    GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                    schedule.AcH = acH;
                    schedule.Away = teams[0].GetElementsByTagName("h2")[0].InnerText.Trim();
                    schedule.Home = teams[0].GetElementsByTagName("h2")[1].InnerText.Trim();

                    // 加入比賽資料
                    schedules[schedule.WebID] = schedule;
                }
            }
            #endregion 处理数据(新版)

            #region 取得日期
            //foreach (HtmlElement div in this.webNBA.Document.GetElementsByTagName("div"))
            //{
            //    if (div.GetAttribute("className") == "key-dates key-dates_sc")
            //    {
            //        // 轉成日期失敗就往下處理
            //        if (!DateTime.TryParse(div.GetElementsByTagName("h2")[0].InnerText.Replace("Scores for", "").Trim(), out gameDate))
            //        {
            //            return null;
            //        }
            //    }
            //}
            #endregion 取得日期
            #region 处理数据
            // 處理資料
            //foreach (HtmlElement game in this.webNBA.Document.GetElementsByTagName("div"))
            //{
            //    if (game.Id != null && game.Id.IndexOf("-gamebox") != -1)
            //    {
            //        string webId = game.Id.Replace("-gamebox", "");
            //        // 時間錯誤就往下處理
            //        if (!DateTime.TryParse(gameDate.ToString("yyyy-MM-dd") + " " + this.webNBA.Document.GetElementById(webId + "-statusLine1").InnerText.Replace("ET", ""), out gameTime))
            //            continue;

            //        GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId);
            //        schedule.AcH = acH;
            //        // 設定
            //        schedule.Away = this.webNBA.Document.GetElementById(webId + "-aNameOffset").InnerText;
            //        schedule.Home = this.webNBA.Document.GetElementById(webId + "-hNameOffset").InnerText;

            //        // 加入比賽資料
            //        schedules[schedule.WebID] = schedule;
            //    }
            //}
            #endregion 处理数据
            // 傳回
            return schedules;
        }

        #endregion NBA - ESPN

        #region nba from www.cbssports.com

        /* private Dictionary<string, GameInfo> GetSchedulesByNBA(int allianceID, string gameType, Dictionary<string, string> teamName, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webNBA.Document == null ||
                this.webNBA.Document.Body == null) return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;

            #region 取得data，資料在iframe

            WebBrowser webIf = new WebBrowser();
            webIf.ScriptErrorsSuppressed = true;
            string webIfUrl = this.webNBA.Document.GetElementById("arenaCalendar").GetAttribute("src");
            //有時會抓到相對路徑
            if (webIfUrl.IndexOf("http") == -1)
            {
                webIfUrl = "http://www.cbssports.com" + webIfUrl;
            }
            webIf.Navigate(webIfUrl);

            // 等待完成
            DateTime IfwaitTime = DateTime.Now;
            double IfspanSeconds = 0;
            double IfmaxSeconds = 10; // 30 秒
            while (webIf.ReadyState != WebBrowserReadyState.Complete)
            {
                TimeSpan spanTime = DateTime.Now.Subtract(IfwaitTime);
                // 經過秒數
                IfspanSeconds = Math.Abs(spanTime.TotalSeconds);
                // 超過就離開
                if (IfspanSeconds >= IfmaxSeconds) break;
                // 避免死當
                Application.DoEvents();
            }
            // 判斷網頁完成
            if (webIf.Document == null) return schedules;

            #endregion 取得data，資料在iframe

            // 資料
            foreach (HtmlElement table in webIf.Document.GetElementsByTagName("table"))
            {
                // 不是資料就往下處理
                if (table.GetAttribute("className") != "data") continue;

                HtmlElementCollection trDoc = table.GetElementsByTagName("tr");
                WebBrowser web = new WebBrowser();

                // 沒有資料就往下處理
                if (trDoc.Count <= 2) continue;

                foreach (HtmlElement tr in trDoc)
                {
                    // 沒有資料就往下處理
                    if (string.IsNullOrEmpty(tr.InnerText)) continue;

                    HtmlElementCollection td = tr.GetElementsByTagName("td");
                    string webId = null;

                    if (td.Count == 1)
                    {
                        #region 取得日期

                        if (tr.GetAttribute("className") == "title")
                        {
                            // 轉成日期失敗就往下處理
                            if (!DateTime.TryParse(tr.InnerText, out gameDate))
                            {
                                continue;
                            }
                        }

                        #endregion 取得日期

                        #region 取webId

                        if (true)
                        {
                            // 開啟比賽編號網站
                            web.ScriptErrorsSuppressed = true;
                            web.Navigate("http://www.cbssports.com/nba/scoreboard/" + gameDate.ToString("yyyyMMdd"));

                            // 等待完成
                            DateTime waitTime = DateTime.Now;
                            double spanSeconds = 0;
                            double maxSeconds = 10; // 30 秒
                            while (web.ReadyState != WebBrowserReadyState.Complete)
                            {
                                TimeSpan spanTime = DateTime.Now.Subtract(waitTime);
                                // 經過秒數
                                spanSeconds = Math.Abs(spanTime.TotalSeconds);
                                // 超過就離開
                                if (spanSeconds >= maxSeconds) break;
                                // 避免死當
                                Application.DoEvents();
                            }
                            // 判斷網頁完成
                            if (web.Document == null) break;
                        }

                        #endregion 取webId
                    }
                    // 比賽資料
                    if (tr.GetAttribute("className").IndexOf("row") != -1 && td.Count == 7)
                    {
                        // 轉成時間失敗就往下處理
                        if (!DateTime.TryParse(gameDate.ToString("yyyy-MM-dd") + " " + td[2].InnerText, out gameTime)) continue;

                        //取隊伍識別
                        //客隊
                        string[] teamStrTmp = td[0].Children[0].GetAttribute("href").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        teamStrTmp = teamStrTmp[teamStrTmp.Length - 1].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                        string teamStr = teamStrTmp[teamStrTmp.Length - 1];
                        //主隊
                        teamStrTmp = td[1].Children[0].GetAttribute("href").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        teamStrTmp = teamStrTmp[teamStrTmp.Length - 1].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                        teamStr = teamStr + "," + teamStrTmp[teamStrTmp.Length - 1];
                        string[] team = teamStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        // 找到跟盤編號
                        foreach (HtmlElement span in web.Document.GetElementsByTagName("span"))
                        {
                            // 判斷跟盤 ID
                            if (span.Id != null && span.Id.ToLower().IndexOf("board") != -1)
                            {
                                string id = span.Id.ToLower().Replace("board", "").Replace(" ", "");
                                HtmlElementCollection spanTr = span.GetElementsByTagName("tr");
                                // 判斷是否為正確的格式
                                if (spanTr.Count >= 3)
                                {
                                    string[] awayTmp = spanTr[1].GetElementsByTagName("td")[0].Children[0].GetAttribute("href").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                                    awayTmp = awayTmp[awayTmp.Length - 1].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] homeTmp = spanTr[2].GetElementsByTagName("td")[0].Children[0].GetAttribute("href").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                                    homeTmp = homeTmp[homeTmp.Length - 1].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                                    // 相同的隊伍識別
                                   if (awayTmp[awayTmp.Length-1].Replace(".", "") == team[0].Replace(".", "") &&
                                        homeTmp[homeTmp.Length - 1].Replace(".", "") == team[1].Replace(".", ""))
                                    {
                                        webId = id;
                                        break;
                                    }
                                }
                            }
                        }
                        // 沒有編號就往下處理
                        if (string.IsNullOrEmpty(webId)) continue;

                        GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                        schedule.AcH = acH;
                        // 設定 第一個字元轉大寫 為取得中文隊名
                        schedule.Away = team[0].Substring(0, 1).ToUpper() + team[0].Substring(1,team[0].Length-1);
                        schedule.Home = team[1].Substring(0, 1).ToUpper() + team[1].Substring(1, team[1].Length-1);

                        // 判斷隊名
                        if (teamName != null)
                        {
                            if (teamName.ContainsKey(schedule.Away)) schedule.Away = teamName[schedule.Away];
                            if (teamName.ContainsKey(schedule.Home)) schedule.Home = teamName[schedule.Home];
                        }
                        // 加入比賽資料
                        schedules[schedule.WebID] = schedule;
                    }
                }
            }
            // 傳回
            return schedules;
        }*/

        #endregion nba from www.cbssports.com

        #endregion BKUS - 美國職籃 (NBA)
    }
}