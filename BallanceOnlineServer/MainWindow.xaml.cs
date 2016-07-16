using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using BallanceOnline;
using BallanceOnline.Const;

namespace BallanceOnlineServer {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        GlobalManager gm;

        TcpListener globalListener;

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            gm = new GlobalManager();

            var selectPort = new Random().Next(6000, 40000);
            uiPort.Text = selectPort.ToString();

            var localName = System.Net.Dns.GetHostName();
            uiIP.Text = System.Net.Dns.GetHostByName(localName).AddressList.GetValue(0).ToString();

            //开始监听
            globalListener = new TcpListener(IPAddress.Any, selectPort);

            globalListener.Start();
            globalListener.BeginAcceptTcpClient(new AsyncCallback(acceptCallback), globalListener);
   
        }

        /// <summary>
        /// 接受链接的函数
        /// </summary>
        /// <param name="ar"></param>
        private void acceptCallback(IAsyncResult ar) {

            TcpListener listen = (TcpListener)ar.AsyncState;

            //get client
            TcpClient client = listen.EndAcceptTcpClient(ar);

            //add to player list
            var newPlayer = new Player(ref client, gm.MainMessageProcess);
            //boradcast
            gm.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.NewPlayer, newPlayer.PlayerIPAddress));
            //single user boradcast
            var cache = new List<string>();
            cache.Add(newPlayer.PlayerIPAddress);

            if (gm.clientPlayerList.Count == 0) { return; }
            foreach (Player item in gm.clientPlayerList) {
                cache.Add(item.PlayerIPAddress);
            }
            //send
            newPlayer.gameData.SendData(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.ReturnAllPlayer, new StringGroup(cache, ",").ToString()));
            //add to list
            gm.clientPlayerList.Add(newPlayer);

            if (gm.clientPlayerList.Count >= 2) {
                uiNext.IsEnabled = true;
            }

            //开始下一次
            listen.BeginAcceptTcpClient(new AsyncCallback(acceptCallback), globalListener);

        }

        /// <summary>
        /// 用户下线
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="host"></param>
        private void pingOut(string ip,Player host) {
            uiPlayerList.ItemsSource = null;
            gm.clientPlayerList.Remove(host);
            gm.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.DeletePlayer, ip));
            uiPlayerList.ItemsSource = gm.clientPlayerList;
        }

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiNext_Click(object sender, RoutedEventArgs e) {
            //让客户端切换
            gm.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.OrderTurnIn, ""));
            //自身切换
            var newWin = new PlayerMark();
            newWin.Show(gm);

            this.Close();
        }


    }

}
