namespace Monitor
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.nfiMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.tspMain = new System.Windows.Forms.ToolStrip();
            this.tspbtnExit = new System.Windows.Forms.ToolStripButton();
            this.tspbtnReload = new System.Windows.Forms.ToolStripButton();
            this.tspbtnLastDate = new System.Windows.Forms.ToolStripButton();
            this.tspbtnNeedFollow = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.lbServerStatus = new System.Windows.Forms.ToolStripLabel();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.colGID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTypeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAway = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatusText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWebId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChangeCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChangeTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timerLoad = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.dbSourcelb = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.weblb = new System.Windows.Forms.ToolStripLabel();
            this.lbServerInfo = new System.Windows.Forms.ToolStripLabel();
            this.tspMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nfiMain
            // 
            this.nfiMain.Text = "notifyIcon1";
            this.nfiMain.Visible = true;
            this.nfiMain.BalloonTipClicked += new System.EventHandler(this.nfiMain_BalloonTipClicked);
            this.nfiMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.nfiMain_MouseDoubleClick);
            // 
            // tspMain
            // 
            this.tspMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tspMain.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tspMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tspbtnExit,
            this.tspbtnReload,
            this.tspbtnLastDate,
            this.tspbtnNeedFollow,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.lbServerInfo,
            this.lbServerStatus});
            this.tspMain.Location = new System.Drawing.Point(0, 0);
            this.tspMain.Name = "tspMain";
            this.tspMain.Size = new System.Drawing.Size(784, 31);
            this.tspMain.TabIndex = 0;
            this.tspMain.Text = "Tools";
            // 
            // tspbtnExit
            // 
            this.tspbtnExit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tspbtnExit.Image = ((System.Drawing.Image)(resources.GetObject("tspbtnExit.Image")));
            this.tspbtnExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspbtnExit.Name = "tspbtnExit";
            this.tspbtnExit.Size = new System.Drawing.Size(60, 28);
            this.tspbtnExit.Text = "結束";
            this.tspbtnExit.Click += new System.EventHandler(this.tspbtnExit_Click);
            // 
            // tspbtnReload
            // 
            this.tspbtnReload.Image = ((System.Drawing.Image)(resources.GetObject("tspbtnReload.Image")));
            this.tspbtnReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspbtnReload.Name = "tspbtnReload";
            this.tspbtnReload.Size = new System.Drawing.Size(84, 28);
            this.tspbtnReload.Text = "重讀賽程";
            this.tspbtnReload.Click += new System.EventHandler(this.tspbtnReload_Click);
            // 
            // tspbtnLastDate
            // 
            this.tspbtnLastDate.Image = ((System.Drawing.Image)(resources.GetObject("tspbtnLastDate.Image")));
            this.tspbtnLastDate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspbtnLastDate.Name = "tspbtnLastDate";
            this.tspbtnLastDate.Size = new System.Drawing.Size(144, 28);
            this.tspbtnLastDate.Text = "最後建立的賽程日期";
            this.tspbtnLastDate.Click += new System.EventHandler(this.tspbtnLastDate_Click);
            // 
            // tspbtnNeedFollow
            // 
            this.tspbtnNeedFollow.Image = ((System.Drawing.Image)(resources.GetObject("tspbtnNeedFollow.Image")));
            this.tspbtnNeedFollow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspbtnNeedFollow.Name = "tspbtnNeedFollow";
            this.tspbtnNeedFollow.Size = new System.Drawing.Size(120, 28);
            this.tspbtnNeedFollow.Text = "需要開啟的跟盤";
            this.tspbtnNeedFollow.Click += new System.EventHandler(this.tspbtnNeedFollow_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(22, 0, 0, 0);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(104, 28);
            this.toolStripLabel1.Text = "伺服器連線狀態：";
            // 
            // lbServerStatus
            // 
            this.lbServerStatus.ForeColor = System.Drawing.Color.Red;
            this.lbServerStatus.Name = "lbServerStatus";
            this.lbServerStatus.Size = new System.Drawing.Size(44, 28);
            this.lbServerStatus.Text = "未連線";
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colGID,
            this.colType,
            this.colTypeName,
            this.colDate,
            this.colTime,
            this.colDateTime,
            this.colAway,
            this.colHome,
            this.colStatus,
            this.colStatusText,
            this.colWebId,
            this.colChangeCount,
            this.colChangeTime});
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvData.Location = new System.Drawing.Point(0, 62);
            this.dgvData.Name = "dgvData";
            this.dgvData.ReadOnly = true;
            this.dgvData.RowTemplate.Height = 23;
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.Size = new System.Drawing.Size(784, 500);
            this.dgvData.TabIndex = 1;
            this.dgvData.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvData_DataBindingComplete);
            this.dgvData.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvData_RowPostPaint);
            // 
            // colGID
            // 
            this.colGID.DataPropertyName = "GID";
            this.colGID.HeaderText = "GID";
            this.colGID.Name = "colGID";
            this.colGID.ReadOnly = true;
            this.colGID.Width = 50;
            // 
            // colType
            // 
            this.colType.DataPropertyName = "Type";
            this.colType.HeaderText = "類型";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Visible = false;
            // 
            // colTypeName
            // 
            this.colTypeName.DataPropertyName = "TypeName";
            this.colTypeName.HeaderText = "名稱";
            this.colTypeName.Name = "colTypeName";
            this.colTypeName.ReadOnly = true;
            this.colTypeName.Width = 140;
            // 
            // colDate
            // 
            this.colDate.DataPropertyName = "Date";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colDate.DefaultCellStyle = dataGridViewCellStyle5;
            this.colDate.HeaderText = "日期";
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            this.colDate.Width = 75;
            // 
            // colTime
            // 
            this.colTime.DataPropertyName = "Time";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTime.DefaultCellStyle = dataGridViewCellStyle6;
            this.colTime.HeaderText = "時間";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.Width = 55;
            // 
            // colDateTime
            // 
            this.colDateTime.DataPropertyName = "DateTime";
            this.colDateTime.HeaderText = "日期時間";
            this.colDateTime.Name = "colDateTime";
            this.colDateTime.ReadOnly = true;
            this.colDateTime.Visible = false;
            // 
            // colAway
            // 
            this.colAway.DataPropertyName = "Away";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colAway.DefaultCellStyle = dataGridViewCellStyle7;
            this.colAway.HeaderText = "客隊";
            this.colAway.Name = "colAway";
            this.colAway.ReadOnly = true;
            this.colAway.Width = 180;
            // 
            // colHome
            // 
            this.colHome.DataPropertyName = "Home";
            this.colHome.HeaderText = "主隊";
            this.colHome.Name = "colHome";
            this.colHome.ReadOnly = true;
            this.colHome.Width = 180;
            // 
            // colStatus
            // 
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "狀態";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Visible = false;
            this.colStatus.Width = 60;
            // 
            // colStatusText
            // 
            this.colStatusText.DataPropertyName = "StatusText";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colStatusText.DefaultCellStyle = dataGridViewCellStyle8;
            this.colStatusText.HeaderText = "狀態";
            this.colStatusText.Name = "colStatusText";
            this.colStatusText.ReadOnly = true;
            this.colStatusText.Width = 55;
            // 
            // colWebId
            // 
            this.colWebId.DataPropertyName = "WebId";
            this.colWebId.HeaderText = "WebId";
            this.colWebId.Name = "colWebId";
            this.colWebId.ReadOnly = true;
            this.colWebId.Width = 120;
            // 
            // colChangeCount
            // 
            this.colChangeCount.DataPropertyName = "ChangeCount";
            this.colChangeCount.HeaderText = "Count";
            this.colChangeCount.Name = "colChangeCount";
            this.colChangeCount.ReadOnly = true;
            this.colChangeCount.Width = 40;
            // 
            // colChangeTime
            // 
            this.colChangeTime.DataPropertyName = "ChangeTime";
            this.colChangeTime.HeaderText = "ChangeTime";
            this.colChangeTime.Name = "colChangeTime";
            this.colChangeTime.ReadOnly = true;
            this.colChangeTime.Width = 110;
            // 
            // timerLoad
            // 
            this.timerLoad.Tick += new System.EventHandler(this.timerLoad_Tick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dbSourcelb,
            this.toolStripSeparator1,
            this.weblb});
            this.toolStrip1.Location = new System.Drawing.Point(0, 31);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(784, 31);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "Tools";
            // 
            // dbSourcelb
            // 
            this.dbSourcelb.AutoSize = false;
            this.dbSourcelb.Name = "dbSourcelb";
            this.dbSourcelb.Size = new System.Drawing.Size(370, 28);
            this.dbSourcelb.Text = "資料庫來源：";
            this.dbSourcelb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // weblb
            // 
            this.weblb.AutoSize = false;
            this.weblb.Name = "weblb";
            this.weblb.Size = new System.Drawing.Size(370, 28);
            this.weblb.Text = "前台監控來源：";
            this.weblb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbServerInfo
            // 
            this.lbServerInfo.Name = "lbServerInfo";
            this.lbServerInfo.Size = new System.Drawing.Size(76, 28);
            this.lbServerInfo.Text = "127.0.0.1:80";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dgvData);
            this.Controls.Add(this.tspMain);
            this.Name = "frmMain";
            this.Text = "監控台 - 速報跟盤";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tspMain.ResumeLayout(false);
            this.tspMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon nfiMain;
        private System.Windows.Forms.ToolStrip tspMain;
        private System.Windows.Forms.ToolStripButton tspbtnExit;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.Timer timerLoad;
        private System.Windows.Forms.ToolStripButton tspbtnLastDate;
        private System.Windows.Forms.ToolStripButton tspbtnNeedFollow;
        private System.Windows.Forms.ToolStripButton tspbtnReload;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTypeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAway;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHome;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatusText;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWebId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChangeCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChangeTime;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel dbSourcelb;
        private System.Windows.Forms.ToolStripLabel weblb;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel lbServerStatus;
        private System.Windows.Forms.ToolStripLabel lbServerInfo;
    }
}

