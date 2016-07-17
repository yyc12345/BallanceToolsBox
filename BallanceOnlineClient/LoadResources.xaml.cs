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
    /// LoadResources.xaml 的交互逻辑
    /// </summary>
    public partial class LoadResources : Window {
        public LoadResources() {
            InitializeComponent();
        }

        GlobalManager gm;

        public void Show(GlobalManager oldgm) {
            gm = oldgm;
            gm.ChangeTransportWindow("LoadResources");

            gm.LoadResources_addPlayerInformation = new Action<StringGroup>(addPlayerInformation);
            gm.LoadResources_singlePlayerReady = new Action<string>(singlePlayerReady);
            gm.LoadResources_turnToNewWindow = new Action(turnToNewWindow);

            //以talk模式拦截-拦过了，继承拦截
            //gm.kh.SetHook(false);

            this.Show();
        }

        public void addPlayerInformation(StringGroup playerInfo) {
            var cache1 = playerInfo.ToStringGroup();

            List<string> cache2 = new StringGroup(cache1[5], "#").ToList();
            List<string> cache3 = new StringGroup(cache1[7], "#").ToList();

            //add to core
            foreach (Player item in gm.gamePlayerList) {
                if (item.PlayerIPAddress == cache1[10]) {
                    item.PlayerName = cache1[0];
                    item.ModList = cache3;
                    item.BackgroundName = cache1[8];
                    item.BGMName = cache1[9];
                    item.DutyUnit = cache2;
                    item.PlayerGroupName = cache1[6];
                }
            }

            if (gm.ms.MapName == "") {
                gm.ms.MapName = cache1[1];
                gm.ms.MapMD5 = cache1[2];
                gm.ms.GameMode = cache1[4];
                gm.ms.CountMode = cache1[3];
            }

            //add team
            if (gm.ms.TeamAName == "") {
                gm.ms.TeamAName = cache1[6];
                uiTeamAName.Dispatcher.Invoke(() => { uiTeamAName.Text = cache1[6]; });
            } else if (gm.ms.TeamBName == "") {
                gm.ms.TeamBName = cache1[6];
                uiTeamBName.Dispatcher.Invoke(() => { uiTeamBName.Text = cache1[6]; });
            } else { }

            //add to ui
            uiGameMapName.Dispatcher.Invoke(() =>
            {
                if (uiGameMapName.Text == "") {
                    uiGameMapName.Text = cache1[1];
                }
            });
            uiGameMode.Dispatcher.Invoke(() =>
            {
                if (uiGameMode.Text == "") {
                    switch (cache1[4]) {
                        case BallanceOnline.Const.GameMode.RankedRace:
                            uiGameMode.Text = "排位赛";
                            break;
                        case BallanceOnline.Const.GameMode.RelayRace:
                            uiGameMode.Text = "接力赛";
                            break;
                    }
                }
            });
            uiGameRule.Dispatcher.Invoke(() =>
            {
                if (uiGameRule.Text == "") {
                    switch (cache1[3]) {
                        case BallanceOnline.Const.CountMode.HighScore:
                            uiGameRule.Text = "HS";
                            break;
                        case BallanceOnline.Const.CountMode.SpeedRun:
                            uiGameRule.Text = "SR";
                            break;
                        case BallanceOnline.Const.CountMode.CrazyHighScore:
                            uiGameRule.Text = "疯狂HS";
                            break;
                        case BallanceOnline.Const.CountMode.CrazySpeedRun:
                            uiGameRule.Text = "疯狂SR";
                            break;

                    }
                }
            });


            //show player
            var playerSplit = from item in gm.gamePlayerList
                              where item.PlayerGroupName != ""
                              group item by item.PlayerGroupName;
            foreach (var item in playerSplit) {
                if (item.Key == gm.ms.TeamAName) {
                    uiTeamAList.Dispatcher.Invoke(() => { uiTeamAList.ItemsSource = item.ToList<Player>();});                 
                } else {
                    uiTeamBList.Dispatcher.Invoke(() => { uiTeamBList.ItemsSource = item.ToList<Player>();});              
                }
            }



        }

        public void singlePlayerReady(string playerName) {

            //search
            foreach (Player item in gm.gamePlayerList) {
                if (item.PlayerName == playerName) {
                    item.playerIsReady();
                    break;
                }
            }

            //show player
            var playerSplit = from item in gm.gamePlayerList
                              where item.PlayerGroupName != ""
                              group item by item.PlayerGroupName;
            foreach (var item in playerSplit) {
                if (item.Key == gm.ms.TeamAName) {
                    uiTeamAList.Dispatcher.Invoke(() =>
                    {
                        uiTeamAList.ItemsSource = null;
                        uiTeamAList.ItemsSource = item.ToList<Player>();
                    });  
                } else {
                    uiTeamBList.Dispatcher.Invoke(() =>
                    {
                        uiTeamBList.ItemsSource = null;
                        uiTeamBList.ItemsSource = item.ToList<Player>();
                    });     
                }
            }

        }

        public void turnToNewWindow() {
            var newWin = new PlayNow();
            newWin.Show(gm);

            this.Close();
        }

    }


    //public class PlayerResourcesListItem {

    //    public PlayerResourcesListItem() {
    //        readyColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
    //        life = "3";
    //        time = "1000";
    //        state = "正在游戏";
    //    }

    //    public SolidColorBrush readyColor { get; set; }
    //    public string name { get; set; }
    //    public string duty { get; set; }

    //    public void playerIsReady() {
    //        readyColor = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
    //    }

    //    public string life { get; set; }
    //    public string time { get; set; }
    //    public string state { get; set; }
    //    public string unit { get; set; }
    //}

}
