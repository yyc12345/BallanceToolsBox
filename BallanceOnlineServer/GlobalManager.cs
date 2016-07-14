using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BallanceOnline;
using System.Net.Sockets;

namespace BallanceOnlineServer {

    public class GlobalManager {

        public GlobalManager() {
            clientPlayerList = new List<Player>();
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        public List<BallanceOnlineServer.Player> clientPlayerList;

        /// <summary>
        /// 全体成员广播
        /// </summary>
        /// <param name="data"></param>
        public void allPlayerBroadcast(byte[] data) {

        }

        /// <summary>
        /// 个人广播
        /// </summary>
        /// <param name="playerIP">用户ip</param>
        /// <param name="data"></param>
        public void singlePlayerBroadcast(string playerIP, byte[] data) {

        }

    }

    /// <summary>
    /// 服务器专用类
    /// </summary>
    public class Player : BallanceOnline.Player {

        public Player(ref TcpClient inputClient, Action<string, byte[], Player> replay) {
            gameData = new GameData(ref inputClient, this.replyInsider);
            replyEx = new Action<string, byte[], Player>(replay);
        }

        /// <summary>
        /// 对内部委托封装，加入传出自身实例的功能
        /// </summary>
        Action<string, byte[], Player> replyEx;

        /// <summary>
        /// 对内部类响应处理的方式
        /// </summary>
        /// <param name="cache1"></param>
        /// <param name="cache2"></param>
        protected void replyInsider(string cache1, byte[] cache2) {
            replyEx(cache1, cache2, this);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void SendData(byte[] data) {
            gameData.SendData(data);
        }

        /// <summary>
        /// 此用户数据传输
        /// </summary>
        public GameData gameData;

    }

}
