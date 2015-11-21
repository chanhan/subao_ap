namespace Client
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tspMain = new System.Windows.Forms.ToolStrip();
            this.tsplabServerIp = new System.Windows.Forms.ToolStripLabel();
            this.tspcbbServerIp = new System.Windows.Forms.ToolStripComboBox();
            this.tspbtnStop = new System.Windows.Forms.ToolStripButton();
            this.tspbtnStart = new System.Windows.Forms.ToolStripButton();
            this.tspSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tspbtnCloseAllProcess = new System.Windows.Forms.ToolStripButton();
            this.lbCtrlLock = new System.Windows.Forms.ToolStripLabel();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabProcesses = new System.Windows.Forms.TabPage();
            this.dgvProcesses = new System.Windows.Forms.DataGridView();
            this.timerSocket = new System.Windows.Forms.Timer(this.components);
            this.colProcessName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProcessStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProcessStart = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colProcessClose = new System.Windows.Forms.DataGridViewButtonColumn();
            this.tspMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabProcesses.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProcesses)).BeginInit();
            this.SuspendLayout();
            // 
            // tspMain
            // 
            this.tspMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tspMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsplabServerIp,
            this.tspcbbServerIp,
            this.tspbtnStop,
            this.tspbtnStart,
            this.tspSeparator1,
            this.tspbtnCloseAllProcess,
            this.lbCtrlLock});
            this.tspMain.Location = new System.Drawing.Point(0, 0);
            this.tspMain.Name = "tspMain";
            this.tspMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tspMain.Size = new System.Drawing.Size(624, 25);
            this.tspMain.TabIndex = 1;
            this.tspMain.Text = "Tools";
            // 
            // tsplabServerIp
            // 
            this.tsplabServerIp.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.tsplabServerIp.Name = "tsplabServerIp";
            this.tsplabServerIp.Size = new System.Drawing.Size(44, 22);
            this.tsplabServerIp.Text = "來源：";
            // 
            // tspcbbServerIp
            // 
            this.tspcbbServerIp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tspcbbServerIp.Name = "tspcbbServerIp";
            this.tspcbbServerIp.Size = new System.Drawing.Size(200, 25);
            // 
            // tspbtnStop
            // 
            this.tspbtnStop.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tspbtnStop.Enabled = false;
            this.tspbtnStop.Image = ((System.Drawing.Image)(resources.GetObject("tspbtnStop.Image")));
            this.tspbtnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspbtnStop.Name = "tspbtnStop";
            this.tspbtnStop.Size = new System.Drawing.Size(52, 22);
            this.tspbtnStop.Text = "斷開";
            this.tspbtnStop.Click += new System.EventHandler(this.tspbtnStop_Click);
            // 
            // tspbtnStart
            // 
            this.tspbtnStart.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tspbtnStart.Image = ((System.Drawing.Image)(resources.GetObject("tspbtnStart.Image")));
            this.tspbtnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspbtnStart.Name = "tspbtnStart";
            this.tspbtnStart.Size = new System.Drawing.Size(52, 22);
            this.tspbtnStart.Text = "連接";
            this.tspbtnStart.Click += new System.EventHandler(this.tspbtnStart_Click);
            // 
            // tspSeparator1
            // 
            this.tspSeparator1.Name = "tspSeparator1";
            this.tspSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tspbtnCloseAllProcess
            // 
            this.tspbtnCloseAllProcess.Enabled = false;
            this.tspbtnCloseAllProcess.Image = ((System.Drawing.Image)(resources.GetObject("tspbtnCloseAllProcess.Image")));
            this.tspbtnCloseAllProcess.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspbtnCloseAllProcess.Name = "tspbtnCloseAllProcess";
            this.tspbtnCloseAllProcess.Size = new System.Drawing.Size(100, 22);
            this.tspbtnCloseAllProcess.Text = "關閉全部程式";
            this.tspbtnCloseAllProcess.Click += new System.EventHandler(this.tspbtnCloseAllProcess_Click);
            // 
            // lbCtrlLock
            // 
            this.lbCtrlLock.ForeColor = System.Drawing.Color.Red;
            this.lbCtrlLock.Margin = new System.Windows.Forms.Padding(30, 1, 0, 2);
            this.lbCtrlLock.Name = "lbCtrlLock";
            this.lbCtrlLock.Size = new System.Drawing.Size(68, 22);
            this.lbCtrlLock.Text = "等待回應中";
            this.lbCtrlLock.Visible = false;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabProcesses);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 25);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(624, 637);
            this.tabMain.TabIndex = 2;
            // 
            // tabProcesses
            // 
            this.tabProcesses.Controls.Add(this.dgvProcesses);
            this.tabProcesses.Location = new System.Drawing.Point(4, 22);
            this.tabProcesses.Name = "tabProcesses";
            this.tabProcesses.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcesses.Size = new System.Drawing.Size(616, 611);
            this.tabProcesses.TabIndex = 0;
            this.tabProcesses.Text = "程式列表";
            this.tabProcesses.UseVisualStyleBackColor = true;
            // 
            // dgvProcesses
            // 
            this.dgvProcesses.AllowUserToAddRows = false;
            this.dgvProcesses.AllowUserToDeleteRows = false;
            this.dgvProcesses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProcesses.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProcessName,
            this.colProcessStatus,
            this.colDescription,
            this.colProcessStart,
            this.colProcessClose});
            this.dgvProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProcesses.Location = new System.Drawing.Point(3, 3);
            this.dgvProcesses.MultiSelect = false;
            this.dgvProcesses.Name = "dgvProcesses";
            this.dgvProcesses.ReadOnly = true;
            this.dgvProcesses.RowTemplate.Height = 23;
            this.dgvProcesses.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProcesses.Size = new System.Drawing.Size(610, 605);
            this.dgvProcesses.TabIndex = 3;
            this.dgvProcesses.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProcesses_CellContentClick);
            this.dgvProcesses.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.RowPostPaint);
            // 
            // timerSocket
            // 
            this.timerSocket.Interval = 60000;
            this.timerSocket.Tick += new System.EventHandler(this.timerSocket_Tick);
            // 
            // colProcessName
            // 
            this.colProcessName.HeaderText = "程式名稱";
            this.colProcessName.Name = "colProcessName";
            this.colProcessName.ReadOnly = true;
            this.colProcessName.Width = 90;
            // 
            // colProcessStatus
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colProcessStatus.DefaultCellStyle = dataGridViewCellStyle1;
            this.colProcessStatus.HeaderText = "狀態";
            this.colProcessStatus.Name = "colProcessStatus";
            this.colProcessStatus.ReadOnly = true;
            this.colProcessStatus.Width = 55;
            // 
            // colDescription
            // 
            this.colDescription.HeaderText = "說明";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            this.colDescription.Width = 300;
            // 
            // colProcessStart
            // 
            this.colProcessStart.HeaderText = "";
            this.colProcessStart.Name = "colProcessStart";
            this.colProcessStart.ReadOnly = true;
            this.colProcessStart.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colProcessStart.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colProcessStart.Width = 50;
            // 
            // colProcessClose
            // 
            this.colProcessClose.HeaderText = "";
            this.colProcessClose.Name = "colProcessClose";
            this.colProcessClose.ReadOnly = true;
            this.colProcessClose.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colProcessClose.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colProcessClose.Width = 50;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(624, 662);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.tspMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "控制台 - 速報跟盤";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.tspMain.ResumeLayout(false);
            this.tspMain.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabProcesses.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProcesses)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tspMain;
        private System.Windows.Forms.ToolStripLabel tsplabServerIp;
        private System.Windows.Forms.ToolStripButton tspbtnStop;
        private System.Windows.Forms.ToolStripButton tspbtnStart;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabProcesses;
        private System.Windows.Forms.DataGridView dgvProcesses;
        private System.Windows.Forms.Timer timerSocket;
        private System.Windows.Forms.ToolStripSeparator tspSeparator1;
        private System.Windows.Forms.ToolStripButton tspbtnCloseAllProcess;
        private System.Windows.Forms.ToolStripComboBox tspcbbServerIp;
        private System.Windows.Forms.ToolStripLabel lbCtrlLock;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProcessName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProcessStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewButtonColumn colProcessStart;
        private System.Windows.Forms.DataGridViewButtonColumn colProcessClose;
    }
}

