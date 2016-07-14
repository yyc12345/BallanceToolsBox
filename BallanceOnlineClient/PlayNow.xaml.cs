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

        List<PlayerResourcesListItem> teamAList;
        List<PlayerResourcesListItem> teamBList;

        bool stopTurnIn;

        public void Show(GlobalManager oldgm, string teamAName, string teamBName, List<PlayerResourcesListItem> teamA, List<PlayerResourcesListItem> teamB) {
            gm = oldgm;
            gm.ChangeTransportWindow("PlayNow");

            //set team name and shadow
            uiTeamAName.Text = teamAName;
            uiTeamBName.Text = teamBName;

            teamAList = teamA;
            teamBList = teamB;

            string myself = "";
            foreach (Player item in gm.gamePlayerList) {
                if (item.PlayerName == gm.gameSettings.playerName) { myself = item.PlayerGroupName; break; }
            }

            if (myself == teamAName) {
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

            stopTurnIn = false;

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

            uiTeamAList.ItemsSource = null;
            foreach (PlayerResourcesListItem item in teamAList) {
                if (item.name == cache[4]) {
                    item.life = cache[1];
                    item.time = cache[0];
                    item.unit = cache[2];

                    ShowPrize(cache[3], cache[4]);

                    uiTeamAList.ItemsSource = teamAList;
                    return;
                }
            }
            uiTeamAList.ItemsSource = teamAList;
            uiTeamBList.ItemsSource = null;
            foreach (PlayerResourcesListItem item in teamBList) {
                if (item.name == cache[4]) {
                    item.life = cache[1];
                    item.time = cache[0];
                    item.unit = cache[2];

                    ShowPrize(cache[3], cache[4]);

                    uiTeamBList.ItemsSource = teamBList;
                    return;
                }
            }
            uiTeamBList.ItemsSource = teamBList;
        }

        public void playerDied(string name) {

            if (name == gm.gameSettings.playerName) {
                gm.kh.UnHook();
                gm.kh.SetHook(false);
                stopTurnIn = true;
            }

            uiTeamAList.ItemsSource = null;
            foreach (PlayerResourcesListItem item in teamAList) {
                if (name == item.name) {
                    item.state = "已死亡";
                    uiTeamAList.ItemsSource = teamAList;
                    return;
                }
            }
            uiTeamAList.ItemsSource = teamAList;
            uiTeamBList.ItemsSource = null;
            foreach (PlayerResourcesListItem item in teamBList) {
                if (name == item.name) {
                    item.state = "已死亡";
                    uiTeamBList.ItemsSource = teamBList;
                    return;
                }
            }
            uiTeamBList.ItemsSource = teamBList;
        }

        public void playerPaused(string name) {

            if (name == gm.gameSettings.playerName) {
                gm.kh.UnHook();
                gm.kh.SetHook(false);
            }

            uiTeamAList.ItemsSource = null;
            foreach (PlayerResourcesListItem item in teamAList) {
                if (name == item.name) {
                    item.state = "已暂停";
                    uiTeamAList.ItemsSource = teamAList;
                    return;
                }
            }
            uiTeamAList.ItemsSource = teamAList;
            uiTeamBList.ItemsSource = null;
            foreach (PlayerResourcesListItem item in teamBList) {
                if (name == item.name) {
                    item.state = "已暂停";
                    uiTeamBList.ItemsSource = teamBList;
                    return;
                }
            }
            uiTeamBList.ItemsSource = teamBList;
        }

        public void playerContinue(string name) {

            if (name == gm.gameSettings.playerName) {
                gm.kh.UnHook();
                gm.kh.SetHook(true);
            }

            uiTeamAList.ItemsSource = null;
            foreach (PlayerResourcesListItem item in teamAList) {
                if (name == item.name) {
                    item.state = "正在游戏";
                    uiTeamAList.ItemsSource = teamAList;
                    return;
                }
            }
            uiTeamAList.ItemsSource = teamAList;
            uiTeamBList.ItemsSource = null;
            foreach (PlayerResourcesListItem item in teamBList) {
                if (name == item.name) {
                    item.state = "正在游戏";
                    uiTeamBList.ItemsSource = teamBList;
                    return;
                }
            }
            uiTeamBList.ItemsSource = teamBList;
        }

        public void playerSuccess(string name) {

            if (name == gm.gameSettings.playerName) {
                gm.kh.UnHook();
                gm.kh.SetHook(false);
                stopTurnIn = true;
            }

            uiTeamAList.ItemsSource = null;
            foreach (PlayerResourcesListItem item in teamAList) {
                if (name == item.name) {
                    item.state = "已胜利";
                    uiTeamAList.ItemsSource = teamAList;
                    return;
                }
            }
            uiTeamAList.ItemsSource = teamAList;
            uiTeamBList.ItemsSource = null;
            foreach (PlayerResourcesListItem item in teamBList) {
                if (name == item.name) {
                    item.state = "已胜利";
                    uiTeamBList.ItemsSource = teamBList;
                    return;
                }
            }
            uiTeamBList.ItemsSource = teamBList;
        }

        public void teamDied(string teamName) {

            if (teamName == uiTeamAName.Text) {
                uiTeamAList.ItemsSource = null;
                foreach (PlayerResourcesListItem item in teamAList) {
                    if (item.name == gm.gameSettings.playerName) {
                        gm.kh.UnHook();
                        gm.kh.SetHook(false);
                        stopTurnIn = true;
                    }
                    item.state = "已死亡";
                }
                uiTeamAList.ItemsSource = teamAList;
            } else {
                uiTeamBList.ItemsSource = null;
                foreach (PlayerResourcesListItem item in teamBList) {
                    if (item.name == gm.gameSettings.playerName) {
                        gm.kh.UnHook();
                        gm.kh.SetHook(false);
                        stopTurnIn = true;
                    }
                    item.state = "已死亡";
                }
                uiTeamBList.ItemsSource = teamBList;
            }
        }

        public void turnToNewWindow() {
            stopTurnIn = true;

            var newWin = new GameResult();
            newWin.Show(gm, uiTeamAName.Text, uiTeamBName.Text);

            this.Close();
        }

        public void ShowPrize(string prizeName, string playerName) {
            //todo:
        }

    }

    public class NowPlayerListItem {
        public string name { get; set; }
        public string life { get; set; }
        public string time { get; set; }
        public string state { get; set; }
    }

}
