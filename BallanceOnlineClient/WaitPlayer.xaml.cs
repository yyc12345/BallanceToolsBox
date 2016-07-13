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
using System.Net.Sockets;
using BallanceOnline;

namespace BallanceOnlineClient {
    /// <summary>
    /// WaitPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class WaitPlayer : Window {
        public WaitPlayer() {
            InitializeComponent();
        }

        GlobalManager gm;

        List<WaitPlayerListItem> playerList;
        List<WaitPlayerListItem> talkList;

        public void Show(GlobalManager oldgm, ref TcpClient tc) {
            gm = oldgm;
            gm.ChangeTransportWindow("WaitPlayer");

            gm.WaitPlayer_addAllPlayer = new Action<StringGroup>(addAllPlayer);
            gm.WaitPlayer_addSinglePlayer = new Action<string>(addSinglePlayer);
            gm.WaitPlayer_newMessage = new Action<string>(newMessage);
            gm.WaitPlayer_turnToNewWindow = new Action(turnToNewWindow);
            //以talk模式拦截-拦过了，继承拦截
            //gm.kh.SetHook(false);
            playerList = new List<WaitPlayerListItem>();
            talkList = new List<WaitPlayerListItem>();

            this.Show();

        }

        public void addAllPlayer(StringGroup pList) {
            uiPlayerList.ItemsSource = null;

            var cache = pList.ToStringGroup();
            foreach (string item in cache) {
                playerList.Add(new WaitPlayerListItem { word = item });
            }

            uiPlayerList.ItemsSource = playerList;
        }

        public void addSinglePlayer(string playerIP) {
            uiPlayerList.ItemsSource = null;

            playerList.Add(new WaitPlayerListItem { word = playerIP });

            uiPlayerList.ItemsSource = playerList;
        }

        public void newMessage(string msg) {
            uiTalkList.ItemsSource = null;

            talkList.Add(new WaitPlayerListItem { word = msg });

            uiTalkList.ItemsSource = playerList;
        }

        public void turnToNewWindow() {
            var newWin = new LoadResources();
            newWin.Show(gm);

            this.Close();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiConnect_Click(object sender, RoutedEventArgs e) {
            if (uiMsg.Text != "")
                gm.dataGiveIn.SendData(CombineAndSplitSign.Combine(BallanceOnline.Const.ClientAndServerSign.Client, BallanceOnline.Const.SocketSign.Message, uiMsg.Text));
            else MessageBox.Show("发送的消息不能为空");
        }

    }

    public class WaitPlayerListItem {
        public string word { get; set; }
    }
}
