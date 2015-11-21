using System;
using System.Linq;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmMain
    {
        #region TabPage SelectedIndexChanged 事件

        // 各類球賽選單
        private void tabPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnWeb.Enabled = true;
            this.btnWebId.Enabled = false;
            bfLoadComplete = false;
            string sport = GetSportType().ToLower();
            switch (sport)
            {
                case "ncaa":
                    this.btnWeb.Enabled = false;
                    this.btnWebId.Enabled = true;
                    break;

                case "奧訊":
                    this.btnWeb.Enabled = false;
                    this.btnWebId.Enabled = false;
                    break;

                case "asiascore":
                    this.btnWeb.Enabled = false;
                    ShowWebIdByWebBrowser();
                    break;

                case "bf籃球":
                    this.btnWeb.Enabled = false;
                    this.btnWebId.Enabled = false;
                    break;
                case "lmp":
                case "lmb":
                case "pcl":
                case "il":
                case "abl":
                case "hb":
                    this.btnWeb.Enabled = false;
                    this.btnWebId.Enabled = true;
                    break;

                default:
                    ShowWebIdByWebBrowser();
                    break;
            }
        }

        /// <summary>
        /// 找到有 WebBrowser 的頁面, 判斷是否 WebBrowser 已經有資料
        /// 若有資料則開啟 WebId 按鈕
        /// </summary>
        private void ShowWebIdByWebBrowser()
        {
            TabPage tabPage = GetSelectdTabPage();
            var webBrowserList = tabPage.Controls.OfType<WebBrowser>().Where(x => x.Document != null);
            if (webBrowserList.Any()) { this.btnWebId.Enabled = true; }
        }


        #endregion TabPage SelectedIndexChanged 事件
    }
}