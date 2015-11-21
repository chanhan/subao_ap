using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NLog;

namespace Server
{
    static class Program
    {
        private static Logger ServerError = LogManager.GetLogger("Server_Error");
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
            catch (Exception ex)
            {
                ServerError.Error("Program 發生錯誤! /n/r Message: {0},\r\n StackTrace: {1}\r\n", ex.Message, ex.StackTrace);
            }
        }
    }
}
