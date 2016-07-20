using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Media;
using BallanceOnline.Const;

namespace BallanceOnline {

    public class Player {

        public Player() {

            playerIPAddress = "";
            playerName = "";

            modList = new List<string>();
            backgroundName = "";
            bgmName = "";

            historyFailCount = "";
            historyRankedRaceCount = "";
            historyRelayRaceCount = "";
            historyWinCount = "";

            dutyUnit = new List<string>();
            playerGroupName = "";

            readyColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

            nowLife = "";
            nowTime = "";
            nowUnit = "1";
            nowState = PlayerState.Playing;
            nowStateColor = new SolidColorBrush(Color.FromArgb(80, 0, 0, 0));

            playerUnitPrize = new List<PlayerUnitData>();
            finallyMark = "";
            finallyPP = "";
            finallyPrize = "";

        }

        #region 基础信息
        /// <summary>
        /// ip地址
        /// </summary>
        protected string playerIPAddress;
        /// <summary>
        /// ip地址
        /// </summary>
        public string PlayerIPAddress { get { return playerIPAddress; } set { playerIPAddress = value; } }
        /// <summary>
        /// 玩家名称
        /// </summary>
        protected string playerName;
        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName {
            get { return playerName; }
            set {

                //if (value.IndexOf(",") >= 0) { value = value.Replace(",", ""); }
                //if (value.IndexOf("#") >= 0) { value = value.Replace("#", ""); }
                //if (value.IndexOf("@") >= 0) { value = value.Replace("@", ""); }
                //if (value.IndexOf("%") >= 0) { value = value.Replace("%", ""); }

                //if (value.Length == 0) {
                //    var cache = new Random();
                //    value = cache.Next(int.MinValue, int.MaxValue).ToString();
                //}

                playerName = value;
            }
        }

        #endregion

        #region 游戏设置信息

        /// <summary>
        /// mod列表
        /// </summary>
        protected List<string> modList;
        /// <summary>
        /// mod列表
        /// </summary>
        public List<string> ModList { get { return modList; } set { modList = value; } }
        /// <summary>
        /// mod列表字符串表达式，用,分割
        /// </summary>
        public string ModListToString {
            get {
                if (modList.Count != 0) return new StringGroup(modList, ",").ToString();
                else return "";
            }
        }
        /// <summary>
        /// 背景名
        /// </summary>
        protected string backgroundName;
        /// <summary>
        /// 背景名
        /// </summary>
        public string BackgroundName { get { return backgroundName; } set { backgroundName = value; } }
        /// <summary>
        /// bgm名
        /// </summary>
        protected string bgmName;
        /// <summary>
        /// bgm名
        /// </summary>
        public string BGMName { get { return bgmName; } set { bgmName = value; } }

        #endregion

        #region 玩家历史纪录功能

        /// <summary>
        /// 胜利场数
        /// </summary>
        protected string historyWinCount;
        /// <summary>
        /// 胜利场数
        /// </summary>
        public string HistoryWinCount { get { return historyWinCount; } set { historyWinCount = value; } }

        /// <summary>
        /// 失败场数
        /// </summary>
        protected string historyFailCount;
        /// <summary>
        /// 失败场数
        /// </summary>
        public string HistoryFailCount { get { return historyFailCount; } set { historyFailCount = value; } }


        /// <summary>
        /// 排位赛次数
        /// </summary>
        protected string historyRankedRaceCount;
        /// <summary>
        /// 排位赛次数
        /// </summary>
        public string HistoryRankedRaceCount { get { return historyRankedRaceCount; } set { historyRankedRaceCount = value; } }

        /// <summary>
        /// 接力赛次数
        /// </summary>
        protected string historyRelayRaceCount;
        /// <summary>
        /// 接力赛次数
        /// </summary>
        public string HistoryRelayRaceCount { get { return historyRelayRaceCount; } set { historyRelayRaceCount = value; } }

        #endregion

        #region 游戏分配信息

        /// <summary>
        /// 负责的小节列表
        /// </summary>
        protected List<string> dutyUnit;
        /// <summary>
        /// 负责的小节列表
        /// </summary>
        public List<string> DutyUnit { get { return dutyUnit; } set { dutyUnit = value; } }
        /// <summary>
        /// 负责的小节列表-字符形式
        /// </summary>
        public string DutyUnitToString {
            get {
                if (dutyUnit.Count != 0) return new StringGroup(dutyUnit, ",").ToString();
                else return "";
            }
        }
        /// <summary>
        /// 需要完成任务的数量
        /// </summary>
        public int DutyUnitCount {
            get {
                return dutyUnit.Count;
            }
        }
        /// <summary>
        /// 当前完成的任务数量
        /// </summary>
        public int NowFinishDutyCount {
            get {
                int cross = 0;

                var cache = new StringGroup(dutyUnit, ",").ToStringGroup();
                foreach (string item in cache) {
                    if (int.Parse(item) < int.Parse(nowUnit)) {
                        cross++;
                    }
                }

                return cross;
            }
        }

        /// <summary>
        /// 所属组名
        /// </summary>
        protected string playerGroupName;
        /// <summary>
        /// 所属组名
        /// </summary>
        public string PlayerGroupName { get { return playerGroupName; } set { playerGroupName = value; } }

