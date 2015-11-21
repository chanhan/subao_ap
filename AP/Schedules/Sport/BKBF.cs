using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        //private Dictionary<string, GameInfo> GetSchedulesByBkBF(int allianceID, string gameType, string webCountry, string webAllianceName, bool acH = false)
        //{
        //    Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
        //    Dictionary<string, string> TeamInfo = new Dictionary<string, string>();
        //    // 沒有資料就離開
        //    if (this.webBF.Document == null ||
        //        this.webBF.Document.Body == null ||
        //        this.webBF.Document.GetElementById("preload") == null ||
        //        this.webBF.Document.GetElementById("preload").Style == null ||
        //        this.webBF.Document.GetElementById("preload").Style.IndexOf("none") == -1)
        //        return null;
        //    HtmlElement dataDoc = this.webBF.Document.GetElementById("fsbody"); // fs
        //    // 判斷資料
        //    if (dataDoc != null)
        //    {
        //        // 取得來源ID
        //        string sourceId = GetGameUseSourceID(allianceID, gameType);
        //        // 資料
        //        foreach (HtmlElement table in dataDoc.GetElementsByTagName("table"))
        //        {
        //            HtmlElementCollection trDoc = table.GetElementsByTagName("tr");
        //            // 判斷資料
        //            if (trDoc.Count <= 1)
        //                continue;

        //            HtmlElementCollection tdDoc = trDoc[0].GetElementsByTagName("td");
        //            // 判斷資料
        //            if (tdDoc.Count != 2)
        //                continue;
        //            GameInfo schedule = null;
        //            HtmlAgilityPack.HtmlDocument spanHtml = new HtmlAgilityPack.HtmlDocument();
        //            spanHtml.LoadHtml(tdDoc[1].InnerHtml);
        //            HtmlAgilityPack.HtmlNode nodeCountry = spanHtml.DocumentNode.SelectSingleNode("span[@class='country left']/span[@class='name']/span[@class='country_part']");
        //            HtmlAgilityPack.HtmlNode nodeAlliance = spanHtml.DocumentNode.SelectSingleNode("span[@class='country left']/span[@class='name']/span[@class='tournament_part']");
        //            if (nodeCountry == null || nodeAlliance == null)//国家、联盟不为null，才往下处理
        //            {
        //                continue;
        //            }
        //            string country = nodeCountry.InnerText;
        //            country = country.Substring(0, country.LastIndexOf(':'));
        //            string alliance = nodeAlliance.InnerText;
        //            if (webCountry == country && webAllianceName == alliance)
        //            {
        //                for (int i = 1; i < trDoc.Count; i++)
        //                {
        //                    // 沒有編號就往下處理
        //                    if (trDoc[i].Id == null)
        //                        continue;
        //                    HtmlElement tr = trDoc[i];
        //                    string webId = tr.Id.Substring(tr.Id.LastIndexOf("_") + 1);
        //                    DateTime gameTime = DateTime.Now;
        //                    tdDoc = tr.GetElementsByTagName("td");
        //                    // 客隊
        //                    if (tdDoc.Count == 12)
        //                    {
        //                        #region 比賽時間

        //                        if (tdDoc[1].InnerText == null)
        //                            continue;
        //                        // 取得字串
        //                        string gameTimeStr = tdDoc[1].InnerText.Replace("\r\n", " ");
        //                        // 錯誤處理
        //                        try
        //                        {
        //                            gameTimeStr = gameTime.ToString("yyyy/MM/dd") + " " + gameTimeStr.Substring(0, 2) + ":" + gameTimeStr.Substring(3, 2);
        //                            // 轉成日期
        //                            if (!DateTime.TryParse(gameTimeStr, out gameTime))
        //                                continue;
        //                        }
        //                        catch { continue; } // 錯誤就往下處理
        //                        #endregion 比賽時間
        //                        schedule = null;
        //                        schedule = new GameInfo(allianceID, gameType, gameTime, webId);
        //                        schedule.AcH = acH;
        //                        // 設定
        //                        schedule.Away = tdDoc[3].InnerText.Trim();
        //                    }
        //                    // 主隊
        //                    if (tdDoc.Count == 7)
        //                    {
        //                        // 資料不正確就往下處理
        //                        if (schedule == null || schedule.WebID != webId)
        //                            continue;
        //                        // 設定
        //                        schedule.Home = tdDoc[0].InnerText.Trim();
        //                        // 加入比賽資料
        //                        schedule.SourceID = sourceId;
        //                        schedules[schedule.WebID] = schedule;
        //                        schedule = null;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    // 傳回
        //    return schedules;
        //}
        private bool bfLoadComplete = false;
        private string bfHtml = "";
        WebClient client = new WebClient();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        void Init()
        {
            client.DownloadDataCompleted += DownLoadCompleted;
            headers["X-GeoIP"] = "1";
            headers["Accept"] = "*/*";
            headers["X-utime"] = "1";
            headers["Accept-Language"] = "*";
            headers["User-Agent"] = "core";
            headers["X-Fsign"] = "SW9D1eZo";
            headers["X-Requested-With"] = "XMLHttpRequest";
            headers["Referer"] = "http://d.asiascore.com/x/feed/proxy";
            headers["Accept-Encoding"] = "gzip, deflate";
            headers["Host"] = "d.asiascore.com";
            headers["DNT"] = "1";
            headers["Cookie"] = "__utmc=190588603; __utma=190588603.693684061.1417682377.1417682377.1417682377.1; __utmb=190588603.1.10.1417682377; __utmz=190588603.1417682377.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); __utmt=1";
        }

        private void DownLoadCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.UserState != null && e.UserState.ToString() == "gzip")
            {
                StreamReader reader = reader = new StreamReader(new GZipStream(new MemoryStream(e.Result), CompressionMode.Decompress), System.Text.Encoding.UTF8);
                bfHtml = reader.ReadToEnd();
                this.btnWebId.Enabled = true;
                bfLoadComplete = true;
            }
        }
        public void DownloadData(Dictionary<string, string> header)
        {
            try
            {
                if (!client.IsBusy)
                {
                    if (client.Headers.Count == 0 && header != null)
                    {

                        foreach (KeyValuePair<string, string> data in header)
                        {
                            client.Headers.Add(data.Key, data.Value);
                        }
                    }
                    client.DownloadDataAsync(new Uri("http://d.asiascore.com/x/feed/f_3_0_8_en-asia_1~"), "gzip");
                }
            }
            catch
            {

            }
        }
        
        private Dictionary<string, GameInfo> GetSchedulesByBkBF(int allianceID, string gameType, string webCountry, string webAllianceName, bool acH = false)
        {
            // 沒有資料就離開
            if (!bfLoadComplete || string.IsNullOrEmpty(bfHtml))
                return null;

            Dictionary<string, GameInfo> result = new Dictionary<string, GameInfo>();
            GameInfo gameInfo = null;
            // 判斷資料
            string[] all = bfHtml.Split(new string[] { "¬~ZA÷" }, StringSplitOptions.RemoveEmptyEntries);

            // 聯盟 (第一筆是多餘的)
            for (int allianceIndex = 1; allianceIndex < all.Length; allianceIndex++)
            {
                string countryAndAlliance = all[allianceIndex].Split(new string[] { "¬ZB÷" }, StringSplitOptions.RemoveEmptyEntries)[0];
                string country = countryAndAlliance.Split(':')[0].Trim();
                string alliance = countryAndAlliance.Split(':')[1].Trim();
                if (webCountry != country || webAllianceName != alliance)//按國家、聯盟選擇
                {
                    continue;
                }
                // 比賽集合
                string[] games = ("ZA÷" + all[allianceIndex]).Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                string allianceName = null;
                // 聯盟資料
                // 比賽資料
                for (int gameIndex = 0; gameIndex < games.Length; gameIndex++)
                {
                    Dictionary<string, string> info = new Dictionary<string, string>();

                    #region 取出資料
                    string[] data = games[gameIndex].Split(new string[] { "¬" }, StringSplitOptions.RemoveEmptyEntries);

                    // 資料
                    foreach (string d in data)
                    {
                        string[] txt = d.Split(new string[] { "÷" }, StringSplitOptions.RemoveEmptyEntries);
                        // 記錄
                        info[txt[0]] = txt[1];
                    }
                    #endregion
                    #region 第一筆是聯盟
                    if (gameIndex == 0)
                    {
                        allianceName = info["ZA"];
                        continue;
                    }
                    else
                    {
                        // 沒有編號就往下處理
                        if (!info.ContainsKey("AA"))
                            continue;
                    }
                    #endregion

                    // 沒有隊伍就往下處理
                    if (!info.ContainsKey("AE") || !info.ContainsKey("AF"))
                        continue;

                    // 時間是 1970 年加上 Ti
                    DateTime gameTime = DateTime.Parse("1970/1/1 00:00:00").AddTicks(long.Parse(info["AD"]) * 10000000);
                    // 轉成台灣時間 UTC+8
                    gameTime = gameTime.AddHours(8);

                    gameInfo = null;
                    gameInfo = new GameInfo(allianceID, gameType, gameTime, info["AA"]);
                    gameInfo.Away = info["AE"].Replace("GOAL", "");
                    gameInfo.Home = info["AF"].Replace("GOAL", "");
                    gameInfo.AcH = acH;
 
                    result[gameInfo.WebID] = gameInfo;
                }
            }
            // 傳回
            return result;
        }
    }
}
