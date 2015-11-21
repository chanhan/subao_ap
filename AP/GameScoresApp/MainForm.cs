using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace GameScoresApp
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer apTimer;
        private System.Timers.Timer dateChangeTimer;

        SqlDependencyCache cache;
        public MainForm()
        {
            InitializeComponent();

            // 版本
            this.Text += "  v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //DB連線測試
            this.TestConnection();           

            BigBallRequest.IsEnableBigballRequest();//大球是否處理            

            //連線檢查 與初始化設定
            ProcessRequest.init((s) =>
            {
                this.InvokeIfRequired(() =>
                    {
                        if (s.ToLower().IndexOf("error") != -1)
                            sendRequestlb.ForeColor = Color.Red;

                        sendRequestlb.Text = s;//設定來源
                    });               
            });

            cache = new SqlDependencyCache(this);

            Task.Run(()=>ProcessRequest.requestQueueWork());//啟動送分流程
        }

        private void TestConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["sport"].ConnectionString;     

            bool result = false;
            SqlConnection conn = null;
            // 錯誤處理
            try
            {
                conn = new SqlConnection(connectionString);
                // 開啟
                conn.Open();
                // 關閉
                conn.Close();
                // 完成
                result = true;
            }
            catch { }
            finally
            {
                if (conn != null)
                {
                    conn.Dispose();
                    conn = null;
                }
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            string dbSource = builder.DataSource;
            if (result == false)//DB連線失敗
                MessageBox.Show("無法連接資料庫，應用程式無法執行。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                StartBtn.Enabled = true;//可以啟動

            dbSourcelb.Text = dbSource;//設定資料來源
            
        }

        #region Timer Event

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            cache.ClearCache();
            cache.ReadAllTeamToCache();
        }

        DateTime checkDate = DateTime.Now;
        private void OnDateChangeTimedEvent(object sender, ElapsedEventArgs e)
        {
            if(checkDate.Date != DateTime.Now.Date)//換日
            {
                checkDate = DateTime.Now;

                cache.ClearCache();
                BigBallRequest.ClearChangeCache();//清除逾期的緩存
            }
        }
        #endregion

        //程式關閉才釋放資源
        private void MainForm_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (apTimer != null) { apTimer.Close(); }
            if (dateChangeTimer != null) { dateChangeTimer.Close(); }
            cache.StopDependency();         
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            StartBtn.Enabled = false;
            StopBtn.Enabled = true;
            clearBtn.Enabled = true;
            ResetTeamBtn.Enabled = true;
            ResetBtn.Enabled = true;

            //查詢通知 固定時間重新註冊
            apTimer = new System.Timers.Timer();
            apTimer.Elapsed += OnTimedEvent;

            apTimer.Interval = 60 * 60 * 1000;//每小時重建
            apTimer.Enabled = true;

            //查詢通知 換日時重新註冊
            dateChangeTimer = new System.Timers.Timer();
            dateChangeTimer.Elapsed += OnDateChangeTimedEvent;
            dateChangeTimer.Interval = 60 * 1000; //每分鐘檢查
            dateChangeTimer.Enabled = true;

            Task.Run(() => cache.StartDependency());//啟動查詢通知            
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            apTimer.Close();
            dateChangeTimer.Close();
            cache.StopDependency();

            StartBtn.Enabled = true;
            StopBtn.Enabled = false;
            clearBtn.Enabled = false;
            ResetTeamBtn.Enabled = false;
            ResetBtn.Enabled = false;
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            ClearInfo();
        }

        private void  ResetTeamBtn_Click(object sender, EventArgs e)
        {
            cache.ReadAllTeamToCache();
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            cache.ClearCache();
        }

        public void ClearInfo()
        {
            this.lbInfo.Items.Clear();
            this.lbInfo.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  ClearLog");
        }

        // 加入訊息
        public void AddInfo(string msg)
        {
            if (this.lbInfo.Items.Count > 100)
                ClearInfo();

            int index = 0;
            // 空白行
            if (msg == null || string.IsNullOrEmpty(msg.Trim()))
                index = this.lbInfo.Items.Add("");
            else
                index = this.lbInfo.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "                  " + msg);

            this.lbInfo.SelectedIndex = index;

        }
    }

    public static class ExtensionControl
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
