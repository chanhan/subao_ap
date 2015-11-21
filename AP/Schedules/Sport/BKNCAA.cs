using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace Schedules
{
    public partial class FrmMain
    {
        DateTime gameTime;
        #region BKNCAA - 美國大學男子籃球聯賽

        private Dictionary<string, GameInfo> GetSchedulesByNCAA(int allianceID, string gameType, bool acH = false)
        {
            string sourceId = GetGameUseSourceID(allianceID, gameType);
            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();

            DateTime startDate = dtpNCAASDate.Value.Date;
            DateTime endDate = dtpNCAAEDate.Value.Date;

            WebClient w = new WebClient();

            while (startDate <= endDate)
            {
                string url = String.Format(@"http://data.ncaa.com/jsonp/scoreboard/basketball-men/d1/{0}/{1:00}/{2:00}/scoreboard.html", startDate.Year, startDate.Month, startDate.Day);

                // 設定下一日
                startDate = startDate.AddDays(1);

                string data= String.Empty;

                try
                {
                    data = w.DownloadString(url);
                }
                catch
                {
                    continue;
                }

                if (!String.IsNullOrEmpty(data))
                {
                    try
                    {
                        // 取得 json 資料
                        data = data.Replace(data.Substring(0, data.IndexOf("{")), "")
                             .Replace(data.Substring(data.LastIndexOf("}") + 1), "");

                        JObject obj = JsonConvert.DeserializeObject<JObject>(data);
                        JArray array = obj["scoreboard"] as JArray;
                        if (array != null)
                        {
                            JObject scoreboard = array[0] as JObject;
                            JArray games = scoreboard["games"] as JArray;
                            // 賽程資料
                            if (games != null)
                            {
                                foreach (JObject game in games)
                                {
                                    string comment = String.Empty;
                                    string webID = game["id"].ToString();
                                    string gameState = game["gameState"].ToString().ToLower();
                                    // 賽程結束, 不處理
                                    if (gameState.Equals("final")) { continue; }

                                    string date = game["startDate"].ToString();
                                    string time = game["startTime"].ToString().ToUpper();
                                    // 賽程時間未定, 標記訊息
                                    if (time.Equals("TBA")) { comment = "(尚未確認開賽時間,賽事暫時無法建立)"; }

                                    //// 取得 Utc 紀元時間
                                    //double epoch = Convert.ToDouble(game["startTimeEpoch"].ToString());
                                    //DateTime gameTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch);
                                    
                                    string [] strTime=time.Split(' ');
                                    if (strTime.Length>=3)
                                    {
                                        if (strTime[1]=="PM")
                                        {
                                           DateTime dateTemp= (Convert.ToDateTime(date+" "+strTime[0]).AddHours(12));
                                           gameTime = dateTemp.AddHours(13);
                                        }
                                        else if (strTime[1] == "AM")
                                        {
                                            DateTime dateTemp = (Convert.ToDateTime(date + " " + strTime[0]));
                                            gameTime = dateTemp.AddHours(13);
                                        }
                                    }
                                    string home = String.Empty;
                                    string away = String.Empty;
                                    //主場隊伍
                                    JObject teamHome = game["home"] as JObject;
                                    if (teamHome != null) { home = teamHome["nameRaw"].ToString(); }
                                    //客場對伍
                                    JObject teamAway = game["away"] as JObject;
                                    if (teamAway != null) { away = teamAway["nameRaw"].ToString(); }

                                    //若主隊或客隊是空值, 不處理
                                    if (String.IsNullOrEmpty(home) || String.IsNullOrEmpty(away)) { continue; }

                                    GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webID)
                                    {
                                        Home = home,
                                        Away = away,
                                        SourceID = sourceId,
                                        AcH = acH,
                                        Comment = comment
                                    };

                                    schedules[webID] = schedule;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }

                    // 釋放 webclient
                    w.Dispose();
                }
            }

            return schedules;
        }

        #endregion BKNCAA - 美國大學男子籃球聯賽
    }
}