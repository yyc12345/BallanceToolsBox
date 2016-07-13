using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using BallanceOnline;
using System.Windows.Forms;
using System.Threading;

namespace BallanceOnlineClient.Online {

    /// <summary>
    /// 带有超时的tcp链接
    /// </summary>
    public class TcpClientWithTimeout {

        TcpClient client;
        IPAddress globalIP;
        int globalPort;
        bool connected;

        public TcpClientWithTimeout() {
            client = new TcpClient();
            connected = false;
        }

        /// <summary>
        /// 开启一个超时限制的链接
        /// </summary>
        /// <param name="ip">ip</param>
        /// <param name="port">端口</param>
        /// <param name="timeoutNumber">超时时间，毫秒单位</param>
        /// <param name="returnClient">返回的接受的client，超时返回null</param>
        public void Connect(IPAddress ip, int port, int timeoutNumber, out TcpClient returnClient) {
            globalIP = ip;
            globalPort = port;

            Thread th = new Thread(new ThreadStart(TryConnect));
            th.IsBackground = true;
            th.Start();

            //wait
            th.Join(timeoutNumber);

            if (connected == true) {
                //返回对象
                returnClient = client;
                th.Abort();
                return;
            } else {
                returnClient = null;
                th.Abort();
                return;
            }

        }

        private void TryConnect() {
            client.Connect(globalIP, globalPort);
            connected = true;
        }

    }

}
