using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region BKJP - 日本職籃

        private Dictionary<string, GameInfo> GetSchedulesByBJ(int allianceID, string gameType, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webBJ.Document == null ||
                this.webBJ.Document.Body == null ||
                this.webBJ.Document.GetElementById("contents") == null)
                return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;
            // 處理資料
            foreach (HtmlElement games in this.webBJ.Document.GetElementById("contents").GetElementsByTagName("table"))
            {
                foreach (HtmlElement game in games.GetElementsByTagName("tr"))
                {
                    // 沒有資料就往下處理
                    if (game.GetElementsByTagName("th").Count != 1 ||
                        game.GetElementsByTagName("td").Count != 1)
                        continue;

                    string[] txt = game.GetElementsByTagName("th")[0].InnerText.Split(new string[] { "　" }, StringSplitOptions.RemoveEmptyEntries);
                    string webId = game.GetElementsByTagName("td")[0].Id;
                    // 資料錯誤就往下處理
                    if (txt.Length != 4)
                        continue;
                    if (string.IsNullOrEmpty(webId))
                        continue;
                    // 判斷文字
                    if (txt[0].IndexOf("（") != -1)
                        txt[0] = txt[0].Substring(0, txt[0].IndexOf("（")).Trim();
                    txt[0] = txt[0].Replace("月", "/").Replace("日", "").Trim();
                    // 轉換日期
                    if (DateTime.TryParse(txt[0], out gameDate) &&
                        DateTime.TryParse(gameDate.ToString("yyyy-MM-dd") + " " + txt[1], out gameTime))
                    {
                        txt = txt[2].Split(new string[] { "vs." }, StringSplitOptions.RemoveEmptyEntries);
                        // 判斷隊伍
                        if (txt.Length == 2)
                        {
                            GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                            schedule.AcH = acH;
                            // 設定
                            schedule.Away = txt[0];
                            schedule.Home = txt[1];

                            // 加入比賽資料
                            schedules[schedule.WebID] = schedule;
                        }
                    }
                }
            }
            // 傳回
            return schedules;
        }

        #endregion BKJP - 日本職籃
    }
}