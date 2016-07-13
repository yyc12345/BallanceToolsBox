using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using BallanceOnlineClient.Local;
using BallanceOnlineClient.Online;
using System.Net.Sockets;
using BallanceOnline.Const;
using BallanceOnline;
using System.Collections;

namespace BallanceOnlineClient {

    public class GlobalManager {

        public GlobalManager(string needWindow) {
            //window
            needTransportWindowTitle = needWindow;
            transportWindowFlag = true;

            Thread windowT = new Thread(new ThreadStart(() =>
            {

                while (true) {

                    try {
                        int x, y, width, height;
                        FindBallanceWindow.GetWindow(out x, out y, out width, out height);

                        var aaa = Win32.SetWindowPos(Win32.FindWindow(null, needTransportWindowTitle), Win32.HWND_TOPMOST, x, y, width, height, Win32.SWP_NOZORDER);

                    } catch (Exception ex) {
                        //没进程
                    }

                    Thread.Sleep(1000);
                }

            }));
            windowT.IsBackground = true;
            windowT.Start();


            gameSettings = new GameSettings();
            gameSettings.Update();

            kh = new Hook();

            tcpConnect = new TcpClientWithTimeout();

            gamePlayerList = new List<Player>();
            df = new DownloadFile();
        }

        #region 窗口位移方面

        string needTransportWindowTitle;

        bool transportWindowFlag;
        public bool TransportWindowFlag { get { return transportWindowFlag; } set { transportWindowFlag = value; } }

        public void ChangeTransportWindow(string newWindow) {
            transportWindowFlag = false;
            needTransportWindowTitle = newWindow;
            transportWindowFlag = true;
        }

        #endregion

        #region cheatEngine方面

        CheatEngine markMonitor;
        CheatEngine lifeMonitor;
        CheatEngine unitMonitor;

        /// <summary>
        /// 设定监视器
        /// </summary>
        /// <param name="mark"></param>
        /// <param name="life"></param>
        /// <param name="unit"></param>
        public void SetMonitor(CheatEngine mark, CheatEngine life, CheatEngine unit) {
            markMonitor = mark;
            lifeMonitor = life;
            unitMonitor = unit;
        }

        #endregion

        #region 游戏数据方面

        public GameSettings gameSettings;

        #endregion

        #region Hook方面

        public Hook kh;

        #endregion

        #region 服务器连接方面

        public TcpClientWithTimeout tcpConnect;

        #endregion

        #region 服务器信息发送方面
        /// <summary>
        /// 当前页面0=waitPlayer 1=loadResources 2=playNow 3=gameResult
        /// </summary>
        private int nowPage;

        public GameData dataGiveIn;

