namespace Server
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tspMain = new System.Windows.Forms.ToolStrip();
            this.tsplabServerIp = new System.Windows.Forms.ToolStripLabel();
            this.tsptxbServerIp = new System.Windows.Forms.ToolStripTextBox();
            this.tsplabServerPort = new System.Windows.Forms.ToolStripLabel();
            this.tsptxbServerPort = new System.Windows.Forms.ToolStripTextBox();
            this.tspSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tspbtnCloseAllProcess = new System.Windows.Forms.ToolStripButton();
            this.tspbtnExit = new System.Windows.Forms.ToolStripButton();
            this.tspbtnStart = new System.Windows.Forms.ToolStripButton();
            this.timerProcess = new System.Windows.Forms.Timer(this.components);
            this.nfiMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.tabProcesses = new System.Windows.Forms.TabPage();
            this.dgvProcesses = new System.Windows.Forms.DataGridView();
            this.colProcessClose = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colProcessStart = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProcessStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProcessName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tspMain.SuspendLayout();
            this.tabProcesses.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProcesses)).BeginInit();
            this.tabMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tspMain
            // 
            this.tspMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tspMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsplabServerIp,
            this.tsptxbServerIp,
            this.tsplabServerPort,
            this.tsptxbServerPort,
            this.tspSeparator1,
            this.tspbtnCloseAllProcess,
            this.tspbtnExit,
            this.tspbtnStart});
            this.tspMain.Location = new System.Drawing.Point(0, 0);
            this.tspMain.Name = "tspMain";
            this.tspMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tspMain.Size = new System.Drawing.Size(624, 25);
            this.tspMain.TabIndex = 0;
            this.tspMain.Text = "Tools";
            // 
            // tsplabServerIp
            // 
            this.tsplabServerIp.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.tsplabServerIp.Name = "tsplabServerIp";
            this.tsplabServerIp.Size = new System.Drawing.Size(30, 22);
            this.tsplabServerIp.Text = "IP：";
            // 
            // tsptxbServerIp
            // 
            this.tsptxbServerIp.Name = "tsptxbServerIp";
            this.tsptxbServerIp.Size = new System.Drawing.Size(100, 25);
            // 
            // tsplabServerPort
            // 
            this.tsplabServerPort.Name = "tsplabServerPort";
            this.tsplabServerPort.Size = new System.Drawing.Size(43, 22);
            this.tsplabServerPort.Text = "Port：";
            // 
            // tsptxbServerPort
            // 
            this.tsptxbServerPort.Name = "tsptxbServerPort";
            this.tsptxbServerPort.Size = new System.Drawing.Size(50, 25);
            // 
            // tspSeparator1
            // 
            this.tspSeparator1.Name = "tspSeparator1";
            this.tspSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tspbtnCloseAllProcess
            // 
            this.tspbtnCloseAllProcess.Image = ((System.Drawing.Image)(resources.GetObject("tspbtnCloseAllProcess.Image")));
            this.tspbtnCloseAllProcess.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspbtnCloseAllProcess.Name = "tspbtnCloseAllProcess";
            this.tspbtnCloseAllProcess.Size = new System.Drawing.Size(100, 22);
            this.tspbtnCloseAllProcess.Text = "關閉全部程式";
            this.tspbtnCloseAllProcess.Click += new System.EventHandler(this.tspbtnCloseAllProcess_Click);
            // 
            // tspbtnExit
            // 
            this.tspbtnExit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tspbtnExit.Image = ((System.Drawing.Image)(resources.GetObject("tspbtnExit.Image")));
            this.tspbtnExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspbtnExit.Name = "tspbtnExit";
            this.tspbtnExit.Size = new System.Drawing.Size(76, 22);
            this.tspbtnExit.Text = "結束程式";
            this.tspbtnExit.Click += new System.EventHandler(this.tspbtnExit_Click);
            // 
            // tspbtnStart
            // 
            this.tspbtnStart.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tspbtnStart.Image = ((System.Drawing.Image)(resources.GetObject("tspbtnStart.Image")));
            this.tspbtnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tspbtnStart.Name = "tspbtnStart";
            this.tspbtnStart.Size = new System.Drawing.Size(52, 22);
            this.tspbtnStart.Text = "啟動";
            this.tspbtnStart.Click += new System.EventHandler(this.tspbtnStart_Click);
            // 
            // timerProcess
            // 
            this.timerProcess.Enabled = true;
            this.timerProcess.Interval = 10000;
            this.timerProcess.Tick += new System.EventHandler(this.timerProcess_Tick);
            // 
            // nfiMain
            // 
            this.nfiMain.Icon = ((System.Drawing.Icon)(resources.GetObject("nfiMain.Icon")));
            this.nfiMain.Text = "伺服器 - 速報跟盤";
            this.nfiMain.Visible = true;
            this.nfiMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.nfiMain_MouseDoubleClick);
            // 
            // tabProcesses
            // 
            this.tabProcesses.Controls.Add(this.dgvProcesses);
            this.tabProcesses.Location = new System.Drawing.Point(4, 22);
            this.tabProcesses.Name = "tabProcesses";
            this.tabProcesses.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcesses.Size = new System.Drawing.Size(616, 391);
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
            this.dgvProcesses.Size = new System.Drawing.Size(610, 385);
            this.dgvProcesses.TabIndex = 3;
            this.dgvProcesses.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProcesses_CellContentClick);
            this.dgvProcesses.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProcesses_CellValueChanged);
            this.dgvProcesses.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.RowPostPaint);
            this.dgvProcesses.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvProcesses_RowsAdded);
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
            // colProcessStart
            // 
            this.colProcessStart.HeaderText = "";
            this.colProcessStart.Name = "colProcessStart";
            this.colProcessStart.ReadOnly = true;
            this.colProcessStart.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colProcessStart.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colProcessStart.Width = 50;
            // 
            // colDescription
            // 
            this.colDescription.HeaderText = "說明";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            this.colDescription.Width = 300;
            // 
            // colProcessStatus
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colProcessStatus.DefaultCellStyle = dataGridViewCellStyle1;
            this.colProcessStatus.HeaderText = "狀態";
            this.colProcessStatus.Name = "colProcessStatus";
            this.colProcessStatus.ReadOnly = true;
            this.colProcessStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.colProcessStatus.Width = 55;
            // 
            // colProcessName
            // 
            this.colProcessName.HeaderText = "程式名稱";
            this.colProcessName.Name = "colProcessName";
            this.colProcessName.ReadOnly = true;
            this.colProcessName.Width = 90;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabProcesses);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 25);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(624, 417);
            this.tabMain.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.tspMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "伺服器 - 速報跟盤";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.tspMain.ResumeLayout(false);
            this.tspMain.PerformLayout();
            this.tabProcesses.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProcesses)).EndInit();
            this.tabMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tspMain;
        private System.Windows.Forms.ToolStripLabel tsplabServerIp;
        private System.Windows.Forms.ToolStripTextBox tsptxbServerIp;
        private System.Windows.Forms.ToolStripLabel tsplabServerPort;
        private System.Windows.Forms.ToolStripTextBox tsptxbServerPort;
        private System.Windows.Forms.ToolStripButton tspbtnStart;
        private System.Windows.Forms.Timer timerProcess;
        private System.Windows.Forms.ToolStripSeparator tspSeparator1;
        private System.Windows.Forms.ToolStripButton tspbtnCloseAllProcess;
        private System.Windows.Forms.ToolStripButton tspbtnExit;
        private System.Windows.Forms.NotifyIcon nfiMain;
        private System.Windows.Forms.TabPage tabProcesses;
        private System.Windows.Forms.DataGridView dgvProcesses;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProcessName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProcessStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewButtonColumn colProcessStart;
        private System.Windows.Forms.DataGridViewButtonColumn colProcessClose;
        private System.Windows.Forms.TabControl tabMain;
    }
}

