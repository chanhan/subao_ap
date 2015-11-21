using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text;

namespace Schedules
{
    public partial class FrmMain
    {
        #region Asiascore

        private Dictionary<string, GameInfo> GetSchedulesByAsiascore(int allianceID, string gameType, string nation, string category, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webAsiascore.Document == null ||
                this.webAsiascore.Document.Body == null ||
                this.webAsiascore.Document.GetElementById("preload") == null ||
                this.webAsiascore.Document.GetElementById("preload").Style == null ||
                this.webAsiascore.Document.GetElementById("preload").Style.IndexOf("none") == -1)
                return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            Dictionary<string, string> TeamInfo = new Dictionary<string, string>();
            HtmlElement dataDoc = this.webAsiascore.Document.GetElementById("fsbody"); // fs
            // 判斷資料
            if (dataDoc != null)
            {
                // 取得來源ID
                string sourceId = GetGameUseSourceID(allianceID, gameType);

                string[] UfcGameDate = null;
                foreach (HtmlElement span in dataDoc.GetElementsByTagName("span"))
                {
                    if (span.GetAttribute("classname") == "day today")//取得開賽日期
                    {
                        UfcGameDate = span.InnerText.Split(new char[] { ' ' })[0].Split(new char[] { '/' });// 22/02
                        break;
                    }
                }

                // 資料
                foreach (HtmlElement table in dataDoc.GetElementsByTagName("table"))
                {
                    HtmlElementCollection trDoc = table.GetElementsByTagName("tr");
                    // 判斷資料
                    if (trDoc.Count <= 1)
                        continue;

                    HtmlElementCollection tdDoc = trDoc[0].GetElementsByTagName("td");
                    // 判斷資料
                    if (tdDoc.Count != 2)
                        continue;

                    string gameNation = tdDoc[1].InnerText.ToLower().Trim();
                    GameInfo schedule = null;
                    // 判斷資料
                    if (gameNation.IndexOf(nation) == -1 ||
                        gameNation.IndexOf(category) == -1)
                        continue;

                    for (int i = 1; i < trDoc.Count; i++)
                    {
                        // 沒有編號就往下處理
                        if (trDoc[i].Id == null)
                            continue;

                        HtmlElement tr = trDoc[i];
                        string webId = tr.Id.Substring(tr.Id.LastIndexOf("_") + 1);
                        DateTime gameTime = DateTime.Now;
                        tdDoc = tr.GetElementsByTagName("td");
                        // 客隊
                        if (tdDoc.Count == 12 ||
                            (gameType.ToLower() == "ufc" && tdDoc.Count == 7))//格鬥賽
                        {
                            #region 比賽時間

                            if (tdDoc[1].InnerText == null)
                                continue;
                            // 取得字串
                            string gameTimeStr = tdDoc[1].InnerText.Replace("\r\n", " ");
                            // 錯誤處理
                            try
                            {
                                if (gameType.ToLower() == "ufc" && UfcGameDate != null)//格鬥賽
                                {
                                    //比賽時間
                                    gameTimeStr = string.Format("{0}/{1}/{2} {3}", gameTime.ToString("yyyy"), UfcGameDate[1], UfcGameDate[0], gameTimeStr);

                                    string[] magnitude = {"FLYWEIGHT", "BANTAMWEIGHT", "FEATHERWEIGHT", "LIGHTWEIGHT",
                                        "WELTERWEIGHT", "MIDDLEWEIGHT", "LIGHT HEAVYWEIGHT", "HEAVYWEIGHT"};
                                    foreach (HtmlElement span in table.GetElementsByTagName("span"))
                                    {
                                        if (span.GetAttribute("classname") == "country_part")//取得量級
                                        {
                                            string w = span.InnerText.Replace(":", "").Trim();
                                            for (int index = 0; index < magnitude.Length; index++)
                                            {
                                                if (w == magnitude[index])
                                                {
                                                    allianceID = index + 2; //[dbo].[UFCAlliance] 區別量級
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // 判斷日期並格式化日期
                                    if (gameTimeStr.Length > 5)
                                    {
                                        gameTimeStr = gameTime.ToString("yyyy") + "/" + gameTimeStr.Substring(3, 2) + "/" + gameTimeStr.Substring(0, 2) + " " + gameTimeStr.Substring(7);
                                    }
                                    else
                                    {
                                        gameTimeStr = gameTime.ToString("yyyy/MM/dd") + " " + gameTimeStr.Substring(0, 2) + ":" + gameTimeStr.Substring(3, 2);
                                    }
                                }
                                // 轉成日期
                                if (!DateTime.TryParse(gameTimeStr, out gameTime))
                                    continue;

                                //新年1月分 跨年问题
                                if (gameTime.Month == 1 && DateTime.Now.Month == 12)
                                {
                                    gameTime = gameTime.AddYears(1);
                                }
                            }
                            catch { continue; } // 錯誤就往下處理
                            // 开赛时间小于当前时间 就不于显示 添加
                            if (gameTime < DateTime.Now)
                            {
                                continue;
                            }  
                            #endregion 比賽時間

                            schedule = null;
                            schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                            schedule.AcH = acH;
                            // 設定
                            schedule.Away = tdDoc[3].InnerText.Trim();
                        }
                        // 主隊
                        if ((tdDoc.Count == 7 && gameType.ToLower().IndexOf("ufc") == -1) ||
                            (tdDoc.Count == 1 && gameType.ToLower().IndexOf("ufc") > -1))//格鬥賽
                        {
                            // 資料不正確就往下處理
                            if (schedule == null || schedule.WebID != webId)
                                continue;

                            // 設定
                            schedule.Home = tdDoc[0].InnerText.Trim();

                            // 加入比賽資料
                            schedule.SourceID = sourceId;
                            if (schedule.GameType.ToLower() == "ufc")//格鬥賽 預設比賽類型 3回合
                                schedule.GameType += "3";

                            schedules[schedule.WebID] = schedule;
                            schedule = null;
                        }
                        // 主隊+客隊 (新格式)
                        if (tdDoc.Count == 4 || tdDoc.Count == 6)
                        {
                            #region 比賽時間

                            if (tdDoc[1].InnerText == null)
                                continue;
                            // 取得字串
                            string gameTimeStr = tdDoc[1].InnerText.Replace("\r\n", " ");
                            // 錯誤處理
                            try
                            {

                                // 判斷日期並格式化日期
                                if (gameTimeStr.Length > 5)
                                {
                                    gameTimeStr = gameTime.ToString("yyyy") + "/" + gameTimeStr.Substring(3, 2) + "/" + gameTimeStr.Substring(0, 2) + " " + gameTimeStr.Substring(7);
                                }
                                else
                                {
                                    gameTimeStr = gameTime.ToString("yyyy/MM/dd") + " " + gameTimeStr.Substring(0, 2) + ":" + gameTimeStr.Substring(3, 2);
                                }
                                // 轉成日期
                                if (!DateTime.TryParse(gameTimeStr, out gameTime))
                                    continue;

                                //新年1月分 跨年问题
                                if (gameTime.Month == 1 && DateTime.Now.Month == 12)
                                {
                                    gameTime = gameTime.AddYears(1);
                                }
                            }
                            catch { continue; } // 錯誤就往下處理
                            // 开赛时间小于当前时间 就不于显示 添加
                            if (gameTime < DateTime.Now)
                            {
                                continue;
                            }                            
                            #endregion 比賽時間

                            schedule = null;
                            schedule = new GameInfo(allianceID, gameType, gameTime, webId);
                            schedule.AcH = acH;
                            // 設定
                            schedule.Away = tdDoc[2].InnerText.Trim();
                            schedule.Home = tdDoc[3].InnerText.Trim();

                            // 加入比賽資料
                            schedule.SourceID = sourceId;
                            schedules[schedule.WebID] = schedule;
                            schedule = null;
                        }
                    }
                }
            }

            // 傳回
            return schedules;
        }       
        #endregion Asiascore
    }
}