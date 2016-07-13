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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BallanceOnlineClient.Local;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualBasic;

namespace BallanceOnlineClient {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();

            bool flag = false;
            Process myself = null;
            foreach (Process p in Process.GetProcesses()) {
                if (p.ProcessName == "Player") { flag = true; }
                if (p.ProcessName == "BallanceOnlineClient") { myself = p; }
            }

            if (flag == false) {
                MessageBox.Show("必须在开启Ballance的情况下再运行此程序");
                Environment.Exit(1);
            }

        }
        GlobalManager gm;

        CheatEngine mark;
        CheatEngine life;
        CheatEngine unit;

        /// <summary>
        /// 0=mark 1=life 2=unit
        /// </summary>
        int measureProgress;
        /// <summary>
        /// 输入数据的次数
        /// </summary>
        int inputTick;
        /// <summary>
        /// 即将退出
        /// </summary>
        bool willExit;

        /// <summary>
        /// 窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            gm = new GlobalManager("SearchMemory");

            //set map
            System.IO.File.Delete(gm.gameSettings.rootFolder + @"3D Entities\Level\Level_01.NMO");
            System.IO.File.Copy(Environment.CurrentDirectory + @"\TestMap.nmo", gm.gameSettings.rootFolder + @"3D Entities\Level\Level_01.NMO");

            //init
            mark = new CheatEngine();
            life = new CheatEngine();
            unit = new CheatEngine();

            measureProgress = 0;
            inputTick = 0;
            willExit = false;

            uiTestStep.Text = "请打开游戏第一关，按下Q跳过教程，等待2-3秒，按下Esc键，将左下角分数填入下部文本框中，按确认键";
        }

        /// <summary>
        /// 数据输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e) {

            //即将退出，先判定
            if (willExit == true) {
                gm.SetMonitor(mark, life, unit);
                var newWin = new Login();
                newWin.Show(gm);

                this.Hide();
            }

            long data;
            try {
                long.TryParse(uiData.Text, out data);
            } catch (Exception ex) {
                MessageBox.Show("输入数字格式错误");
                uiData.Text = "";
                return;
            }

            //显示
            uiWait.Visibility = Visibility.Visible;

            if (inputTick == 0) {

                switch (measureProgress) {
                    case 0:
                        //mark
                        mark.StartSearching();
                        mark.SearchedData = data;
                        await mark.FirstSearchAsync();

                        uiTestStep.Text = "第" + (inputTick + 2).ToString() + "次查找：返回至游戏，等待2-3秒，再按下Esc键，将左下角分数填入下部文本框中，按确认键";

                        break;
                    case 1:
                        //life
                        life.StartSearching();
                        life.SearchedData = data;
                        await life.FirstSearchAsync();

                        uiTestStep.Text = "第" + (inputTick + 2).ToString() + "次查找：返回至游戏，死亡一次，再按下Esc键，将右下角生命数填入下部文本框中，按确认键";

                        break;
                    case 2:
                        //unit
                        unit.StartSearching();
                        unit.SearchedData = data;
                        await unit.FirstSearchAsync();

                        uiTestStep.Text = "第" + (inputTick + 2).ToString() + "次查找：返回至游戏，按住前进键，通过一个存盘点后，立即再按下Esc键，将当前小节数数填入下部文本框中，按确认键。提示：当前小节数可能为：" +
                            (inputTick + 2).ToString();

                        break;
                }

                inputTick += 1;

            } else {

                switch (measureProgress) {
                    case 0:
                        //mark
                        mark.SearchedData = data;
                        await mark.SearchAgainAsync();

                        if (mark.ResultCount <= 10) {
                            //可以结束了
                            inputTick = 0;
                            measureProgress += 1;
                            uiMarkMeasure.Background = new SolidColorBrush(Color.FromArgb(80, 0, 255, 0));
                            uiTestStep.Text = "请直接输入右下角生命的个数到下部文本框中，按确认键";
                        } else {
                            uiTestStep.Text = "第" + (inputTick + 2).ToString() + "次查找：返回至游戏，等待2-3秒，再按下Esc键，将左下角分数填入下部文本框中，按确认键";
                            inputTick += 1;
                        }


                        break;
                    case 1:
                        //life
                        life.SearchedData = data;
                        await life.SearchAgainAsync();

                        if (life.ResultCount <= 10) {
                            //可以结束了
                            inputTick = 0;
                            measureProgress += 1;
                            uiLifeMeasure.Background = new SolidColorBrush(Color.FromArgb(80, 0, 255, 0));
                            uiTestStep.Text = "请直接输入当前小节数到下部文本框中，按确认键。提示：当前小节数可能为：1";
                        } else {
                            uiTestStep.Text = "第" + (inputTick + 2).ToString() + "次查找：返回至游戏，死亡一次，再按下Esc键，将右下角生命数填入下部文本框中，按确认键";
                            inputTick += 1;
                        }

                        break;
                    case 2:
                        //unit
                        unit.SearchedData = data;
                        await unit.SearchAgainAsync();

                        if (inputTick + 2 == 6) {
                            //超限了。。直接结束，后面用众数解决
                            uiUnitMeasure.Background = new SolidColorBrush(Color.FromArgb(80, 0, 255, 0));
                            uiTestStep.Text = "请按Esc退出直至退出至初始菜单，退出完成后按下部确认键";
                            willExit = true;
                        }

                        if (life.ResultCount <= 10) {
                            //可以结束了
                            uiUnitMeasure.Background = new SolidColorBrush(Color.FromArgb(80, 0, 255, 0));
                            uiTestStep.Text = "请按Esc退出直至推出至初始菜单，退出完成后按下部确认键";
                            willExit = true;
                        } else {
                            uiTestStep.Text = "第" + (inputTick + 2).ToString() + "次查找：返回至游戏，按住前进键，通过一个存盘点后，立即再按下Esc键，将当前小节数数填入下部文本框中，按确认键。提示：当前小节数可能为：" +
                            (inputTick + 2).ToString();
                            inputTick += 1;
                        }

                        break;
                }

            }

            uiData.Text = "";
            uiWait.Visibility = Visibility.Collapsed;

        }
    }
}
