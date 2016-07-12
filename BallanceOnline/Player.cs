using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace BallanceOnline {

    public class Player {
        /// <summary>
        /// ip地址
        /// </summary>
        protected IPAddress playerIPAddress;
        /// <summary>
        /// ip地址
        /// </summary>
        public IPAddress PlayerIPAddress { get { return playerIPAddress; } }
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

                if (value.IndexOf(",") >= 0) { value = value.Replace(",", ""); }
                if (value.IndexOf("#") >= 0) { value = value.Replace("#", ""); }
                if (value.IndexOf("@") >= 0) { value = value.Replace("@", ""); }
                if (value.IndexOf("%") >= 0) { value = value.Replace("%", ""); }

                if (value.Length == 0) {
                    var cache = new Random();
                    value = cache.Next(int.MinValue, int.MaxValue).ToString();
                }

                playerName = value;
            }
        }

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

        /// <summary>
        /// 地图名
        /// </summary>
        protected string mapName;
        /// <summary>
        /// 地图名
        /// </summary>
        public string MapName { get { return mapName; } set { mapName = value; } }
        /// <summary>
        /// 地图md5-用于验证
        /// </summary>
        protected string mapMD5;
        /// <summary>
        /// 地图md5-用于验证
        /// </summary>
        public string MapMD5 { get { return mapMD5; } set { mapMD5 = value; } }
        /// <summary>
        /// 游戏模式
        /// </summary>
        protected string gameMode;
        /// <summary>
        /// 游戏模式
        /// </summary>
        public string GameMode { get { return gameMode; } set { gameMode = value; } }
        /// <summary>
        /// 游戏计分模式
        /// </summary>
        protected string countMode;
        /// <summary>
        /// 游戏计分模式
        /// </summary>
        public string CountMode { get { return countMode; } set { countMode = value; } }
        /// <summary>
        /// 负责的小节列表
        /// </summary>
        protected List<int> dutyUnit;
        /// <summary>
        /// 负责的小节列表
        /// </summary>
        public List<int> DutyUnit { get { return dutyUnit; } set { dutyUnit = value; } }
        /// <summary>
        /// 所属组名
        /// </summary>
        protected string playerGroupName;
        /// <summary>
        /// 所属组名
        /// </summary>
        public string PlayerGroupName { get { return playerGroupName; } set { playerGroupName = value; } }

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
        protected uint finallyPP;
        /// <summary>
        /// 最终pp点数
        /// </summary>
        public uint FinallyPP { get { return finallyPP; } set { finallyPP = value; } }
        /// <summary>
        /// 最终的称号
        /// </summary>
        protected string finallyPrize;
        /// <summary>
        /// 最终的称号
        /// </summary>
        public string FinallyPrize { get { return finallyPrize; } set { finallyPrize = value; } }

    }

    public class PlayerUnitData {
        /// <summary>
        /// 分数
        /// </summary>
        public uint Mark;
        /// <summary>
        /// 生命
        /// </summary>
        public uint Life;
        /// <summary>
        /// pp
        /// </summary>
        public uint PerfomancePoint;

    }


}
