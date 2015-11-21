using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace GameScoresApp
{
    class WebRequestData
    {
        public WebRequestData(string gameType, string gameStatus, string GID,string sourceType=null)
        {
            this.iGameType = 0;//無此比賽 不處理

            if (string.IsNullOrEmpty(gameType) ||
                string.IsNullOrEmpty(GID) ||
                string.IsNullOrEmpty(gameStatus))
                return;//必要資料不存在

            this.GID = GID;
            this.gameType = gameType;   //比賽類型
            this.gameStatus = gameStatus;//比賽狀態
            this.gameStatusQsf = false; //是否處理首分
            this.gameStatusR = false;   //是否處理全場  
            this.gameStatusQwf2 = false; //是否處理冰球不包含加时赛的尾分
            this.gameStatusQsf2 = false;//是否處理冰球不包含加时赛的首分
            #region 設定比賽類型
            if (gameType.IndexOf("bb") == 0)//設定比賽類型/名稱
            {
                if (gameType.IndexOf("us") > -1)//美棒
                    iGameType = 1;
                else if (gameType.IndexOf("jp") > -1)//日棒
                    this.iGameType = 2;
                else if (gameType.IndexOf("tw") > -1)//台棒
                    //針對台棒爆米花改其他類別
                    //this.iGameType = 3;
                    this.iGameType = (gameType.Contains("bbtw7")) ? 14 : 3;
                else if (gameType.IndexOf("kr") > -1)//韓棒
                    this.iGameType = 4;
                else
                    this.iGameType = 14;//其他類棒球

                this.GameTeam = "BaseballTeam";
                this.Runs = new string[10][];
            }
            else if (gameType.IndexOf("bk") == 0)//籃球: BK*     奧遜:BKOS  bf:BKBF
            {
                if (gameType.IndexOf("bkus") == 0)
                    this.iGameType = 6;  //NBA, WNBA
                else
                    this.iGameType = 13; //日籃, 歐籃, 陸籃...etc

                this.alliance = sourceType;    //存放的奧遜聯盟ID
                this.GameTeam = "BasketballTeam";
                this.Runs = new string[5][];
            }
            else if (gameType.IndexOf("ih") == 0)//冰球
            {                
                this.GameTeam = "IceHockeyTeam";
                
                this.iGameType = 5;
                this.Runs = new string[5][];
                this.RunsQsf2 = new string[2];
            }
            else if (gameType.IndexOf("af") == 0)//美足
            {
                this.GameTeam = "AFBTeam";
                this.iGameType = 8;
            }
            else if (gameType == "tn")
            {                
                this.GameTeam = "TennisTeam";
                this.iGameType = 9;
                this.Runs = new string[5][];
            }
            else
            {
                this.alliance = gameType;

                //奥讯篮球 BF篮球
                this.gameType = sourceType;   //比賽類型
              
                this.iGameType = 13;

                this.GameTeam = "BasketballTeam";
                this.Runs = new string[5][];
            }
            #endregion

            this.RunsQsf = new string[2];
            this.RunsHalf = new string[2][];
            this.RunsQwf = new string[2];
            this.RunsR = new string[2];
        }

        public string GID { get; set; }
        public int iGameType { get; set; }
        public string GameTeam { get; set; }
        public string gameType { get; set; }
        public string alliance { get; set; }
        public string gameDate { get; set; }
        public string NA { get; set; }
        public string NB { get; set; }

        //單節(局)
        public string[][] Runs { get; set; }
        public string gameStatus { get; set; }

        //首分
        public string[] RunsQsf { get; set; }
        public bool gameStatusQsf { get; set; }

        /// <summary>
        /// 冰球 不包括加时赛 的首分
        /// </summary>
        public string[] RunsQsf2 { get; set; }

        /// <summary>
        /// 冰球 不包括加时赛
        /// </summary>
        public bool gameStatusQsf2 { get; set; }

        //半場
        public string[][] RunsHalf { get; set; }

        //尾分
        public string[] RunsQwf { get; set; }

        /// <summary>
        ///  区分冰球不包含加时赛的
        /// </summary>
        public bool gameStatusQwf2 { get; set; }
        //全場
        public string[] RunsR { get; set; }
        public bool gameStatusR { get; set; }
    }

    public class RequestState
    {
        public WebRequest Request = null;
        public string state = "";
        public string url = "";
        public int errorTimes = 0;
    }
}
