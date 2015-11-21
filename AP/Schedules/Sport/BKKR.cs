using System;
using System.Collections.Generic;
using System.Web;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region BKKR - 韓國職籃

        private Dictionary<string, GameInfo> GetSchedulesByKBL(int allianceID, string gameType, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webKBL.Document == null ||
                this.webKBL.Document.Body == null ||
                this.webKBL.Document.GetElementById("content") == null ||
                this.webKBL.Document.GetElementById("content").GetElementsByTagName("table").Count != 3)
                return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;

            // 取得來源ID
            string sourceId = GetGameUseSourceID(allianceID, gameType);

            // 處理資料
            foreach (HtmlElement game in this.webKBL.Document.GetElementById("content").GetElementsByTagName("table")[2].GetElementsByTagName("tr"))
            {
                // 沒有資料就往下處理
                if (game.GetElementsByTagName("td").Count != 3 ||
                    game.GetElementsByTagName("td")[2].GetElementsByTagName("a").Count == 0)
                    continue;

                // 取得日期
                if (game.GetElementsByTagName("th").Count == 1)
                {
                    string txt = game.GetElementsByTagName("th")[0].InnerText;
                    // 判斷文字
                    if (txt.IndexOf("(") != -1)
                        txt = txt.Substring(0, txt.IndexOf("(")).Trim();
                    txt = txt.Replace("월", "/").Replace("일", "").Trim();
                    // 轉換日期
                    if (!DateTime.TryParse(txt, out gameDate))
                        continue;
                }
                if (gameDate.Date < DateTime.Parse("2013-10-17").Date)
                    continue;
                // 取得時間
                if (!DateTime.TryParse(gameDate.ToString("yyyy-MM-dd") + " " + game.GetElementsByTagName("td")[1].InnerText, out gameTime))
                    continue;

                string webId = game.GetElementsByTagName("td")[2].GetElementsByTagName("a")[0].GetAttribute("href");
                string[] team = game.GetElementsByTagName("td")[0].InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                // 資料錯誤就往下處理
                if (string.IsNullOrEmpty(webId))
                    continue;
                if (team.Length < 2)
                    continue;
                // 錯誤處理
                try
                {
                    Uri url = new Uri(webId);
                    if (url.Query != null && !string.IsNullOrEmpty(url.Query))
                    {
                        HttpRequest req = new HttpRequest("", url.AbsoluteUri, url.Query.Substring(1));
                        // 判斷資料
                        if (!string.IsNullOrEmpty(req["gameid"].Trim()))
                        {
                            webId = req["gameid"].Trim();

                            GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                            schedule.AcH = acH;
                            // 設定
                            schedule.Away = team[0];
                            schedule.Home = team[team.Length - 1];

                            // 加入比賽資料
                            schedule.SourceID = sourceId;
                            schedules[schedule.WebID] = schedule;
                        }
                    }
                }
                catch { }
            }
            // 傳回
            return schedules;
        }

        #endregion BKKR - 韓國職籃
    }
}