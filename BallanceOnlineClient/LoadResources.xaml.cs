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

        List<PlayerResourcesListItem> teamAList;
        List<PlayerResourcesListItem> teamBList;

        public void Show(GlobalManager oldgm) {
            gm = oldgm;
            gm.ChangeTransportWindow("LoadResources");

            gm.LoadResources_addPlayerInformation = new Action<StringGroup>(addPlayerInformation);
            gm.LoadResources_singlePlayerReady = new Action<string>(singlePlayerReady);
            gm.LoadResources_turnToNewWindow = new Action(turnToNewWindow);

            teamAList = new List<PlayerResourcesListItem>();
            teamBList = new List<PlayerResourcesListItem>();

            //以talk模式拦截-拦过了，继承拦截
            //gm.kh.SetHook(false);

            this.Show();
        }

        public void addPlayerInformation(StringGroup playerInfo) {
            var cache1 = playerInfo.ToStringGroup();

            List<string> cache2 = new StringGroup(cache1[5], "#").ToList();
            List<string> cache3 = new StringGroup(cache1[7], "#").ToList();

            //add to core
            gm.gamePlayerList.Add(new Player {
                PlayerName = cache1[0],
                ModList = cache3,
                BackgroundName = cache1[8],
                BGMName = cache1[9],
  
                DutyUnit = cache2,
                PlayerGroupName = cache1[6]
            });

            if (gm.ms.MapName == "") {
                gm.ms.MapName = cache1[1];
                gm.ms.MapMD5 = cache1[2];
                gm.ms.GameMode = cache1[4];
                gm.ms.CountMode = cache1[3];
            }

            //add to ui
            if (uiGameMapName.Text == "") {
                uiGameMapName.Text = cache1[1];
            }
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

            //true=a false=b
            bool selectTeam = true;
            if (uiTeamAName.Text == "") {
                uiTeamAName.Text = cache1[6];
                selectTeam = true;
            } else if (uiTeamAName.Text == cache1[6]) {
                selectTeam = true;
            } else if (uiTeamBName.Text == "") {
                uiTeamBName.Text = cache1[6];
                selectTeam = false;
            } else {
                selectTeam = false;
            }

            if (selectTeam) {
                uiTeamAList.ItemsSource = null;
                teamAList.Add(new PlayerResourcesListItem { name = cache1[0], duty = new StringGroup(cache1[5], "#").ToNewSplitWord(",") });
                uiTeamAList.ItemsSource = teamAList;
            } else {
                uiTeamBList.ItemsSource = null;
                teamBList.Add(new PlayerResourcesListItem { name = cache1[0], duty = new StringGroup(cache1[5], "#").ToNewSplitWord(",") });
                uiTeamBList.ItemsSource = teamBList;
            }

        }

        public void singlePlayerReady(string playerName) {
            foreach (PlayerResourcesListItem item in teamAList) {
                if (item.name == playerName) {
                    uiTeamAList.ItemsSource = null;
                    item.playerIsReady();
                    uiTeamAList.ItemsSource = teamAList;
                    return;
                }
            }
            foreach (PlayerResourcesListItem item in teamBList) {
                if (item.name == playerName) {
                    uiTeamBList.ItemsSource = null;
                    item.playerIsReady();
                    uiTeamBList.ItemsSource = teamBList;
                    return;
                }
            }
        }

        public void turnToNewWindow() {
            var newWin = new PlayNow();
            newWin.Show(gm, uiTeamAName.Text, uiTeamBName.Text, teamAList, teamBList);

            this.Close();
        }

    }


    public class PlayerResourcesListItem {

        public PlayerResourcesListItem() {
            readyColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            life = "3";
            time = "1000";
            state = "正在游戏";
        }

        public SolidColorBrush readyColor { get; set; }
        public string name { get; set; }
        public string duty { get; set; }

        public void playerIsReady() {
            readyColor = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
        }

        public string life { get; set; }
        public string time { get; set; }
        public string state { get; set; }
        public string unit { get; set; }
    }

}
