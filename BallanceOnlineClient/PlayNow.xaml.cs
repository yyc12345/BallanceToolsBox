using BallanceOnline;
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
using System.Threading;

namespace BallanceOnlineClient {
    /// <summary>
    /// PlayNow.xaml 的交互逻辑
    /// </summary>
    public partial class PlayNow : Window {
        public PlayNow() {
            InitializeComponent();
        }

        GlobalManager gm;
        List<TalkListItem> talkList;
        /// <summary>
        /// 停止上交表示符
        /// </summary>
        bool stopTurnIn;

        public void Show(GlobalManager oldgm) {
            gm = oldgm;
            gm.ChangeTransportWindow("PlayNow");

            //set team name and shadow
            uiTeamAName.Text = gm.ms.TeamAName;
            uiTeamBName.Text = gm.ms.TeamBName;

            string myself = "";
            foreach (Player item in gm.gamePlayerList) {
                if (item.PlayerName == gm.gameSettings.playerName) { myself = item.PlayerGroupName; break; }
            }

            if (myself == gm.ms.TeamAName) {
                uiTeamAShadow.Color = Color.FromArgb(255, 0, 0, 255);
                uiTeamBShadow.Color = Color.FromArgb(255, 255, 0, 0);
            } else {
                uiTeamAShadow.Color = Color.FromArgb(255, 255, 0, 0);
                uiTeamBShadow.Color = Color.FromArgb(255, 0, 0, 255);
            }

            //以talk模式拦截-拦过了，继承拦截
            //gm.kh.SetHook(false);

            gm.PlayNow_inputPlayerData = new Action<StringGroup>(inputPlayerData);
            gm.PlayNow_playerDied = new Action<string>(playerDied);
            gm.PlayNow_playerPaused = new Action<string>(playerPaused);
            gm.PlayNow_playerSuccess = new Action<string>(playerSuccess);
            gm.PlayNow_teamDied = new Action<string>(teamDied);
            gm.PlayNow_turnToNewWindow = new Action(turnToNewWindow);
            gm.PlayNow_playerContinue = new Action<string>(playerContinue);
            gm.PlayNow_newMessage = new Action<string>(newMessage);

            //show player
            var playerSplit = from item in gm.gamePlayerList
                              where item.PlayerGroupName != ""
                              group item by item.PlayerGroupName;
            foreach (var item in playerSplit) {
                if (item.Key == gm.ms.TeamAName) {
                    uiTeamAList.ItemsSource = item.ToList<Player>();
                } else {
                    uiTeamBList.ItemsSource = item.ToList<Player>();
                }
            }

            stopTurnIn = false;
            talkList = new List<TalkListItem>();

            this.Show();

            Task.Run(async () =>
           {
               uiTimer.Text = "20";

               //显示欢迎来到

               for (int i = 0; i < 5; i++) {
                   Thread.Sleep(1000);
                   uiTimer.Text = (int.Parse(uiTimer.Text) - 1).ToString();
               }

               //此时=15
               //显示负责的关卡

               for (int i = 0; i < 10; i++) {
                   Thread.Sleep(1000);
                   uiTimer.Text = (int.Parse(uiTimer.Text) - 1).ToString();
               }

               //此时=5
               //准备开始

               for (int i = 0; i < 5; i++) {
                   Thread.Sleep(1000);
                   uiTimer.Text = (int.Parse(uiTimer.Text) - 1).ToString();
               }

               //隐藏
               uiTimer.Visibility = Visibility.Collapsed;
               //全军出击

               //提交循环
               while (true) {

                   if (stopTurnIn == true) { break; }

                   long mark = gm.markMonitor.Mode(await gm.markMonitor.ReadDataAsync());
                   long life = gm.lifeMonitor.Mode(await gm.lifeMonitor.ReadDataAsync());
                   long unit = gm.unitMonitor.Mode(await gm.unitMonitor.ReadDataAsync());

                   gm.dataGiveIn.SendData(CombineAndSplitSign.Combine(BallanceOnline.Const.ClientAndServerSign.Client, BallanceOnline.Const.SocketSign.GameDataTurnIn, mark.ToString() + "," + life.ToString() + "," + unit.ToString()));

                   Thread.Sleep(1000);

               }

               //结束

           });

        }


        public void inputPlayerData(StringGroup input) {
            var cache = input.ToStringGroup();

            foreach (Player item in gm.gamePlayerList) {
                if (item.PlayerName == cache[4]) {
                    item.NowLife = cache[1];
                    item.NowTime = cache[0];
                    item.NowUnit = cache[2];

                    ShowPrize(cache[3], cache[4]);
                    return;
                }
            }
        }

        public void playerDied(string name) {

            if (name == gm.gameSettings.playerName) {
                gm.kh.UnHook();
                gm.kh.SetHook(false);
                stopTurnIn = true;
            }

            foreach (Player item in gm.gamePlayerList) {
                if (name == item.PlayerName) {
                    item.NowState = "已死亡";
                    return;
                }
            }

        }

        public void playerPaused(string name) {

            if (name == gm.gameSettings.playerName) {
                gm.kh.UnHook();
                gm.kh.SetHook(false);
            }

            foreach (Player item in gm.gamePlayerList) {
                if (name == item.PlayerName) {
                    item.NowState = "已暂停";
                    return;
                }
            }
        }

        public void playerContinue(string name) {

            if (name == gm.gameSettings.playerName) {
                gm.kh.UnHook();
                gm.kh.SetHook(true);
            }

            foreach (Player item in gm.gamePlayerList) {
                if (name == item.PlayerName) {
                    item.NowState = "正在游戏";
                    return;
                }
            }
        }

        public void playerSuccess(string name) {

            if (name == gm.gameSettings.playerName) {
                gm.kh.UnHook();
                gm.kh.SetHook(false);
                stopTurnIn = true;
            }

            foreach (Player item in gm.gamePlayerList) {
                if (name == item.PlayerName) {
                    item.NowState = "已胜利";
                    return;
                }
            }

        }

        public void teamDied(string teamName) {

            foreach (Player item in gm.gamePlayerList) {
                if (item.PlayerGroupName == teamName) {
                    if (item.PlayerName == gm.gameSettings.playerName) {
                        gm.kh.UnHook();
                        gm.kh.SetHook(false);
                        stopTurnIn = true;
                    }
                    item.NowState = "已死亡";
                }
            }

        }

        public void newMessage(string msg) {
            uiTalkList.ItemsSource = null;

            talkList.Add(new TalkListItem { word = msg });

            uiTalkList.ItemsSource = talkList;
        }

        public void turnToNewWindow() {
            stopTurnIn = true;

            var newWin = new GameResult();
            newWin.Show(gm);

            this.Close();
        }

        public void ShowPrize(string prizeName, string playerName) {
            //todo:
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

    public class NowPlayerListItem {
        public string name { get; set; }
        public string life { get; set; }
        public string time { get; set; }
        public string state { get; set; }
    }

}
