namespace Encrypt
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.decoderBtn = new System.Windows.Forms.Button();
            this.encoderBtn = new System.Windows.Forms.Button();
            this.encryptTxt = new System.Windows.Forms.TextBox();
            this.decryptTxt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Encrypt:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Decrypt:";
            // 
            // decoderBtn
            // 
            this.decoderBtn.Location = new System.Drawing.Point(190, 95);
            this.decoderBtn.Name = "decoderBtn";
            this.decoderBtn.Size = new System.Drawing.Size(75, 23);
            this.decoderBtn.TabIndex = 2;
            this.decoderBtn.Text = "Decrypt";
            this.decoderBtn.UseVisualStyleBackColor = true;
            this.decoderBtn.Click += new System.EventHandler(this.decoderBtn_Click);
            // 
            // encoderBtn
            // 
            this.encoderBtn.Location = new System.Drawing.Point(187, 2);
            this.encoderBtn.Name = "encoderBtn";
            this.encoderBtn.Size = new System.Drawing.Size(75, 23);
            this.encoderBtn.TabIndex = 3;
            this.encoderBtn.Text = "Encrypt";
            this.encoderBtn.UseVisualStyleBackColor = true;
            this.encoderBtn.Click += new System.EventHandler(this.encoderBtn_Click);
            // 
            // encryptTxt
            // 
            this.encryptTxt.Location = new System.Drawing.Point(13, 30);
            this.encryptTxt.MaxLength = 128;
            this.encryptTxt.Multiline = true;
            this.encryptTxt.Name = "encryptTxt";
            this.encryptTxt.Size = new System.Drawing.Size(250, 50);
            this.encryptTxt.TabIndex = 4;
            // 
            // decryptTxt
            // 
            this.decryptTxt.Location = new System.Drawing.Point(15, 124);
            this.decryptTxt.Multiline = true;
            this.decryptTxt.Name = "decryptTxt";
            this.decryptTxt.Size = new System.Drawing.Size(250, 50);
            this.decryptTxt.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 186);
            this.Controls.Add(this.decryptTxt);
            this.Controls.Add(this.encryptTxt);
            this.Controls.Add(this.encoderBtn);
            this.Controls.Add(this.decoderBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Encrypt/Decrypt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button decoderBtn;
        private System.Windows.Forms.Button encoderBtn;
        private System.Windows.Forms.TextBox encryptTxt;
        private System.Windows.Forms.TextBox decryptTxt;
    }
}

