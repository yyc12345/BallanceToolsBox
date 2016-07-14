using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Media;

namespace BallanceOnline {

    public class Player {

        public Player() {
            readyColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            nowLife = "3";
            nowTime = "1000";
            nowUnit = "1";
            nowUnit = "1";
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
                return new StringGroup(dutyUnit, ",").ToString();
            }
        }
        /// <summary>
        /// 需要完成任务的数量
        /// </summary>
        public int DutyUnitCount { get { return dutyUnit.Count; } }
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
        /// 最终的hs/sr成绩
        /// </summary>
        protected string finallyMark;
        /// <summary>
        /// 最终的hs/sr成绩
        /// </summary>
        public string FinallyResult { get { return finallyMark; } set { finallyMark = value; } }
        /// <summary>
        /// 最终pp点数
        /// </summary>
        protected string finallyPP;
        /// <summary>
        /// 最终pp点数
        /// </summary>
        public string FinallyPP { get { return finallyPP; } set { finallyPP = value; } }
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
        /// 分数
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
        public const string Playing = "正在游戏";
        /// <summary>
        /// 暂停中
        /// </summary>
        public const string Paused = "暂停中";
        /// <summary>
        /// 已死亡
        /// </summary>
        public const string Died = "已死亡";
        /// <summary>
        /// 已通关
        /// </summary>
        public const string Success = "已通关";
    }


}
