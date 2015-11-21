using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using NLog;

namespace Monitor
{
    class SocketClient
    {
        private static Logger apSendLog = LogManager.GetLogger("MonitorAP_Send_Log");
        private static Logger apSocketLog = LogManager.GetLogger("MonitorAP_Socket_Log");

        private static Queue<string> CommandQueue;
        private static Action<bool> SetStatus;
        private Socket Client;
        private static string serverIP, serverPort;

        public SocketClient(string ip, string port, Action<bool> action)
        {
            serverIP = ip;
            serverPort = port;
            SetStatus = action;
            CommandQueue = new Queue<string>();
        }

        public void Start()
        {
            Connect();//建立連線

            ClientWork();
        }

        public void Stop()
        {
            string cmd = "exit";

            if (this.Connected == true)
            {
                try
                {
                    byte[] byteData = Encoding.ASCII.GetBytes(cmd + "<EOF>");
                    int bytesSent = Client.Send(byteData);

                    apSendLog.Info(cmd);
                }
                catch { }
            }
        }

        //是否連線
        private bool Connected = false;
        private void ClientWork()
        {
            while (true)
            {
                Thread.Sleep(3000);

                //檢查連線
                CheckConnect();

                if (this.Connected == true && CommandQueue.Count > 0)
                {
                    string cmd = CommandQueue.Dequeue();

                    try
                    {
                        byte[] byteData = Encoding.ASCII.GetBytes(cmd + "<EOF>");

                        Client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), Client);

                        var msg = string.Format("SendToServer {0}, QueueCount: {1} \r\n {2}", Client.RemoteEndPoint, CommandQueue.Count, cmd);
                        apSendLog.Info(msg);
                       // Console.WriteLine(msg);
                    }
                    catch
                    {
                        CommandQueue.Enqueue(cmd);//連線中斷，把資料塞回queue
                        if (Client == null)
                            apSendLog.Error("SocketError Client NULL, QueueCount: {0} \r\n {1}", CommandQueue.Count, cmd);
                        else
                            apSendLog.Error("SocketError {0}, QueueCount: {1} \r\n {2}", Client.RemoteEndPoint, CommandQueue.Count, cmd);
                    }
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
               // Console.WriteLine("EndSend");
            }
            catch{}
        }

        private void Connect()
        {
            try
            {
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Client.Connect(new IPEndPoint(IPAddress.Parse(serverIP), int.Parse(serverPort)));
                Client.SendTimeout = 1000;
                Client.NoDelay = true;

                apSocketLog.Info("SocketConnect Success");
            }
            catch
            {
                Client = null;
                apSocketLog.Error("SocketConnect fail");
            }
        }


        public void Send(string data, string type)
        {
            try
            {
                CommandQueue.Enqueue(data);
                apSendLog.Info(string.Format("{0} AddToQueue, QueueCount: {1}", type, CommandQueue.Count));                
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private bool IsConnected()
        {
            if (Client.Connected)
            {
                if ((Client.Poll(0, SelectMode.SelectWrite)) && (!Client.Poll(0, SelectMode.SelectError)))
                {
                    try
                    {
                        byte[] tmp = Encoding.ASCII.GetBytes("tryok<EOF>");
                        Client.Send(tmp, 0, 0);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void CheckConnect()
        {
            try
            {
                if (Client == null)
                {
                    apSocketLog.Error("SocketConnect Null");

                    SetServerStatus(false);//連線失敗
                    return;
                }

                if(IsConnected())
                    SetServerStatus(true);//連線失敗
                else
                    SetServerStatus(false);
            }
            catch (Exception)
            {
                Client = null;
                apSocketLog.Error("SocketCheck fail");
            }
        }

        bool SocketStatus = false;
        private void SetServerStatus(bool isLink)
        {
            if (isLink != SocketStatus)//狀態變動
            {
                SocketStatus = isLink;
                apSocketLog.Info("SocketStatus {0}", isLink.ToString());
            }

            SetStatus(isLink);

            if (isLink == false)
            {
                if (Client != null)
                {
                    try
                    {
                        Client.Shutdown(SocketShutdown.Both);
                        Client.Close();
                        Client.Dispose();
                    }
                    catch (SocketException) { }
                    Client = null;
                }

                Connect();//建立連線
                //CheckConnect();
                return;
            }

            this.Connected = isLink;
        }
    }
}
