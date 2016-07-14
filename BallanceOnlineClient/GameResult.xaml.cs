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

            foreach (string item in playerSplit) {

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
                        item3.FinallyResult = dataSplit[1];
                        item3.FinallyPP = dataSplit[2];
                        item3.FinallyPrize = dataSplit[3];
                    }
                }
            }

            //input to list
            var teamAList = new List<Player>();
            var teamBList = new List<Player>();

            var result = from i in gm.gamePlayerList
                         where i.PlayerGroupName != ""
                         group i by i.PlayerGroupName;

            foreach (IGrouping<string, Player> item in result) {
                if (item.Key == uiTeamAName.Text) {
                    foreach (Player item2 in item) {
                        teamAList.Add(item2);
                    }
                } else {
                    foreach (Player item2 in item) {
                        teamBList.Add(item2);
                    }
                }
            }

            //show team result
            int teamAPP = 0, teamBPP = 0;
            foreach (var item in teamAList) {
                teamAPP += int.Parse(item.FinallyPP);
            }
            teamAPP = teamAPP / teamAList.Count;
            foreach (var item in teamBList) {
                teamBPP += int.Parse(item.FinallyPP);
            }
            teamBPP = teamBPP / teamBList.Count;

            string teamAMark, teamBMark;
            if (gm.ms.CountMode == BallanceOnline.Const.CountMode.SpeedRun || gm.ms.CountMode == BallanceOnline.Const.CountMode.CrazySpeedRun) {
                var markAList = from item in teamAList
                                select DateToSecound(item.FinallyResult);
                var markBList = from item in teamBList
                                select DateToSecound(item.FinallyResult);

                teamAMark = SecoundToDate(Average(markAList, teamAList.Count));
                teamBMark = SecoundToDate(Average(markBList, teamBList.Count));
            } else {
                var markAList = from item in teamAList
                                select item.FinallyResult;
                var markBList = from item in teamBList
                                select item.FinallyResult;

                teamAMark = SecoundToDate(Average(markAList, teamAList.Count));
                teamBMark = SecoundToDate(Average(markBList, teamBList.Count));
            }

            uiTeamAMark.Text = teamAMark;
            uiTeamBMark.Text = teamBMark;
            uiTeamAPP.Text = teamAPP.ToString();
            uiTeamBPP.Text = teamBPP.ToString();


            //show
            uiTeamAList.ItemsSource = teamAList;
            uiTeamBList.ItemsSource = teamBList;

            uiNewPlay.IsEnabled = true;
            uiExit.IsEnabled = true;

        }

        private void uiNewPlay_Click(object sender, RoutedEventArgs e) {
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


        private string Average(IEnumerable<string> list, int count) {
            int sum = 0;
            foreach (string item in list) {
                sum += int.Parse(item);
            }

            return (sum / count).ToString();
        }

        public string DateToSecound(string date) {
            var result = date.Split(':');
            return (int.Parse(result[0]) * 60 + int.Parse(result[1])).ToString();
        }

        public string SecoundToDate(string secound) {
            int hour = int.Parse(secound) / 60;
            int _secound = int.Parse(secound) % 60;
            return hour.ToString() + ":" + _secound.ToString();
        }
        //todo:把组的评判移到服务器
    }
}
