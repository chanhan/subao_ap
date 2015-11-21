using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Configuration;
using System.Net;
using System.IO;
using NLog;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.Web;
using Newtonsoft.Json.Linq;

namespace GameScoresApp
{
    class ProcessRequest
    {
        private static Logger _logRequest = LogManager.GetLogger("Request_Log");
        private static Logger _logQueue = LogManager.GetLogger("Queue_Log");

        private static string _BigballAddress = null;
        private static ConcurrentQueue<RequestState> requestQueue = new ConcurrentQueue<RequestState>();
        private static int iResendTime = 60;//預設60秒
        private static int iResendTimes = 5;//預設5次

        private static Action<string> SetSendRequestTxt;//設定請求目的

        public static void init(Action<string> action)
        {
            int.TryParse(ConfigurationManager.AppSettings["ResendTime"], out iResendTime);
            if (iResendTime < 60)
                iResendTime = 60;

            int.TryParse(ConfigurationManager.AppSettings["ResendTimes"], out iResendTimes);
            if (iResendTimes < 5)
                iResendTimes = 5;

            SetSendRequestTxt = action;

            SetBigballAddress();//連線檢查
        }

        private static void SetBigballAddress()
        {
            try
            {
                string address = ConfigurationManager.AppSettings["BigballAddress"];
                if (string.IsNullOrEmpty(address))
                    return;
                //return false;

                WebRequest req = WebRequest.Create(address);
                req.Method = "HEAD";

                RequestState rs = new RequestState();
                rs.Request = req;
                rs.url = address;
                rs.state = "check";

                req.BeginGetResponse(new AsyncCallback(RespCallback), rs);//非同步處理
            }
            catch { }

            //return false;
        }

        public static void SendRequest(string cache, ref WebRequestData wData, string[] Runs, string gameStatus, int sendCount)
        {
            if (_BigballAddress == null)
            {
                _logRequest.Error("error url");
                return;
            }

            if (Runs.Length == 0 ||//資料未經過初始化
                string.IsNullOrEmpty(Runs[0]) ||
                string.IsNullOrEmpty(Runs[1]) ||
                string.IsNullOrEmpty(gameStatus))
                return;//空值 不處理            

            if (wData.iGameType == 6)//因WNBA可能會開在其他(gameType=13)，所以遇到gameType=6則加送一筆gameType=13
            {
                WebRequestData copyData = new WebRequestData(wData.gameType, wData.gameStatus, wData.GID);
                copyData.iGameType = 13;
                copyData.NA = wData.NA;
                copyData.NB = wData.NB;
                copyData.gameDate = wData.gameDate;
                SendRequest(cache, ref copyData, Runs, gameStatus, sendCount);
            }

            string gamesInfo = string.Format("## [{0}] {1}:{2} ## [{3}] {4}:{5}", wData.gameType, wData.NA, wData.NB, gameStatus, Runs[0], Runs[1]);

            //紀錄請求log
            if (wData.gameType == "bkos" || wData.gameType == "bkbf" || wData.gameType == "tn")
                ReadyLog.Info(wData.gameType, wData.GID, wData.alliance, gamesInfo);
            else
                ReadyLog.Info(wData.gameType, wData.GID, gamesInfo);


            string url = null;
            StringBuilder md5format = new StringBuilder();

            try
            {
                string na = HttpUtility.UrlEncode(wData.NA);
                string nb = HttpUtility.UrlEncode(wData.NB);
                
                //資料建立
                //md5format.Append(wData.iGameType + "&");//因WNBA與NBA的type會送6跟13
                md5format.Append(wData.gameDate + "&");
                md5format.Append(na + "&");
                md5format.Append(nb + "&");
                md5format.Append(Runs[0] + "&");
                md5format.Append(Runs[1] + "&");
                md5format.Append(gameStatus + "&");

                string GameCode = getMd5Hash(md5format.ToString());

                var md5key = string.Format("{0}_{1}_md5", wData.iGameType, gameStatus);
                var oldGameCode = BigBallRequest.ChangeCache[cache][wData.GID]["status"][md5key];
                if (oldGameCode != null)
                {
                    if (oldGameCode.ToString() == GameCode)//相同資料已送出過 則不送出                    
                        return;                    
                }

                //紀錄未重複log
                if (wData.gameType == "bkos" || wData.gameType == "bkbf" || wData.gameType == "tn")
                    md5Log.Info(wData.gameType, wData.GID, wData.alliance, gamesInfo);
                else
                    md5Log.Info(wData.gameType, wData.GID, gamesInfo);

                BigBallRequest.ChangeCache[cache][wData.GID]["status"][md5key] = GameCode;//寫入暫存

                //送出webRequest
                string formatStr = "GameType={0}&GameDate={1}&TeamA={2}&TeamB={3}&TeamARuns={4}&TeamBRuns={5}&GameStatus={6}&GameCode={7}";
                string requestString = string.Format(formatStr, wData.iGameType, wData.gameDate, na, nb, Runs[0], Runs[1], gameStatus, GameCode);
                url = _BigballAddress + "?" + requestString;

                RequestState rs = new RequestState();
                rs.url = url;
                rs.state = gamesInfo;

                AddToQueue(rs, sendCount);//放到堆疊
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SendRequest Error!!");
                sb.AppendLine("md5format:");
                sb.AppendLine(md5format.ToString());                
                sb.AppendLine("Message: " + ex.Message);
                sb.AppendLine("StackTrace:");
                sb.AppendLine(ex.StackTrace);

                _logRequest.Error(sb.ToString());
            }
        }

