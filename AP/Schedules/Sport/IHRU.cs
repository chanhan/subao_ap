using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region IHRU - 俄羅斯冰球

        private Dictionary<string, GameInfo> GetSchedulesByKHL(int allianceID, string gameType, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webKHL.Document == null ||
                this.webKHL.Document.Body == null ||
                this.webKHL.Document.GetElementById("content") == null)
                return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            string gameDateStr = null;
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;
            // 處理資料
            foreach (HtmlElement div in this.webKHL.Document.GetElementById("content").Children)
            {
                // 判斷日期
                if (div.GetAttribute("className") == "matchDate")
                {
                    gameDateStr = div.InnerText;
                    if (gameDateStr.IndexOf("(") != -1)
                        gameDateStr = gameDateStr.Substring(0, gameDateStr.IndexOf("(")).Trim();
                    // 轉換日期失敗就往下處理
                    if (!DateTime.TryParse(gameDateStr, out gameDate))
                    {
                        gameDateStr = null;
                        continue;
                    }
                }
                // 判斷比賽
                if (div.GetAttribute("className") == "matches" && !string.IsNullOrEmpty(gameDateStr))
                {
                    foreach (HtmlElement game in div.GetElementsByTagName("div"))
                    {
                        if (game.GetAttribute("className") == "match")
                        {
                            string webId = game.GetElementsByTagName("div")[1].InnerText;
                            string gameTimeStr = game.GetElementsByTagName("div")[2].InnerHtml.Replace("<!--", "").Replace("-->", "").Trim();
                            if (gameTimeStr.IndexOf(" ") != -1)
                                gameTimeStr = gameTimeStr.Substring(0, gameTimeStr.IndexOf(" ")).Trim();
                            // 轉換日期失敗就往下處理
                            if (!DateTime.TryParse(gameDate.ToString("yyyy/MM/dd") + " " + gameTimeStr, out gameTime))
                                continue;

                            GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                            schedule.AcH = acH;
                            // 設定
                            schedule.Away = game.GetElementsByTagName("table")[0].GetElementsByTagName("tr")[0].GetElementsByTagName("td")[0].InnerText;
                            schedule.Home = game.GetElementsByTagName("table")[0].GetElementsByTagName("tr")[1].GetElementsByTagName("td")[0].InnerText;

                            // 加入比賽資料
                            schedules[schedule.WebID] = schedule;
                        }
                    }
                }
                //string webId = null;
                //string txt = game.GetElementsByTagName("h4")[0].InnerText;
                //// 判斷時間
                //if (txt.LastIndexOf(" ") != -1)
                //    txt = txt.Substring(0, txt.LastIndexOf(" ")).Trim();
                //// 時間錯誤就往下處理
                //if (!DateTime.TryParse(gameDate.ToString("yyyy-MM-dd") + " " + this.webNBA.Document.GetElementById(webId + "-statusLine1").InnerText.Replace("ET", ""), out gameTime)) continue;

                //GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                //schedule.AcH = acH;
                //// 設定
                //schedule.Away = this.webNBA.Document.GetElementById(webId + "-aNameOffset").InnerText;
                //schedule.Home = this.webNBA.Document.GetElementById(webId + "-hNameOffset").InnerText;

                //// 判斷隊名
                //if (teamName != null)
                //{
                //    if (teamName.ContainsKey(schedule.Away)) schedule.Away = teamName[schedule.Away];
                //    if (teamName.ContainsKey(schedule.Home)) schedule.Home = teamName[schedule.Home];
                //}
                //// 加入比賽資料
                //schedules[schedules.Count.ToString()] = schedule;
            }
            // 傳回
            return schedules;
        }

        #endregion IHRU - 俄羅斯冰球
    }
}