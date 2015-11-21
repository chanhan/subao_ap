using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Schedules
{
    public partial class FrmMain
    {
        #region BBMX2 - 墨西哥夏季聯盟 (LMB)

        private Dictionary<string, GameInfo> GetSchedulesByLMB(int allianceID, string gameType, bool acH = false)
        {

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;

            DateTime startDate =this.dtpLMBSDate.Value.Date;
            DateTime endDate = this.dtpLMBEDate.Value.Date;

            while (startDate.CompareTo(endDate) <= 0)
            {
                string gameDateStr = startDate.ToString("yyyy-MM-dd");

                // 轉成日期
                if (DateTime.TryParse(gameDateStr, out gameDate))
                {
                    WebClient web = new WebClient();
                    // 下載資料
                    string xmlText = web.DownloadString("http://www.milb.com/lookup/xml/named.schedule_vw_complete.bam?game_date='" + gameDate.ToString("yyyy/MM/dd").Replace("-", "/") + "'&season=" + gameDate.ToString("yyyy") + "&league_id=125");
                    // 判斷網頁完成
                    if (!string.IsNullOrEmpty(xmlText))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        // 錯誤處理
                        try
                        {
                            xmlDoc.LoadXml(xmlText);
                            // 判斷資料
                            if (xmlDoc["schedule_vw_complete"] != null &&
                                xmlDoc["schedule_vw_complete"]["queryResults"] != null)
                            {
                                foreach (XmlNode info in xmlDoc["schedule_vw_complete"]["queryResults"].ChildNodes)
                                {
                                    // 判斷時間
                                    if (DateTime.TryParse(info.Attributes["game_time_local"].Value, out gameTime))
                                    {
                                        // 計算時間
                                        if (info.Attributes["time_zone_local"] != null)
                                        {
                                            int zone = 0;
                                            int.TryParse(info.Attributes["time_zone_local"].Value, out zone);
                                            gameTime = gameTime.AddHours(0 - zone + 8);
                                        }
                                        // 建立賽程
                                        GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, "gid_" + info.Attributes["game_id"].Value.Replace("/", "_").Replace("-", "_"));
                                        schedule.AcH = acH;
                                        // 設定
                                        schedule.Away = info.Attributes["away_team_short"].Value;
                                        schedule.Home = info.Attributes["home_team_short"].Value;

                                        // 加入比賽資料
                                        schedules[schedule.WebID] = schedule;
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    startDate = startDate.AddDays(1);
                }
            }
            // 傳回
            return schedules;
        }

        #endregion BBMX2 - 墨西哥夏季聯盟 (LMB)
    }
}