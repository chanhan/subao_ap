using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace GameScoresApp
{
    class UpdateLog
    {
        private static Logger log = LogManager.GetLogger("Update_Log");
        public static void Info(string gameType, string gid, string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("-- UpdateLog --");
            sb.AppendLine(text);

            Info(gameType, gid, null, sb.ToString());
        }

        public static void Info(string gameType, string gid, string alliance, string text)
        {
            try
            {
                LogEventInfo evt = new LogEventInfo(LogLevel.Info, "", text);
                evt.Level = LogLevel.Info;
                evt.Properties["GameType"] = gameType;
                evt.Properties["GID"] = gid;                
                evt.Properties["LogTime"] = string.Format("{0}'{1}", DateTime.Now.Hour.ToString(), DateTime.Now.Minute.ToString());

                if (string.IsNullOrEmpty(alliance) == false)
                    evt.Properties["Alliance"] = alliance;

                log.Log(evt);
            }
            catch { }
        }
    }

    class RunsLog
    {
        private static Logger log = LogManager.GetLogger("Runs_Log");
        public static void Info(string gameType, string gid, string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("-- RunsLog --");
            sb.AppendLine(text);

            Info(gameType, gid, null, sb.ToString());
        }

        public static void Info(string gameType, string gid, string alliance, string text)
        {
            try
            {
                LogEventInfo evt = new LogEventInfo(LogLevel.Info, "", text);
                evt.Level = LogLevel.Info;
                evt.Properties["GameType"] = gameType;
                evt.Properties["GID"] = gid;
                evt.Properties["LogTime"] = string.Format("{0}'{1}", DateTime.Now.Hour.ToString(), DateTime.Now.Minute.ToString());

                if (string.IsNullOrEmpty(alliance) == false)
                    evt.Properties["Alliance"] = alliance;

                log.Log(evt);
            }
            catch { }
        }
    }

    class ReadyLog
    {
        private static Logger log = LogManager.GetLogger("Ready_Log");
        public static void Info(string gameType, string gid, string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("-- ReadyLog --");
            sb.AppendLine(text);

            Info(gameType, gid, null, sb.ToString());
        }

        public static void Info(string gameType, string gid, string alliance, string text)
        {
            try
            {
                LogEventInfo evt = new LogEventInfo(LogLevel.Info, "", text);
                evt.Level = LogLevel.Info;
                evt.Properties["GameType"] = gameType;
                evt.Properties["GID"] = gid;
                evt.Properties["LogTime"] = string.Format("{0}'{1}", DateTime.Now.Hour.ToString(), DateTime.Now.Minute.ToString());

                if (string.IsNullOrEmpty(alliance) == false)
                    evt.Properties["Alliance"] = alliance;

                log.Log(evt);
            }
            catch { }
        }
    }

    class md5Log
    {
        private static Logger log = LogManager.GetLogger("md5_Log");
        public static void Info(string gameType, string gid, string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("-- md5Log --");
            sb.AppendLine(text);

            Info(gameType, gid, null, sb.ToString());
        }

        public static void Info(string gameType, string gid, string alliance, string text)
        {
            try
            {
                LogEventInfo evt = new LogEventInfo(LogLevel.Info, "", text);
                evt.Level = LogLevel.Info;
                evt.Properties["GameType"] = gameType;
                evt.Properties["GID"] = gid;
                evt.Properties["LogTime"] = string.Format("{0}'{1}", DateTime.Now.Hour.ToString(), DateTime.Now.Minute.ToString());

                if (string.IsNullOrEmpty(alliance) == false)
                    evt.Properties["Alliance"] = alliance;

                log.Log(evt);
            }
            catch { }
        }
    }
}
