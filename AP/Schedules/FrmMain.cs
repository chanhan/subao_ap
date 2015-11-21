using Schedules.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Common = Schedules.CommonWS;
using Schedule = Schedules.ScheduleWS;
using Team = Schedules.TeamWS;

namespace Schedules
{
    public partial class FrmMain : Form
    {
        private List<SourceInfo> _lstSourceInfo;
        private Common.CommonService _wsCommon = new Common.CommonService();
        private Team.TeamService _wsTeam = new Team.TeamService();
        private Schedule.ScheduleService _wsSchedule = new Schedule.ScheduleService();
        private Dictionary<string, Dictionary<string, string>> _teamNameMapping = new Dictionary<string, Dictionary<string, string>>();

        private List<RadioButton> _bet007RadioButtonGroup = new List<RadioButton>();
        private List<RadioButton> _bfRadioButtonGroup = new List<RadioButton>();


        private readonly string _token = EncryptHelper.AESEncrypt("sp8888.net/schedules/validate");

        private FrmProgress _frmProgress = new FrmProgress();


        #region Form

        public FrmMain()
        {
            InitializeComponent();
            // 設定
            this.Icon = Properties.Resources.Flag;

            // 版本
            this.Text += "  v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // WS client timeout 時間
            int timeout;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["WSTimeout"], out timeout))
            {
                timeout = 120;
            }

            // 轉為毫秒
            timeout *= 1000;

            _wsCommon.Timeout = timeout;
            _wsSchedule.Timeout = timeout;
            _wsTeam.Timeout = timeout;

            // 進度條視窗
            _frmProgress.Hide();


            // 取得 ip
            //IPAddress ip = GetLocalIP();
            //_token = (ip == null) ? String.Empty : ip.ToString();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // 登入驗證
            LoginCheck();

            this.Size = new Size(1050, 768);

            // 隱藏目前沒有的
            this.tabMain.TabPages.Remove(this.tabHockey);

            // 取得奧訊 RadioButtonGroup
            GetBet007RadioButtonGroup();
            // 取得BF籃球 RadioButtonGroup
            GetBFRadioButtonGroup();
            // 判斷使用資料來源
            ChangeScheduleSource();

