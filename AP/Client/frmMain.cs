using SHGG.DataStructerService;
using SHGG.FileService;
//using SHGG.SocketService;
//using SHGG.SystemService.Threads;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Forms;
using Common = Client.CommonWS;
using System.Configuration;

namespace Client
{
    public partial class FrmMain : Form
    {
        private const string CONFIG_FILE_PATH = @".\Client.xml";  // 設定檔
        //private const string LOG_FILE_PATH = @".\Client.log";           // 記錄檔

        private readonly string _token = EncryptHelper.AESEncrypt("sp8888.net/schedules/validate"); // 驗證碼

        //ErrorNote FileLog;                                              // 記錄檔操作
        //SocketClient Client;                                       // 客運端
        List<Dictionary<string, string>> ProcessList;                   // 執行程式清單
        List<Dictionary<string, string>> ServersList;                   // 伺服器清單

        Socket Client; // 先行宣告Socket

        private Common.CommonService _wsCommon = new Common.CommonService();
        // 程式關閉旗標
        private bool _appClose = false;

        #region Form
        public FrmMain()
        {
            InitializeComponent();
            // 設定
            this.Icon = Properties.Resources.Control;
            // 建立記錄檔操作
            //this.FileLog = new SHGG.FileService.ErrorNote(LOG_FILE_PATH);
            // 讀取設定檔
            XmlAdapter xmlAdapter;
            // 讀取伺服器清單
            xmlAdapter = new XmlAdapter(CONFIG_FILE_PATH);
            xmlAdapter.GoToNode("XML", "Servers");
            this.ServersList = xmlAdapter.GetAllSubNodes("Server", "Name", "Ip", "Port");
            // 加入清單
            foreach (Dictionary<string, string> server in this.ServersList)
            {
                this.tspcbbServerIp.Items.Add(string.Format("{0}", server["Name"]));
            }
            // 預設
            this.tspcbbServerIp.SelectedIndex = 0;
            // 讀取執行程式清單
            xmlAdapter = new XmlAdapter(CONFIG_FILE_PATH);
            xmlAdapter.GoToNode("XML", "Processes");
            this.ProcessList = xmlAdapter.GetAllSubNodes("Process", "Name", "Description");

            // WS client timeout 時間
            int timeout;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["WSTimeout"], out timeout))
            {
                timeout = 120;
            }

            // 轉為毫秒
            timeout *= 1000;
            _wsCommon.Timeout = timeout;
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            // 登入驗證
            bool login = LoginCheck();
            if (!login)
            {
                AppClose();
                return;
            }

