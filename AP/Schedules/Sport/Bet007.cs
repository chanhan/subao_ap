using SHGG.DataStructerService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Schedules
{
    public partial class FrmMain
    {
        #region 奧訊

        /// <summary>
        /// 取得奧訊賽程
        /// </summary>
        /// <param name="allianceID"></param>
        /// <param name="gameType"></param>
        /// <param name="lsID"></param>
        /// 
        /// <param name="acH"></param>
        /// <returns></returns>
        private Dictionary<string, GameInfo> GetSchedulesByBet007(int allianceID, string gameType, string lsID, bool acH = false)
        {
            DateTime sDate = txtBet007SDate.Value;
            DateTime eDate = txtBet007EDate.Value;
            DateTime currentDate = sDate;

            WebClient client = new WebClient();
            client.Encoding = Encoding.GetEncoding("gb2312");
            string result = string.Empty;
            string sourceId = GetGameUseSourceID(allianceID, gameType);

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();

            // 尋覽指定日期區間取得資料
            while (currentDate.Date.CompareTo(eDate.Date) <= 0)
            {
                // 下載資料
                try
                {
                    result = client.DownloadString(new Uri(string.Format(@"http://dxbf.bet007.com/nba_date.aspx?time={0}", currentDate.ToString("yyyy-MM-dd"))));
                }
                catch
                {
                    result = client.DownloadString(new Uri(string.Format(@"http://dxbf.titan007.com/nba_date.aspx?time={0}", currentDate.ToString("yyyy-MM-dd"))));
                }

                if (!string.IsNullOrEmpty(result))
                {
                    // 處理 XML
                    XmlAdapter xmlAdapter = new XmlAdapter(result, false);
                    xmlAdapter.GoToNode("c", "m");

                    // 取得所有比賽集合
                    List<string> gameRecord = xmlAdapter.GetAllSubColumns("h");
                    if (gameRecord.Count == 0)
                        return null;

                    // 尋覽取回的資料集
                    foreach (var game in gameRecord)
                    {
                        // 切割資料欄位
                        string[] gameCell = game.Split('^');
                        // 判斷聯盟ID是否等於指定的聯盟
                        if (gameCell[37] == lsID)
                        {
                            GameInfo schedule = null;

                            // 比賽ID
                            string webId = gameCell[0];

                            // 比賽時間
                            DateTime gameTime = DateTime.Parse(gameCell[42] + "年" + gameCell[4].Replace("<br>", " "));

                            schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                            schedule.AcH = acH; // 主客調換

                            // 主隊
                            string homeName =  gameCell[8].Split(',')[2];
                            schedule.Home = homeName.Substring(0, (homeName.IndexOf("[") >= 0) ? homeName.IndexOf("[") : homeName.Length);

                            // 客隊
                            string awayName = gameCell[10].Split(',')[2];
                            schedule.Away = awayName.Substring(0, (awayName.IndexOf("[") >= 0) ? awayName.IndexOf("[") : awayName.Length);

                            // 指定來源
                            schedule.SourceID = sourceId;
     
                            // 加入賽事
                            schedules[schedule.WebID] = schedule;
                        }
                        else
                            continue;
                    }
                }

                currentDate = currentDate.AddDays(1);
            }

            return schedules;
        }

        #endregion 奧訊
    }
}