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

        List<TalkListItem> talkList;

        public void Show(GlobalManager oldgm, ref TcpClient tc) {
            gm = oldgm;
            gm.ChangeTransportWindow("WaitPlayer");

            //set client
            gm.dataGiveIn = new GameData(ref tc, gm.dataProcess);

            gm.WaitPlayer_addAllPlayer = new Action<StringGroup>(addAllPlayer);
            gm.WaitPlayer_addSinglePlayer = new Action<string>(addSinglePlayer);
            gm.WaitPlayer_newMessage = new Action<string>(newMessage);
            gm.WaitPlayer_turnToNewWindow = new Action(turnToNewWindow);
            //以talk模式拦截-拦过了，继承拦截
            //gm.kh.SetHook(false);
            uiPlayerList.ItemsSource = gm.gamePlayerList;
            talkList = new List<TalkListItem>();
            uiTalkList.ItemsSource = talkList;

            this.Show();

        }

        public void addAllPlayer(StringGroup pList) {

            uiPlayerList.Dispatcher.Invoke(() => {
                uiPlayerList.ItemsSource = null;

                var cache = pList.ToStringGroup();
                foreach (string item in cache) {
                    gm.gamePlayerList.Add(new Player { PlayerIPAddress = item });
                }

                uiPlayerList.ItemsSource = gm.gamePlayerList;

            });

        }

        public void addSinglePlayer(string playerIP) {
            uiPlayerList.Dispatcher.Invoke(() => {
                uiPlayerList.ItemsSource = null;

                gm.gamePlayerList.Add(new Player { PlayerIPAddress = playerIP });

                uiPlayerList.ItemsSource = gm.gamePlayerList;

            });

        }

        public void deletePlayer(string playerIP) {
            uiPlayerList.Dispatcher.Invoke(() => {
                uiPlayerList.ItemsSource = null;

                int index = 0;
                foreach (Player item in gm.gamePlayerList) {
                    if (item.PlayerIPAddress == playerIP) { gm.gamePlayerList.RemoveAt(index); break; }
                    index++;
                }

                uiPlayerList.ItemsSource = gm.gamePlayerList;

            });

        }

        public void newMessage(string msg) {

            uiTalkList.Dispatcher.Invoke(() => {
                uiTalkList.ItemsSource = null;
                talkList.Add(new TalkListItem { word = msg });
                uiTalkList.ItemsSource = talkList;
            });

            uiQuickMsgText.Dispatcher.Invoke(() => {
                uiQuickMsgText.Text = msg;
            });

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

        /// <summary>
        /// 隐藏消息面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiMsg_LostFocus(object sender, RoutedEventArgs e) {
            uiTalkList.Visibility = Visibility.Collapsed;

            uiQuickMsg.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 显示消息面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiMsg_GotFocus(object sender, RoutedEventArgs e) {
            uiTalkList.Visibility = Visibility.Visible;

            uiQuickMsg.Visibility = Visibility.Collapsed;
        }
    }

}

public class TalkListItem {
    public string word { get; set; }
}
