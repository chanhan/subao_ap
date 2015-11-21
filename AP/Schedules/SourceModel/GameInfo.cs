using System;
using System.Collections.Generic;

namespace Schedules
{
    #region GameInfo

    public class GameInfo
    {
        public int AllianceID { set; get; }
        public string GameType { set; get; }
        public string WebID { set; get; }
        public DateTime GameTime { set; get; }
        public string Away { set; get; }
        public string Home { set; get; }
        public bool AcH { set; get; }
        public string SourceID { set; get; }
        public string Comment { set; get; }
        public bool IsReschedule { set; get; }//補賽判斷

        public GameInfo(int allianceID, string gameType, DateTime gameTime, string webID, bool isReschedule = false)
        {
            // 設定
            this.AllianceID = allianceID;
            this.WebID = webID;
            this.GameType = gameType;
            this.GameTime = gameTime;
            this.IsReschedule = isReschedule;
        }

        public override string ToString()
        {
            string home = String.Empty;
            string away = String.Empty;

            // 有資料
            if (this.Away != null && this.Home != null)
            {
                // 隊名互換
                SwapTeam(ref home, ref away);

                // 補賽
                OnReschedule(ref home, ref away);
            }

            string result = String.Format("《{0}》{1:yyyy-MM-dd HH:mm} {2} vs {3}", this.WebID, GameTime, CHT_PadLeft(away, 24, ' '), home);
            
            // 傳回
            return result;
        }

        public string ToString(Dictionary<string, string> teamNameMapping)
        {
            if (teamNameMapping == null || teamNameMapping.Count == 0)
            {
                return ToString();
            }

            string home = String.Empty;
            string away = String.Empty;

            // 有資料
            if (this.Away != null && this.Home != null)
            {
                home = (teamNameMapping.ContainsKey(this.Home)) ? teamNameMapping[this.Home] : this.Home;
                away = (teamNameMapping.ContainsKey(this.Away)) ? teamNameMapping[this.Away] : this.Away;

                // 隊名互換
                SwapTeam(ref home, ref away);

                // 補賽
                OnReschedule(ref home, ref away);
            }

            string result = String.Format("《{0}》{1:yyyy-MM-dd HH:mm} {2} vs {3}", this.WebID, GameTime, CHT_PadLeft(away, 24, ' '), home);

            // 傳回
            return result;
        }

        #region Other

        /// <summary>
        /// 主客隊互換
        /// </summary>
        /// <param name="home">主隊</param>
        /// <param name="away">客隊</param>
        public void SwapTeam(ref string home, ref string away)
        {
            if (this.AcH)
            {
                string tmp = home;
                home = away;
                away = tmp;
            }
        }

        /// <summary>
        /// 補賽
        /// </summary>
        /// <param name="home">主隊</param>
        /// <param name="away">客隊</param>
        private void OnReschedule(ref string home, ref string away)
        {
            if (this.IsReschedule)
            {
                home += "-補";
                away += "-補";
            }
        }

        /// <summary>
        /// 中文字截字，不足補左側字串。
        /// </summary>
        /// <param name="org">原始字串</param>
        /// <param name="sLen">長度</param>
        /// <param name="padStr">替代字元</param>
        private static string CHT_PadLeft(string org, int sLen, char padStr)
        {
            var sResult = "";
            int orgLen = 0;
            int tLen = 0;
            // 計算轉換過實際的總長
            for (int i = 0; i < org.Length; i++)
            {
                string s = org.Substring(i, 1);
                int vLen = 0;
                //判斷 asc 表是否介於 0~128
                if (Convert.ToInt32(s[0]) > 128 || Convert.ToInt32(s[0]) < 0)
                {
                    vLen = 2;
                }
                else
                {
                    vLen = 1;
                }
                orgLen += vLen;
                if (orgLen > sLen)
                {
                    orgLen -= vLen;
                    break;
                }
                sResult += s;
            }
            // 計算轉換過後，最後實際的長度
            tLen = sLen - (orgLen - org.Length);
            // 傳回
            return sResult.PadLeft(tLen, padStr);
        }

        /// <summary>
        /// 中文字截字，不足補右側字串。
        /// </summary>
        /// <param name="org">原始字串</param>
        /// <param name="sLen">長度</param>
        /// <param name="padStr">替代字元</param>
        private static string CHT_PadRight(string org, int sLen, char padStr)
        {
            var sResult = "";
            int orgLen = 0;
            int tLen = 0;
            // 計算轉換過實際的總長
            for (int i = 0; i < org.Length; i++)
            {
                string s = org.Substring(i, 1);
                int vLen = 0;
                // 判斷 ASC 表是否介於 0~128
                if (Convert.ToInt32(s[0]) > 128 || Convert.ToInt32(s[0]) < 0)
                {
                    vLen = 2;
                }
                else
                {
                    vLen = 1;
                }
                orgLen += vLen;
                if (orgLen > sLen)
                {
                    orgLen -= vLen;
                    break;
                }
                sResult += s;
            }
            // 計算轉換過後，最後實際的長度
            tLen = sLen - (orgLen - org.Length);
            // 傳回
            return sResult.PadRight(tLen, padStr);
        }
        #endregion
    }
    #endregion
}
