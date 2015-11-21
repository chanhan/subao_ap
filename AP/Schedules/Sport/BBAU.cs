using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Schedules
{
    public partial class FrmMain
    {
        #region BBAU - 澳洲棒球 (ABL)

        private Dictionary<string, GameInfo> GetSchedulesByABL(int allianceID, string gameType, bool acH = false)
        {

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            DateTime gameDate = DateTime.Now;
            DateTime gameTime = DateTime.Now;

            DateTime startDate = this.dtpABLSDate.Value.Date;
            DateTime endDate = this.dtpABLEDate.Value.Date;

            while (startDate.CompareTo(endDate) <= 0)
            {
                string gameDateStr = startDate.ToString("yyyy-MM-dd");

                // 轉成日期
                if (DateTime.TryParse(gameDateStr, out gameDate))
                {
                    WebClient web = new WebClient();
                    // 錯誤處理
                    try
                    {
                        // 下載資料 http://web.theabl.com.au/gdcross/components/game/win/year_2014/month_12/day_20/master_scoreboard.xml
                        string xmlText = web.DownloadString("http://web.theabl.com.au/gdcross/components/game/win/year_" + gameDate.ToString("yyyy") + "/month_" + gameDate.ToString("MM") + "/day_" + gameDate.ToString("dd") + "/master_scoreboard.xml");
                        // 判斷網頁完成
                        if (!string.IsNullOrEmpty(xmlText))
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            //加载资料
                            xmlDoc.LoadXml(xmlText);

                            XmlElement rootElem = xmlDoc.DocumentElement;
                            //获取到ABL联盟的gamelist
                            XmlNodeList personNodes = rootElem.SelectNodes("//game[@league='ABL']");
                            //检查有没有读到资料
                            if (personNodes == null && personNodes.Count == 0) { return null; }
                            //循环game节点
                            foreach (XmlNode node in personNodes)
                            {
                                //node 为game节点
                                XmlElement game = (XmlElement)node;
                                string webID = game.GetAttribute("id").Trim();

                                //日期时间处理
                                string sDateTime = string.Empty;
                                string sZone = game.GetAttribute("time_zone").ToUpper();//时区
                                string sAmPm = game.GetAttribute("ampm").ToUpper();//上午or下午
                                string sTime = game.GetAttribute("time");//时间
                                string sDate = Convert.ToDateTime(game.GetAttribute("original_date")).ToString("yyyy-MM-dd");//只取日期部分

                                ////转为24小时制
                                //string time = string.Format("{0} {1} {2}", sDate, sAmPm, sTime);
                                //DateTime.TryParseExact(time, "yyyy-M-d tt h:m", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out gameTime);

                                gameTime = Convert.ToDateTime(string.Concat(sDate, " ", sTime));
                                if (sAmPm.ToLower() == "pm")
                                {
                                    gameTime = gameTime.AddHours(12);
                                }

                                //转为台湾时间
                                switch (sZone)
                                {
                                    case "AWST":
                                        //等于台湾时间不需要转换
                                        break;
                                    case "AEST":
                                        //UTC+10  转为台湾时间 -2H
                                        gameTime = gameTime.AddHours(-2);
                                        break;
                                    case "ACDT":
                                    case "ACT":
                                        //UTC+10:30 转为台湾时间 -2.5H
                                        gameTime = gameTime.AddHours(-2.5);
                                        break;
                                    case "AEDT":
                                    case "AET":
                                        //UTC +11 转为台湾时间 -3H
                                        gameTime = gameTime.AddHours(-3);
                                        break;
                                    default:
                                        //列外时区 按照2小时计算
                                        gameTime = gameTime.AddHours(-2);
                                        break;
                                }

                                //双重赛事的 第二场时间 暂定为第一场时间+2H
                                int iCount = 1;
                                string[] arrWebId = webID.Trim().Split('-');
                                int.TryParse(arrWebId[2], out iCount);
                                if (iCount > 1)
                                {
                                    string oldWebId = string.Concat(arrWebId[0], "-", arrWebId[1], "-", iCount - 1);
                                    if (schedules.ContainsKey(oldWebId))
                                    {
                                        gameTime = schedules[oldWebId].GameTime.AddHours(2);
                                    }
                                }

                                // 建立賽程
                                GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webID);

                                //队伍名称 【城市 队名】中间空格
                                schedule.Home = string.Format("{0} {1}", game.GetAttribute("home_team_city"), game.GetAttribute("home_team_name"));
                                schedule.Away = string.Format("{0} {1}", game.GetAttribute("away_team_city"), game.GetAttribute("away_team_name"));
                                // 加入比賽資料
                                schedules[schedule.WebID] = schedule;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //如果错误是属于 来源网404 就不报错(来源网的问题不属于程序问题)
                        if (ex.Message.IndexOf("404") == -1)
                        {
                            throw new Exception(ex.Message);
                        }
                    }

                    startDate = startDate.AddDays(1);
                }
            }
            // 傳回
            return schedules;
        }
        #endregion BBAU - 澳洲棒球 (ABL)
    }
}