using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceOnline.Const {

    /// <summary>
    /// 计分方式
    /// </summary>
    public class CountMode {
        /// <summary>
        /// SR
        /// </summary>
        public const string SpeedRun = "SR";
        /// <summary>
        /// HS
        /// </summary>
        public const string HighScore = "HS";
        /// <summary>
        /// 疯狂SR
        /// </summary>
        public const string CrazySpeedRun = "CSR";
        /// <summary>
        /// 疯狂HS
        /// </summary>
        public const string CrazyHighScore = "CHS";

    }

    /// <summary>
    /// 游戏模式
    /// </summary>
    public class GameMode {
        /// <summary>
        /// 接力赛
        /// </summary>
        public const string RelayRace = "ReR";
        /// <summary>
        /// 排位赛
        /// </summary>
        public const string RankedRace = "RaR";
    }

    /// <summary>
    /// 服务器和客户端标识符
    /// </summary>
    public class ClientAndServerSign {
        /// <summary>
        /// 客户端
        /// </summary>
        public const string Client = "C_";
        /// <summary>
        /// 服务器
        /// </summary>
        public const string Server = "S_";
    }

    /// <summary>
    /// 游戏成就
    /// </summary>
    public class GamePrize {
        /// <summary>
        /// MVP
        /// </summary>
        public const string MostValuablePlayer = "MVP";
        /// <summary>
        /// 球魂
        /// </summary>
        public const string BallSoul = "BS";
    }

    /// <summary>
    /// 交流符号
    /// </summary>
    public class SocketSign {

        /// <summary>
        /// 服务器使用
        /// 返回所有玩家，除了自己。数据区格式：SG 分隔符,
        /// 分隔符分割玩家IP
        /// 客户端不使用
        /// </summary>
        public const string ReturnAllPlayer = "RAP";
        /// <summary>
        /// 服务器使用
        /// 返回新加入的玩家。数据区格式：string 玩家IP
        /// 客户端不使用
        /// </summary>
        public const string NewPlayer = "NP";

        /// <summary>
        /// 服务器使用
        /// 广播消息。数据区格式：string 消息内容
        /// 客户端使用
        /// 提交要广播的消息。数据区格式：string 消息内容
        /// </summary>
        public const string Message = "M";

        /// <summary>
        /// 服务器使用
        /// 让指定玩家回答存在。无数据区
        /// 客户端使用
        /// 返回服务器标明自己存在。无数据区
        /// </summary>
        public const string Ping = "P";

        /// <summary>
        /// 服务器使用
        /// 要求玩家上交个人数据。无数据区
        /// 客户端不使用
        /// </summary>
        public const string OrderTurnIn = "OTI";
        /// <summary>
        /// 服务器不使用
        /// 客户端使用
        /// 上交个人数据。数据区格式：SG 分隔符,
        /// [SG mod列表 分隔符#,背景名,bgm名,玩家名]
        /// </summary>
        public const string InformationTurnIn = "ITI";
        /// <summary>
        /// 服务器使用
        /// 分发用户数据。数据区格式：SG 分隔符,
        /// [SG mod列表 分隔符#,背景名,bgm名,玩家名]
        /// 客户端不使用
        /// </summary>
        public const string InformationGiveOut = "IGO";

        /// <summary>
        /// 服务器使用
        /// 给玩家指派任务。数据区格式：SG 分隔符,
        /// [玩家名,地图名称,地图md5,比赛计分方式,比赛模式,SG 此玩家负责的小节 分隔符#,所属组别]
        /// 客户端不使用
        /// </summary>
        public const string PlayerTask = "PT";

        /// <summary>
        /// 服务器不使用
        /// 客户端使用
        /// 表明玩家准备完毕。无数据区
        /// </summary>
        public const string ReplyReady = "RR";

        /// <summary>
        /// 服务器使用
        /// 要求每个人进入3秒倒计时，准备开始游戏。无数据区
        /// 客户端不使用
        /// </summary>
        public const string ReadyPlay = "RP";


        /*服务器维护一个列表
         * 列表负责记录每个小节始末2处的
         * 游戏数据方便分析
         */

        /// <summary>
        /// 服务器不使用
        /// 客户端使用
        /// 提交当前游戏信息。数据区格式：SG 分隔符,
        /// [游戏左下分数,游戏生命,当前小节数]
        /// </summary>
        public const string GameDataTurnIn = "GDTI";
        /// <summary>
        /// 服务器使用
        /// 提交当前游戏信息。数据区格式：SG 分隔符,
        /// [游戏左下分数,游戏生命,当前小节数,奖励描述符,玩家名称]
        /// 客户端不使用
        /// </summary>
        public const string GameDataGiveOut = "GDGO";

        /// <summary>
        /// 服务器使用
        /// 广播某人死亡。数据区格式：string 死亡玩家名
        /// 客户端使用
        /// 提交某人死亡。无数据区
        /// </summary>
        public const string PlayerDied = "PD";
        /// <summary>
        /// 服务器使用
        /// 广播某组死亡，结算游戏数据，组死亡和人死亡选择一个广播，通常，玩家完不成他所负责的小节时，会通知组死亡。数据区格式：SG 分隔符,
        /// 分隔符分割死亡组里的玩家名称
        /// 客户端不使用
        /// </summary>
        public const string TeamDied = "TD";

        /// <summary>
        /// 服务器使用
        /// 表明游戏结束，等待结果。无数据区
        /// 客户端不使用
        /// </summary>
        public const string GameEnd = "GE";

        /// <summary>
        /// 服务器使用
        /// 分发每位玩家的数据。数据区格式：SG 主分隔符, 次分隔符#
        /// 主分隔符分割每位玩家，次分隔符[SG 每一小节的数据统计 主分隔符@ 次分隔符% 主分隔符分割每一小节，次分隔符(分数%生命%pp数) #总的hs/sr分数#总pp数#评定奖励]
        /// 客户端不使用
        /// </summary>
        public const string AllPlayerGameData = "APGD";

        /*根据比赛模式不同，
         * 如需要处理组内数据，
         * 在本地处理掉
         */

        //随后服务器关闭连接，结束
    }
}



