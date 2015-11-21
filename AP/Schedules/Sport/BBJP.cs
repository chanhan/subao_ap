using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region 日棒 YAHOO

        private Dictionary<string, GameInfo> GetSchedulesByNPB(int allianceID, string gameType, bool acH = false)
        {
            bool isWarmUp = false;
            // 沒有資料就離開
            if (this.webNPB.Document == null ||
                this.webNPB.Document.Body == null)
                return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;

            //判断是否热身赛界面 
            if (this.webNPB.DocumentTitle == "プロ野球 - オープン戦 - 日程・結果 -スポーツナビ")
            {
                isWarmUp = true;
            }

            //資料
            foreach (HtmlElement table in this.webNPB.Document.GetElementsByTagName("table"))
            {
                // 不是資料就往下處理
                if (table.GetAttribute("classname").ToLower().IndexOf("yjms") != 0)
                    continue;

                string gameDateYear = null;

                #region 取得年份
                foreach (HtmlElement div in this.webNPB.Document.GetElementsByTagName("div"))
                {
                    // 不是資料就往下處理
                    if (div.GetAttribute("classname").ToLower().IndexOf("npbsubtitle") == -1)
                        continue;
                    if (div.GetElementsByTagName("strong").Count != 2)
                        continue;

                    gameDateYear = div.GetElementsByTagName("strong")[0].InnerText.Replace("年", "").Trim();
                    break;
                }
                // 沒有年份就離開
                if (string.IsNullOrEmpty(gameDateYear))
                    break;
                #endregion


                #region 取得資料
                //資料
                foreach (HtmlElement tr in table.GetElementsByTagName("tr"))
                {
                    HtmlElementCollection td = tr.GetElementsByTagName("td");
                    // 不是資料就往下處理
                    if (td.Count < 6)
                        continue;

                    string webId = null;

                    #region 取得日期
                    if (tr.GetElementsByTagName("th").Count == 1)
                    {
                        string gameDateStr = tr.GetElementsByTagName("th")[0].InnerText;
                        // 處理資料
                        if (gameDateStr.IndexOf("（") != -1)
                        {
                            gameDateStr = gameDateStr.Substring(0, gameDateStr.IndexOf("（")).Trim();
                            gameDateStr = gameDateStr.Replace("月", "/").Replace("日", "");
                        }
                        // 轉成日期失敗就往下處理
                        if (!DateTime.TryParse(gameDateYear + "/" + gameDateStr, out gameDate))
                            continue;
                    }
                    #endregion
                    #region 取得時間
                    string[] txt = isWarmUp ? tr.NextSibling.GetElementsByTagName("td")[0].InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                        : td[4].InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    // 轉成時間失敗就往下處理
                    if (txt.Length == 0 || !DateTime.TryParse(gameDate.ToString("yyyy-MM-dd") + " " + txt[0], out gameTime))
                        continue;
                    #endregion
                    //已经开赛的比赛不建立
                    if (gameTime < DateTime.Now)
                    { continue; }
                    GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                    schedule.AcH = acH;
                    // 設定
                    schedule.Away = td[0].InnerText;
                    schedule.Home = td[2].InnerText;

                    // 加入比賽資料
                    schedules[schedules.Count.ToString()] = schedule;

                }
                #endregion 取得資料

            }
            // 傳回
            return schedules;
        }
        #endregion
    }
}
