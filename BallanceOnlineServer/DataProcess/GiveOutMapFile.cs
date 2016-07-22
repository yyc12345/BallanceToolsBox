using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BallanceOnline;

namespace BallanceOnlineServer.DataProcess {
    /// <summary>
    /// 分发地图类
    /// </summary>
    public class GiveOutMapFile {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <param name="outAction">将读取的数据发送出去的委托</param>
        public GiveOutMapFile(string path, Action<byte[]> outAction) {
            fr = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            outPut = new Action<byte[]>(outAction);
        }

        Action<byte[]> outPut;

        FileStream fr;

        /// <summary>
        /// 一次读取字节数
        /// </summary>
        private const int readBlockCount = 1000;

        public void Start() {

            //通知玩家
            outPut(CombineAndSplitSign.Combine(BallanceOnline.Const.ClientAndServerSign.Server, BallanceOnline.Const.SocketSign.StartDownloadingMap, ""));

            //读取循环
            while (true) {

                //先读取那么多
                byte[] cacheChar = new byte[readBlockCount];
                fr.Read(cacheChar, 0, readBlockCount);

                //如果为0，没有内容，结束读取
                if (cacheChar.Length == 0) {
                    outPut(CombineAndSplitSign.Combine(BallanceOnline.Const.ClientAndServerSign.Server, BallanceOnline.Const.SocketSign.EndDownloadingMap, ""));
                    break;
                }

                //读取数量不足，说明也到文件尾了
                if (cacheChar.Length < readBlockCount) {
                    outPut(CombineAndSplitSign.Combine(BallanceOnline.Const.ClientAndServerSign.Server, BallanceOnline.Const.SocketSign.DownloadingMapData, cacheChar));
                    outPut(CombineAndSplitSign.Combine(BallanceOnline.Const.ClientAndServerSign.Server, BallanceOnline.Const.SocketSign.EndDownloadingMap, ""));
                    break;
                } else {
                    //继续读取
                    outPut(CombineAndSplitSign.Combine(BallanceOnline.Const.ClientAndServerSign.Server, BallanceOnline.Const.SocketSign.DownloadingMapData, cacheChar));
                }
            }

            fr.Close();
            fr.Dispose();

        }

    }
}
