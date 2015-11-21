using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region BKKRW - 韓國女子職籃 (WKBL)

        private Dictionary<string, GameInfo> GetSchedulesByWKBL(int allianceID, string gameType, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webWKBL.Document == null ||
                this.webWKBL.Document.Body == null ||
                this.webWKBL.Document.GetElementById("print") == null)
                return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            string gameDateStr = null;
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;
            string gameYear = null;

            // 取得來源ID
            string sourceId = GetGameUseSourceID(allianceID, gameType);

            #region 取得年

            // 處理資料
            //foreach (HtmlElement div in this.webWKBL.Document.GetElementsByTagName("div"))
            //{
            //    //if (div.GetAttribute("className") == "game_month" &&
            //    //    div.GetElementsByTagName("span").Count == 3)
            //    //{
            //    //    gameYear = div.GetElementsByTagName("img")[0].GetAttribute("alt").Replace("년", "").Trim();
            //    //}
            //    if (div.GetAttribute("className") == "sel_gsch")
            //    {
            //        gameYear = (div.Children[1] as HtmlElement).GetAttribute("alt").Replace("년", ""); //년 = 年
            //    }

            //}

            foreach (HtmlElement div in this.webWKBL.Document.GetElementsByTagName("div"))
            {
                if (div.GetAttribute("className") == "ymbox")
                {
                    gameYear = (div as HtmlElement).GetElementsByTagName("strong")[0].InnerHtml.Replace(".", "");
                    break;
                }
            }
            if (string.IsNullOrEmpty(gameYear))
                return null;
            DateTime dGameYear;
            if (!DateTime.TryParse(gameYear, out dGameYear))
                dGameYear = DateTime.Now;

            #endregion 取得年

            // 處理資料
            //foreach (HtmlElement game in this.webWKBL.Document.GetElementById("print").GetElementsByTagName("table"))
            //取賽程資料
            foreach (HtmlElement game in this.webWKBL.Document.GetElementById("sch").GetElementsByTagName("tr"))
            {
                HtmlElementCollection td = game.GetElementsByTagName("td");

                string webId = null;
                //確定是否有WebID的值
                //string checkWebId = game.GetElementsByTagName("td")[4].InnerHtml;
                //if (checkWebId.Contains("onclick"))
                //{
                //    //切割取得WebId的字串
                //    webId = checkWebId.Split('&')[2].Split('=')[1];
                //}

                // 數量不對就往下處理
                if (td.Count != 5)
                    continue;

                gameDateStr = td[0].InnerText;
                if (gameDateStr.IndexOf("(") != -1)
                    gameDateStr = gameDateStr.Substring(0, gameDateStr.IndexOf("(")).Trim();
                gameDateStr = gameDateStr.Replace("월", "/"); //月
                gameDateStr = gameDateStr.Replace("일", "");  //日
                if (dGameYear == null)
                    gameDateStr = gameYear + "/" + gameDateStr.Replace(" ", "");
                else
                    gameDateStr = dGameYear.Year + "/" + gameDateStr.Replace(" ", "");

                // 轉換日期
                if (DateTime.TryParse(gameDateStr, out gameDate) &&
                    DateTime.TryParse(gameDate.ToString("yyyy-MM-dd") + " " + td[3].InnerText, out gameTime))
                {
                    //尋找隊伍
                    td = td[1].GetElementsByTagName("dd");
                    // 判斷隊伍
                    if (td.Count < 2)
                        continue;

                    GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                    schedule.AcH = acH;
                    // 設定
                    schedule.Away = td[0].InnerText.Replace("\r\n", "").Trim();
                    schedule.Home = td[td.Count - 1].InnerText.Replace("\r\n", "").Trim();

                    // 加入比賽資料
                    schedule.SourceID = sourceId;
                    schedules[schedules.Count.ToString()] = schedule;
                }
            }
            // 傳回
            return schedules;
        }

        #endregion BKKRW - 韓國女子職籃 (WKBL)
    }
}