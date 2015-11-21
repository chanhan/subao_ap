using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region 台棒

        #region 取得 PL 爆米花夏季聯盟賽程表

        // 2014/05/21: PL 爆米花夏季聯盟
        private Dictionary<string, GameInfo> GetSchedulesByPL(int allianceID, string gameType, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webPL.Document == null ||
                this.webPL.Document.Body == null)
                return null;

            HtmlElement divMain = this.webPL.Document.GetElementById("main");
            if (divMain == null) { return null; }

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();

            // 賽局編號(自訂)
            int gameNum = 0;
            string gameDateYear = null;
            string gameDateMonth = null;
            string webID = null;
            string sourceId = GetGameUseSourceID(allianceID, gameType);
            //DateTime gameDate = DateTime.Now;

            #region 取得年度/月份

            foreach (HtmlElement select in divMain.GetElementsByTagName("select"))
            {
                string name = select.GetAttribute("name").ToLower();
                if (!name.Equals("month") && !name.Equals("year")) { continue; }

                string value = "";
                // 取得值
                foreach (HtmlElement option in select.Children)
                {
                    if (option.GetAttribute("selected").ToLower().Equals("true"))
                    {
                        value = option.GetAttribute("value");
                        break;
                    }
                }

                // 月份
                if (name.Equals("month"))
                {
                    gameDateMonth = value;
                }
                // 年度
                else if (name.Equals("year"))
                {
                    gameDateYear = value;
                }
            }

            // 沒有年份/月份就離開
            if (String.IsNullOrEmpty(gameDateYear) || String.IsNullOrEmpty(gameDateMonth)) { return null; }

            #endregion

            #region 取得資料

            foreach (HtmlElement table in divMain.GetElementsByTagName("table"))
            {
                var tdColl = (from HtmlElement td in table.GetElementsByTagName("td")
                              let className = td.GetAttribute("className").ToLower()
                              where className.Contains("gridc")
                              select td);

                if (tdColl.Any())
                {
                    foreach (HtmlElement td in tdColl)
                    {
                        HtmlElementCollection spanColl = td.GetElementsByTagName("span");
                        // 小於 2 表示無賽事
                        if (spanColl.Count < 1) { continue; }

                        // 日期
                        string day = spanColl[0].InnerText;
                        // 沒有日期則不處理
                        if (String.IsNullOrEmpty(day)) { continue; }

                        //隊伍資訊
                        var teamColl = (from HtmlElement span in spanColl
                                        let className = span.GetAttribute("className").ToLower()
                                        where className.Contains("fcdark")
                                        select span);
                        // 沒有隊伍則不處理
                        if (!teamColl.Any()) { continue; }

                        foreach (HtmlElement span in teamColl)
                        {
                            // 範例: 崇越隼鷹《天母》合作金庫@12:00
                            string[] info = span.InnerText.Split(new string[] { "《", "》", "@" }, StringSplitOptions.None);

                            if (info.Length != 4) { continue; }

                            //比賽時間
                            DateTime gameTime;
                            string dateTime = String.Format("{0}/{1}/{2} {3}", gameDateYear, gameDateMonth, day, info[3]);
                            // 無法解析時間 不處理
                            if (!DateTime.TryParse(dateTime, out gameTime)) { continue; }

                            GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webID)
                            {
                                Away = info[0],
                                Home = info[2],
                                SourceID = sourceId,
                                AcH = acH
                            };

                            gameNum++;
                          
                            string key = gameNum.ToString();
                            schedules[key] = schedule;
                        }
                    }
                }
            }

            #endregion

            // 傳回
            return schedules;
        }

        #endregion

        #endregion

    }
}
