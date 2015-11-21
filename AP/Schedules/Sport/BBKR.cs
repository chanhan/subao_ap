using System;
using System.Collections.Generic;
using System.Web;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region 韓棒 NAVER

        private Dictionary<string, GameInfo> GetSchedulesByKBO(int allianceID, string gameType, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webKBO.Document == null ||
                this.webKBO.Document.Body == null ||
                this.webKBO.Document.GetElementById("calendarWrap") == null)
                return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            HtmlElement dataDoc = this.webKBO.Document.GetElementById("calendarWrap");
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;
            string gameDateStr = String.Empty;
            string gameTimeStr = String.Empty;

            // 資料
            foreach (HtmlElement table in dataDoc.GetElementsByTagName("table"))
            {
                foreach (HtmlElement tr in table.GetElementsByTagName("tr"))
                {
                    HtmlElementCollection td = tr.GetElementsByTagName("td");

                    GameInfo schedule = null;
                    string webId = null;
                    int index = 0;

                    #region 取得日期 / 時間

                    HtmlElementCollection spanColl = tr.GetElementsByTagName("span");
                    foreach (HtmlElement span in spanColl)
                    {
                        // 取得日期
                        if ("td_date".Equals(span.GetAttribute("className")))
                        {
                            gameDateStr = span.InnerText;
                            // 處理資料
                            if (gameDateStr.IndexOf("(") != -1)
                            {
                                gameDateStr = gameDateStr.Substring(0, gameDateStr.IndexOf("(")).Trim();
                                gameDateStr = gameDateStr.Replace(".", "/");
                            }
                        }

                        // 取得時間
                        if ("td_hour".Equals(span.GetAttribute("className")))
                        {
                            gameTimeStr = span.InnerText;

                            // 取得 WebID Index
                            int i = 0;
                            HtmlElement el = span.Parent;
                            foreach (HtmlElement elTD in td)
                            {
                                if (el == elTD)
                                {
                                    index = i;
                                    break;
                                }
                                i++;
                            }
                        }
                    }

                    // 轉換日期失敗就離開
                    if (!DateTime.TryParse(gameDateStr, out gameDate)) { continue; }
                                      
                    // 轉成時間失敗就離開
                    string gameDateTime = String.Format("{0:yyyy/MM/dd} {1}", gameDate, gameTimeStr);
                    if (!DateTime.TryParse(gameDateTime, out gameTime)) { continue; }

                    #endregion

                    #region 跟盤 ID
                    HtmlElementCollection aDoc = td[index + 2].GetElementsByTagName("a");
                    // 判斷資料
                    if (aDoc.Count == 0) { continue; }
                    
                    // 取出資料
                    webId = aDoc[0].GetAttribute("href");
                    Uri uri = null;
                    HttpRequest req = null;
                    // 錯誤處理
                    try
                    {
                        uri = new Uri(webId);
                        // 判斷是否有資料
                        if (uri.Query != null && !string.IsNullOrEmpty(uri.Query))
                        {
                            req = new HttpRequest("", uri.AbsoluteUri, uri.Query.Substring(1));
                            // 判斷資料
                            if (req["gameid"] != null && !string.IsNullOrEmpty(req["gameid"].Trim()))
                            {
                                webId = req["gameid"];
                            }
                        }
                    }
                    catch { continue; } // 錯誤，往下處理
                    #endregion

                    schedule = null;
                    schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                    schedule.AcH = acH;
                    // 設定
                    schedule.Away = td[index + 1].Children[0].InnerText;
                    schedule.Home = td[index + 1].Children[td[index + 1].Children.Count - 1].InnerText;

                    // 加入比賽資料
                    schedules[schedule.WebID] = schedule;
                }
            }
            // 傳回
            return schedules;
        }

        #endregion
    }
}