        #endregion

        #region 游戏等待阶段

        /// <summary>
        /// 表示用户就绪状态的颜色
        /// </summary>
        public SolidColorBrush readyColor { get; set; }
        /// <summary>
        /// 设置用户准备完毕
        /// </summary>
        public void playerIsReady() {
            readyColor = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
        }

        #endregion

        #region 游戏进行阶段

        /// <summary>
        /// 当前生命
        /// </summary>
        protected string nowLife;
        /// <summary>
        /// 当前生命
        /// </summary>
        public string NowLife { get { return nowLife; } set { nowLife = value; } }

        /// <summary>
        /// 当前时间
        /// </summary>
        protected string nowTime;
        /// <summary>
        /// 当前时间
        /// </summary>
        public string NowTime { get { return nowTime; } set { nowTime = value; } }

        /// <summary>
        /// 当前状态
        /// </summary>
        protected string nowState;
        /// <summary>
        /// 当前状态
        /// </summary>
        public string NowState { get { return nowState; } set { nowState = value; } }

        /// <summary>
        /// 表示用户游戏进行状态的颜色
        /// </summary>
        public SolidColorBrush nowStateColor { get; set; }
        /// <summary>
        /// 设置用户游戏进行状态的颜色
        /// </summary>
        public void SetNowStateColor() {
            switch (nowState) {
                case PlayerState.Playing:
                    nowStateColor = new SolidColorBrush(Color.FromArgb(80, 0, 0, 0));
                    break;
                case PlayerState.Died:
                    nowStateColor = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0));
                    break;
                case PlayerState.Success:
                    nowStateColor = new SolidColorBrush(Color.FromArgb(80, 0, 255, 0));
                    break;
            }

        }

        /// <summary>
        /// 当前所在小节
        /// </summary>
        protected string nowUnit;
        /// <summary>
        /// 当前所在小节
        /// </summary>
        public string NowUnit { get { return nowUnit; } set { nowUnit = value; } }

        #endregion

        #region 游戏结果

        /// <summary>
        /// 每一小节成绩
        /// </summary>
        protected List<PlayerUnitData> playerUnitPrize;
        /// <summary>
        /// 每一小节成绩
        /// </summary>
        public List<PlayerUnitData> PlayerUnitPrize { get { return playerUnitPrize; } set { playerUnitPrize = value; } }
        /// <summary>
        /// 小节数据字符串表达
        /// </summary>
        public string PlayerUnitPrizeToString {
            get {

                List<string> returnData = new List<string>();

                foreach (PlayerUnitData item in playerUnitPrize) {
                    returnData.Add("小节：" + item.Unit + " 成绩：" + item.Mark + " 生命：" + item.Life + " PP：" + item.PerfomancePoint);
                }

                return new StringGroup(returnData, Environment.NewLine).ToString();

            }
        }
        /// <summary>
        /// 最终的hs/sr成绩
        /// </summary>
        protected string finallyMark;
        /// <summary>
        /// 最终的hs/sr成绩
        /// </summary>
        public string FinallyMark { get { return finallyMark; } set { finallyMark = value; } }
        /// <summary>
        /// 最终的hs/sr成绩，比较专用字符
        /// </summary>
        public int FinallyMarkCompareNumber {
            get {
                if (finallyMark != "") {
                    if (finallyMark.Contains(".") == true) {

                        //translate
                        if (finallyMark.Contains(":")) {
                            var result = finallyMark.Split(':');
                            var result2 = result[1].Split('.');
                            return int.Parse(result[0]) * 600 + int.Parse(result2[0]) * 10 + int.Parse(result2[1]);
                        } else {
                            var result = finallyMark.Split('.');
                            return int.Parse(result[0]) * 10 + int.Parse(result[1]);
                        }

                    } else {
                        return int.Parse(finallyMark);
                    }
                } else { return 0; }
            }
        }
        /// <summary>
        /// 最终pp点数，pp大约取值范围是0-10
        /// </summary>
        protected string finallyPP;
        /// <summary>
        /// 最终pp点数
        /// </summary>
        public string FinallyPP { get { return finallyPP; } set { finallyPP = value; } }
        /// <summary>
        /// 最终pp点数数字表达式
        /// </summary>
        public int FinallyPPNumber {
            get {
                if (finallyPP != "") {
                    return int.Parse(finallyPP);
                } else { return 0; }
            }
        }
        /// <summary>
        /// 最终的称号
        /// </summary>
        protected string finallyPrize;
        /// <summary>
        /// 最终的称号
        /// </summary>
        public string FinallyPrize { get { return finallyPrize; } set { finallyPrize = value; } }

        #endregion

    }

    public class PlayerUnitData {
        /// <summary>
        /// 小节
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 分数
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// 生命
        /// </summary>
        public string Life { get; set; }
        /// <summary>
        /// pp
        /// </summary>
        public string PerfomancePoint { get; set; }

    }

    public class PlayerState {
        /// <summary>
        /// 正在游戏
        /// </summary>
        public const string Playing = "P";
        /// <summary>
        /// 已死亡
        /// </summary>
        public const string Died = "D";
        /// <summary>
        /// 已通关
        /// </summary>
        public const string Success = "S";
    }


}
