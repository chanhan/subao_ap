using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Encrypt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void encoderBtn_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(encryptTxt.Text))
                decryptTxt.Text = EncryptHelper.AESEncrypt(encryptTxt.Text);
        }

        private void decoderBtn_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(decryptTxt.Text))
                encryptTxt.Text = EncryptHelper.AESDecrypt(decryptTxt.Text);
        }
    }
}
