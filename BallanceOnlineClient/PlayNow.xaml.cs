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

namespace BallanceOnlineClient {
    /// <summary>
    /// PlayNow.xaml 的交互逻辑
    /// </summary>
    public partial class PlayNow : Window {
        public PlayNow() {
            InitializeComponent();
        }

        GlobalManager gm;

        public void Show(GlobalManager oldgm, string teamAName, string teamBName) {
            gm = oldgm;
            gm.ChangeTransportWindow("PlayNow");

            //set team name and shadow
            uiTeamAName.Text = teamAName;
            uiTeamBName.Text = teamBName;

            string myself = "";
            foreach (Player item in gm.gamePlayerList) {
                if (item.PlayerName == gm.gameSettings.playerName) { myself = item.PlayerGroupName; break; }
            }

            if (myself == teamAName) {
                uiTeamAShadow.Color = Color.FromArgb(255, 0, 0, 255);
                uiTeamBShadow.Color = Color.FromArgb(255, 255, 0, 0);
            }else {
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

            this.Show();
        }


        public void inputPlayerData(StringGroup input) {

        }

        public void playerDied(string name) {

        }

        public void playerPaused(string name) {

        }

        public void playerSuccess(string name) {

        }

        public void teamDied(string teamName) {

        }

        public void turnToNewWindow() {

        }

    }

    public class NowPlayerListItem {
        public string name { get; set; }
        public string life { get; set; }
        public string time { get; set; }
        public string state { get; set; }
    }

}