            // 版本
            this.Text += "   v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            // 顯示清單
            this.InvokeIfRequired(() => this.ShowProcesses());
        }
        private void frmMain_Shown(object sender, EventArgs e)
        {
            // 寫入記錄
            //this.FileLog.WriteError(" 執行");
            // Debug 模式
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.Text += "【開發模式】";
            }
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 詢問關閉
            if (e.CloseReason == CloseReason.UserClosing && !_appClose)
            {
                e.Cancel = !(MessageBox.Show(this, "您確定要關閉程式？\r\n關閉程式需要一些時間。", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes);
            }
        }
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 寫入記錄
            //this.FileLog.WriteError(" 關閉");
            // 關閉連線
            this.tspbtnStop_Click(this.tspbtnStop, new EventArgs());
        }
        #endregion
        #region Tools
        private void tspbtnStart_Click(object sender, EventArgs e)
        {
            // 錯誤處理
            try
            {
                Dictionary<string, string> server = this.ServersList[this.tspcbbServerIp.SelectedIndex];

                string ip = EncryptHelper.AESDecrypt(server["Ip"]);
                string port = EncryptHelper.AESDecrypt(server["Port"]);

                // 連線伺服器
                //this.Client = new SocketClient(ip, port);
                this.Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                this.Client.Connect(new IPEndPoint(IPAddress.Parse(ip), int.Parse(port)));

                //開始監聽
                Task.Factory.StartNew(() => ReceiveWork(), TaskCreationOptions.LongRunning);

                //定時檢查
                this.timerSocket.Start();

                // 傳送資料 - 取得目前執行程式的狀態
                this.Send("getprocessstatus");
                sendDone.WaitOne();


                // 寫入記錄
                //this.FileLog.WriteError(" 連線到伺服器");
                // 按扭
                this.tspcbbServerIp.Enabled = false;
                this.tspbtnStart.Enabled = false;
                this.tspbtnStop.Enabled = true;
                this.tspbtnCloseAllProcess.Enabled = true;
            }
            catch
            {
                MessageBox.Show(this, "連線到伺服器失敗。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                // 寫入記錄
                //this.FileLog.WriteError(" 連線到伺服器失敗");
            }
        }
        private void tspbtnStop_Click(object sender, EventArgs e)
        {
            // 錯誤處理
            try
            {
                // 關閉連線
                if (this.Client != null)
                {
                    this.Send("exit");
                    sendDone.WaitOne();

                    this.Client.Shutdown(SocketShutdown.Both);
                    this.Client.Close();
                    this.Client.Dispose();
                    this.Client = null;

                    // 寫入記錄
                    //this.FileLog.WriteError(" 斷開與伺服器的連線");
                }

                //定時檢查
                this.timerSocket.Stop();

                // 按扭
                this.tspcbbServerIp.Enabled = true;
                this.tspbtnStart.Enabled = true;
                this.tspbtnStop.Enabled = false;
                this.tspbtnCloseAllProcess.Enabled = false;

                this.InvokeIfRequired(() => this.ShowProcesses());
            }
            catch
            {
                MessageBox.Show(this, "關閉連線失敗。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                // 寫入記錄
                //this.FileLog.WriteError("關閉連線失敗");
            }
        }
        private void tspbtnCloseAllProcess_Click(object sender, EventArgs e)
        {
            // 詢問並關閉程式
            if (MessageBox.Show(this, "您確定要關閉全部程式？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                // 寫入記錄
                //this.FileLog.WriteError(" 關閉全部程式");
                // 傳送資料
                this.Send("closeallprocess");
                sendDone.WaitOne();
            }
        }

        //控制項暫時鎖定
        private void ControllerLock(bool isLock)
        {

            if (isLock)
            {
                //控制項暫時鎖定
                lbCtrlLock.Visible = true;
                dgvProcesses.Enabled = false;
            }
            else
            {
                //控制項解鎖
                dgvProcesses.Enabled = true;
                lbCtrlLock.Visible = false;
            }
        }

        #endregion
        #region Client
        private static ManualResetEvent sendDone = new ManualResetEvent(false);

        // 傳送資料
        private void Send(string data)
        {
            // 連線中
            if (SocketConnected())
            {
                try
                {
                    this.InvokeIfRequired(() => ControllerLock(true));//控制項暫時鎖定

                    byte[] byteData = Encoding.ASCII.GetBytes(data + "<EOF>");

                    this.Client.BeginSend(byteData, 0, byteData.Length, 0,
                        new AsyncCallback(SendCallback), this.Client);
                }
                catch
                {
                    this.tspbtnStop_Click(this.tspbtnStop, new EventArgs());
                    MessageBox.Show(this, "與伺服器的連線被中斷。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    // 寫入記錄
                    //this.FileLog.WriteError(" 與伺服器的連線被中斷");
                }

            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket) ar.AsyncState;
                int bytesSent = client.EndSend(ar);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private bool SocketConnected()
        {
            if (this.Client == null)
                return false;

            try
            {
                bool part1 = this.Client.Poll(1000, SelectMode.SelectRead);
                bool part2 = (this.Client.Available == 0);
                if (part1 && part2)
                    return false;
                else
                    return true;
            }
            catch { }//忽略socket錯誤

            return false;
        }

        private void ReceiveWork()
        {
            while (SocketConnected())
            {
                try
                {
                    byte[] bytes = new byte[1024];
                    int bytesRec = this.Client.Receive(bytes);
                    string msg = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    Console.WriteLine(string.Format("{0} Receive: {1}\r\n Msg: {2}", DateTime.Now.ToString("mm/dd HH:mm:ss"), this.Client.LocalEndPoint, msg));

                    //定時檢查 重啟
                    this.timerSocket.Start();

                    // 顯示清單
                    Task.Run(() =>
                    {
                        this.InvokeIfRequired(() => this.UpdateProcessStatus(msg));//介面更新
                        this.InvokeIfRequired(() => ControllerLock(false));//控制項解鎖
                    });
                }
                catch
                {
                    this.InvokeIfRequired(() => this.tspbtnStop_Click(this.tspbtnStop, new EventArgs()));
                    this.InvokeIfRequired(() => ControllerLock(false));//控制項解鎖
                }
            }
        }
        #endregion

        #region DataGridView
        // 執行程式
        private void OpenProcess(Dictionary<string, string> process)
        {
            string cmd = "<XML><Command>open</Command><Process>{0}</Process></XML>";
            // 寫入記錄
            //this.FileLog.WriteError(" 執行程式：" + process["Name"] + " 說明：" + process["Description"]);
            // 傳送資料
            this.Send(string.Format(cmd, process["Name"]));
            sendDone.WaitOne();
        }
        // 關閉程式
        private void KillProcess(Dictionary<string, string> process)
        {
            string cmd = "<XML><Command>kill</Command><Process>{0}</Process></XML>";
            // 寫入記錄
            //this.FileLog.WriteError(" 關閉程式：" + process["Name"] + " 說明：" + process["Description"]);
            // 傳送資料
            this.Send(string.Format(cmd, process["Name"]));
            sendDone.WaitOne();
        }
        // 印出索引位置
        private void RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView dgv = (DataGridView) sender;
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, rectangle, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        // 程式列表
        private void dgvProcesses_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 沒有資料就離開
            if (e.RowIndex == -1)
                return;

            DataGridView dgv = (DataGridView) sender;
            DataGridViewRow row = dgv.Rows[e.RowIndex];
            int key = int.Parse(row.Tag.ToString());
            Dictionary<string, string> process = this.ProcessList[key];

            // 執行
            if (e.ColumnIndex == this.colProcessStart.Index && SocketConnected())
            {
                // 詢問並執行程式
                if (MessageBox.Show(this, "您確定要執行 " + process["Name"] + "？\r\n程式說明：" + process["Description"], this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    // 執行程式
                    this.OpenProcess(process);
                }
            }
            // 關閉
            if (e.ColumnIndex == this.colProcessClose.Index && SocketConnected() && process["Run"] == "執行中")
            {
                // 詢問並關閉程式
                if (MessageBox.Show(this, "您確定要關閉 " + process["Name"] + "？\r\n程式說明：" + process["Description"], this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    // 關閉程式
                    this.KillProcess(process);
                }
            }
        }
        #endregion
        #region Timer
        private void timerSocket_Tick(object sender, EventArgs e)
        {
            // 傳送資料 - 取得目前執行程式的狀態
            this.Send("getprocessstatus");
            sendDone.WaitOne();
        }
        #endregion

        #region Other
        // 顯示清單
        private void ShowProcesses()
        {
            // 清除目前的資料
            this.dgvProcesses.Rows.Clear();
            // 執行程式清單
            for (int i = 0; i < this.ProcessList.Count; i++)
            {
                Dictionary<string, string> process = this.ProcessList[i];

                // 設定執行狀態
                process["Run"] = ("未執行");

                // 加入並顯示
                int index = this.dgvProcesses.Rows.Add(new string[] { process["Name"], process["Run"], process["Description"], "執行", "關閉" });
                // 記錄資料方便處理按鈕事件
                this.dgvProcesses.Rows[index].Tag = i;
                //this.dgvProcesses.Rows[index].Cells[this.colProcessStatus.Index].Style.BackColor = Color.Red; //尚未連線不設定顏色
            }

            // 排序狀態
            //this.dgvProcesses.Sort(this.colProcessStatus, ListSortDirection.Descending);
            // 隱藏資料行
            //this.colProcessStatus.Visible = false;
            //this.colDescription.Width += this.colProcessStatus.Width;
        }

        private void UpdateProcessStatus(string processStatus)
        {
            // 執行程式清單
            foreach (DataGridViewRow row in this.dgvProcesses.Rows)
            {
                int i = int.Parse(row.Tag.ToString());
                Dictionary<string, string> process = this.ProcessList[i];
                // 設定執行狀態
                process["Run"] = processStatus.Contains(process["Name"]) ? ("執行中") : ("未執行");

                // 顯示
                if (process["Run"] != row.Cells[this.colProcessStatus.Index].Value.ToString())
                    row.Cells[this.colProcessStatus.Index].Value = process["Run"];//介面狀態設定                                    

                if (process["Run"] == "執行中")//狀態顏色設定
                {
                    row.Cells[this.colProcessStatus.Index].Style.BackColor = Color.Green;
                    row.Cells[this.colProcessStart.Index].Value = "重啟";
                }
                else
                {
                    row.Cells[this.colProcessStatus.Index].Style.BackColor = Color.Red;
                    row.Cells[this.colProcessStart.Index].Value = "執行";
                }
            }
        }

        // 傳送執行程式清單
        private void SendProcessList(TcpClient client)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<Processes>");
            // 執行程式清單
            for (int i = 0; i < this.ProcessList.Count; i++)
            {
                Dictionary<string, string> process = this.ProcessList[i];
                sb.Append("<Process>");
                // 資料
                sb.AppendFormat("<Name>{0}</Name>", process["Name"]);
                sb.AppendFormat("<Path>{0}</Path>", process["Path"]);
                sb.AppendFormat("<Command>{0}</Command>", process["Command"]);
                sb.AppendFormat("<Description>{0}</Description>", process["Description"]);
                sb.Append("</Process>");
            }
            sb.Append("</Processes>");
            // 傳送
            client.Client.Send(Encoding.UTF8.GetBytes(sb.ToString()));
        }
        #endregion


        #region 登入驗證

        /// <summary>
        /// 登入驗證
        /// </summary>
        private bool LoginCheck()
        {
            bool success = true;

            try
            {
                Common.ExecuteResult result = _wsCommon.LoginCheck(_token);
                if (result.ResultType > 0)
                {
                    // 驗證失敗 關閉程式
                    ShowExecuteResultError("Warning", "登入驗證失敗。", result.ResultMessage);
                    success = false;
                }
            }
            catch (WebException wex)
            {
                MessageBox.Show(this, wex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (wex.Status == WebExceptionStatus.ConnectFailure) { success = false; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("無法檢查登入驗證。 {0}", ex.Message), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                success = false;
            }

            return success;
        }

        #endregion

        #region 程式關閉

        /// <summary>
        /// 程式關閉 (非手動關閉)
        /// </summary>
        private void AppClose()
        {
            _appClose = true;
            this.Close();
            _appClose = false;
        }

        #endregion


        #region 顯示

        /// <summary>
        /// 顯示 WebService 執行錯誤訊息
        /// </summary>
        /// <param name="title">標題(GameType)</param>
        /// <param name="errIdentity">錯誤識別</param>
        /// <param name="resultMsg">執行錯誤訊息</param>
        private void ShowExecuteResultError(string title, string errIdentity, string resultMsg)
        {
            MessageBox.Show(this, String.Format("{0} {1}", errIdentity, resultMsg), title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        #endregion 顯示
    }

    //扩展方法必须在非泛型静态类中定义
    public static class ExtensionForm
    {
        //非同步委派更新UI
        public static void InvokeIfRequired(this Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)//在非當前執行緒內 使用委派
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
