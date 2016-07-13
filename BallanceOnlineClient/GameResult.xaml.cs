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

            //以talk模式拦截-拦过了，继承拦截
            //gm.kh.SetHook(false);

            gm.GameResult_allPlayerData = new Action<StringGroup>(allPlayerData);

            this.Show();
        }

        public void allPlayerData(StringGroup data) {

        }

    }
}
