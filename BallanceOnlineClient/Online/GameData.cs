using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using BallanceOnline;
using System.Windows.Forms;
using System.Threading;

namespace BallanceOnlineClient.Online {

    /// <summary>
    /// 游戏数据传输
    /// </summary>
    public class GameData : IDisposable {

        /// <summary>
        /// 链接器
        /// </summary>
        private TcpClient client;
        /// <summary>
        /// 处理新数据的委托，委托到外部
        /// </summary>
        private Action<string, byte[]> ReplyNewMessage;
        /// <summary>
        /// 停止操作
        /// </summary>
        private bool stopReadFlag;
        /// <summary>
        /// 写入写出的流对象
        /// </summary>
        private NetworkStream stream;

        public GameData(ref TcpClient inputClient, Action<string, byte[]> replay) {
            client = inputClient;
            ReplyNewMessage = new Action<string, byte[]>(replay);
            stopReadFlag = false;

            Task.Run(() =>
            {

                while (true) {
                    if (stopReadFlag == true) { break; }

                    //读取数据长度
                    byte[] buffer = new byte[4];
                    stream.Read(buffer, 0, 4);

                    int length = BitConverter.ToInt32(buffer, 0);

                    //读取正文
                    buffer = new byte[length];
                    stream.Read(buffer, 0, length);

                    string head;
                    byte[] contant;

                    BallanceOnline.CombineAndSplitSign.Split(buffer, out head, out contant);

                    //调用委托处理
                    ReplyNewMessage(head, contant);
                }

            });

        }

        public void Dispose() {
            stopReadFlag = true;
        }


    }

}