        static string getMd5Hash(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(string.Format("@!^{0}@!^", input)));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            return sBuilder.ToString();
        }

        public static void AddToQueue(RequestState rs, int waitingTimeSec, bool isResend = false)
        {
            //非同步處理
            Task.Run(() =>
            {
                if (waitingTimeSec <= 0)
                    waitingTimeSec = 1;//最低處理間隔

                var state = rs.state;
                if (isResend)//比分重送
                {
                    state += " >> resend";
                    Thread.Sleep(waitingTimeSec * 1000);
                }
                else
                    Thread.Sleep(waitingTimeSec * 3000);//正常送分增加每筆送分間隔                

                //寫入Queue log
                _logQueue.Info(state);

                requestQueue.Enqueue(rs);//放到堆疊
            });
        }

        public static void requestQueueWork()
        {
            while (true)
            {
                Thread.Sleep(100);

                try
                {
                    if (requestQueue.Count > 0)
                    {
                        RequestState rs;
                        if (requestQueue.TryDequeue(out rs))
                        {

                            WebRequest req = WebRequest.Create(rs.url);
                            req.Timeout = 3000;
                            rs.Request = req;

                            req.BeginGetResponse(new AsyncCallback(RespCallback), rs);//非同步處理
                        }
                    }
                }
                catch { }
            }
        }

        //const int BUFFER_SIZE = 1024;
        private static void RespCallback(IAsyncResult ar)
        {
            RequestState rs = (RequestState)ar.AsyncState;
            try
            {
                WebRequest req = rs.Request;
                switch (rs.state)
                {
                    case "check"://連線檢查
                        {
                            HttpWebResponse response = req.EndGetResponse(ar) as HttpWebResponse;

                            string sendRequst = "";
                            string[] tmp = rs.url.Split('/');
                            if (tmp.Length > 2)
                                sendRequst = tmp[2];

                            if (response.StatusCode == HttpStatusCode.OK)
                                _BigballAddress = rs.url;
                            else
                                sendRequst += " Error";

                            SetSendRequestTxt(sendRequst);

                            response.Close();
                        }
                        break;

                    default://大球比分
                        {
                            WebResponse resp = req.EndGetResponse(ar);
                            StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                            string endString = sr.ReadToEnd().Trim();

                            resp.Close();
                            sr.Close();

                            StringBuilder sb = new StringBuilder();                            
                            sb.AppendLine(rs.state);
                            sb.AppendLine(endString);
                            sb.AppendLine(rs.url);
                            sb.AppendLine("QueueCount:" + requestQueue.Count);

                            SqlDependencyCache.AddMessage("success! " + rs.state, 0);
                            _logRequest.Info(sb.ToString());
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                if (rs.state == "check")
                    SqlDependencyCache.AddMessage("check address fail!!", 2);
                else
                {
                    rs.errorTimes++;

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(rs.state);
                    sb.AppendLine(ex.Message);
                    sb.AppendLine(rs.url);
                    sb.AppendLine("ErrorTimes: " + rs.errorTimes.ToString());

                    SqlDependencyCache.AddMessage("fail! " + rs.state, 0);
                    _logRequest.Error(sb.ToString());

                    if (rs.errorTimes < iResendTimes)//失敗重送                   
                        AddToQueue(rs, iResendTime, true);//放到堆疊                    
                }
            }
        }
    }
}
