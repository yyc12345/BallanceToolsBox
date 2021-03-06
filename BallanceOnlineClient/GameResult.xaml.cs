﻿using System;
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
using BallanceOnline;

namespace BallanceOnlineClient {
    /// <summary>
    /// GameResult.xaml 的交互逻辑
    /// </summary>
    public partial class GameResult : Window {

        public GameResult() {
            InitializeComponent();
        }

        GlobalManager gm;

        public void Show(GlobalManager oldgm) {
            gm = oldgm;
            gm.ChangeTransportWindow("GameResult");

            //set team name and shadow
            uiTeamAName.Text = gm.ms.TeamAName;
            uiTeamBName.Text = gm.ms.TeamBName;

            uiGameMapName.Text = gm.ms.MapName;
            switch (gm.ms.GameMode) {
                case BallanceOnline.Const.GameMode.RankedRace:
                    uiGameMode.Text = "排位赛";
                    break;
                case BallanceOnline.Const.GameMode.RelayRace:
                    uiGameMode.Text = "接力赛";
                    break;
            }
            switch (gm.ms.CountMode) {
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

            //以talk模式拦截
            gm.kh.UnHook();
            gm.kh.SetHook(false);

            gm.GameResult_allPlayerData = new Action<StringGroup>(allPlayerData);

            this.Show();
        }

        public void allPlayerData(StringGroup data) {

            //split player
            var playerSplit = data.ToStringGroup();


            //先取出组信息
            var groupMsg = new StringGroup(playerSplit[0], "#").ToStringGroup();
            if (groupMsg[0] == gm.ms.TeamAName) {
                uiTeamAMark.Dispatcher.Invoke(() => { uiTeamAMark.Text = groupMsg[1]; });
                uiTeamBMark.Dispatcher.Invoke(() => { uiTeamBMark.Text = groupMsg[4]; });
                uiTeamAPP.Dispatcher.Invoke(() => { uiTeamAPP.Text = groupMsg[2]; });
                uiTeamBPP.Dispatcher.Invoke(() => { uiTeamBPP.Text = groupMsg[5]; });
            } else {
                uiTeamAMark.Dispatcher.Invoke(() => { uiTeamAMark.Text = groupMsg[4]; });
                uiTeamBMark.Dispatcher.Invoke(() => { uiTeamBMark.Text = groupMsg[1]; });
                uiTeamAPP.Dispatcher.Invoke(() => { uiTeamAPP.Text = groupMsg[5]; });
                uiTeamBPP.Dispatcher.Invoke(() => { uiTeamBPP.Text = groupMsg[2]; });
            }


            int index = 0;
            foreach (string item in playerSplit) {
                if (index == 0) { index++; continue; }

                //split data
                var dataSplit = new StringGroup(item, "#").ToStringGroup();

                //process untiData
                var cache1 = new StringGroup(dataSplit[0], "@").ToStringGroup();
                var unitData = new List<PlayerUnitData>();

                foreach (string item2 in cache1) {
                    var cache2 = new StringGroup(item2, "%").ToStringGroup();
                    unitData.Add(new PlayerUnitData { Unit = cache2[3], Mark = cache2[0], Life = cache2[1], PerfomancePoint = cache2[2] });
                }

                foreach (Player item3 in gm.gamePlayerList) {
                    if (item3.PlayerName == dataSplit[4]) {
                        item3.PlayerUnitPrize = unitData;
                        item3.FinallyMark = dataSplit[1];
                        item3.FinallyPP = dataSplit[2];
                        item3.FinallyPrize = dataSplit[3];
                    }
                }

                index++;
            }

            //input to list
            var teamAList = new List<Player>();
            var teamBList = new List<Player>();

            var result = from i in gm.gamePlayerList
                         where i.PlayerGroupName != ""
                         group i by i.PlayerGroupName;

            foreach (IGrouping<string, Player> item in result) {
                if (item.Key == gm.ms.TeamAName) {
                    foreach (Player item2 in item) {
                        teamAList.Add(item2);
                    }
                } else {
                    foreach (Player item2 in item) {
                        teamBList.Add(item2);
                    }
                }
            }

            //show
            uiTeamAList.Dispatcher.Invoke(() => { uiTeamAList.ItemsSource = teamAList;});
            uiTeamBList.Dispatcher.Invoke(() => { uiTeamBList.ItemsSource = teamBList; });

            uiNewPlay.Dispatcher.Invoke(() => { uiNewPlay.IsEnabled = true; });
            uiExit.Dispatcher.Invoke(() => { uiExit.IsEnabled = true;});

        }

        private void uiNewPlay_Click(object sender, RoutedEventArgs e) {
            gm.kh.UnHook();

            var newWin = new Login();
            var newgm = new GlobalManager("Login");
            newgm.SetMonitor(gm.markMonitor, gm.lifeMonitor, gm.unitMonitor);
            newWin.Show(newgm);

            this.Close();
        }

        private void uiExit_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("在线对战平台会退出，但需要手动关闭Ballance");
            Environment.Exit(0);
        }

    }
}
