using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceOnline {
    public class MapSetting {

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

    }
}
