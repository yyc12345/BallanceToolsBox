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
using BallanceOnline.Const;

namespace BallanceOnlineServer {
    /// <summary>
    /// PlayerMark.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerMark : Window {

        public PlayerMark() {
            InitializeComponent();
        }


        GlobalManager gm;

        /// <summary>
        /// 当前进行的步骤0=确认地图 1=确认任务 2=正在游戏 3=输出结果
        /// </summary>
        int nowStep;

        public void Show(GlobalManager oldgm) {
            gm = oldgm;

            //set resources
            uiPlayerList.ItemsSource = gm.clientPlayerList;
            nowStep = 0;

            this.Show();
        }

        #region 确认地图

        /// <summary>
        /// 选择地图文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) {

            if (nowStep != 0) { return; }

            var fileOpen = new System.Windows.Forms.OpenFileDialog();
            fileOpen.Filter = "Ballance地图文件|*.nmo";

            if (fileOpen.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                uiMapPath.Text = fileOpen.FileName;
            }

            //md5
            uiMapMD5.Text = BallanceOnline.MD5.GetMD5HashFromFile(fileOpen.FileName);

        }

        /// <summary>
        /// 确认地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiGSetMap_Click(object sender, RoutedEventArgs e) {

            if (nowStep != 0) { return; }

            if (uiMapPath.Text != "" && uiGroupAName.Text != "" && uiGroupBName.Text != "") {

                gm.ms.TeamAName = uiGroupAName.Text;
                gm.ms.TeamBName = uiGroupBName.Text;

                //set map name
                var cache = new StringGroup(uiMapPath.Text, @"\").ToStringGroup();
                string cache2 = cache[cache.Length - 1];

                cache2 = cache2.Replace(",", "");
                cache2 = cache2.Replace("#", "");
                cache2 = cache2.Replace("@", "");
                cache2 = cache2.Replace("%", "");

                if (cache2.Length == 0) {
                    cache2 = "UnknowMap";
                }

                gm.ms.MapName = cache2;

                gm.ms.MapMD5 = uiMapMD5.Text;
                switch (uiGameMode.SelectedIndex) {
                    case 0:
                        gm.ms.GameMode = GameMode.RankedRace;
                        break;
                    case 1:
                        gm.ms.GameMode = GameMode.RelayRace;
                        break;
                }
                switch (uiCountMode.SelectedIndex) {
                    case 0:
                        gm.ms.CountMode = CountMode.HighScore;
                        break;
                    case 1:
                        gm.ms.CountMode = CountMode.SpeedRun;
                        break;
                    case 2:
                        gm.ms.CountMode = CountMode.CrazyHighScore;
                        break;
                    case 3:
                        gm.ms.CountMode = CountMode.CrazySpeedRun;
                        break;
                }

                //lock contorl
                uiGroupAName.IsReadOnly = true;
                uiGroupBName.IsReadOnly = true;

                uiMapPath.IsReadOnly = true;
                uiSelectMap.IsEnabled = false;

                uiSetMap.IsEnabled = false;
                uiGiveOutTask.IsEnabled = true;

                nowStep++;
            }

        }

        #endregion

        #region 确认任务

        /// <summary>
        /// 确定选择player的属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiPlayerList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

            if (nowStep != 1) { return; }
            if (uiPlayerList.SelectedIndex == -1) { return; }

            //set duty
            if (gm.clientPlayerList[uiPlayerList.SelectedIndex].DutyUnitCount != 0)
                uiSetDuty.Text = gm.clientPlayerList[uiPlayerList.SelectedIndex].DutyUnitToString;
            else uiSetDuty.Text = "";

            //set group
            if (gm.clientPlayerList[uiPlayerList.SelectedIndex].PlayerGroupName == "") {
                uiSetGroup.SelectedIndex = 0;
            } else {
                if (gm.clientPlayerList[uiPlayerList.SelectedIndex].PlayerGroupName == gm.ms.TeamAName) {
                    uiSetGroup.SelectedIndex = 0;
                } else {
                    uiSetGroup.SelectedIndex = 1;
                }
            }

            uiSet.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// 分发数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiGiveOutTask_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("数据即将分发，我们将异步发送，期间可能会无法操作但不会卡顿。当发送完毕后，开始游戏按钮将变得可用");

            uiGiveOutTask.IsEnabled = false;
            nowStep++;

            //异步发送
            Task.Run(() =>
            {

                //发送基本信息
                foreach (Player item in gm.clientPlayerList) {
                    var cache = new List<string> {
                        item.PlayerName,
                        gm.ms.MapName,
                        gm.ms.MapMD5,
                        gm.ms.CountMode,
                        gm.ms.GameMode,
                        new StringGroup(item.DutyUnit, "#").ToString(),
                        item.PlayerGroupName,
                        new StringGroup(item.ModList, "#").ToString(),
                        item.BackgroundName,
                        item.BGMName,
                        item.PlayerIPAddress
                    };

                    //全员广播
                    gm.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.PlayerTask, new StringGroup(cache, ",").ToString()));

                }

                //发送地图
                var sendMap = new GiveOutMapFile(uiMapPath.Text, (byte[] data) =>
                 {
                     gm.allPlayerBroadcast(data);
                 });

                sendMap.Start();

                //发完了
                uiRunGame.IsEnabled = true;

            });

        }

        /// <summary>
        /// 修改所选人物
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e) {
            //set group
            switch (uiSetGroup.SelectedIndex) {
                case 0:
                    gm.clientPlayerList[uiPlayerList.SelectedIndex].PlayerGroupName = gm.ms.TeamAName;
                    gm.clientPlayerList[uiPlayerList.SelectedIndex].ChangeGroupColor(1);
                    break;
                case 1:
                    gm.clientPlayerList[uiPlayerList.SelectedIndex].PlayerGroupName = gm.ms.TeamBName;
                    gm.clientPlayerList[uiPlayerList.SelectedIndex].ChangeGroupColor(2);
                    break;
            }

            //set duty
            gm.clientPlayerList[uiPlayerList.SelectedIndex].DutyUnit = new StringGroup(uiSetDuty.Text, ",").ToList();

            uiSet.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region 正在游戏

        //处理全在处理函数里处理了

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiRunGame_Click(object sender, RoutedEventArgs e) {

            uiRunGame.IsEnabled = false;

            //全员广播开始
            gm.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.ReadyPlay, ""));

            uiShowMark.IsEnabled = true;
        }


        #endregion

        #region 游戏结果

        //处理全在处理函数里处理了

        /// <summary>
        /// 展示成绩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiShowMark_Click(object sender, RoutedEventArgs e) {

            uiShowMark.IsEnabled = false;

            gm.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.GameEnd, ""));

            MessageBox.Show("游戏已结束，分发成绩需要一段时间，请等待");

            Task.Run(() =>
            {
                List<string> outputData = new List<string>();

                foreach (Player item in gm.clientPlayerList) {
                    //读取所有小节信息
                    var allDutyUnitData = item.dataCache.ReturnData();
                    //负责小节信息
                    var selectDutyUnitData = from p in allDutyUnitData
                                             where item.DutyUnit.Contains(p.Unit)
                                             select p;

                    int fMark = 0, fPP = 0;
                    //后文用于传输数据的列表
                    List<string> cacheUnitString = new List<string>();

                    foreach (var i in selectDutyUnitData) {
                        fMark += int.Parse(i.Mark);
                        fPP += int.Parse(i.PerfomancePoint);
                        cacheUnitString.Add(i.Mark + "%" + i.PerfomancePoint + "%" + i.Unit);
                    }

                    //如果是sr。。。修正一下成绩的定义
                    if (gm.ms.CountMode == CountMode.CrazySpeedRun || gm.ms.CountMode == CountMode.SpeedRun) {
                        item.FinallyMark = item.gameTime.NowTime;
                    } else { item.FinallyMark = fMark.ToString(); }

                    item.FinallyPP = fPP.ToString();
                    //todo:称号判定
                    item.FinallyPrize = "";

                    item.PlayerUnitPrize = selectDutyUnitData.ToList<PlayerUnitData>();

                    outputData.Add((new StringGroup(cacheUnitString, "@").ToString()) + "#" + item.FinallyMark + "#" + item.FinallyPP + "#" + item.FinallyPrize + "#" + item.PlayerName);

                }

                //分析一下组数据
                //input to list
                var teamAList = new List<Player>();
                var teamBList = new List<Player>();

                var result = from i in gm.clientPlayerList
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
                                    select DateToSecound(item.FinallyMark);
                    var markBList = from item in teamBList
                                    select DateToSecound(item.FinallyMark);

                    teamAMark = SecoundToDate(Average(markAList, teamAList.Count));
                    teamBMark = SecoundToDate(Average(markBList, teamBList.Count));
                } else {
                    var markAList = from item in teamAList
                                    select item.FinallyMark;
                    var markBList = from item in teamBList
                                    select item.FinallyMark;

                    teamAMark = SecoundToDate(Average(markAList, teamAList.Count));
                    teamBMark = SecoundToDate(Average(markBList, teamBList.Count));
                }

                var cache = new List<string> { gm.ms.TeamAName, teamAMark, teamAPP.ToString(), gm.ms.TeamBName, teamBMark, teamBPP.ToString() };

                //广播数据
                gm.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server,
                    SocketSign.AllPlayerGameData,
                    new StringGroup(cache, "#").ToString() + "," + new StringGroup(outputData, ",").ToString()));

                //关闭连接
                gm.stopPing = true;
                foreach(Player item in gm.clientPlayerList) {
                    item.gameData.Dispose();
                }

                MessageBox.Show("成绩分发完成，如若需要再来一局，请关闭程序重开即可");

            });

        }

        //************************************************************组评判要用的3个函数
        private string Average(IEnumerable<string> list, int count) {
            int sum = 0;
            foreach (string item in list) {
                sum += int.Parse(item);
            }

            return (sum / count).ToString();
        }

        public string DateToSecound(string date) {
            if (date.Contains(":")) {
                var result = date.Split(':');
                var result2 = result[1].Split('.');
                return (int.Parse(result[0]) * 600 + int.Parse(result2[0]) * 10 + int.Parse(result2[1])).ToString();
            } else {
                var result = date.Split('.');
                return (int.Parse(result[0]) * 10 + int.Parse(result[1])).ToString();
            }

        }

        public string SecoundToDate(string S_secound) {
            int secound = int.Parse(S_secound);

            if ((secound / 600) <= 0) {
                //没上分钟
                return (secound / 10).ToString() + "." + (secound % 10).ToString();
            } else {
                return (secound / 600).ToString() + ":" + (secound / 10).ToString() + "." + (secound % 10).ToString();
            }
        }

        #endregion

    }
}