            // 取得隊伍對應表
            GetTeamNameMapping();
        }

        private void tabMain_Click(object sender, EventArgs e)
        {
            txtBet007SDate.Value = DateTime.Now.Date;
            txtBet007EDate.Value = DateTime.Now.AddDays(1).Date;
        }

        #endregion Form

        #region Asiascore

        /// <summary>
        /// AsiaScore 取得 WebId
        /// </summary>
        /// <returns>Tuple 物件
        /// <para>Item1: 賽事種類</para>
        /// <para>Item1: 標題</para>
        /// <para>Item2: 賽程資料</para>
        /// </returns>
        private Tuple<string, string, Dictionary<string, GameInfo>> OnAsiascoreWebId()
        {
            string gameType = String.Empty;
            string title = String.Empty;
            Dictionary<string, GameInfo> schedules = null;

            // 依選擇取得資料並顯示 WebID
            if (robCBA.Checked)
            {
                gameType = "BKCN";
                title = robCBA.Text;
                schedules = this.GetSchedulesByAsiascore(16, gameType, "china", " cba");
            }
            if (robEuroleague.Checked)
            {
                gameType = "BKEL";
                title = robEuroleague.Text;
                schedules = this.GetSchedulesByAsiascore(13, gameType, "europe", " euroleague");
            }
            if (robEurocup.Checked)
            {
                gameType = "BKEL2";
                title = robEurocup.Text;
                schedules = this.GetSchedulesByAsiascore(26, gameType, "europe", " eurocup");
            }
            if (robVTB.Checked)
            {
                gameType = "BKVTB";
                title = robVTB.Text;
                schedules = this.GetSchedulesByAsiascore(18, gameType, "europe", " vtb");
            }
            if (robNBL.Checked)
            {
                gameType = "BKAU";
                title = robNBL.Text;
                schedules = this.GetSchedulesByAsiascore(17, gameType, "australia", " nbl");
            }
            if (robFIBA.Checked)
            {
                gameType = "BKFIBA";
                title = robFIBA.Text;
                schedules = this.GetSchedulesByAsiascore(22, gameType, "asia", " asia championship", true);
            }
            if (robEBT.Checked)
            {
                gameType = "BKEBT";
                title = robEBT.Text;
                schedules = this.GetSchedulesByAsiascore(23, gameType, "europe", " eurobasket", true);
            }
            if (robNHL.Checked)
            {
                gameType = "IHUS";
                title = robNHL.Text;
                schedules = this.GetSchedulesByAsiascore(1, gameType, "usa", " nhl", true);
            }
            if (robAHL.Checked)
            {
                gameType = "IHUS2";
                title = robAHL.Text;
                schedules = this.GetSchedulesByAsiascore(21, gameType, "usa", " ahl", true);
            }
            if (robKHL.Checked)
            {
                gameType = "IHRU";
                title = robKHL.Text;
                schedules = this.GetSchedulesByAsiascore(18, gameType, "russia", " khl");
            }
            if (robACB.Checked)
            {
                gameType = "BKACB";
                title = robACB.Text;
                schedules = this.GetSchedulesByAsiascore(27, gameType, "spain", " acb");
            }
            if (robBBL.Checked)
            {
                gameType = "BKBBL";
                title = robBBL.Text;
                schedules = this.GetSchedulesByAsiascore(28, gameType, "germany", " bbl");
            }
            if (robUFC.Checked)
            {
                gameType = "UFC";
                title = robUFC.Text;
                schedules = this.GetSchedulesByAsiascore(0, gameType, "world", "ufc");
            }
            //if (robBF.Checked)
            //{
            //    gameType = "BKBF";
            //    title = robBF.Text;
            //    schedules = this.GetSchedulesByAsiascore(31, gameType, "", "");
            //}
            return Tuple.Create(gameType, title, schedules);
        }

        private string robUrl = null;

        private void rob_CheckedChanged(object sender, EventArgs e)
        {
            string url = null;

            if (robCBA.Checked)
                url = "http://www.asiascore.com/basketball/china/cba/";
            if (robEuroleague.Checked)
                url = "http://www.asiascore.com/basketball/europe/euroleague/";
            if (robEurocup.Checked)
                url = "http://www.asiascore.com/basketball/europe/eurocup/";
            if (robVTB.Checked)
                url = "http://www.asiascore.com/basketball/europe/vtb-united-league/";
            if (robNBL.Checked)
                url = "http://www.asiascore.com/basketball/australia/nbl/";
            if (robFIBA.Checked)
                url = "http://www.asiascore.com/basketball/asia/asia-championship/";
            if (robEBT.Checked)
                url = "http://www.asiascore.com/basketball/europe/eurobasket/";
            if (robNHL.Checked)
                url = "http://www.asiascore.com/hockey/usa/nhl/";
            if (robAHL.Checked)
                url = "http://www.asiascore.com/hockey/usa/ahl/";
            if (robKHL.Checked)
                url = "http://www.asiascore.com/hockey/russia/khl/";
            if (robACB.Checked)
                url = "http://www.asiascore.com/basketball/spain/acb/";
            if (robBBL.Checked)
                url = "http://www.asiascore.com/basketball/germany/bbl/";
            if (robUFC.Checked)
                url = "http://www.asiascore.com/mma/";
            //if (robBF.Checked)
            //    url = "http://www.asiascore.com/basketball/";
            if (!string.IsNullOrEmpty(url) && robUrl != url)
            {
                robUrl = url;

                this.btnWebId.Enabled = true;

                // 顯示網頁
                this.ShowWeb(this.webAsiascore, url);
            }
        }

        #endregion Asiascore

        #region 奧訊

        private Tuple<string, string, Dictionary<string, GameInfo>> OnBet007WebId()
        {
            if (txtBet007SDate.Value.Date.CompareTo(txtBet007EDate.Value.Date) > 0)
            {
                MessageBox.Show("起始日不可大於結束日。");
                return null;
            }

            string gameType = String.Empty;
            string title = String.Empty;
            Dictionary<string, GameInfo> schedules = null;

            if (robBet007ACB.Checked)
            {
                gameType = "BKACB";
                title = robBet007ACB.Text;
                schedules = this.GetSchedulesByBet007(27, gameType, "20", true);
            }
            if (robBet007Euroleague.Checked)
            {
                gameType = "BKEL";
                title = robBet007Euroleague.Text;
                schedules = this.GetSchedulesByBet007(13, gameType, "7", true);
            }
            if (robBet007Eurocup.Checked)
            {
                gameType = "BKEL2";
                title = robBet007Eurocup.Text;
                schedules = this.GetSchedulesByBet007(26, gameType, "21", true);
            }
            if (robBet007NBL.Checked)
            {
                gameType = "BKAU";
                title = robBet007NBL.Text;
                schedules = this.GetSchedulesByBet007(17, gameType, "14", true);
            }
            if (robBet007KBL.Checked)
            {
                gameType = "BKKR";
                title = robBet007KBL.Text;
                schedules = this.GetSchedulesByBet007(14, gameType, "15", true);
            }
            if (robBet007VTB.Checked)
            {
                gameType = "BKVTB";
                title = robBet007VTB.Text;
                schedules = this.GetSchedulesByBet007(18, gameType, "171", true);
            }
            if (robBet007WKBL.Checked)
            {
                gameType = "BKKRW";
                title = robBet007WKBL.Text;
                schedules = this.GetSchedulesByBet007(15, gameType, "106", true);
            }
            if (robBet007CBA.Checked)
            {
                gameType = "BKCN";
                title = robBet007CBA.Text;
                schedules = this.GetSchedulesByBet007(16, gameType, "5", true);
            }
            if (robBet007NCAA.Checked)//NCAA 改跟官網 因為主客隊互換問題 無使用
            {
                gameType = "BKNCAA";
                title = robBet007NCAA.Text;
                schedules = this.GetSchedulesByBet007(30, gameType, "8", true);
            }
            if (robBet007CNBL.Checked) //NBL 中國男子籃球甲級聯賽
            {
                gameType = "BKNBL";
                title = robBet007CNBL.Text;
                schedules = this.GetSchedulesByBet007(35, gameType, "41", true);
            }

            return Tuple.Create(gameType, title, schedules);
        }

        private void btnBet007_CheckedChanged(object sender, EventArgs e)
        {
            // 同一時間僅能點選一個 RadioButton
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {
                foreach (RadioButton other in _bet007RadioButtonGroup)
                {
                    if (other == rb) { continue; }
                    other.Checked = false;
                }
            }

            this.btnWebId.Enabled = true;
        }

        #endregion 奧訊

        #region BF籃球

        private Tuple<string, string, Dictionary<string, GameInfo>> OnBKBFWebId()
        {
            string gameType = String.Empty;
            string title = String.Empty;
            Dictionary<string, GameInfo> schedules = null;
            //if (robBFVTB.Checked)
            //{
            //    gameType = "BKVTB";
            //    title = robBFVTB.Text;
            //    schedules = this.GetSchedulesByBkBF(18, gameType, "VTB", true);
            //}
            //if (robBFNBA.Checked)
            //{
            //    gameType = "BKUS";
            //    title = robBFNBA.Text;
            //    schedules = this.GetSchedulesByBkBF(1, gameType,"USA", "NBA", true);
            //}
            if (robBFEuroleague.Checked)
            {
                gameType = "BKEL";
                title = robBFEuroleague.Text;
                schedules = this.GetSchedulesByBkBF(13, gameType, "EUROPE", "Euroleague", true);
            }
            if (robBFEurocup.Checked)
            {
                gameType = "BKEL2";
                title = robBFEurocup.Text;
                schedules = this.GetSchedulesByBkBF(26, gameType, "EUROPE", "Eurocup", true);
            }
           
            //if (robBFEuroChallenge.Checked)//聯盟未設置
            //{
            //   // gameType = "BKEL2";
            //    title = robBFEuroChallenge.Text;
            //    //schedules = this.GetSchedulesByBkBF(26, gameType,  "EUROPE","EuroChallenge - First stage", true);
            //}
            //if (robBFEuroleagueWomen.Checked)//聯盟未設置
            //{
            //   // gameType = "BKEL2";
            //    title = robBFEuroleagueWomen.Text;
            //    //schedules = this.GetSchedulesByBkBF(26, gameType,  "EUROPE","Euroleague Women", true);
            //}
            if (robBFEuroleague.Checked)//聯盟未設置
            {
               // gameType = "BKEL2";
                title = robBFEuroleague.Text;
                //schedules = this.GetSchedulesByBkBF(26, gameType, "EUROPE", "EuroCup Women", true);
            }
            
            //if (robBet007NBL.Checked)
            //{
            //    gameType = "BKAU";
            //    title = robBet007NBL.Text;
            //    schedules = this.GetSchedulesByBet007(17, gameType, "14", true);
            //}
            if (robBFKBL.Checked)
            {
                gameType = "BKKR";
                title = robBFKBL.Text;
                schedules = this.GetSchedulesByBkBF(14, gameType, "SOUTH KOREA", "KBL", true);
            }
            if (robBFKBLWomen.Checked)
            {
                gameType = "BKKRW";
                title = robBFKBLWomen.Text;
                schedules = this.GetSchedulesByBkBF(15, gameType, "SOUTH KOREA", "WKBL Women", false);
            }
            //if (robBet007VTB.Checked)
            //{
            //    gameType = "BKVTB";
            //    title = robBet007VTB.Text;
            //    schedules = this.GetSchedulesByBet007(18, gameType, "171", true);
            //}
            //if (robBet007WKBL.Checked)
            //{
            //    gameType = "BKKRW";
            //    title = robBet007WKBL.Text;
            //    schedules = this.GetSchedulesByBet007(15, gameType, "106", true);
            //}
            if (robBFCBA.Checked)
            {
                gameType = "BKCN";
                title = robBFCBA.Text;
                schedules = this.GetSchedulesByBkBF(16, gameType, "CHINA", "CBA", true);
            }
            //if (robBFACB.Checked)
            //{
            //    gameType = "BKACB";
            //    title = robBFACB.Text;
            //    schedules = this.GetSchedulesByBkBF(27, gameType, "ACB", true);
            //}
            
            if (robBFNCAA.Checked)
            {
                gameType = "BKNCAA";
                title = robBFNCAA.Text;
                schedules = this.GetSchedulesByBkBF(30, gameType, "USA", "NCAA I-A", true);
            }
            //if (robBet007CNBL.Checked) //NBL 中國男子籃球甲級聯賽
            //{
            //    gameType = "BKNBL";
            //    title = robBet007CNBL.Text;
            //    schedules = this.GetSchedulesByBet007(35, gameType, "41", true);
            //}

            return Tuple.Create(gameType, title, schedules);
        }

        private void robBF_CheckedChanged(object sender, EventArgs e)
        {
            // 同一時間僅能點選一個 RadioButton
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {
                foreach (RadioButton other in _bfRadioButtonGroup)
                {
                    if (other == rb) { continue; }
                    other.Checked = false;
                }
            }
            if (bfLoadComplete == false)
            {
                //this.ShowWeb(webBF, "http://www.asiascore.com/basketball/");
                Init();
                DownloadData(headers);
            }
        }
        #endregion BF籃球
     
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

        #region 其他方法

        #region 取得奧訊 RadioButtonList

        /// <summary>
        /// 取得奧訊 RadioButtonList
        /// </summary>
        private void GetBet007RadioButtonGroup()
        {
            var group = gpbBet007.Controls.OfType<GroupBox>();

            foreach (GroupBox gpb in group)
            {
                var rbList = gpb.Controls.OfType<RadioButton>();
                _bet007RadioButtonGroup.AddRange(rbList);
            }
        }

        #endregion 取得奧訊 RadioButtonList


        #region 取得BF RadioButtonList

        /// <summary>
        /// 取得BF RadioButtonList
        /// </summary>
        private void GetBFRadioButtonGroup()
        {
            var group = gpbBF.Controls.OfType<GroupBox>();

            foreach (GroupBox gpb in group)
            {
                var rbList = gpb.Controls.OfType<RadioButton>();
                _bfRadioButtonGroup.AddRange(rbList);
            }
        }

        #endregion 取得BF RadioButtonList

        #region 登入驗證

        /// <summary>
        /// 登入驗證
        /// </summary>
        private void LoginCheck()
        {
            try
            {
                Common.ExecuteResult result = _wsCommon.LoginCheck(_token);
                if (result.ResultType > 0)
                {
                    // 驗證失敗 關閉程式
                    ShowExecuteResultError("Warning", "登入驗證失敗。", result.ResultMessage);
                    this.Close();
                }
            }
            catch (WebException wex)
            {
                MessageBox.Show(this, wex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (wex.Status == WebExceptionStatus.ConnectFailure) { this.Close(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("無法檢查登入驗證。 {0}", ex.Message), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
        }

        #endregion


        #region 取得隊伍對應資料表

        /// <summary>
        /// 取得隊伍對應資料表(全部)
        /// </summary>
        private void GetTeamNameMapping()
        {
            try
            {
                Team.ExecuteDataResult result = _wsTeam.GetTeamByUnionAll(_token);
                if (result.ResultType == (int)ErrorCode.Success)
                {
                    Dictionary<string, string> dic = null;
                    DataSet ds = result.ResultDs;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            string gameType = dr.Field<string>("GameType");
                            if (String.IsNullOrEmpty(gameType)) { continue; }

                            if (_teamNameMapping.ContainsKey(gameType) == false) { _teamNameMapping[gameType] = new Dictionary<string, string>(); }
                            dic = _teamNameMapping[gameType];

                            string showName = dr.Field<string>("ShowName");
                            string webName = (dr.IsNull("WebName")) ? String.Empty : dr.Field<string>("WebName");

                            if (!String.IsNullOrEmpty(webName))
                            {
                                // 切割多來源 WebName
                                string[] names = webName.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string s in names)
                                {
                                    dic[s] = showName;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ShowExecuteResultError("Warning", "取得隊伍名稱對應表發生錯誤。", result.ResultMessage);
                    // 驗證失敗 關閉程式
                    if (result.ResultType == (int)ErrorCode.ValidateFail) { this.Close(); }
                }
            }
            catch (WebException wex)
            {
                MessageBox.Show(this, wex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (wex.Status == WebExceptionStatus.ConnectFailure) { this.Close(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("無法取得隊伍名稱對應資料表。 {0}", ex.Message), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// 取得隊伍資料
        /// </summary>
        /// <param name="gameType">賽事種類</param>
        /// <returns>隊伍名稱資料</returns>
        private Dictionary<string, string> GetTeamName(string gameType)
        {
            Dictionary<string, string> dic = new Dictionary<string,string>();

            if (!String.IsNullOrEmpty(gameType) && _teamNameMapping.ContainsKey(gameType))
            {
                dic = _teamNameMapping[gameType];
            }

            return dic;
        }

        #endregion 取得隊伍對應資料表

        #region 取得資料來源相關 method

        /// <summary>
        /// 判斷使用資料來源
        /// </summary>
        private void ChangeScheduleSource()
        {
            // 取得來源設定
            _lstSourceInfo = GetSourceInfo();

            foreach (SourceInfo info in _lstSourceInfo)
            {
                // 檢查來源編號跟來源名稱都要存在
                if (!string.IsNullOrEmpty(info.SourceID) && !string.IsNullOrEmpty(info.GameSource))
                {
                    // 判斷 UI 是否啟用
                    if (info.GameType == "BKACB")
                    {
                        if (string.Compare(info.GameSource, "Asiascore", true) == 0)
                        {
                            robACB.Enabled = true;
                            robBFACB.Enabled = true;
                        }
                        else if (string.Compare(info.GameSource, "奧訊", true) == 0)
                            robBet007ACB.Enabled = true;
                    }
                    else if (info.GameType == "BKEL")
                    {
                        if (string.Compare(info.GameSource, "Asiascore", true) == 0)
                        {
                            robEuroleague.Enabled = true;
                            robBFEuroleague.Enabled = true;
                        }
                        else if (string.Compare(info.GameSource, "奧訊", true) == 0)
                            robBet007Euroleague.Enabled = true;
                    }
                    else if (info.GameType == "BKEL2")
                    {
                        if (string.Compare(info.GameSource, "Asiascore", true) == 0)
                        {
                            robEurocup.Enabled = true;
                            robBFEuroleague.Enabled = true;
                        }
                        else if (string.Compare(info.GameSource, "奧訊", true) == 0)
                            robBet007Eurocup.Enabled = true;
                    }
                    else if (info.GameType == "BKAU")
                    {
                        if (string.Compare(info.GameSource, "Asiascore", true) == 0)
                        {
                            robNBL.Enabled = true;
                            robBFNBL.Enabled = true;
                        }
                        else if (string.Compare(info.GameSource, "奧訊", true) == 0)
                            robBet007NBL.Enabled = true;
                    }
                    else if (info.GameType == "BKKR")
                    {
                        if (string.Compare(info.GameSource, "Asiascore", true) == 0)
                            robBet007KBL.Enabled = false;
                        else if (string.Compare(info.GameSource, "奧訊", true) == 0)
                        {
                            robBet007KBL.Enabled = true;
                            this.tabBasketballPage.TabPages.Remove(this.tabKBL);
                        }
                    }
                    else if (info.GameType == "BKVTB")
                    {
                        if (string.Compare(info.GameSource, "Asiascore", true) == 0)
                        {
                            robVTB.Enabled = true;
                            robBFVTB.Enabled = true;
                        }
                        else if (string.Compare(info.GameSource, "奧訊", true) == 0)
                            robBet007VTB.Enabled = true;
                    }
                    else if (info.GameType == "BKCN")
                    {
                        if (string.Compare(info.GameSource, "Asiascore", true) == 0)
                        {
                            robCBA.Enabled = true;
                            robBFCBA.Enabled = true;
                        }
                        else if (string.Compare(info.GameSource, "奧訊", true) == 0)
                            robBet007CBA.Enabled = true;
                    }
                    else if (info.GameType == "BKKRW")
                    {
                        if (string.Compare(info.GameSource, "官網", true) == 0)
                            robBet007WKBL.Enabled = false;
                        else if (string.Compare(info.GameSource, "奧訊", true) == 0)
                        {
                            robBet007WKBL.Enabled = true;
                            this.tabBasketballPage.TabPages.Remove(this.tabWKBL);
                        }
                    }
                    else if (info.GameType == "BKNCAA")
                    {
                        if (string.Compare(info.GameSource, "Asiascore", true) == 0)
                        {
                            robBFNCAA.Enabled = true;
                        }
                        else if (string.Compare(info.GameSource, "奧訊", true) == 0)
                        {
                            robBFNCAA.Enabled = true;
                        }
                    }
                    else
                        continue;
                }
                else
                    continue;
            }
        }

        /// <summary>
        /// 取得資料來源資訊
        /// </summary>
        /// <returns></returns>
        private List<SourceInfo> GetSourceInfo()
        {
            List<SourceInfo> lst = new List<SourceInfo>();

            try
            {
                Common.ExecuteDataResult result = _wsCommon.GetGameSourceBySourceID(_token, null);
                if (result.ResultType == (int)ErrorCode.Success)
                {
                    DataSet ds = result.ResultDs;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            SourceInfo info = new SourceInfo()
                            {
                                AllianceID = dr.Field<int>("AllianceID"),
                                GameType = dr.Field<string>("GameType"),
                                SourceID = dr.Field<string>("SourceType"),
                                GameSource = dr.Field<string>("GameSource")
                            };
                            lst.Add(info);
                        }
                    }
                }
                else
                {
                    ShowExecuteResultError("Warning", "取得賽事來源資料發生錯誤。", result.ResultMessage);
                    // 驗證失敗 關閉程式
                    if (result.ResultType == (int)ErrorCode.ValidateFail) { this.Close(); }
                }
            }
            catch (WebException wex)
            {
                MessageBox.Show(this, wex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (wex.Status == WebExceptionStatus.ConnectFailure) { this.Close(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("無法取得賽事來源資料。 {0}", ex.Message), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return lst;
        }

        /// <summary>
        /// 取得球賽使用的來源代碼
        /// </summary>
        /// <param name="allianceId"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        private string GetGameUseSourceID(int allianceId, string gameType)
        {
            var result = from s in _lstSourceInfo
                         where s.AllianceID == allianceId && s.GameType == gameType
                         select s;
            if (result.FirstOrDefault() != null)
                return result.Select(p => p.SourceID).ToList()[0];
            else
                return string.Empty;
        }

        #endregion 取得資料來源相關 method

        #region 取得選取的頁籤

        private TabPage GetSelectdTabPage()
        {
            TabPage tabPage = tabMain.SelectedTab;

            if (tabPage != null)
            {
                var ctrlList = tabPage.Controls.OfType<TabControl>();
                if (ctrlList.Any())
                {
                    foreach (TabControl tc in ctrlList)
                    {
                        if (tc.SelectedTab != null)
                        {
                            tabPage = tc.SelectedTab;
                            break;
                        }
                    }
                }
            }

            return tabPage;
        }

        /// <summary>
        /// 取得比賽種類
        /// </summary>
        /// <returns>種類名稱</returns>
        /// <remarks>取 () 內文字</remarks>
        private string GetSportType()
        {
            string type = String.Empty;
            TabPage tabPage = GetSelectdTabPage();

            if (tabPage != null)
            {
                type = tabPage.Text;
                switch (type)
                {
                    case "AsiaScore":
                    case "奧訊":
                    case "BF籃球":
                        break;

                    default:
                        int sIdx = type.IndexOf("(");
                        int eIdx = type.IndexOf(")");
                        if (sIdx != -1 && eIdx != -1)
                        {
                            type = type.Substring(sIdx + 1, (eIdx - (sIdx + 1)));
                        }
                        break;
                }
            }
            return type;
        }

        #endregion 取得選取的頁籤





        //#region 取得本機 IP 位址
        ///// <summary>
        ///// 取得本機 IP 位址。
        ///// </summary>
        ///// <returns>取得本機 IP 位址中第一個 符合 InterNetwork的 IP 位址</returns>
        ///// using System.Net;
        ///// using System.Net.Sockets;
        //private IPAddress GetLocalIP() // 取得本機 IP 位址
        //{
        //    // 取得本機 IP 容器
        //    IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
        //    // 本機 IP 清單
        //    foreach (IPAddress ip in ipHostEntry.AddressList)
        //    {
        //        // 判斷本機 IP 類型是否為 InterNetwork (IP 第四版位址)
        //        if (ip.AddressFamily == AddressFamily.InterNetwork)
        //            return ip;
        //    }

        //    return null;
        //}
        //#endregion

        #endregion 其他方法
    }
}