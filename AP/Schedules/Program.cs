using SHGG.DataStructerService;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Schedules
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region 讀取 xml 檔
            //string xmlFile = string.Format(@"{0}\{1}.xml", Application.StartupPath, System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath));
            //// 取得資料
            //if (System.IO.File.Exists(xmlFile))
            //{
            //    try
            //    {
            //        XmlAdapter xmlAdapter = new XmlAdapter(xmlFile);
            //        xmlAdapter.GoToNode("XML", "Database");
            //        // 設定
            //        if(!string.IsNullOrEmpty(xmlAdapter.ReadXmlNode("SqlServer")))
            //            frmMain.SqlServer =  EncryptHelper.AESDecrypt(xmlAdapter.ReadXmlNode("SqlServer"));

            //        if (!string.IsNullOrEmpty(xmlAdapter.ReadXmlNode("DB")))
            //            frmMain.SqlDB = EncryptHelper.AESDecrypt(xmlAdapter.ReadXmlNode("DB"));

            //        if (!string.IsNullOrEmpty(xmlAdapter.ReadXmlNode("UID")))
            //            frmMain.SqlUID = EncryptHelper.AESDecrypt(xmlAdapter.ReadXmlNode("UID"));

            //        if (!string.IsNullOrEmpty(xmlAdapter.ReadXmlNode("PWD")))
            //            frmMain.SqlPWD = EncryptHelper.AESDecrypt(xmlAdapter.ReadXmlNode("PWD"));
            //    }
            //    catch { }
            //}
            #endregion

            Application.Run(new FrmMain());
        }
    }
}
