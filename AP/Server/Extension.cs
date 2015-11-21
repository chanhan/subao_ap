using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using NLog;

namespace Server
{
    //扩展方法必须在非泛型静态类中定义
    public static class Extension
    {
        private static Logger ServerWork = LogManager.GetLogger("ServerWork");
        private static Logger ServerReceive = LogManager.GetLogger("ServerReceive");
        private static Logger ServerError = LogManager.GetLogger("ServerError");

        //非同步委派更新UI
        public static void InvokeIfRequired(this Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)//在非當前執行緒內 使用委派
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }


        public static bool CheckProcess(this Dictionary<string, string> process)
        {
            string fileName = null;
            string mainWindowTitle = null;

            // 錯誤處理
            try
            {
                // 檔案名稱
                fileName = System.IO.Path.GetFileNameWithoutExtension(process["Path"]);
                if (fileName.ToLower() != "follow")
                    return false;

                string ProcessName = process["Name"].Replace(")", "");//提高辨識度

                // 執行中的程式
                foreach (Process pro in Process.GetProcessesByName(fileName))
                {
                    mainWindowTitle = pro.MainWindowTitle;
                    if (string.IsNullOrEmpty(mainWindowTitle))
                        continue;

                    // 相同的程式就關閉
                    ProcessModule MainModule = pro.MainModule;
                    if (MainModule != null && MainModule.FileName == process["Path"])
                    {
                        // 不相同就往下處理
                        if (mainWindowTitle.IndexOf(ProcessName) == -1)
                            continue;

                        // 找到了
                        return true;
                    }
                }
            }
            catch (Win32Exception) { }//避免執行 pro.MainModule 發生x86/x64不同的錯誤
            catch (Exception ex)
            {
                ServerError.Error("CheckProcess Error!\n\rMessage: {0},\r\n StackTrace: {1},\r\n fileName: {2}, \r\n mainWindowTitle: {3}",
                    ex.Message, ex.StackTrace, fileName, mainWindowTitle);
            }
            // 傳回
            return false;
        }

        // 執行程式
        public static void OpenProcess(this Dictionary<string, string> process, string date = null)
        {
            Process pro = new Process();
            // 錯誤處理
            try
            {
                // 設定
                pro.StartInfo.FileName = process["Path"];

                if (string.IsNullOrEmpty(date))
                    pro.StartInfo.Arguments = process["Command"];
                else
                    pro.StartInfo.Arguments = string.Format("{0} {1}", process["Command"], date);

                pro.Start();

                ServerWork.Info(string.Format("Open Process: {0}, Date: {1}", process["Name"], date));
            }
            catch (Win32Exception) { }
            catch (Exception ex)
            {
                ServerError.Error("Open Process: {2} Error!\n\rMessage: {0},\r\n StackTrace: {1}\r\n", ex.Message, ex.StackTrace, process["Name"]);
            }
        }

        // 關閉程式
        public static void KillProcess(this Dictionary<string, string> process)
        {
            string fileName = null;
            string mainWindowTitle = null;

            // 錯誤處理
            try
            {
                // 檔案名稱
                fileName = System.IO.Path.GetFileNameWithoutExtension(process["Path"]);
                if (fileName.ToLower() != "follow")
                    return;

                string ProcessName = process["Name"];
                int findIdx = ProcessName.IndexOf("-");
                if (findIdx != -1)//有多個跟分來源
                    ProcessName = ProcessName.Substring(0, findIdx);

                // 執行中的程式
                foreach (Process pro in Process.GetProcessesByName(fileName))
                {
                    mainWindowTitle = pro.MainWindowTitle;
                    if (string.IsNullOrEmpty(mainWindowTitle))
                        continue;

                    // 相同的程式就關閉
                    ProcessModule MainModule = pro.MainModule;
                    if (MainModule != null && MainModule.FileName == process["Path"])
                    {
                        // 不相同就往下處理
                        if (mainWindowTitle.IndexOf(ProcessName) == -1)
                            continue;

                        // 錯誤處理
                        try
                        {
                            // 關閉
                            //pro.Kill(); //直接關閉會造成 process  has exited的錯誤
                            if (!pro.WaitForExit(2000))
                            {
                                if (!pro.HasExited) pro.Kill();
                            }

                            ServerWork.Info("Close Process： " + process["Name"]);
                        }
                        catch (Exception ex)
                        {
                            ServerError.Error("Close Process: {2} Error!\n\rMessage: {0},\r\n StackTrace: {1}\r\n", ex.Message, ex.StackTrace, process["Name"]);
                        }
                    }

                }
            }
            catch (Win32Exception) { }//避免執行 pro.MainModule 發生x86/x64不同的錯誤
            catch (Exception ex)
            {
                ServerError.Error("KillProcess Error!\n\rMessage: {0},\r\n StackTrace: {1},\r\n fileName: {2}, \r\n mainWindowTitle: {3}",
                    ex.Message, ex.StackTrace, fileName, mainWindowTitle);
            }
        }

        // 尋找程式
        public static bool FindProcess(this Dictionary<string, string> process)
        {
            bool result = false;
            string mainWindowTitle = null;
            // 錯誤處理
            try
            {
                // 檔案名稱
                string fileName = System.IO.Path.GetFileNameWithoutExtension(process["Path"]);
                string ProcessName = process["Name"];
                int findIdx = ProcessName.IndexOf("-");
                if (findIdx != -1)//有多個跟分來源
                    ProcessName = ProcessName.Substring(0, findIdx);

                // 執行中的程式
                foreach (Process pro in Process.GetProcessesByName(fileName))
                {
                    mainWindowTitle = pro.MainWindowTitle;
                    if (string.IsNullOrEmpty(mainWindowTitle))
                        continue;

                    // 相同的程式就關閉
                    ProcessModule MainModule = pro.MainModule;
                    if (MainModule != null && MainModule.FileName == process["Path"])
                    {
                        // 不相同就往下處理
                        if (fileName.ToLower() == "follow" &&
                            mainWindowTitle.IndexOf(ProcessName) == -1)
                        {
                            continue;
                        }
                        result = true;
                    }
                }
            }
            catch (Win32Exception) { }//避免執行 pro.MainModule 發生x86/x64不同的錯誤
            catch (Exception ex)
            {
                ServerError.Error("FindProcess Error!/n/rMessage: {0},\r\n StackTrace: {1}\r\n, MainWindowTitle: {2}",
                    ex.Message, ex.StackTrace, mainWindowTitle);
            }
            // 傳回
            return result;
        }
    }
}
