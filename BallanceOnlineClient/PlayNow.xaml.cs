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
using BallanceOnline.Const;
using System.Speech;
using System.Speech.Synthesis;

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
        /// <summary>
        /// 停止显示成就
        /// </summary>
        bool stopShowPrize;

        /// <summary>
        /// 成就的队列
        /// </summary>
        Queue<PrizeStructure> prizeLine;

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
            gm.PlayNow_playerSuccess = new Action<string>(playerSuccess);
            gm.PlayNow_teamDied = new Action<string>(teamDied);
            gm.PlayNow_turnToNewWindow = new Action(turnToNewWindow);
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
            stopShowPrize = false;
            talkList = new List<TalkListItem>();
            prizeLine = new Queue<PrizeStructure>();

            this.Show();

            //数据传输的操作
            Task.Run(async () =>
           {

               SpeechSynthesizer speakStart = new SpeechSynthesizer();
               speakStart.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);

               uiTimer.Dispatcher.Invoke(() => { uiTimer.Text = "20"; });

               //显示欢迎来到
               uiNoticeText.Dispatcher.Invoke(() => { uiNoticeText.Text = "欢迎来到Ballance的世界"; });
               speakStart.SpeakAsync("欢迎来到Ballance的世界");

               for (int i = 0; i < 5; i++) {
                   Thread.Sleep(1000);
                   uiTimer.Dispatcher.Invoke(() => { uiTimer.Text = (int.Parse(uiTimer.Text) - 1).ToString(); });
               }

               //此时=15
               //显示负责的关卡
               foreach (Player item in gm.gamePlayerList) {
                   if (item.PlayerName == gm.gameSettings.playerName) {
                       uiNoticeText.Dispatcher.Invoke(() => { uiNoticeText.Text = "你的任务：完成" + item.DutyUnitToString + "小节，请在这些小节好好表现"; });
                       speakStart.SpeakAsync("鼓足干劲，力争上游");
                   }
               }

               for (int i = 0; i < 10; i++) {
                   Thread.Sleep(1000);
                   uiTimer.Dispatcher.Invoke(() => { uiTimer.Text = (int.Parse(uiTimer.Text) - 1).ToString(); });
               }

               //此时=5
               //准备开始
               uiNoticeText.Dispatcher.Invoke(() => { uiNoticeText.Text = "请就绪"; });
               speakStart.SpeakAsync("还有五秒开始游戏");

               for (int i = 0; i < 5; i++) {
                   Thread.Sleep(1000);
                   uiTimer.Dispatcher.Invoke(() => { uiTimer.Text = (int.Parse(uiTimer.Text) - 1).ToString(); });
               }

               //隐藏
               uiTimerContainer.Dispatcher.Invoke(() => { uiTimerContainer.Visibility = Visibility.Collapsed; });
               //全军出击
               uiNoticeText.Dispatcher.Invoke(() => { uiNoticeText.Text = ""; });
               uiNotice.Dispatcher.Invoke(() => { uiNotice.Visibility = Visibility.Hidden; });
               speakStart.SpeakAsync("全军出击");

               //提交循环
               long previousMark = 1000;
               int similarityCount = 20;
               //是否达到极限溢出了
               bool overCount = false;
               while (true) {

                   if (stopTurnIn == true) { break; }

                   long mark = gm.markMonitor.Mode(await gm.markMonitor.ReadDataAsync());
                   long life = gm.lifeMonitor.Mode(await gm.lifeMonitor.ReadDataAsync());
                   long unit = gm.unitMonitor.Mode(await gm.unitMonitor.ReadDataAsync());

                   gm.dataGiveIn.SendData(CombineAndSplitSign.Combine(BallanceOnline.Const.ClientAndServerSign.Client, BallanceOnline.Const.SocketSign.GameDataTurnIn, mark.ToString() + "," + life.ToString() + "," + unit.ToString()));

                   //如果有相同情况出现，提醒警告
                   if (overCount == false) {
                       if (previousMark == mark) {
                           similarityCount--;
                           //检查超限
                           if (similarityCount < 0) {
                               uiTimer.Dispatcher.Invoke(() => { uiTimer.Text = ""; });
                               uiTimerContainer.Dispatcher.Invoke(() => { uiTimerContainer.Visibility = Visibility.Collapsed; });
                               overCount = true;
                           } else {
                               uiTimer.Dispatcher.Invoke(() => { uiTimer.Text = similarityCount.ToString(); });
                               uiTimerContainer.Dispatcher.Invoke(() => { uiTimerContainer.Visibility = Visibility.Visible; });
                           }

                       } else {
                           if (similarityCount != 20) {
                               similarityCount = 20;
                               uiTimer.Dispatcher.Invoke(() => { uiTimer.Text = ""; });
                               uiTimerContainer.Dispatcher.Invoke(() => { uiTimerContainer.Visibility = Visibility.Collapsed; });
                           }
                       }
                   }

                   previousMark = mark;

                   Thread.Sleep(1000);

               }

               speakStart.Dispose();

               //结束

           });

            //显示成就的操作
            Task.Run(() =>
            {
                while (true) {
                    if (stopShowPrize == true) { break; }
                    if (prizeLine.Count == 0) { Thread.Sleep(500); continue; }

                    //展示
                    var cache = prizeLine.Dequeue();
                    string sayWord = "";
                    string showWord = cache.PlayerName + " ";

                    switch (cache.PrizeName) {
                        case GamePrize.FirstBlood:
                            showWord += GamePrize.FirstBloodShow;
                            sayWord = GamePrize.FirstBloodSpeech;
                            break;
                        case GamePrize.Reborn:
                            showWord += GamePrize.RebornShow;
                            sayWord = GamePrize.RebornSpeech;
                            break;
                        case GamePrize.Silence:
                            showWord += GamePrize.SilenceShow;
                            sayWord = GamePrize.SilenceSpeech;
                            break;
                        case GamePrize.Time:
                            showWord += GamePrize.TimeShow;
                            sayWord = GamePrize.TimeSpeech;
                            break;
                        case GamePrize.Ace:
                            showWord += GamePrize.AceShow;
                            sayWord = GamePrize.AceSpeech;
                            break;
                    }

                    //show
                    uiNoticeText.Dispatcher.Invoke(() =>
                    {
                        uiNoticeText.Text = showWord;
                    });
                    uiNotice.Dispatcher.Invoke(() =>
                    {
                        uiNotice.Visibility = Visibility.Visible;
                    });

                    //say
                    SpeechSynthesizer speak = new SpeechSynthesizer();
                    speak.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
                    speak.Speak(sayWord);
                    speak.Dispose();

                    //hide
                    uiNotice.Dispatcher.Invoke(() =>
                    {
                        uiNotice.Visibility = Visibility.Visible;
                    });
                }

            });

        }


        public void inputPlayerData(StringGroup input) {
            var cache = input.ToStringGroup();

            foreach (Player item in gm.gamePlayerList) {
                if (item.PlayerName == cache[4]) {
                    item.NowLife = cache[1];
                    item.NowTime = cache[0];
                    item.NowUnit = cache[2];

                    //压入成就宣布队列
                    if (cache[4] != "") {
                        prizeLine.Enqueue(new PrizeStructure { PlayerName = cache[3], PrizeName = cache[4] });
                    }

                    return;
                }
            }

            //刷新
            this.FlushPlayerList();

        }

        public void playerDied(string name) {

            if (name == gm.gameSettings.playerName) {
                gm.kh.UnHook();
                gm.kh.SetHook(false);
                stopTurnIn = true;
            }

            foreach (Player item in gm.gamePlayerList) {
                if (name == item.PlayerName) {
                    item.NowState = PlayerState.Died;
                    item.SetNowStateColor();

                    //刷新
                    this.FlushPlayerList();

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
                    item.NowState = PlayerState.Success;

                    //刷新
                    this.FlushPlayerList();

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
                    item.NowState = PlayerState.Died;


                }
            }

            //刷新
            this.FlushPlayerList();

        }

        public void newMessage(string msg) {

            uiTalkList.Dispatcher.Invoke(() =>
            {
                uiTalkList.ItemsSource = null;

                talkList.Add(new TalkListItem { word = msg });

                uiTalkList.ItemsSource = talkList;
            });

            //快速显示栏
            uiQuickMsgText.Dispatcher.Invoke(() =>
            {
                uiQuickMsgText.Text = msg;
            });

        }

        public void turnToNewWindow() {
            stopTurnIn = true;
            stopShowPrize = true;

            var newWin = new GameResult();
            newWin.Show(gm);

            this.Close();
        }

        public void FlushPlayerList() {

            //show player
            var playerSplit = from item in gm.gamePlayerList
                              where item.PlayerGroupName != ""
                              group item by item.PlayerGroupName;
            foreach (var item in playerSplit) {
                if (item.Key == gm.ms.TeamAName) {
                    uiTeamAList.Dispatcher.Invoke(() =>
                    {
                        uiTeamAList.ItemsSource = item.ToList<Player>();
                    });
                } else {
                    uiTeamBList.Dispatcher.Invoke(() =>
                    {
                        uiTeamBList.ItemsSource = item.ToList<Player>();
                    });
                }
            }


        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiMsgSend_Click(object sender, RoutedEventArgs e) {
            if (uiMsg.Text != "") {
gm.dataGiveIn.SendData(CombineAndSplitSign.Combine(BallanceOnline.Const.ClientAndServerSign.Client, BallanceOnline.Const.SocketSign.Message, uiMsg.Text));
                uiMsg.Text = "";
            } else MessageBox.Show("发送的消息不能为空");
        }

        /// <summary>
        /// 显示消息面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiMsg_GotFocus(object sender, RoutedEventArgs e) {
            uiTalkList.Visibility = Visibility.Visible;

            uiQuickMsg.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 隐藏消息面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiMsg_LostFocus(object sender, RoutedEventArgs e) {
            uiTalkList.Visibility = Visibility.Collapsed;

            uiQuickMsg.Visibility = Visibility.Visible;
        }
    }

    //public class NowPlayerListItem {
    //    public string name { get; set; }
    //    public string life { get; set; }
    //    public string time { get; set; }
    //    public string state { get; set; }
    //}

    /// <summary>
    /// 成就展示的类
    /// </summary>
    public class PrizeStructure {
        public PrizeStructure() {
            PrizeName = "";
            PlayerName = "";
        }
        public string PrizeName;
        public string PlayerName;
    }

}
