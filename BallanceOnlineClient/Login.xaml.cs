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
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using BallanceOnlineClient.Online;

namespace BallanceOnlineClient {
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window {
        public Login() {
            InitializeComponent();
        }

        GlobalManager gm;

        public void Show(GlobalManager oldgm) {
            gm = oldgm;
            gm.ChangeTransportWindow("Login");
            //以talk模式拦截
            gm.kh.SetHook(false);

            this.Show();
        }

        /// <summary>
        /// 链接服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) {
            if (uiPort.Text != "" && uiIP.Text != "") {

                IPAddress serverIp;
                int serverPort;

                try {
                    int.TryParse(uiPort.Text, out serverPort);
                    IPAddress.TryParse(uiIP.Text, out serverIp);
                } catch (Exception ex) {
                    MessageBox.Show("请确认输入字符正确");
                    return;
                }

                TcpClient getClient;
                uiConnect.Content = "正在连接";
                uiConnect.IsEnabled = false;
                gm.tcpConnect = new TcpClientWithTimeout();
                gm.tcpConnect.Connect(serverIp, serverPort, 5000, out getClient);

                if (getClient == null) {
                    //fail
                    uiConnect.IsEnabled = true;
                    uiConnect.Content = "连接";

                    MessageBox.Show("连接失败，请确认服务器在线或本计算机在线");
                } else {
                    //success
                    //切换窗口
                    var newWin = new WaitPlayer();
                    newWin.Show(gm, ref getClient);

                    this.Close();
                }

            }

        }
    }
}
