using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region 共用按鈕事件

        /// <summary>
        /// 顯示網頁按鈕事件
        /// </summary>
        private void btnWeb_Click(object sender, EventArgs e)
        {
            try
            {
                // 取得賽事種類 取tabpages中括号中的 EX:荷兰棒球(HB) 取HB
                string sport = GetSportType().ToLower();

                switch (sport)
                {
                    #region 國際棒球

                    // 日棒
                    case "npb":
                        this.ShowWeb(this.webNPB, this.txbNPB.Text);
                        break;
                    // 韓棒
                    case "kbo":
                        this.ShowWeb(this.webKBO, this.txbKBO.Text);
                        break;
                    // 美棒(MLB)
                    case "mlb":
                        this.ShowWeb(this.webMLB, this.txbMLB.Text);
                        break;
                    // 美棒(IL)
                    case "il":
                        break;
                    // 美棒(PCL)
                    case "pcl":
                        break;
                    // 墨棒(LMP)
                    case "lmp":
                        break;
                    // 墨棒(LMB)
                    case "lmb":
                        break;
                    //澳洲棒球
                    case "abl":
                        break;
                    //荷兰棒球(HB)
                    case "hb":
                        break;
                    #endregion 國際棒球

                    #region 台棒

                    // 中華職棒
                    case "cpbl":
                        this.ShowWeb(this.webCPBL, this.txbCPBL.Text);
                        break;
                    // 爆米花
                    case "pl":
                        this.ShowWeb(this.webPL, this.txbPL.Text);
                        break;

                    #endregion 台棒

                    #region 籃球

                    // 日籃
                    case "bj":
                        this.ShowWeb(this.webBJ, this.txbBJ.Text);
                        break;
                    // 韓籃
                    case "kbl":
                        this.ShowWeb(this.webKBL, this.txbKBL.Text);
                        break;
                    // 韓國女籃
                    case "wkbl":
                        this.ShowWeb(this.webWKBL, this.txbWKBL.Text);
                        break;
                    // 美籃(NBA)
                    case "nba":
                        this.ShowWeb(this.webNBA, this.txbNBA.Text);
                        break;
                    // 美國女籃
                    case "wnba":
                        this.ShowWeb(this.webWNBA, this.txbWNBA.Text);
                        break;

                    #endregion 籃球

                    #region 美式足球

                    // NFL
                    case "nfl":
                        this.ShowWeb(this.webNFL, this.txbNFL.Text);
                        break;

                    #endregion 美式足球

                    #region 曲棍球(冰球)

                    // KHL
                    case "khl":
                        this.ShowWeb(this.webKHL, this.txbKHL.Text);
                        break;

                    #endregion 曲棍球(冰球)
                }

                this.btnWebId.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("顯示網頁發生錯誤。{0}{1}", Environment.NewLine, ex.Message), "Warning", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 顯示跟盤編號按鈕事件
        /// </summary>
        private void btnWebId_Click(object sender, EventArgs e)
        {
            //// 開啟 progress bar
            //this.BeginInvoke(new MethodInvoker(() =>
            //{
            //    _frmProgress.ShowDialog();
            //}));
            
            try
            {
                btnWebId.Enabled = false;

                string title = String.Empty;
                Dictionary<string, GameInfo> scheduleList = null;
                Tuple<string, string, Dictionary<string, GameInfo>>  sc = null;

                // 取得賽事種類
                string sport = GetSportType().ToLower();
                string gameType = String.Empty;

                switch (sport)
                {
                    #region 國際棒球

                    // 日棒
                    case "npb":
                        gameType = "BBJP";
                        title = this.tabNPB.Text;
                        scheduleList = this.GetSchedulesByNPB(46, gameType, true);
                        break;
                    // 韓棒
                    case "kbo":
                        gameType = "BBKR";
                        title = this.tabKBO.Text;
                        scheduleList = this.GetSchedulesByKBO(66, gameType);
                        break;
                    // 美棒(MLB)
                    case "mlb":
                        gameType = "BBUS";
                        title = this.tabMLB.Text;
                        scheduleList = this.GetSchedulesByMLB(53, gameType);
                        break;
                    // 美棒(IL)
                    case "il":

                        if (dtpILSDate.Value.Date.CompareTo(dtpILEDate.Value.Date) > 0)
                        {
                            MessageBox.Show("起始日不可大於結束日。");
                            return;
                        }

                        if (dtpILEDate.Value.Date.Subtract(dtpILSDate.Value.Date).Days > 7)
                        {
                            MessageBox.Show("選取總天數不可超過7天。");
                            return;
                        }

                        gameType = "BB3AIL";
                        title = this.tabIL.Text;
                        scheduleList = this.GetSchedulesByIL(73, gameType);
                        break;
                    // 美棒(PCL)
                    case "pcl":

                        if (dtpPCLSDate.Value.Date.CompareTo(dtpPCLEDate.Value.Date) > 0)
                        {
                            MessageBox.Show("起始日不可大於結束日。");
                            return;
                        }

                        if (dtpPCLEDate.Value.Date.Subtract(dtpPCLSDate.Value.Date).Days > 7)
                        {
                            MessageBox.Show("選取總天數不可超過7天。");
                            return;
                        }

                        gameType = "BB3APCL";
                        title = this.tabPCL.Text;
                        scheduleList = this.GetSchedulesByPCL(77, gameType);
                        break;
                    // 墨棒(LMP)
                    case "lmp":

                        if (dtpLMPSDate.Value.Date.CompareTo(dtpLMPEDate.Value.Date) > 0)
                        {
                            MessageBox.Show("起始日不可大於結束日。");
                            return;
                        }

                        if (dtpLMPEDate.Value.Date.Subtract(dtpLMPSDate.Value.Date).Days > 7)
                        {
                            MessageBox.Show("選取總天數不可超過7天。");
                            return;
                        }

                        gameType = "BBMX";
                        title = this.tabLMP.Text;
                        scheduleList = this.GetSchedulesByLMP(71, gameType);
                        break;

                    // 墨棒(LMB)
                    case "lmb":

                        if (dtpLMBSDate.Value.Date.CompareTo(dtpLMBEDate.Value.Date) > 0)
                        {
                            MessageBox.Show("起始日不可大於結束日。");
                            return;
                        }

                        if (dtpLMBEDate.Value.Date.Subtract(dtpLMBSDate.Value.Date).Days > 7)
                        {
                            MessageBox.Show("選取總天數不可超過7天。");
                            return;
                        }

                        gameType = "BBMX2";
                        title = this.tabLMB.Text;
                        scheduleList = this.GetSchedulesByLMB(72, gameType);
                        break;
                    // 澳洲棒球(abl)
                    case "abl":

                        if (dtpABLSDate.Value.Date.CompareTo(dtpABLEDate.Value.Date) > 0)
                        {
                            MessageBox.Show("起始日不可大於結束日。");
                            return;
                        }

                        if (dtpABLEDate.Value.Date.Subtract(dtpABLSDate.Value.Date).Days > 7)
                        {
                            MessageBox.Show("選取總天數不可超過7天。");
                            return;
                        }

                        gameType = "BBAU";
                        title = this.tabABL.Text;
                        scheduleList = this.GetSchedulesByABL(104, gameType);
                        break;
                    // 荷兰棒球(hb)
                    case "hb":

                        if (dtpHBSDate.Value.Date.CompareTo(dtpHBEDate.Value.Date) > 0)
                        {
                            MessageBox.Show("起始日不可大於結束日。");
                            return;
                        }

                        if (dtpHBEDate.Value.Date.Subtract(dtpHBSDate.Value.Date).Days > 7)
                        {
                            MessageBox.Show("選取總天數不可超過7天。");
                            return;
                        }

                        gameType = "BBNL";
                        title = this.tabHB.Text;
                        scheduleList = this.GetSchedulesByHB(119, gameType);
                        break;
                    #endregion 國際棒球

                    #region 台棒

                    // 中華職棒
                    case "cpbl":
                        gameType = "BBTW";
                        title = this.tabCPBL.Text;
                        scheduleList = this.GetSchedulesByCPBL(30, gameType);
                        break;
                    // 爆米花
                    case "pl":
                        gameType = "BBTW7";
                        title = this.tabPL.Text;
                        scheduleList = this.GetSchedulesByPL(98, gameType);
                        break;

                    #endregion 台棒

                    #region 籃球

                    // 日籃
                    case "bj":
                        gameType = "BKJP";
                        title = this.tabBJ.Text;
                        scheduleList = this.GetSchedulesByBJ(10, gameType);
                        break;
                    // 韓籃
                    case "kbl":
                        gameType = "BKKR";
                        title = this.tabKBL.Text;
                        scheduleList = this.GetSchedulesByKBL(14, gameType);
                        break;
                    // 韓國女籃
                    case "wkbl":
                        gameType = "BKKRW";
                        title = this.tabWKBL.Text;
                        scheduleList = this.GetSchedulesByWKBL(15, gameType);
                        break;
                    // 美籃(NBA)
                    case "nba":
                        gameType = "BKUS";
                        title = this.tabNBA.Text;
                        scheduleList = this.GetSchedulesByNBA(1, gameType);
                        break;
                    // 美國女籃
                    case "wnba":
                        gameType = "BKUSW";
                        title = this.tabWNBA.Text;
                        scheduleList = this.GetSchedulesByWNBA(19, gameType);
                        break;
                    // NCAA
                    case "ncaa":

                        if (dtpNCAASDate.Value.Date.CompareTo(dtpNCAAEDate.Value.Date) > 0)
                        {
                            MessageBox.Show("起始日不可大於結束日。");
                            return;
                        }

                        gameType = "BKNCAA";
                        title = this.tabNCAA.Text;
                        scheduleList = this.GetSchedulesByNCAA(30, gameType);
                        break;

                    #endregion 籃球

                    #region 美式足球

                    // NFL
                    case "nfl":
                        gameType = "AFUS";
                        title = this.tabNFL.Text;
                        scheduleList = this.GetSchedulesByNFL(1, gameType);
                        break;

                    #endregion 美式足球

                    #region 曲棍球(冰球)

                    // KHL
                    case "khl":
                        gameType = "IHRU";
                        title = this.tabKHL.Text;
                        scheduleList = this.GetSchedulesByKHL(18, gameType);
                        break;

                    #endregion 曲棍球(冰球)

                    #region Asiascore

                    case "asiascore":
                        sc = OnAsiascoreWebId();
                        if (sc == null) { return; }
                        gameType = sc.Item1;
                        title = sc.Item2;
                        scheduleList = sc.Item3;
                        break;

                    #endregion Asiascore

                    #region 奧訊

                    case "奧訊":
                        sc = OnBet007WebId();
                        if (sc == null) { return; }
                        gameType = sc.Item1;
                        title = sc.Item2;
                        scheduleList = sc.Item3;
                        break;

                    #endregion 奧訊

                    #region BF籃球

                    case "bf籃球":
                        sc = OnBKBFWebId();
                        if (sc == null) { return; }
                        gameType = sc.Item1;
                        title = sc.Item2;
                        scheduleList = sc.Item3;
                        break;

                    #endregion BF籃球
                }

                //// 關閉 progress bar
                //this.BeginInvoke(new MethodInvoker(() =>
                //{
                //    _frmProgress.Close();
                //}));

                this.ShowWebId(gameType, title, scheduleList);

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("顯示跟盤編號發生錯誤。{0}{1}", Environment.NewLine, ex.Message), "Warning", MessageBoxButtons.OK);
            }
            finally
            {
                btnWebId.Enabled = true;
            }
        }
        #endregion 共用按鈕事件
    }
}