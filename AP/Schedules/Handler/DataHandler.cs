using Schedules.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common = Schedules.CommonWS;
using Schedule = Schedules.ScheduleWS;
using Team = Schedules.TeamWS;

namespace Schedules
{
    public partial class FrmMain
    {
        /// <summary>
        /// 顯示網頁
        /// </summary>
        /// <param name="web">WebBrowser</param>
        /// <param name="url">網址</param>
        private void ShowWeb(WebBrowser web, string url)
        {
            // 顯示網頁
            web.Navigate(url);
        }

        /// <summary>
        /// 顯示來源編號
        /// </summary>
        /// <param name="gameType">賽事種類</param>
        /// <param name="title">標題</param>
        /// <param name="schedules">賽事資訊</param>
        private void ShowWebId(string gameType, string title, Dictionary<string, GameInfo> schedules)
        {
            if (schedules == null || schedules.Count == 0)
            {
                MessageBox.Show("無賽程資料。");
                return;
            }

            FrmWebId frm = new FrmWebId() { Text = title };
            frm.OnDataBind(schedules, this.GetTeamName(gameType));
            frm.OnScheduleSave += FrmWebId_OnScheduleSave;
            frm.ShowDialog(this);
        }

        /// <summary>
        /// 儲存賽程
        /// </summary>
        /// <param name="title">標題</param>
        /// <param name="schedules">賽程資料</param>
        /// <param name="setTypeVal">TypeVal 設定值</param>
        /// <param name="delTime">是否調整時間</param>
        /// <returns>是否成功</returns>
        private bool SaveSchedule(string title, Dictionary<string, GameInfo> schedules, string setTypeVal, bool delTime)
        {
            try
            {
                if (schedules == null || schedules.Count == 0)
                {
                    MessageBox.Show("無賽程資料。");
                    return true;
                }
                else
                {
                    // 過濾 Comment 不為空的資料
                    var keysToRemove = schedules.Where(p => !String.IsNullOrEmpty(p.Value.Comment))
                                        .Select(p => p.Key).ToArray();
                    foreach (var key in keysToRemove)
                    {
                        schedules.Remove(key);
                    }

                    // 詢問
                    if (MessageBox.Show(this, string.Format("您確定要新增 {0} 個賽程？", schedules.Count), title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return false;
                    }
                }

                // 賽程
                int successCount = 0;
                int existCount = 0;
                //int errorCount = 0;
                int teamCount = 0;
                Dictionary<string, Team.Team> teamList = new Dictionary<string, Team.Team>();
                List<Schedule.Schedule> scheduleList = new List<Schedule.Schedule>();

                // 取得 TypeVal
                int time = 0;
                if (!String.IsNullOrEmpty(setTypeVal))
                {
                    Common.ExecuteResult result = _wsCommon.GetValueFromSetTypeVal(_token, setTypeVal, null);
                    if (result.ResultType == (int) ErrorCode.Success)
                    {
                        // 時間調整值
                        if (Int32.TryParse(result.ResultMessage, out time))
                        {
                            if (delTime)
                            {
                                time = 0 - time;
                            }
                        }
                    }
                    else
                    {
                        ShowExecuteResultError(title, "取得時間調整值錯誤。", result.ResultMessage);
                        return false;
                    }
                }

                // 加入隊伍
                foreach (KeyValuePair<string, GameInfo> schedule in schedules)
                {
                    GameInfo info = schedule.Value;
                    string home = info.Home;
                    string away = info.Away;
                    // 主客隊互換
                    info.SwapTeam(ref home, ref away);

                    // 加入隊伍(主)
                    AddToTeamList(teamList, info.GameType, info.AllianceID, home, info.SourceID);
                    // 加入隊伍(客)
                    AddToTeamList(teamList, info.GameType, info.AllianceID, away, info.SourceID);
                }

                // 隊伍檢查
                foreach (Team.Team team in teamList.Values)
                {
                    string teamID = "";
                    // 以 WebName, SourceID 做檢查
                    Team.Team teamCheck = new Team.Team()
                    {
                        GameType = team.GameType,
                        WebName = team.WebName,
                        SourceID = team.SourceID,
                        IsDeleted = team.IsDeleted
                    };

                    Team.ExecuteResult execResult = _wsTeam.CheckTeamExists(_token, teamCheck);
                    if (execResult.ResultType == (int) ErrorCode.Success)
                    {
                        // 資料不存在, 新增隊伍
                        execResult = _wsTeam.AddTeam(_token, team);
                        if (execResult.ResultType == (int) ErrorCode.Success)
                        {
                            teamCount++;
                            teamID = execResult.ResultMessage;
                        }
                        else
                        {
                            ShowExecuteResultError(title, "隊伍新增發生錯誤。", execResult.ResultMessage);
                            return false;
                        }
                    }
                    else if (execResult.ResultType == 1)
                    {
                        // 資料存在
                        teamID = execResult.ResultMessage;
                    }
                    else
                    {
                        ShowExecuteResultError(title, "隊伍檢查發生錯誤。", execResult.ResultMessage);
                        return false;
                    }

                    int val;
                    if (Int32.TryParse(teamID, out val)) { team.ID = val; }
                }

                // 加入賽程
                foreach (KeyValuePair<string, GameInfo> schedule in schedules)
                {
                    GameInfo info = schedule.Value;
                    AddToSchedule(scheduleList, teamList, info, time);
                }

                // 判斷賽程是否存在
                Schedule.Schedule[] scArr = scheduleList.ToArray();
                foreach (Schedule.Schedule sc in scArr)
                {
                    Schedule.ExecuteResult execResult = _wsSchedule.CheckScheduleExists(_token, sc);
                    if (execResult.ResultType == 1)
                    {
                        // 已存在賽程
                        scheduleList.Remove(sc);
                        existCount++;
                    }
                    else if (execResult.ResultType >= 1)
                    {
                        ShowExecuteResultError(title, "賽程檢查發生錯誤。", execResult.ResultMessage);
                        return false;
                    }
                }

                // 批次建立賽程
                scArr = scheduleList.ToArray();
                Schedule.ExecuteDataChangeResult changeResult = _wsSchedule.AddScheduleBatch(_token, scArr);

                if (changeResult.ResultType == (int) ErrorCode.Success)
                {
                    successCount = changeResult.ChangeCount;
                }
                else
                {
                    ShowExecuteResultError(title, "批次建立賽程發生錯誤。", changeResult.ResultMessage);
                    return false;
                }

                // 顯示
                string msg = string.Format("新增了 {0} 個賽程, {1} 個隊伍。\r已存在 {2} 個賽程。",
                                            successCount, teamCount, existCount);
                MessageBox.Show(this, msg, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("發生錯誤。 {0}", ex.Message), title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        /// <summary>
        /// 加入賽程列表
        /// </summary>
        /// <param name="scheduleList">賽程列表</param>
        /// <param name="teamList">隊伍列表</param>
        /// <param name="info">賽程資訊</param>
        /// <param name="time">時間調整值</param>
        private void AddToSchedule(List<Schedule.Schedule> scheduleList, Dictionary<string, Team.Team> teamList, GameInfo info, int time)
        {
            // 建立 schedule 物件
            Schedule.Schedule sc = new Schedule.Schedule()
            {
                AllianceID = info.AllianceID,
                FullGameTime = info.GameTime.AddHours(time),
                GameType = info.GameType,
                WebID = info.WebID,
                ControlStates = 2, // 自動操盤
                GameStates = "X", // 未開賽
                OrderBy = 0
            };

            string gameType = info.GameType.ToLower();

            // Order By
            switch (gameType)
            {
                // MLB, NBA 排序置頂
                case "bkus":
                case "bbus":
                    sc.OrderBy = 1;
                    break;
            }

            // 棒球(補賽)
            if (gameType.StartsWith("bb"))
            {
                // IsReschedule
                sc.IsReschedule = false;
            }

            string home = info.Home;
            string away = info.Away;
            // 主客隊互換
            info.SwapTeam(ref home, ref away);

            // 取得隊伍編號
            int? teamAID = (!teamList.ContainsKey(away)) ? null : teamList[away].ID;
            int? teamBID = (!teamList.ContainsKey(home)) ? null : teamList[home].ID;

            if (teamAID.HasValue && teamBID.HasValue)
            {
                sc.TeamAID = teamAID;
                sc.TeamBID = teamBID;
            }

            if (!scheduleList.Contains(sc)) { scheduleList.Add(sc); }
        }

        /// <summary>
        /// 加入隊伍列表
        /// </summary>
        /// <param name="teamList">隊伍列表</param>
        /// <param name="gameType">賽事種類</param>
        /// <param name="allianceID">聯盟編號</param>
        /// <param name="webName">來源名稱</param>
        /// <param name="sourceId">來源編號</param>
        private void AddToTeamList(Dictionary<string, Team.Team> teamList, string gameType, int allianceID, string webName, string sourceId)
        {
            Team.Team team = new Team.Team()
            {
                GameType = gameType,
                AllianceID = allianceID,
                WebName = webName,
                ShowName = webName,
                TeamName = webName,
                SourceID = sourceId
            };

            if (!teamList.ContainsKey(webName))
            {
                teamList.Add(webName, team);
            }
        }
    }
}