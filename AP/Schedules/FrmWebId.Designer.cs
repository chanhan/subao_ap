namespace Schedules
{
    partial class FrmWebId
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnChecked = new System.Windows.Forms.Button();
            this.cboDate = new System.Windows.Forms.ComboBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSchedule = new System.Windows.Forms.Button();
            this.dgvSchedule = new System.Windows.Forms.DataGridView();
            this.colChecked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colWebId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeamAway = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeamHome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSchedule)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.btnChecked);
            this.panel1.Controls.Add(this.cboDate);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnSchedule);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(764, 34);
            this.panel1.TabIndex = 2;
            // 
            // btnChecked
            // 
            this.btnChecked.Location = new System.Drawing.Point(318, 3);
            this.btnChecked.Name = "btnChecked";
            this.btnChecked.Size = new System.Drawing.Size(75, 23);
            this.btnChecked.TabIndex = 15;
            this.btnChecked.Text = "勾選賽程";
            this.btnChecked.UseVisualStyleBackColor = true;
            this.btnChecked.Click += new System.EventHandler(this.btnChecked_Click);
            // 
            // cboDate
            // 
            this.cboDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDate.FormattingEnabled = true;
            this.cboDate.Location = new System.Drawing.Point(177, 4);
            this.cboDate.Name = "cboDate";
            this.cboDate.Size = new System.Drawing.Size(135, 20);
            this.cboDate.TabIndex = 14;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(86, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "關閉";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSchedule
            // 
            this.btnSchedule.Location = new System.Drawing.Point(5, 3);
            this.btnSchedule.Name = "btnSchedule";
            this.btnSchedule.Size = new System.Drawing.Size(75, 23);
            this.btnSchedule.TabIndex = 12;
            this.btnSchedule.Tag = "";
            this.btnSchedule.Text = "建立賽程";
            this.btnSchedule.UseVisualStyleBackColor = true;
            this.btnSchedule.Click += new System.EventHandler(this.btnSchedule_Click);
            // 
            // dgvSchedule
            // 
            this.dgvSchedule.AllowUserToAddRows = false;
            this.dgvSchedule.AllowUserToDeleteRows = false;
            this.dgvSchedule.AllowUserToResizeRows = false;
            this.dgvSchedule.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("新細明體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSchedule.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSchedule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colChecked,
            this.colWebId,
            this.colTime,
            this.colTeamAway,
            this.colTeamHome,
            this.colComment,
            this.colKey});
            this.dgvSchedule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSchedule.Location = new System.Drawing.Point(0, 34);
            this.dgvSchedule.MultiSelect = false;
            this.dgvSchedule.Name = "dgvSchedule";
            this.dgvSchedule.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dgvSchedule.RowTemplate.Height = 24;
            this.dgvSchedule.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSchedule.Size = new System.Drawing.Size(764, 528);
            this.dgvSchedule.TabIndex = 3;
            // 
            // colChecked
            // 
            this.colChecked.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colChecked.HeaderText = "　全選";
            this.colChecked.MinimumWidth = 60;
            this.colChecked.Name = "colChecked";
            this.colChecked.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colChecked.Width = 60;
            // 
            // colWebId
            // 
            this.colWebId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colWebId.FillWeight = 89.54314F;
            this.colWebId.HeaderText = "來源編號";
            this.colWebId.MinimumWidth = 100;
            this.colWebId.Name = "colWebId";
            this.colWebId.ReadOnly = true;
            this.colWebId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colTime
            // 
            this.colTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Format = "yyyy/MM/dd HH:mm";
            dataGridViewCellStyle2.NullValue = null;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.colTime.DefaultCellStyle = dataGridViewCellStyle2;
            this.colTime.FillWeight = 89.54314F;
            this.colTime.HeaderText = "開賽時間";
            this.colTime.MinimumWidth = 120;
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTime.Width = 120;
            // 
            // colTeamAway
            // 
            this.colTeamAway.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colTeamAway.HeaderText = "客隊";
            this.colTeamAway.MinimumWidth = 120;
            this.colTeamAway.Name = "colTeamAway";
            this.colTeamAway.ReadOnly = true;
            this.colTeamAway.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTeamAway.Width = 120;
            // 
            // colTeamHome
            // 
            this.colTeamHome.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colTeamHome.HeaderText = "主隊";
            this.colTeamHome.MinimumWidth = 120;
            this.colTeamHome.Name = "colTeamHome";
            this.colTeamHome.ReadOnly = true;
            this.colTeamHome.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTeamHome.Width = 120;
            // 
            // colComment
            // 
            this.colComment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Red;
            this.colComment.DefaultCellStyle = dataGridViewCellStyle3;
            this.colComment.FillWeight = 89.54314F;
            this.colComment.HeaderText = "備註";
            this.colComment.MinimumWidth = 220;
            this.colComment.Name = "colComment";
            this.colComment.ReadOnly = true;
            this.colComment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colComment.Width = 220;
            // 
            // colKey
            // 
            this.colKey.HeaderText = "編號";
            this.colKey.Name = "colKey";
            this.colKey.Visible = false;
            this.colKey.Width = 62;
            // 
            // FrmWebId
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 562);
            this.ControlBox = false;
            this.Controls.Add(this.dgvSchedule);
            this.Controls.Add(this.panel1);
            this.IsMdiContainer = true;
            this.Name = "FrmWebId";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmWebId";
            this.Load += new System.EventHandler(this.FrmWebId_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSchedule)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSchedule;
        private System.Windows.Forms.DataGridView dgvSchedule;
        private System.Windows.Forms.Button btnChecked;
        private System.Windows.Forms.ComboBox cboDate;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colChecked;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWebId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeamAway;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeamHome;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKey;
    }
}