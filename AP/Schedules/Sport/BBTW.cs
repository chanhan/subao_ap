using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region 台棒

        #region 取得台棒賽程表
        
        private Dictionary<string, GameInfo> GetSchedulesByCPBL(int allianceID, string gameType, bool acH = false)
        {
            // 沒有資料就離開
            if (this.webCPBL.Document == null ||
                this.webCPBL.Document.Body == null)
                return null;

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();

            // 賽局編號(自訂)
            int gameNum = 0;
            string gameDateYear = null;
            string gameDateMonth = null;
            string sourceId = GetGameUseSourceID(allianceID, gameType);
            DateTime gameDate = DateTime.Now;

            #region 取得年份
            HtmlElement yearDoc = this.webCPBL.Document.GetElementById("ctl00_cphBox_ddl_year");
            // 判斷資料
            if (yearDoc != null)
            {
                foreach (HtmlElement select in yearDoc.Children)
                {
                    // 判斷選擇
                    if (select.GetAttribute("selected").ToLower().Equals("selected") || select.GetAttribute("selected").ToLower().Equals("true"))
                    {
                        gameDateYear = select.GetAttribute("value");
                        break;
                    }
                }
            }
            // 沒有年份就離開
            if (String.IsNullOrEmpty(gameDateYear)) { return null; }

            #endregion
            #region 取得月份
            HtmlElement monthDoc = this.webCPBL.Document.GetElementById("ctl00_cphBox_ddl_month");
            // 判斷資料
            if (monthDoc != null)
            {
                foreach (HtmlElement select in monthDoc.Children)
                {
                    // 判斷選擇
                    if (select.GetAttribute("selected").ToLower().Equals("selected") || select.GetAttribute("selected").ToLower().Equals("true"))
                    {
                        string[] tmp= select.GetAttribute("value").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        if (tmp.Length > 0)
                        {
                            gameDateMonth = tmp[0];
                            break;
                        }
                    }
                }
            }
            // 沒有月份就離開
            if (String.IsNullOrEmpty(gameDateMonth)) { return null; }

            #endregion

            #region 取得資料
            // 取出 class="day" 起始的 table (表示每日賽程的 table)
            List<HtmlElement> tableList = (from HtmlElement tb
                                                    in this.webCPBL.Document.GetElementsByTagName("table")
                                           where tb.GetAttribute("className").StartsWith("day") == true
                                           select tb).ToList<HtmlElement>();

            foreach (HtmlElement table in tableList)
            {
                var trList = table.Children[0].Children.Cast<HtmlElement>().ToList<HtmlElement>();
                // 沒有比賽, 不處理
                if (trList.Count <= 1) { continue; }

                // 取得日期
                HtmlElement trDate = trList[0];
                string gameDateStr = String.Format("{0}/{1}/{2}", gameDateYear, gameDateMonth, trDate.Children[0].InnerText);
                // 轉成日期失敗就往下處理
                if (!DateTime.TryParse(gameDateStr, out gameDate)) { continue; }

                // 找到隊伍標籤
                List<HtmlElement> trTeamList = (from HtmlElement tr in trList
                                                where tr.GetAttribute("className").Equals("team") == true
                                                select tr).ToList<HtmlElement>();

                // 當日沒賽事, 不處理
                if (trTeamList.Count == 0) { continue; }

                foreach (HtmlElement trTeam in trTeamList)
                {
                    string teamAway = null;
                    string teamHome = null;
                    DateTime gameTime = DateTime.Now;
                    string webId = null;

                    HtmlElementCollection imgTeam = trTeam.GetElementsByTagName("img");
                    // 僅有一隊, 不處理
                    if (imgTeam.Count < 2) { continue; }
                    for (int i=0; i < imgTeam.Count; i++)
                    {
                        string src = imgTeam[i].GetAttribute("src");
                        // 僅取得圖片名稱
                        int idx = src.LastIndexOf("/");
                        src = src.Substring((idx + 1), src.Length - (idx + 1));

                        // 隊伍:左客右主(先抓到的項目為客隊)
                        if (i == 0)
                        {
                            teamAway = src;
                        }
                        else
                        {
                            teamHome = src;
                        }
                    }

                    // tr[class='game'] 取得比賽資訊
                    int teamIdx = trList.IndexOf(trTeam);
                    if (teamIdx >= trList.Count) { continue; }

                    HtmlElement trGame = trList[teamIdx + 1];
                    // 找不到比賽資訊, 不處理
                    if (trGame == null) { continue; }


                    HtmlElement tbGameInfo = (from HtmlElement tb in trGame.GetElementsByTagName("table")
                                              select tb).DefaultIfEmpty(null).FirstOrDefault();
                    // 找不到比賽資訊, 不處理
                    if (tbGameInfo == null) { continue; }

                    // tr[class='normal'] 取得 WebID 節點
                    HtmlElementCollection trGameInfoList = tbGameInfo.Children[0].Children;
                    HtmlElementCollection child = trGameInfoList[0].Children;
                    if (child.Count > 0)
                    {
                        HtmlElement tbWebId = child[0];
                        HtmlElementCollection thColl = tbWebId.GetElementsByTagName("th");
                        if (thColl.Count == 3)
                        {
                            // WebId
                            //webId = thColl[1].InnerText;
                        }
                    }

                    // tr 取得時間節點
                    child = trGameInfoList[1].Children;
                    if (child.Count > 0)
                    {
                        HtmlElement tbGameTime = child[0];
                        HtmlElementCollection tdColl =tbGameTime.GetElementsByTagName("td");
                        if (tdColl.Count == 3)
                        {
                            HtmlElement tdTime = tdColl[1];
                            // 時間 <td> 下還有 element: 比賽結束 往下處理
                            if (tdTime.Children.Count > 0) { continue; }
                            string timeStr = tdTime.InnerText;

                            string dateTime = String.Format("{0:yyyy/MM/dd} {1}", gameDate, timeStr);
                            // 若無法解析比賽時間, 往下處理
                            if (!DateTime.TryParse(dateTime, out gameTime)) { continue; }
                        }
                        else { continue; }
                    }
                    else { continue; }

                    gameNum++;

                    // 產生賽程物件
                    GameInfo schedule = new GameInfo(allianceID, gameType, gameTime, webId)
                    {
                        Away = teamAway,
                        Home = teamHome,
                        SourceID = sourceId,
                        AcH = acH
                    };

                    string key = gameNum.ToString();
                    schedules[key] = schedule;
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
