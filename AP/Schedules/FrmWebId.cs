using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Schedules
{
    public partial class FrmWebId : Form
    {
        public event ScheduleSaveEventHandler OnScheduleSave = null;

        public FrmWebId()
        {
            InitializeComponent();
        }

        private void FrmWebId_Load(object sender, EventArgs e)
        {
            #region 建立全選 CheckBox

            //建立個矩形，等下計算 CheckBox 嵌入 GridView 的位置
            Rectangle rect = dgvSchedule.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + rect.Width / 4 - 10;
            rect.Y = rect.Location.Y + (rect.Height / 2 - 9);

            CheckBox cbHeader = new CheckBox()
            {
                Name = "checkboxHeader",
                TextAlign = ContentAlignment.MiddleRight,
                Size = new Size(18, 18),
                Location = rect.Location
            };
            //全選要設定的事件
            cbHeader.CheckedChanged += new EventHandler(cbHeader_CheckedChanged);

            //將 CheckBox 加入到 dataGridView
            dgvSchedule.Controls.Add(cbHeader);

            // 有資料時 設定全選
            if (dgvSchedule.Rows.Count > 0)
            {
                TriggerCheckedAll();
            }

            #endregion 建立全選 CheckBox
        }

        private void cbHeader_CheckedChanged(object sender, EventArgs e)
        {
            bool check = (sender as CheckBox).Checked;

            foreach (DataGridViewRow dr in dgvSchedule.Rows)
            {
                dr.Cells[0].Value = check;
            }

            dgvSchedule.EndEdit();
        }

        internal void OnDataBind(Dictionary<string, GameInfo> schedules, Dictionary<string, string> teamMapping)
        {
            this.Tag = schedules;
            List<string> dateList = new List<string>();

            int idxBgColor = 0;
            Color[] background = new Color[] { Color.White, Color.FromArgb(192, 255, 192) };

            string dateString = String.Empty;

            foreach (KeyValuePair<string, GameInfo> pair in schedules)
            {
                GameInfo info = pair.Value;
                string away = info.Away;
                string home = info.Home;
                DateTime gameTime = info.GameTime;

                // 隊名比對
                home = (teamMapping.ContainsKey(home)) ? teamMapping[home] : home;
                away = (teamMapping.ContainsKey(away)) ? teamMapping[away] : away;

                // 隊名互換
                info.SwapTeam(ref home, ref away);

                string date = gameTime.ToString("yyyy/MM/dd");
                // 日期列表
                if (!dateList.Contains(date)) { dateList.Add(date); }

                // 建立列
                DataGridViewRow dr = new DataGridViewRow();
                dr.CreateCells(dgvSchedule);
                dr.Cells[1].Value = info.WebID;
                dr.Cells[2].Value = gameTime.ToString("yyyy/MM/dd HH:mm");
                dr.Cells[3].Value = away;
                dr.Cells[4].Value = home;
                dr.Cells[5].Value = info.Comment;
                dr.Cells[6].Value = pair.Key;

                // 不同日期顯示不同底色
                if (!String.IsNullOrEmpty(dateString) && !dateString.Equals(date) ) 
                {
                    idxBgColor++;
                    if (idxBgColor >= background.Length) { idxBgColor = 0; }
                }

                dr.DefaultCellStyle.BackColor = background[idxBgColor];
                dateString = date;

                // 新增列
                dgvSchedule.Rows.Add(dr);

            }

            // 產生日期列表
            dateList.Insert(0, "");
            cboDate.DataSource = dateList;
        }

        #region 按鈕事件

        /// <summary>
        /// 建立賽程按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSchedule_Click(object sender, EventArgs e)
        {
            if (OnScheduleSave != null)
            {
                Dictionary<string, GameInfo> schedules = this.Tag as Dictionary<string, GameInfo>;
                if (schedules != null)
                {
                    foreach (DataGridViewRow dr in dgvSchedule.Rows)
                    {
                        // 移除未勾選的賽程
                        bool check = Convert.ToBoolean(dr.Cells[0].Value);
                        if (!check)
                        {
                            string key = dr.Cells[6].Value.ToString();
                            schedules.Remove(key);
                        }
                    }
                }

                // 觸發事件
                ScheduleEventArgs args = new ScheduleEventArgs(this.Text, schedules);
                OnScheduleSave(this, args);
            }
        }

        /// <summary>
        /// 關閉按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 勾選賽程按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChecked_Click(object sender, EventArgs e)
        {
            string dateString = cboDate.SelectedItem.ToString();
            if (String.IsNullOrEmpty(dateString))
            {
                // 全選
                TriggerCheckedAll();
            }
            else
            {
                // 依照日期選取
                TriggerCheckedByDate(dateString);
            }
        }       

        #endregion 按鈕事件

        #region 私有方法

        /// <summary>
        /// 全選事件
        /// </summary>
        private void TriggerCheckedAll()
        {
            CheckBox cbk = dgvSchedule.Controls.Find("checkboxHeader", true)[0] as CheckBox;
            cbk.Checked = true;
            cbHeader_CheckedChanged(cbk, EventArgs.Empty);
        }

        /// <summary>
        /// 依照日期選取
        /// </summary>
        /// <param name="dateString">日期字串</param>
        private void TriggerCheckedByDate(string dateString)
        {
            // 清除全選按鈕
            (dgvSchedule.Controls.Find("checkboxHeader", true)[0] as CheckBox).Checked = false;

            // 根據日期選擇
            foreach (DataGridViewRow dr in dgvSchedule.Rows)
            {
                DateTime gameTime = Convert.ToDateTime(dr.Cells[2].Value);
                if (dateString.Equals(gameTime.ToString("yyyy/MM/dd")))
                {
                    dr.Cells[0].Value = true;
                }
                else
                {
                    dr.Cells[0].Value = false;
                }
            }

            dgvSchedule.EndEdit();
        }

        #endregion 私有方法
    }

    #region FrmWebId 事件

    /// <summary>
    /// 賽程資料事件參數
    /// </summary>
    public class ScheduleEventArgs : EventArgs
    {
        public ScheduleEventArgs(string title, Dictionary<string, GameInfo> schedules, List<string> filters = null)
        {
            Title = title;
            ScheduleList = schedules;
            //FilterItems = filters ?? new List<string>();
        }

        public string Title { private set; get; }

        public Dictionary<string, GameInfo> ScheduleList { private set; get; }

        //public List<string> FilterItems { private set; get; }
    }

    // 事件委派
    public delegate void ScheduleSaveEventHandler(object sender, ScheduleEventArgs e);

    #endregion FrmWebId 事件
}