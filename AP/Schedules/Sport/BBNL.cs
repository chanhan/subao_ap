using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedules
{
    public partial class FrmMain
    {
        #region BBNL - 荷兰棒球

        private Dictionary<string, GameInfo> GetSchedulesByHB(int allianceID, string gameType, bool acH = false)
        {

            Dictionary<string, GameInfo> schedules = new Dictionary<string, GameInfo>();
            DateTime gameDate = DateTime.Now;

            DateTime GameDate = new DateTime();

            DateTime startDate = this.dtpHBSDate.Value.Date;
            DateTime endDate = this.dtpHBEDate.Value.Date;

            System.Net.WebClient web = new System.Net.WebClient();
            web.Encoding = Encoding.UTF8;
            string s = "";
            // 錯誤處理
            try
            {
                // 下載資料http://www.knbsbstats.nl/2015/HB/scheduleHB.htm
                string htmlText = web.DownloadString(string.Format("http://www.knbsbstats.nl/{0}/HB/scheduleHB.htm", startDate.ToString("yyyy")));
                // 判斷網頁完成
                if (!string.IsNullOrEmpty(htmlText))
                {
                    HtmlDocument htmlDoc = new HtmlDocument();
                    //加载资料
                    htmlDoc.LoadHtml(htmlText);

                    IEnumerable<HtmlNode> tables = htmlDoc.DocumentNode.Descendants("table");

                    //检查有没有读到资料
                    if (htmlDoc == null || tables == null || tables.Count() < 2) { return null; }

                    //变量
                    DateTime date = DateTime.MinValue;//日期
                    int count = 0;
                    //循环game节点
                    foreach (HtmlNode node in tables)
                    {
                        HtmlNode td = node.SelectSingleNode(".//tbody[1]/tr[1]/td[1]");
                        if (td == null) { continue; }
                        if (td.Attributes["class"].Value == "style1")
                        {
                            //区域性名称和标识符。 https://msdn.microsoft.com/zh-cn/library/System.Globalization.CultureInfo%28v=vs.80%29.aspx
                            IFormatProvider culture = new CultureInfo("nl", true);
                            s = td.InnerText.Trim();
                            date = DateTime.Parse(td.InnerText.Replace("&nbsp;", "").Trim(), culture);
                        }

                        if (date != DateTime.MinValue && startDate <= date && endDate >= date && td.Attributes["class"].Value == "bianco_pi")
                        {
                            //真正的比赛数据                               
                            if (!DateTime.TryParse(date.ToString("yyyy-MM-dd ") + node.SelectSingleNode(".//tbody[1]/tr[1]/td[2]").InnerText.Trim(), out GameDate)) { continue; }
                            GameDate = GameDate.AddHours(6);//荷兰时间转换为我们的时间

                            // 建立賽程
                            GameInfo schedule = new GameInfo(allianceID, gameType, GameDate, GameDate.ToString("yyyyMMdd") + node.SelectSingleNode(".//tbody[1]/tr[1]/td[7]").InnerText.Trim());

                            //队伍名称 
                            schedule.Home = node.SelectSingleNode(".//tbody[1]/tr[1]/td[4]").InnerText.Replace("&nbsp;", "").Replace("&amp;", "&").Replace("&ccedil;", "ç").Trim();
                            schedule.Away = node.SelectSingleNode(".//tbody[1]/tr[1]/td[5]").InnerText.Replace("&nbsp;", "").Replace("&amp;", "&").Replace("&ccedil;", "ç").Trim(); ;
                            // 加入比賽資料
                            schedules[schedule.WebID] = schedule;
                            count++;
                        }
                        //如果时间超出设定范围就退出循环
                        if ((endDate == date && count == 4) || (endDate < date))
                        {
                            break;
                        }
                        //1天最多只有4场
                        if (count == 4)
                        {
                            date = DateTime.MinValue;
                            count = 0;
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "//" + ex.StackTrace + "==" + s);
            }

            // 傳回
            return schedules;
        }
        #endregion BBNL - 荷兰棒球
    }
}
