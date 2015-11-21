using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region AFUS - 美式足球

        private Dictionary<string, GameInfo> GetSchedulesByNFL(int allianceID, string gameType, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webNFL.Document == null ||
                this.webNFL.Document.Body == null)
                return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;
            HtmlElementCollection week = this.webNFL.Document.GetElementById("marketing_over_top").NextSibling.GetElementsByTagName("a");

            //选择的参数
            string w = string.Empty;
            string sYyyy = string.Empty;
            foreach (HtmlElement a in week)
            {
                if (a.GetAttribute("className").ToLower().Trim()=="optsel")
                {
                    ///nfl/schedules/regular/2014/week17
                    string aHref = a.GetAttribute("href");
                    string temp = aHref.Substring(aHref.LastIndexOf("regular") + 8 );
                    w = temp.Substring(temp.IndexOf('/') + 1);
                    sYyyy = temp.Substring(0, 4);
                    break;
                }
            }

            // 資料
            foreach (HtmlElement table in this.webNFL.Document.GetElementsByTagName("table"))
            {
                // 不是資料就往下處理
                if (table.GetAttribute("className") != "data")
                    continue;

                HtmlElementCollection trDoc = table.GetElementsByTagName("tr");
                WebBrowser web = new WebBrowser();
                // 沒有資料就往下處理
                if (trDoc.Count <= 2)
                    continue;

                foreach (HtmlElement tr in trDoc)
                {
                    // 沒有資料就往下處理
                    if (string.IsNullOrEmpty(tr.InnerText))
                        continue;

                    HtmlElementCollection td = tr.GetElementsByTagName("td");
                    string webId = null;

                    if (td.Count == 1)
                    {
                        #region 取得星期

                        if (tr.GetAttribute("className") == "title")
                        {
                            // 開啟比賽編號網站
                            web.ScriptErrorsSuppressed = true;

                            string url = null;

                            // 判斷
                            if (this.webNFL.Url.ToString().IndexOf("preseason") != -1)
                            {
                                url = "http://www.cbssports.com/nfl/scoreboard/" + sYyyy + "/preseason/" + w;
                            }
                            else
                            {
                                url = "http://www.cbssports.com/nfl/scoreboard/" + sYyyy + "/" + w;
                                //url = "http://www.cbssports.com/nfl/scoreboard/";
                            }
                            web.Navigate(url);

                            // 等待完成
                            DateTime waitTime = DateTime.Now;
                            double spanSeconds = 0;
                            double maxSeconds = 60; // 30 秒
                            while (web.ReadyState != WebBrowserReadyState.Complete)
                            {
                                TimeSpan spanTime = DateTime.Now.Subtract(waitTime);
                                // 經過秒數
                                spanSeconds = Math.Abs(spanTime.TotalSeconds);
                                // 超過就離開
                                if (spanSeconds >= maxSeconds)
                                    break;
                                // 避免死當
                                Application.DoEvents();
                            }
                            // 判斷網頁完成
                            if (web.Document == null)
                                break;
                        }

                        #endregion 取得星期

                        #region 取得日期

                        if (tr.GetAttribute("className") == "subtitle")
                        {
                            // 轉成日期失敗就往下處理
                            if (!DateTime.TryParse(tr.InnerText, out gameDate))
                            {
                                continue;
                            }
                        }

                        #endregion 取得日期
                    }
                    // 比賽資料
                    if (tr.GetAttribute("className").IndexOf("row") != -1 && td.Count == 4)
                    {
                        // 轉成時間失敗就往下處理
                        if (!DateTime.TryParse(gameDate.ToString("yyyy-MM-dd") + " " + td[1].InnerText, out gameTime))
                            continue;
                        string[] team = td[0].InnerText.Split(new string[] { " at " }, StringSplitOptions.RemoveEmptyEntries);
                        // 找到跟盤編號
                        foreach (HtmlElement span in web.Document.GetElementsByTagName("span"))
                        {
                            // 判斷跟盤 ID
                            if (span.Id != null && span.Id.ToLower().IndexOf("board") != -1)
                            {
                                if (((HtmlElement)span.Children[0]).InnerText.ToLower().IndexOf("final") != -1)
                                {
                                    continue;
                                }
                                string id = span.Id.ToLower().Replace("board", "").Replace(" ", "");
                                HtmlElementCollection spanTr = span.GetElementsByTagName("tr");
                                // 判斷是否為正確的格式
                                if (spanTr.Count >= 3)
                                {
                                    List<string> TeamName = new List<string>();
                                    for (int i=0; i < spanTr.Count; i++)
                                    {
                                        HtmlElement elTr = spanTr[i];
                                        if (elTr.GetAttribute("className").IndexOf("teamInfo") != -1)
                                        {
                                            string name = elTr.GetElementsByTagName("td")[0].InnerText;
                                            if (!TeamName.Contains(name)) { TeamName.Add(name); }
                                        }
                                    }

                                    for (int i = 0; i < TeamName.Count; i++)
                                    {
                                        int findIndex = TeamName[i].IndexOf("(");
                                        if (findIndex != -1)
                                            TeamName[i] = TeamName[i].Substring(0, findIndex);
                                    }

                                    if (TeamName[0].Replace(".", "") == team[0].Replace(".", "") &&
                                        TeamName[1].Replace(".", "") == team[1].Replace(".", ""))
                                    {
                                        webId = id;
                                        break;
                                    }

                                    // 相同的隊伍
                                    //if (spanTr[1].GetElementsByTagName("td")[0].InnerText.Replace(".", "") == team[0].Replace(".", "") &&
                                    //    spanTr[2].GetElementsByTagName("td")[0].InnerText.Replace(".", "") == team[1].Replace(".", ""))
                                    //{
                                    //    webId = id;
                                    //    break;
                                    //}
                                }
                            }
                        }
                        // 沒有編號就往下處理
                        if (string.IsNullOrEmpty(webId))
                            continue;

                        GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                        schedule.AcH = acH;
                        // 設定
                        schedule.Away = team[0];
                        schedule.Home = team[1];

                        // 加入比賽資料
                        schedules[schedule.WebID] = schedule;
                    }
                }
            }
            // 傳回
            return schedules;
        }

        #endregion AFUS - 美式足球
    }
}