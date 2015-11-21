using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region BKUSW - 美國女子職籃 (WNBA)

        private Dictionary<string, GameInfo> GetSchedulesByWNBA(int allianceID, string gameType, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webWNBA.Document == null ||
                this.webWNBA.Document.Body == null)
                return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            Dictionary<string, HtmlElement> doc = new Dictionary<string, HtmlElement>();
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;
            // 找到資料
            if (this.webWNBA.Document.GetElementById("gamesLeft") != null)
                doc["Left"] = this.webWNBA.Document.GetElementById("gamesLeft");
            if (this.webWNBA.Document.GetElementById("gamesRight") != null)
                doc["Right"] = this.webWNBA.Document.GetElementById("gamesRight");

            #region 取得日期

            foreach (HtmlElement div in this.webWNBA.Document.GetElementsByTagName("div"))
            {
                if (div.GetAttribute("className") == "key-dates key-dates_sc")
                {
                    // 轉成日期失敗就往下處理
                    if (!DateTime.TryParse(div.GetElementsByTagName("h2")[0].InnerText.Replace("Scores for", "").Trim(), out gameDate))
                    {
                        return null;
                    }
                }
            }

            #endregion 取得日期

            // 處理資料
            foreach (HtmlElement game in this.webWNBA.Document.GetElementsByTagName("div"))
            {
                if (game.Id != null && game.Id.IndexOf("-gameHeader") != -1)
                {
                    string webId = game.Id.Replace("-gameHeader", "");
                    // 時間錯誤就往下處理
                    if (!DateTime.TryParse(gameDate.ToString("yyyy-MM-dd") + " " + this.webWNBA.Document.GetElementById(webId + "-statusLine1").InnerText.Replace("ET", ""), out gameTime))
                        continue;

                    GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                    schedule.AcH = acH;
                    // 設定
                    schedule.Away = this.webWNBA.Document.GetElementById(webId + "-aNameOffset").InnerText;
                    schedule.Home = this.webWNBA.Document.GetElementById(webId + "-hNameOffset").InnerText;

                    // 加入比賽資料
                    schedules[schedule.WebID] = schedule;
                }
            }
            // 傳回
            return schedules;
        }

        #endregion BKUSW - 美國女子職籃 (WNBA)
    }
}