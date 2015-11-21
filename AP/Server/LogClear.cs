using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class LogClear
    {
        string LogPath;
        int iLogLife = 7;

        public LogClear()
        {
            LogPath = ConfigurationManager.AppSettings["LogPath"];
            string life = ConfigurationManager.AppSettings["LogLife"];

            int.TryParse(life, out iLogLife);
            if (iLogLife < 1)
                iLogLife = 1;
        }

        public void Work()
        {
            while(true)
            {
                Thread.Sleep(300*1000);//每5分鐘檢查一次

                Clear();
            }
        }

        private void Clear()
        {
            if (string.IsNullOrEmpty(LogPath))
                return;

            string[] path = LogPath.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in path)
                CheckPath(p.Trim());
        }

        private void CheckPath(string LogPath)
        {
            if (System.IO.Directory.Exists(LogPath))
            {
                string[] files = System.IO.Directory.GetDirectories(LogPath);

                DateTime checkRange = DateTime.Now.AddDays(-iLogLife);
                DateTime fileDate;
                string fileName;
                foreach (string s in files)
                {
                    try
                    {
                        fileName = s;

                        int findIdx = s.LastIndexOf("\\");
                        if (findIdx != -1)
                        {
                            findIdx++;
                            fileName = s.Substring(findIdx);
                        }

                        if (!DateTime.TryParse(fileName, out fileDate) || fileDate < checkRange)
                            System.IO.Directory.Delete(s, true);
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
