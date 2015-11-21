namespace GameScoresApp
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.StartBtn = new System.Windows.Forms.ToolStripButton();
            this.StopBtn = new System.Windows.Forms.ToolStripButton();
            this.clearBtn = new System.Windows.Forms.ToolStripButton();
            this.ResetBtn = new System.Windows.Forms.ToolStripButton();
            this.ResetTeamBtn = new System.Windows.Forms.ToolStripButton();
            this.lbInfo = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.dbSourcelb = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.sendRequestlb = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartBtn,
            this.StopBtn,
            this.clearBtn,
            this.ResetBtn,
            this.ResetTeamBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(383, 28);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // StartBtn
            // 
            this.StartBtn.Enabled = false;
            this.StartBtn.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.StartBtn.Image = ((System.Drawing.Image)(resources.GetObject("StartBtn.Image")));
            this.StartBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StartBtn.Margin = new System.Windows.Forms.Padding(2);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(61, 24);
            this.StartBtn.Text = "啟動";
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // StopBtn
            // 
            this.StopBtn.Enabled = false;
            this.StopBtn.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.StopBtn.Image = ((System.Drawing.Image)(resources.GetObject("StopBtn.Image")));
            this.StopBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StopBtn.Margin = new System.Windows.Forms.Padding(2);
            this.StopBtn.Name = "StopBtn";
            this.StopBtn.Size = new System.Drawing.Size(61, 24);
            this.StopBtn.Text = "停止";
            this.StopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // clearBtn
            // 
            this.clearBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.clearBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.clearBtn.Enabled = false;
            this.clearBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearBtn.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(58, 25);
            this.clearBtn.Text = "清除Log";
            this.clearBtn.Click += new System.EventHandler(this.ClearBtn_Click);
            // 
            // ResetBtn
            // 
            this.ResetBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.ResetBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ResetBtn.Enabled = false;
            this.ResetBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ResetBtn.Margin = new System.Windows.Forms.Padding(0, 1, 5, 2);
            this.ResetBtn.Name = "ResetBtn";
            this.ResetBtn.Size = new System.Drawing.Size(60, 25);
            this.ResetBtn.Text = "重建緩存";
            this.ResetBtn.Click += new System.EventHandler(this.ResetBtn_Click);
            // 
            // ResetTeamBtn
            // 
            this.ResetTeamBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.ResetTeamBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ResetTeamBtn.Enabled = false;
            this.ResetTeamBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ResetTeamBtn.Margin = new System.Windows.Forms.Padding(0, 1, 5, 2);
            this.ResetTeamBtn.Name = "ResetTeamBtn";
            this.ResetTeamBtn.Size = new System.Drawing.Size(60, 25);
            this.ResetTeamBtn.Text = "重讀隊名";
            this.ResetTeamBtn.Click += new System.EventHandler(this.ResetTeamBtn_Click);
            // 
            // lbInfo
            // 
            this.lbInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbInfo.FormattingEnabled = true;
            this.lbInfo.ItemHeight = 12;
            this.lbInfo.Location = new System.Drawing.Point(0, 31);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(383, 364);
            this.lbInfo.TabIndex = 6;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.dbSourcelb,
            this.toolStripSeparator1,
            this.toolStripLabel3,
            this.sendRequestlb});
            this.toolStrip2.Location = new System.Drawing.Point(0, 28);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(383, 25);
            this.toolStrip2.TabIndex = 7;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(59, 22);
            this.toolStripLabel1.Text = "資料來源:";
            // 
            // dbSourcelb
            // 
            this.dbSourcelb.AutoSize = false;
            this.dbSourcelb.Name = "dbSourcelb";
            this.dbSourcelb.Size = new System.Drawing.Size(115, 22);
            this.dbSourcelb.Text = "123.456.789.000";
            this.dbSourcelb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(59, 22);
            this.toolStripLabel3.Text = "發送目的:";
            // 
            // sendRequestlb
            // 
            this.sendRequestlb.Name = "sendRequestlb";
            this.sendRequestlb.Size = new System.Drawing.Size(101, 22);
            this.sendRequestlb.Text = "123.456.789.000";
            this.sendRequestlb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(383, 395);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = " 速報大球比分";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton StartBtn;
        private System.Windows.Forms.ToolStripButton StopBtn;
        private System.Windows.Forms.ListBox lbInfo;
        private System.Windows.Forms.ToolStripButton clearBtn;
        private System.Windows.Forms.ToolStripButton ResetBtn;
        private System.Windows.Forms.ToolStripButton ResetTeamBtn;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel dbSourcelb;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripLabel sendRequestlb;
    }
}

