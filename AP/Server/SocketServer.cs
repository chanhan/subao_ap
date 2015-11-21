using SHGG.DataStructerService;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Server
{
    class SocketServer
    {
        private static Logger ServerWork = LogManager.GetLogger("ServerWork");
        private static Logger ServerReceive = LogManager.GetLogger("ServerReceive");
        private static Logger ServerError = LogManager.GetLogger("ServerError");
        private static frmMain formMain;
        IPEndPoint localEndPoint;

        private static ConcurrentDictionary<string, Socket> clientList = new ConcurrentDictionary<string, Socket>();
        private ManualResetEvent allDone = new ManualResetEvent(false);

        public SocketServer(IPAddress ip, string port, frmMain form)
        {
            formMain = form;
        
            localEndPoint = new IPEndPoint(ip, int.Parse(port));            
        }

        public SocketServer(string ip, string port, frmMain form)
        {
            formMain = form;

            localEndPoint = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
        }

        public static IPAddress LocalIPAddress()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }

        Socket listener;
        public void Start()
        {
            try
            {
                // Create a TCP/IP socket.
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.SendTimeout = 1000;
                listener.NoDelay = true;

                listener.Bind(localEndPoint);
                listener.Listen(10);

                // 寫入記錄
                ServerWork.Info("Start Server Connect " + localEndPoint);

                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
            }
            catch (Exception ex)
            {
                ServerError.Error("建立server發生錯誤：\r\n " + ex.Message);
            }
        }



        public void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // Get the socket that handles the client request.
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                string key = handler.RemoteEndPoint.ToString();
                if (!clientList.ContainsKey(key))
                    clientList.TryAdd(key, handler);//註冊到廣播列表

                ClientWork(handler);
            }
            catch { }
            finally 
            {
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
            }
        }

        private static void ClientWork(Socket handler)
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            state.RemoteEndPoint = handler.RemoteEndPoint.ToString();

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        static XmlDocument xml = new XmlDocument();
        public static void ReadCallback(IAsyncResult ar)
        {
            String data = String.Empty;

            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            string key = handler.RemoteEndPoint.ToString();

            try
            {                
                int bytesRead = handler.EndReceive(ar);
                ar.AsyncWaitHandle.Close();

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    data = state.sb.ToString();
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        var msg = string.Format("{0} Server Receive From {1}\r\n Msg: {2}", DateTime.Now.ToString("HH:mm:ss"), key, data);
                        ServerReceive.Info(msg);//寫log
                        Console.WriteLine(msg);

                        string[] dataArr = data.Split(new string[] { "<EOF>" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < dataArr.Length; i++)
                        {
                            try
                            {
                                switch (dataArr[i])
                                {
                                    case "exit":
                                        CloseClient(handler, key);//客端連線關閉
                                        break;
                                    case "tryok":           // 連線測試
                                        break;
                                    case "closeallprocess": // 關閉全部程式
                                        // 關閉全部程式
                                        formMain.CloseAllProcess();
                                        break;
                                    case "getprocessstatus":
                                        SendProcessStatus(handler);
                                        break;

                                    default:
                                        xml.LoadXml(dataArr[i]);
                                        ProcessData(dataArr[i], handler);//處理封包
                                        break;
                                }
                            }
                            catch { }
                        }

                        if (!data.Contains("exit"))//客端請求中斷
                            ClientWork(handler);
                    }
                    else
                    {
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                }
            }
            catch (SocketException ex)
            {
                CloseClient(handler, state.RemoteEndPoint);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SocketException 接收封包發生錯誤!!");
                sb.AppendLine("content: ");
                sb.AppendLine(data);
                sb.AppendLine("Message: " + ex.Message);
                sb.AppendLine("StackTrace:");
                sb.AppendLine(ex.StackTrace);

                ServerError.Error(sb.ToString());
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Exception 接收封包發生錯誤!!");
                sb.AppendLine("content: ");
                sb.AppendLine(data);
                sb.AppendLine("Message: " + ex.Message);
                sb.AppendLine("StackTrace:");
                sb.AppendLine(ex.StackTrace);

                ServerError.Error(sb.ToString());
            }
        }


        // 接收到資料
        private static void ProcessData(string msg, Socket handler)
        {
            try
            {
                // 讀取設定
                XmlAdapter xmlAdapter = new XmlAdapter(msg, true);
                string cmd = xmlAdapter.ReadXmlNode("Command");
                string name = xmlAdapter.ReadXmlNode("Process");
                string date = xmlAdapter.ReadXmlNode("Date");
                string curkey = "";
                bool isNotSameDate = false;//開賽時間不同
                DateTime gameTime = DateTime.Now;

                //設定當前指定跟分的索引
                curkey = name.Replace("(", "").Replace(")", "").ToUpper();
                if (formMain.CurProcessType.ContainsKey(curkey))
                    name = formMain.CurProcessType[curkey];//ex:將(MLB) 指定成(MLB-ESPN)
                if (curkey.IndexOf("-") != -1)
                    curkey = curkey.Substring(0, curkey.IndexOf("-"));


                if (string.IsNullOrEmpty(date))//若無使定開賽時間則取緩存
                {
                    if (formMain.CurProcessTime.ContainsKey(curkey))
                        date = formMain.CurProcessTime[curkey];//取得當前比賽的指定時間
                }
                else
                {
                    if (DateTime.TryParse(date, out gameTime))
                    {

                        if (formMain.CurProcessTime.ContainsKey(curkey) == false || //指定開賽時間不存在
                            (formMain.CurProcessTime.ContainsKey(curkey) && gameTime > DateTime.Parse(formMain.CurProcessTime[curkey])))//新的開賽時間與舊開賽時間比舊的開賽時間較新
                        {
                            isNotSameDate = true;
                            formMain.CurProcessTime[curkey] = date;//設定當前比賽的指定時間
                        }                        
                    }
                }

                // 判斷命令
                if (cmd == "open")
                {
                    // 執行程式清單
                    foreach (Dictionary<string, string> process in formMain.ProcessList)
                    {
                        if (process["Name"] == name)//多來源的當前指定來源
                        {
                            formMain.CheckTypeProcess(process);

                            process.OpenProcess(gameTime.ToString("yyyy/MM/dd_HH:mm"));//日期合法代入日期參數
                        }
                    }
                }
                else if (cmd == "kill")
                {
                    // 執行程式清單
                    foreach (Dictionary<string, string> process in formMain.ProcessList)
                    {
                        if (process["Name"].ToUpper().IndexOf(curkey) == 1)//模糊比對  ex: (Football)  <-> FOOTBALL
                        {
                            // 關閉程式
                            process.KillProcess();
                        }
                    }
                }
                else if (cmd == "run")//只有Monitor會送出此請求
                {

                    // 執行程式清單
                    foreach (Dictionary<string, string> process in formMain.ProcessList)
                    {
                        if (process["Name"] == name)
                        {
                            // 找到程式但開賽時間不同
                            if (process.FindProcess())
                            {
                                if (isNotSameDate == true)
                                {
                                    process.KillProcess();
                                    process.OpenProcess(gameTime.ToString("yyyy/MM/dd_HH:mm"));//日期合法代入日期參數
                                }
                            }
                            else// 沒有找到程式 直接開啟                            
                                process.OpenProcess(gameTime.ToString("yyyy/MM/dd_HH:mm"));//日期合法代入日期參數
                        }
                    }
                }

                //更新介面狀態
                formMain.UpdateStatus();
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("ProcessData Error!!");
                sb.AppendLine("msg: " + msg);
                sb.AppendLine("Message: " + ex.Message);
                sb.AppendLine("StackTrace:");
                sb.AppendLine(ex.StackTrace);

                ServerError.Error(sb.ToString());
            }
        }

        private static bool SocketConnected(Socket s)
        {
            try
            {
                bool part1 = s.Poll(1000, SelectMode.SelectRead);
                bool part2 = (s.Available == 0);
                if (part1 & part2)
                    return false;
                else
                    return true;
            }
            catch { }

            return false;
        }

        public void BoardcastProcessStatus()//廣播給所有client
        {
            Dictionary<string, Socket> errorClient = new Dictionary<string, Socket>();
            foreach (KeyValuePair<string, Socket> o in clientList)
            {
                if (SendProcessStatus(o.Value) == false)//送出更新狀態給client
                    errorClient.Add(o.Key, o.Value);
            }

            foreach (KeyValuePair<string, Socket> o in errorClient)//有發生傳遞錯誤，釋放該筆連結
                CloseClient(o.Value, o.Key);            
        }

        private static bool SendProcessStatus(Socket handler)
        {
            try
            {
                if (handler != null && SocketConnected(handler))//連線存在
                {
                    List<string> runList = new List<string>();// 執行程式清單
                    foreach (Dictionary<string, string> process in formMain.ProcessList)
                    {
                        if (process["Run"] == "執行中")
                        {
                            runList.Add(process["Name"]);
                        }
                    }

                    string data = string.Join(",", runList.ToArray()) + "<EOF>";
                    byte[] processstatus = Encoding.ASCII.GetBytes(data);
                    handler.Send(processstatus);//送出狀態
                    Console.WriteLine(string.Format("{0} Server Send To {1}\r\n Msg: {2}", DateTime.Now.ToString("HH:mm:ss"), handler.RemoteEndPoint.ToString(), data));
                }
            }
            catch (Exception ex)
            {
                ServerError.Error("傳送狀態發生錯誤：\r\n " + ex.Message);
                return false;
            }

            return true;
        }

        //客端連線關閉
        private static void CloseClient(Socket handler, string key)
        {
            try
            {
                clientList.TryRemove(key, out handler);//連線中斷

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                handler.Dispose();
            }
            catch { }//client斷線時可能發生錯誤
        }
    }

    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        public string RemoteEndPoint;

        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}
