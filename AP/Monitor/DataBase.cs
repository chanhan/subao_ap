using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    class DataBase
    {
        public static string SqlServer { get; set; }
        public static string SqlDB { get; set; }
        public static string SqlUID { get; set; }
        public static string SqlPWD { get; set; }
        public static string ServerIp { get; set; }
        public static string ServerPort { get; set; }

        //  測試連線資料庫
        private bool TestConnection()
        {
            bool result = false;
            SqlConnection conn = null;
            // 錯誤處理
            try
            {
                conn = new SqlConnection(ConnectionString);
                // 開啟
                conn.Open();
                // 關閉
                conn.Close();
                // 完成
                result = true;
            }
            catch { }
            conn = null;
            // 傳回
            return result;
        }
        // 連接字串
        public static string ConnectionString
        {
            get
            {
                return string.Format("Data Source={0};Initial Catalog={1};UID={2};PWD={3};Integrated Security=false;", new string[] { SqlServer, SqlDB, SqlUID, SqlPWD });
            }
        }
    }
}
