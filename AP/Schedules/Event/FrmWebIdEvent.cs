using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region 共用按鈕事件

        private void FrmWebId_OnScheduleSave(object sender, ScheduleEventArgs e)
        {
            string setTypeVal = String.Empty;
            bool delTime = false;
            string sport = GetSportType().ToLower();

            switch (sport)
            {
                #region 國際棒球

                // 日棒
                case "npb":
                    setTypeVal = "JanpenTime";
                    delTime = true;
                    break;
                // 韓棒
                case "kbo":
                    setTypeVal = "JanpenTime";
                    delTime = true;
                    break;
                // 美棒(MLB)
                case "mlb":
                    setTypeVal = "EasternTime";
                    delTime = false;
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

                #endregion 國際棒球

                #region 台棒

                // 中華職棒
                case "cpbl":
                    break;
                // 爆米花
                case "pl":
                    break;

                #endregion 台棒

                #region 籃球

                // 日籃
                case "bj":
                    setTypeVal = "JanpenTime";
                    delTime = true;
                    break;
                // 韓籃
                case "kbl":
                    setTypeVal = "JanpenTime";
                    delTime = true;
                    break;
                // 韓國女籃
                case "wkbl":
                    setTypeVal = "JanpenTime";
                    delTime = true;
                    break;
                // 美籃(NBA)
                case "nba":
                    setTypeVal = "EasternTime";
                    delTime = false;
                    break;
                // 美國女籃
                case "wnba":
                    setTypeVal = "EasternTime";
                    delTime = false;
                    break;
                // NCAA
                case "ncaa":
                    setTypeVal = "";
                    delTime = false;
                    break;

                #endregion 籃球

                #region 美式足球

                // NFL
                case "nfl":
                    setTypeVal = "EasternTime";
                    delTime = false;
                    break;

                #endregion 美式足球

                #region 曲棍球(冰球)

                // KHL
                case "khl":
                    setTypeVal = "RussiaTime";
                    delTime = false;
                    break;

                #endregion 曲棍球(冰球)

                #region Asiascore

                case "asiascore":
                case "bf籃球":
                    break;

                #endregion Asiascore

                #region 奧訊

                case "奧訊":
                    break;

                #endregion 奧訊
            }

            bool success = this.SaveSchedule(e.Title, e.ScheduleList, setTypeVal, delTime);
            // 儲存成功: 關閉顯示視窗
            if (success) 
            {
                FrmWebId frm = sender as FrmWebId;
                if (frm != null) { frm.Close(); }
            }
        }

        #endregion 共用按鈕事件
    }
}