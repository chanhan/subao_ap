using SHGG.DataStructerService;
using NLog;
//using SHGG.SocketService;
//using SHGG.SystemService.Threads;
using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace Server
{
    public partial class frmMain : Form
    {
        private static Logger ServerWork = LogManager.GetLogger("ServerWork");
        private static Logger ServerReceive = LogManager.GetLogger("ServerReceive");
        private static Logger ServerError = LogManager.GetLogger("ServerError");
        private static LogClear logClear;

        private const string CONFIG_FILE_PATH = @".\Server.xml";  // 設定檔

        SocketServer Server;
        public Dictionary<string, string> CurProcessType { get; private set; }      // 多跟分來源的當前來源
        public Dictionary<string, string> CurProcessTime { get; private set; }      // 當前跟分指定開賽時間
        public List<Dictionary<string, string>> ProcessList { get; private set; }   // 執行程式清單
        
        #region Form
        public frmMain()
        {
            InitializeComponent();
            // 設定
            this.Icon = Properties.Resources.Console;
            // 讀取設定檔
            XmlAdapter xmlAdapter;
            // 設定 IP
            xmlAdapter = new XmlAdapter(CONFIG_FILE_PATH);
            xmlAdapter.GoToNode("XML", "Server");
            this.tsptxbServerIp.Text = xmlAdapter.ReadXmlNode("Ip");
            this.tsptxbServerPort.Text = xmlAdapter.ReadXmlNode("Port");
            // 讀取執行程式清單
            xmlAdapter = new XmlAdapter(CONFIG_FILE_PATH);
            xmlAdapter.GoToNode("XML", "Processes");
            // 建立執行程式
            try
            {
                this.ProcessList = xmlAdapter.GetAllSubNodes("Process", "Name", "Path", "Command", "Description", "Default");
            }catch
            {
                MessageBox.Show(this, "載入Process設定檔失敗。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                // 寫入記錄
                ServerWork.Error("載入Process設定檔失敗");
                Application.Exit();
            }

            // 多跟分來源的當前來源
            this.CurProcessType = new Dictionary<string, string>();
            this.CurProcessTime = new Dictionary<string, string>();

            //設定預設值 KEY統一大寫
            foreach (var process in this.ProcessList)
            {
                if (process["Default"] == "true")
                {
                    string key = process["Name"].Replace("(", "").Replace(")", "").ToUpper();
                    if (key.IndexOf("-") != -1)
                        key = key.Substring(0, key.IndexOf("-"));

                    this.CurProcessType.Add(key, process["Name"]);
                }
            }

            logClear = new LogClear();
            Task.Factory.StartNew(() => logClear.Work(), TaskCreationOptions.LongRunning);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // 版本
            this.Text += "   v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            // 執行程式清單
            this.InvokeIfRequired(()=>this.ShowProcesses());
        }
        private void frmMain_Shown(object sender, EventArgs e)
        {
            // 寫入記錄
            ServerWork.Info("Pragram Open");
            // Debug 模式
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.Text += "【偵錯模式】";
                // 啟動
                this.tspbtnStart_Click(this.tspbtnStart, new EventArgs());
            }
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 詢問關閉
            if (!this.IsExit)
            {
                e.Cancel = true;
                // 隱藏
                this.Hide();
            }
        }
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 寫入記錄
            ServerWork.Info("Program Close");
        }

        private bool IsExit = false;
        private void tspbtnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "您確定要關閉程式？\r\n關閉程式需要一些時間。", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.IsExit = true;
                this.Close();
            }
        }

        private void nfiMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 顯示
            this.Show();
            this.Activate();
        }
        #endregion
        #region Tools

        private void tspbtnStart_Click(object sender, EventArgs e)
        {
            // 按扭
            this.tsptxbServerIp.ReadOnly = true;
            this.tsptxbServerPort.ReadOnly = true;
            this.tspbtnStart.Enabled = false;

            try
            {
                // 建立伺服端
                Server = new SocketServer(this.tsptxbServerIp.Text, this.tsptxbServerPort.Text, this);
                // 運行伺服端
                Task.Factory.StartNew(() => Server.Start(), TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                ServerError.Error("啟動伺服器時發生錯誤!\n\rMessage: {0},\r\n StackTrace: {1}\r\n", ex.Message, ex.StackTrace);
            }            
        }


        private void tspbtnCloseAllProcess_Click(object sender, EventArgs e)
        {
            // 詢問並關閉程式
            if (MessageBox.Show(this, "您確定要關閉全部程式？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                // 關閉全部程式
                this.CloseAllProcess();
            }
        }
        #endregion

        #region DataGridView
        // 印出索引位置
        private void RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.InvokeIfRequired(() =>
            {
                DataGridView dgv = (DataGridView)sender;
                Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, e.RowBounds.Height);
                TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, rectangle, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            });
        }

        // 程式列表
        private void dgvProcesses_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // 沒有資料就離開
            if (e.RowIndex == -1) return;

            this.InvokeIfRequired(() =>
                {
                    DataGridView dgv = (DataGridView)sender;
                    DataGridViewRow row = dgv.Rows[e.RowIndex];
                    string run = row.Cells[this.colProcessStatus.Index].Value.ToString();
                    // 判斷
                    if (run == "執行中")
                    {
                        row.Cells[this.colProcessStatus.Index].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        row.Cells[this.colProcessStatus.Index].Style.BackColor = Color.Red;
                    }
                });
        }

        private void dgvProcesses_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 沒有資料就離開
            if (e.RowIndex == -1) return;

            this.InvokeIfRequired(() =>
            {
                DataGridView dgv = (DataGridView)sender;
                DataGridViewRow row = dgv.Rows[e.RowIndex];
                string run = row.Cells[this.colProcessStatus.Index].Value.ToString();
                // 判斷
                if (run == "執行中")
                {
                    row.Cells[this.colProcessStatus.Index].Style.BackColor = Color.Green;
                }
                else
                {
                    row.Cells[this.colProcessStatus.Index].Style.BackColor = Color.Red;
                }
            });
        }

        private void dgvProcesses_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 沒有資料就離開
            if (e.RowIndex == -1) return;

            DataGridView dgv = (DataGridView)sender;
            DataGridViewRow row = dgv.Rows[e.RowIndex];
            int key = int.Parse(row.Tag.ToString());
            Dictionary<string, string> process = this.ProcessList[key];

            // 執行
            if (e.ColumnIndex == this.colProcessStart.Index && !process.CheckProcess())
            {
                // 詢問並執行程式
                if (MessageBox.Show(this, "您確定要執行 " + process["Name"] + "？\r\n程式說明：" + process["Description"], this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Task.Run(() =>
                        {
                            //檢查是否有相同類型的比賽
                            CheckTypeProcess(process);

                            // 執行程式
                            process.OpenProcess();

                            //更新介面狀態
                            UpdateStatus();
                        });
                }
            }
            // 關閉
            if (e.ColumnIndex == this.colProcessClose.Index && process.CheckProcess())
            {
                // 詢問並關閉程式
                if (MessageBox.Show(this, "您確定要關閉 " + process["Name"] + "？\r\n程式說明：" + process["Description"], this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Task.Run(() =>
                        {
                            // 關閉程式
                            process.KillProcess();

                            //更新介面狀態
                            UpdateStatus();
                        });
                }
            }            
        }

        #endregion

        #region Timer
        private void timerProcess_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                //更新介面狀態
                UpdateStatus();
            });
        }

        public void UpdateStatus()
        {
            //更新介面狀態
            this.InvokeIfRequired(() => this.updateProcessStatus());        
        }

        private void updateProcessStatus()
        {
            try
            {
                //計時器停止
                this.timerProcess.Stop();

                Thread.Sleep(1000);

                bool haveSort = false;
                // 執行程式清單
                foreach (DataGridViewRow row in this.dgvProcesses.Rows)
                {
                    int i = int.Parse(row.Tag.ToString());
                    Dictionary<string, string> process = this.ProcessList[i];
                    // 設定執行狀態
                    process["Run"] = process.CheckProcess() ? ("執行中") : ("未執行");

                    // 顯示
                    if (process["Run"] != row.Cells[this.colProcessStatus.Index].Value.ToString())
                    {
                        row.Cells[this.colProcessStatus.Index].Value = process["Run"];
                        haveSort = true;
                    }
                }
                // 判斷排序
                if (haveSort)
                {
                    // 排序狀態
                    this.dgvProcesses.Sort(this.colProcessStatus, ListSortDirection.Descending);

                    //廣播更新所有client介面
                    if (Server != null)
                        Server.BoardcastProcessStatus();
                }

                //計時器啟動
                this.timerProcess.Start();
            }
            catch (Exception ex)
            {
                ServerError.Error("updateProcessStatus 發生錯誤!/n/rMessage: {0},\r\n StackTrace: {1}\r\n", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Other
        // 顯示執行程式清單
        private void ShowProcesses()
        {
            try
            {
                // 清除目前的資料
                this.dgvProcesses.Rows.Clear();
                // 執行程式清單
                for (int i = 0; i < this.ProcessList.Count; i++)
                {
                    Dictionary<string, string> process = this.ProcessList[i];
                    // 設定執行狀態
                    process["Run"] = process.CheckProcess() ? ("執行中") : ("未執行");
                    // 加入並顯示
                    int index = this.dgvProcesses.Rows.Add(new string[] { process["Name"], process["Run"], process["Description"], "執行", "關閉" });
                    // 記錄資料方便處理按鈕事件
                    this.dgvProcesses.Rows[index].Tag = i;
                }
                // 排序狀態
                this.dgvProcesses.Sort(this.colProcessStatus, ListSortDirection.Descending);
            }
            catch (Exception ex)
            {
                ServerError.Error("ShowProcesses 發生錯誤!/n/rMessage: {0},\r\n StackTrace: {1}\r\n", ex.Message, ex.StackTrace);
            }
        }

        // 判斷程式是否執行中
        //檢查是否有相同類型的比賽
        public void CheckTypeProcess(Dictionary<string, string> process)
        {
            string ProcessName = process["Name"];
            int findIdx = ProcessName.IndexOf("-");
            if (findIdx != -1)
            {
                string curkey = ProcessName.Substring(0, findIdx).Replace("(", "").ToUpper();//組成原始Key名稱 ex:MLB
                foreach (Dictionary<string, string> tmp in this.ProcessList)
                {
                    //關閉所有相同類型的比賽
                    //模糊比對  ex: (Football)  <-> FOOTBALL
                    if (tmp["Name"].ToUpper().IndexOf(curkey) == 1)
                    {
                        tmp.KillProcess();
                    }
                }

                if (this.CurProcessType.ContainsKey(curkey) && this.CurProcessType[curkey] != process["Name"])//指定開啟的與當前來源不同
                    this.CurProcessType[curkey] = process["Name"];//變更多來源的指定跟分來源                
            }
            else
                process.KillProcess();//只有單一來源
        }
        
        // 關閉全部程式
        public void CloseAllProcess()
        {
            if (this.tspbtnCloseAllProcess.Enabled == false)
                return;

            // 寫入記錄
            ServerWork.Info("CloseAllProcess");

            this.tspbtnCloseAllProcess.Enabled = false;
            var task = Task.Factory.StartNew(() =>
                {
                    // 執行程式清單
                    foreach (Dictionary<string, string> process in this.ProcessList)
                    {
                        // 關閉程式
                        process.KillProcess();
                    }

                    //更新介面狀態
                    UpdateStatus();

                    this.InvokeIfRequired(() =>
                    {
                        this.tspbtnCloseAllProcess.Enabled = true;
                    });
                });
        }
        #endregion
    }

}