        /// <summary>
        /// 消息主处理函数
        /// </summary>
        /// <param name="head"></param>
        /// <param name="data"></param>
        private void dataProcess(string head, byte[] data) {

            switch (head) {

                case ClientAndServerSign.Server + SocketSign.Ping:
                    dataGiveIn.SendData(CombineAndSplitSign.Combine(ClientAndServerSign.Client, SocketSign.Ping, ""));
                    break;
                case ClientAndServerSign.Server + SocketSign.ReturnAllPlayer:

                    var cache = new StringGroup(CombineAndSplitSign.ConvertToString(data), ",");
                    WaitPlayer_addAllPlayer(cache);

                    break;
                case ClientAndServerSign.Server + SocketSign.NewPlayer:
                    WaitPlayer_addSinglePlayer(CombineAndSplitSign.ConvertToString(data));
                    break;

                case ClientAndServerSign.Server + SocketSign.Message:
                    switch (nowPage) {
                        case 0:
                            WaitPlayer_newMessage(CombineAndSplitSign.ConvertToString(data));
                            break;
                        case 1:
                            //没有
                            break;
                        case 2:

                            break;

                        case 3:
                            //没有
                            break;
                    }

                    break;

                case ClientAndServerSign.Server + SocketSign.OrderTurnIn:
                    WaitPlayer_turnToNewWindow();
                    //返回数据
                    var cache3 = new StringGroup(gameSettings.ModList, "#");
                    var cache2 = new ArrayList { cache3.ToString(), gameSettings.backgroundName, gameSettings.BGMName, gameSettings.playerName };
                    dataGiveIn.SendData(CombineAndSplitSign.Combine(ClientAndServerSign.Client, SocketSign.InformationTurnIn, new StringGroup(cache2, ",").ToString()));

                    nowPage += 1;

                    break;

                case ClientAndServerSign.Server + SocketSign.PlayerTask:
                    LoadResources_addPlayerInformation(new StringGroup(CombineAndSplitSign.ConvertToString(data), ","));
                    break;

                    //********************************************
                case ClientAndServerSign.Server + SocketSign.StartDownloadingMap:
                    df.Start("");
                    break;
                case ClientAndServerSign.Server + SocketSign.DownloadingMapData:
                    df.Write(data);
                    break;
                case ClientAndServerSign.Server + SocketSign.EndDownloadingMap:
                    df.Stop();
                    //change file
                    System.IO.File.Delete(gameSettings.rootFolder + @"3D Entities\Level\Level_01.NMO");
                    System.IO.File.Copy(Environment.CurrentDirectory + @"\cacheMap.nmo", gameSettings.rootFolder + @"3D Entities\Level\Level_01.NMO");
                    //send message
                    dataGiveIn.SendData(CombineAndSplitSign.Combine(ClientAndServerSign.Client, SocketSign.PlayerIsReady, ""));
                    break;
                case ClientAndServerSign.Server + SocketSign.PlayerIsReady:
                    LoadResources_singlePlayerReady(CombineAndSplitSign.ConvertToString(data));
                    break;

                case ClientAndServerSign.Server + SocketSign.ReadyPlay:
                    LoadResources_turnToNewWindow();
                    nowPage += 1;
                    break;

                //********************************************
                case ClientAndServerSign.Server + SocketSign.GameDataGiveOut:
                    PlayNow_inputPlayerData(new StringGroup(CombineAndSplitSign.ConvertToString(data), ","));
                    break;

                case ClientAndServerSign.Server + SocketSign.PlayerDied:
                    PlayNow_playerDied(CombineAndSplitSign.ConvertToString(data));
                    break;

                case ClientAndServerSign.Server + SocketSign.PlayerPaused:
                    PlayNow_playerPaused(CombineAndSplitSign.ConvertToString(data));
                    break;

                case ClientAndServerSign.Server + SocketSign.PlayerSuccess:
                    PlayNow_playerSuccess(CombineAndSplitSign.ConvertToString(data));
                    break;

                case ClientAndServerSign.Server + SocketSign.TeamDied:
                    PlayNow_teamDied(CombineAndSplitSign.ConvertToString(data));
                    break;

                case ClientAndServerSign.Server + SocketSign.GameEnd:
                    PlayNow_turnToNewWindow();
                    nowPage += 1;
                    break;

                //********************************************

                case ClientAndServerSign.Server + SocketSign.AllPlayerGameData:
                    GameResult_allPlayerData(new StringGroup(CombineAndSplitSign.ConvertToString(data), ","));
                    //连接关闭
                    dataGiveIn.Dispose();
                    break;

            }

        }

        public void SetDataGiveIn(ref TcpClient tc) {
            dataGiveIn = new GameData(ref tc, dataProcess);
        }

        #endregion

        #region 推送数据到ui的委托

        #region WaitPlayer

        public Action<StringGroup> WaitPlayer_addAllPlayer;

        public Action<string> WaitPlayer_addSinglePlayer;

        public Action<string> WaitPlayer_newMessage;

        public Action WaitPlayer_turnToNewWindow;

        #endregion

        #region LoadResources

        public Action<StringGroup> LoadResources_addPlayerInformation;

        public Action<string> LoadResources_singlePlayerReady;

        public Action LoadResources_turnToNewWindow;

        #endregion

        #region PlayNow

        public Action<StringGroup> PlayNow_inputPlayerData;

        public Action<string> PlayNow_playerDied;
        public Action<string> PlayNow_playerSuccess;
        public Action<string> PlayNow_playerPaused;

        public Action<string> PlayNow_teamDied;

        public Action PlayNow_turnToNewWindow;

        #endregion

        #region GameResult

        public Action<StringGroup> GameResult_allPlayerData;

        #endregion

        #endregion

        #region 玩家信息加载部分

        public List<Player> gamePlayerList;

        /// <summary>
        /// 返回指定名称在列表中的位置，没有返回-1
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int returnIndexInList(string name) {
            int index = 0;
            foreach (Player item in gamePlayerList) {
                if (item.PlayerName == name) { return index; }
            }

            return -1;

        }

        DownloadFile df;

        #endregion



    }
}
