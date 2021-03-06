﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace BallanceOnline {

    /// <summary>
    /// 游戏数据传输
    /// </summary>
    public class GameData : IDisposable {

        /// <summary>
        /// 链接器
        /// </summary>
        protected TcpClient client;
        /// <summary>
        /// 处理新数据的委托，委托到外部
        /// </summary>
        protected Action<string, byte[]> ReplyNewMessage;
        /// <summary>
        /// 停止操作
        /// </summary>
        protected bool stopReadFlag;
        /// <summary>
        /// 写入写出的流对象
        /// </summary>
        protected NetworkStream stream;

        /// <summary>
        /// 初始化实例并开启侦测
        /// </summary>
        /// <param name="inputClient"></param>
        /// <param name="replay"></param>
        public GameData(ref TcpClient inputClient, Action<string, byte[]> replay) {
            client = inputClient;
            stream = inputClient.GetStream();
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

        public void SendData(byte[] data) {
            int length = data.Length;
            stream.Write(BitConverter.GetBytes(length), 0, 4);
            stream.Write(data, 0, length);
        }

        public void Dispose() {
            stopReadFlag = true;
            client.Close();
        }


    }

}
