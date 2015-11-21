using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using NLog;

namespace Monitor
{
    class MonitorWeb
    {
        private static Logger _log = LogManager.GetLogger("MonitorWeb_Log");

        private System.Timers.Timer timer;
        private string[] data;
        private int[] dataNullTimes;//資料是空的檢查
        private string[] url;
        private DateTime[] chgTime;

        private bool checkWeb = false;

        public MonitorWeb()
        {
            string WebUrl = ConfigurationManager.AppSettings["WebUrl"];
            if (!string.IsNullOrEmpty(WebUrl))
            {
                url = WebUrl.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                //資料初始化
                data = new string[url.Length];
                dataNullTimes = new int[url.Length];
                chgTime = new DateTime[url.Length];
                for (int i = 0; i < url.Length; i++)
                {
                    chgTime[i] = DateTime.Now;
                    dataNullTimes[i] = 0;
                }

                GetWebRequest();//連線取得資料

                //查詢通知 監控
                this.timer = new System.Timers.Timer();
                this.timer.Elapsed += OnMonitorTimedEvent;

                this.timer.Interval = 60 * 1000;//每分鐘檢查一次
                this.timer.Enabled = true;

                this.checkWeb = true;
            }
        }

        private void OnMonitorTimedEvent(object source, ElapsedEventArgs e)
        {
            GetWebRequest();
        }

        private void GetWebRequest()
        {
            string requestUrl = "";
            try
            {                    
                for (int i = 0; i < url.Length; i++)
                {
                    requestUrl = url[i] + "/Dependency.aspx";

                    WebRequest req = WebRequest.Create(requestUrl);
                    WebResponse resp = req.GetResponse();
                    StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);

                    this.data[i] = sr.ReadToEnd().Trim();//資料寫入

                    sr.Close();
                    resp.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error("GetWebRequest Error Message:{0},\r\nStackTrace:{1}\r\n WebUrl:{2}", ex.Message, ex.StackTrace, requestUrl);
            }
        }
        
        public void DataAnalysis(string gameType, string sGameDate, DateTime changeTime)//資料分析
        {
            if (this.checkWeb == false)
                return;

            DateTime gameDate;
            if (!DateTime.TryParse(sGameDate, out gameDate))
                return;

            gameType = gameType.ToLower();

            for (int i = 0; i < url.Length; i++)
            {
                try
                {
                    if (string.IsNullOrEmpty(data[i]))//沒有資料
                    {
                        //if (dataNullTimes[i] < 5)
                        //    dataNullTimes[i]++;
                        //else
                        //{
                        //    //太久沒資料，強迫更新
                        //    dataNullTimes[i] = 0;
                        //    chgTime[i] = DateTime.Now;//更新時間
                        //    RequestClearCache("NULL", url[i]);
                        //}
                        continue;
                    }

                    double chgTimeDiff = new TimeSpan(DateTime.Now.Ticks - chgTime[i].Ticks).TotalSeconds;//該網站的清除間隔
                    if (chgTimeDiff < 60)//短時間內不會對同一個來源重複清緩存
                        continue;

                    string[] sArray = Regex.Split(data[i], "<hr />", RegexOptions.IgnoreCase);
                    if (sArray.Length == 1)//沒有查詢通知資料
                    {
                        if (dataNullTimes[i] < 5)
                            dataNullTimes[i]++;
                        else
                        {
                            //太久沒資料，強迫更新
                            dataNullTimes[i] = 0;
                            chgTime[i] = DateTime.Now;//更新時間
                            RequestClearCache("NULL", url[i]);
                        }
                        continue;
                    }

                    foreach (string str in sArray)
                    {
                        var items = Regex.Split(str, "<br />", RegexOptions.IgnoreCase).Select(s => Regex.Split(s, " : "));
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        foreach (var item in items)
                            if (item.Length > 1)
                                dict.Add(item[0], item[1]);

                        if (dict.Count <= 0)
                            continue;

                        if (gameType.IndexOf(dict["Name"]) != 0)
                            if (dict["Name"] != "tw" || (dict["Name"] == "tw" && gameType.IndexOf("bbtw") != 0))
                                continue;

                        DateTime webGameDateBegin, webGameDateEnd, LastChangeTime;
                        //舊資料格式
                        if (dict.ContainsKey("GameDate") && DateTime.TryParse(dict["GameDate"], out webGameDateBegin) && webGameDateBegin.Date == gameDate.Date)
                        {
                            if (DateTime.TryParse(dict["LastChangeTime"], out LastChangeTime))
                            {
                                double diffTime1 = new TimeSpan(changeTime.Ticks - LastChangeTime.Ticks).TotalSeconds;
                                if (diffTime1 >= 30)//前台更新時間與抓分寫入時間 差異超過1分鐘
                                {
                                    chgTime[i] = DateTime.Now;//更新時間
                                    RequestClearCache(dict["Name"], url[i]);
                                    break;
                                }
                            }
                        }
                            //新資料格式
                        else if(dict.ContainsKey("GameDateBegin") && dict.ContainsKey("GameDateEnd") && 
                            DateTime.TryParse(dict["GameDateBegin"], out webGameDateBegin) && DateTime.TryParse(dict["GameDateEnd"], out webGameDateEnd) &&
                            webGameDateBegin.Date <= gameDate.Date && webGameDateEnd.Date >= gameDate.Date)
                        {
                            if (DateTime.TryParse(dict["LastChangeTime"], out LastChangeTime))
                            {
                                double diffTime1 = new TimeSpan(changeTime.Ticks - LastChangeTime.Ticks).TotalSeconds;
                                if (diffTime1 >= 30)//前台更新時間與抓分寫入時間 差異超過1分鐘
                                {
                                    chgTime[i] = DateTime.Now;//更新時間
                                    RequestClearCache(dict["Name"], url[i]);
                                    break;
                                }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    _log.Error("DataAnalysis Error Message:{0},\r\nStackTrace:{1}\r\n gameType: {2} gameDate: {3}", ex.Message, ex.StackTrace, gameType, sGameDate);
                }
            }
        }

        private static void RequestClearCache(string caller, string url)//清緩存
        {
            if (!string.IsNullOrEmpty(url))
            {
                url = string.Format("{0}/ajax/CacheAction.aspx?ActionName=clear&User=Monitor ( {1} )", url, caller);

                WebRequest req = WebRequest.Create(url);
                RequestState rs = new RequestState();
                rs.url = url;
                rs.Request = req;
                if (!string.IsNullOrEmpty(caller))
                    rs.caller = caller;

                req.BeginGetResponse(new AsyncCallback(RespCallback), rs);//非同步處理
            }
            else
            {
                _log.Error("no set url");
            }

        }

        private static void RespCallback(IAsyncResult ar)
        {
            try
            {
                RequestState rs = (RequestState)ar.AsyncState;
                WebRequest req = rs.Request;
                HttpWebResponse response = req.EndGetResponse(ar) as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (string.IsNullOrEmpty(rs.caller))
                        rs.caller = "Monitor";

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Format(" {0} clear!!", rs.caller));
                    sb.AppendLine("url: " + rs.url);

                    _log.Info(sb.ToString());
                }
                response.Close();
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("RespCallback Error!!");
                sb.AppendLine("Message: " + ex.Message);
                sb.AppendLine("StackTrace:");
                sb.AppendLine(ex.StackTrace);

                _log.Error(sb.ToString());
            }
        }

        public class RequestState
        {
            public WebRequest Request = null;
            public string url = "";
            public string state = "";
            public string caller;// = "AP";
        }
    }
}
