using SHGG.DataStructerService;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Monitor
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region 讀取設定
            string xmlFile = string.Format(@"{0}\{1}.xml", Application.StartupPath, System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath));
            // 取得資料
            if (System.IO.File.Exists(xmlFile))
            {
                try
                {
                    XmlAdapter xmlAdapter = null;
                    // 資料庫
                    xmlAdapter = new XmlAdapter(xmlFile);
                    xmlAdapter.GoToNode("XML", "Database");
                    // 設定
                    frmMain.SqlServer = xmlAdapter.ReadXmlNode("SqlServer");
                    frmMain.SqlDB = xmlAdapter.ReadXmlNode("DB");
                    frmMain.SqlUID = xmlAdapter.ReadXmlNode("UID");
                    frmMain.SqlPWD = xmlAdapter.ReadXmlNode("PWD");
                    // 伺服器
                    xmlAdapter = new XmlAdapter(xmlFile);
                    xmlAdapter.GoToNode("XML", "Server");
                    // 設定
                    frmMain.ServerIp = xmlAdapter.ReadXmlNode("Ip");
                    frmMain.ServerPort = xmlAdapter.ReadXmlNode("Port");
                }
                catch { }
            }
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
