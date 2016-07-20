using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BallanceOnline;
using System.Net.Sockets;
using BallanceOnline.Const;
using System.Windows.Media;
using BallanceOnlineServer.DataProcess;

namespace BallanceOnlineServer {

    public class GlobalManager {

        public bool stopPing;

        public GlobalManager() {
            clientPlayerList = new List<Player>();
            stopPing = false;
            ms = new MapSetting();
            globalPrize = new GlobalPrizeLog();
            nowFinishPlayerCount = 0;

            //自动ping
            //Task.Run(() =>
            //{

            //    while (true) {

            //        if (stopPing == true) { break; }

            //        if (clientPlayerList.Count != 0) {
            //            foreach (Player item in clientPlayerList) {

            //                if (item.PingCount > 4) {
            //                    //超时，掉线
            //                    pingOut(item.PlayerIPAddress, item);
            //                    continue;
            //                }

            //                item.gameData.SendData(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.Ping, ""));
            //                item.PingCount++;
            //            }
            //        }

            //        System.Threading.Thread.Sleep(60000);
            //    }

            //});

        }

        #region 玩家方面

        /// <summary>
        /// 用户列表
        /// </summary>
        public List<BallanceOnlineServer.Player> clientPlayerList;

        /// <summary>
        /// 全体成员广播
        /// </summary>
        /// <param name="data"></param>
        public void allPlayerBroadcast(byte[] data) {
            if (clientPlayerList.Count == 0) { return; }
            foreach (Player item in clientPlayerList) {
                item.SendData(data);
            }
        }

        /// <summary>
        /// 个人广播
        /// </summary>
        /// <param name="playerIP">用户ip</param>
        /// <param name="data"></param>
        public void singlePlayerBroadcast(string playerIP, byte[] data) {
            if (clientPlayerList.Count == 0) { return; }
            foreach (Player item in clientPlayerList) {
                if (item.PlayerIPAddress == playerIP) {
                    item.SendData(data);
                    break;
                }
            }
        }

        /// <summary>
        /// 当前完成任务玩家数量
        /// </summary>
        private int nowFinishPlayerCount;

        #endregion

        #region 消息处理

        /// <summary>
        /// 主消息处理函数
        /// </summary>
        /// <param name="head"></param>
        /// <param name="data"></param>
        /// <param name="host"></param>
        public void MainMessageProcess(string head, byte[] data, Player host) {

            switch (head) {
                case ClientAndServerSign.Client + SocketSign.Ping:
                    host.PingCount--;
                    break;

                case ClientAndServerSign.Client + SocketSign.Message:
                    //转发，不交给界面处理
                    this.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.Message, (host.PlayerName != "" ? host.PlayerName : host.PlayerIPAddress) + "：" + CombineAndSplitSign.ConvertToString(data)));
                    break;

                case ClientAndServerSign.Client + SocketSign.InformationTurnIn:
                    //set data
                    var cache = new StringGroup(CombineAndSplitSign.ConvertToString(data), ",").ToStringGroup();

                    host.ModList = new StringGroup(cache[0], "#").ToList();
                    host.BackgroundName = cache[1];
                    host.BGMName = cache[2];
                    host.PlayerName = cache[3];

                    var cache2 = new StringGroup(cache[4], "#").ToStringGroup();
                    host.HistoryWinCount = cache2[0];
                    host.HistoryFailCount = cache2[1];
                    host.HistoryRankedRaceCount = cache2[2];
                    host.HistoryRelayRaceCount = cache2[3];

                    //刷新
                    flushPlayerList();

                    break;

                case ClientAndServerSign.Client + SocketSign.PlayerIsReady:
                    //ready
                    host.playerIsReady();
                    this.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.PlayerIsReady, host.PlayerName));

                    //刷新
                    flushPlayerList();

                    break;

                case ClientAndServerSign.Client + SocketSign.GameDataTurnIn:
                    var cache3 = new StringGroup(CombineAndSplitSign.ConvertToString(data), ",").ToStringGroup();
                    host.NowTime = cache3[0];
                    host.NowLife = cache3[1];
                    host.NowUnit = cache3[2];
                    host.NowState = "";

                    //判断游戏状态
                    if (host.NowState == PlayerState.Success || host.NowState==PlayerState.Died) {
                        //拒绝数据
                        break;
                    }

                    /* 判定方法
                     * 暂停：状态在正在游戏，数据保持不变<20次，且有命，时间数据不变。（放弃判断）
                     * 继续：状态为暂停，数据有变动，同时清空数据不变标识符。（放弃判断）
                     * 死亡：状态在正在游戏，数据不变>=20次，且命=0
                     * 成功：状态在正在游戏，时间减少，减少量>20点
                     * */

                    //***********************判断状态

                    //要传出的数据
                    List<string> outPrize = new List<string>();


                    if (host.dataCache.previousState == PlayerState.Playing && host.NowTime == host.dataCache.previousTime) {
                        //增加数据静止值
                        host.dataCache.dataCompareNotChangeCount++;
                    } else host.dataCache.dataCompareNotChangeCount = 0;

                    //先判断死亡
                    if (host.dataCache.previousState == PlayerState.Playing && host.dataCache.dataCompareNotChangeCount >= 20 && host.NowLife == "0" && host.NowTime == host.dataCache.previousTime) {

                        //###########################成就-沉默#######################
                        //不是一次性输出，可以多次输出
                        //信息压入玩家队列传出
                        outPrize.Add(GamePrize.Silence);
                        //###########################成就#######################
                        host.NowState = PlayerState.Died;
                        //增加完成任务数量人数
                        nowFinishPlayerCount++;

                        //判断一波组死亡
                        if (this.ms.GameMode == GameMode.RelayRace) {
                            foreach (string item in host.DutyUnit) {
                                if (int.Parse(item) >= int.Parse(host.NowUnit)) {
                                    //组死亡

                                    //组死亡肯定包含上面已经死的人，所以完成人数--防止重复计算
                                    nowFinishPlayerCount--;

                                    //循环死亡
                                    foreach (Player item2 in this.clientPlayerList) {
                                        if (item2.PlayerGroupName == host.PlayerGroupName) {
                                            item2.NowState = PlayerState.Died;
                                            //增加完成任务数量人数
                                            nowFinishPlayerCount++;

                                            //###########################成就-团灭#######################
                                            if (this.globalPrize.Ace == false) {
                                                //信息压入玩家队列传出
                                                outPrize.Add(GamePrize.Ace);
                                                this.globalPrize.Ace = true;
                                            }
                                            //###########################成就#######################

                                            //小节线最终写入
                                            item2.dataCache.AddUnitData(item2.NowTime, item2.NowLife);
                                            //停止计时器
                                            item2.gameTime.Stop();
                                        }
                                    }

                                    this.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.TeamDied, host.PlayerGroupName));
                                }
                            }
                        }else {
                            //不是组死亡
                            //单独广播
                            this.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.PlayerDied, host.PlayerName));
                        }
                    }


                    //成功
                    if (host.dataCache.previousState == PlayerState.Playing && (int.Parse(host.dataCache.previousTime) - int.Parse(host.NowTime)) > 20) {
                        this.allPlayerBroadcast(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.PlayerSuccess, host.PlayerName));
                        host.NowState = PlayerState.Success;
                        //增加完成任务数量人数
                        nowFinishPlayerCount++;

                        //小节线最终写入，用之前的数据写入，更加准确
                        host.dataCache.AddUnitData(host.dataCache.previousTime, host.dataCache.previousLife);
                        //停止计时器
                        host.gameTime.Stop();
                    }

                    //判断计时器
                    if (host.dataCache.previousTime == "") {
                        //第一次，启动计时器
                        host.gameTime.Start();
                    }

                    //判断小节线变动
                    if (host.dataCache.previousState == PlayerState.Playing) {
                        if (host.dataCache.previousTime == "") {
                            //刚开始，立即写入
                            host.dataCache.AddUnitData(host.NowTime, host.NowLife);

                        } else if (host.dataCache.previousUnit != host.NowUnit) {
                            //新小节
                            //###########################成就-通过#######################
                            //不是一次性成就
                            //信息压入玩家队列传出
                            outPrize.Add(GamePrize.Cross);
                            //###########################成就#######################
                            host.dataCache.AddUnitData(host.NowTime, host.NowLife);
                        }//否则不写入
                    }


                    //暂停
                    //if (host.previousState == PlayerState.Playing && host.dataCompareNotChangeCount <10 && host.NowLife != "0" && host.NowTime == host.previousTime) {
                    //    host.gameData.SendData(CombineAndSplitSign.Combine(ClientAndServerSign.Server, SocketSign.PlayerPaused, host.PlayerName));
                    //    host.NowState = PlayerState.Paused;
                    //}

                    //**********************************************************************todo:判断成就

                    //###########################成就-第一次死亡#######################
                    if (this.globalPrize.FirstBlood == false) {
                        //信息压入玩家队列传出
                        if (int.Parse(host.NowLife) < int.Parse(host.dataCache.previousLife)) {
                            outPrize.Add(GamePrize.FirstBlood);
                            this.globalPrize.FirstBlood = true;
                        }
                    }
                    //###########################成就#######################
                    //###########################成就-重生#######################
                    //非一次性成就
                    //信息压入玩家队列传出
                    if (int.Parse(host.NowLife) > int.Parse(host.dataCache.previousLife)) {
                        outPrize.Add(GamePrize.Reborn);
                    }
                    //###########################成就#######################
                    //###########################成就-时间#######################
                    //非一次性成就
                    //信息压入玩家队列传出
                    if (int.Parse(host.NowTime) > int.Parse(host.dataCache.previousTime)) {
                        outPrize.Add(GamePrize.Time);
                    }
                    //###########################成就#######################


                    //判断写入
                    host.dataCache.previousLife = host.NowLife;
                    host.dataCache.previousTime = host.NowTime;
                    host.dataCache.previousUnit = host.NowUnit;
                    if (host.NowState != "") host.dataCache.previousState = host.NowState;

                    //传出
                    this.allPlayerBroadcast(CombineAndSplitSign.Combine(
                        ClientAndServerSign.Server,
                        SocketSign.GameDataGiveOut,
                        new StringGroup(new List<string> {
                            host.dataCache.previousTime,
                            host.dataCache.previousLife,
                            host.dataCache.previousUnit,
                            outPrize.Count == 0 ? "" : new StringGroup(outPrize,"#").ToString(),
                            host.PlayerName
                        }, ",").ToString()));

                    //提醒服务器注意
                    if (nowFinishPlayerCount == clientPlayerList.Count) {
                        //可能都完成了
                        warnPlayerFinishGame();
                    }

                    //刷新
                    flushPlayerList();

                    break;

            }

        }

        #endregion

        #region 消息委托

        //全局掉线
        public Action<string, Player> pingOut;
        //刷新用户列表
        public Action flushPlayerList;
        //提醒用户可能都完成了游戏
        public Action warnPlayerFinishGame;

        #endregion

        #region 地图数据

        public MapSetting ms;

        #endregion

        #region 全局成就

        GlobalPrizeLog globalPrize;

        #endregion

    }

    /// <summary>
    /// 服务器专用类
    /// </summary>
    public class Player : BallanceOnline.Player {

        public Player(ref TcpClient inputClient, Action<string, byte[], Player> replay) {
            //对原构造函数继承

            playerIPAddress = "";
            playerName = "";

            modList = new List<string>();
            backgroundName = "";
            bgmName = "";

            historyFailCount = "";
            historyRankedRaceCount = "";
            historyRelayRaceCount = "";
            historyWinCount = "";

            dutyUnit = new List<string>();
            playerGroupName = "";

            readyColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

            nowLife = "";
            nowTime = "";
            nowUnit = "1";
            nowState = PlayerState.Playing;
            nowStateColor = new SolidColorBrush(Color.FromArgb(80, 0, 0, 0));

            playerUnitPrize = new List<PlayerUnitData>();
            finallyMark = "";
            finallyPP = "";
            finallyPrize = "";

            //=================================自己的初始化
            gameData = new GameData(ref inputClient, this.replyInsider);
            replyEx = new Action<string, byte[], Player>(replay);

            var cache = inputClient.Client.RemoteEndPoint.ToString().Split(':');
            playerIPAddress = cache[0];

            PingCount = 0;

            gameTime = new Timer();
            dataCache = new DataCache();


            groupColor = new SolidColorBrush(Color.FromArgb(80, 0, 0, 0));



        }

        #region 数据传输部分

        /// <summary>
        /// 对内部委托封装，加入传出自身实例的功能
        /// </summary>
        Action<string, byte[], Player> replyEx;

        /// <summary>
        /// 对内部类响应处理的方式
        /// </summary>
        /// <param name="cache1"></param>
        /// <param name="cache2"></param>
        protected void replyInsider(string cache1, byte[] cache2) {
            replyEx(cache1, cache2, this);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void SendData(byte[] data) {
            gameData.SendData(data);
        }

        /// <summary>
        /// 此用户数据传输
        /// </summary>
        public GameData gameData;

        #endregion

        /// <summary>
        /// 用户ping响应次数，服务端发出一个++，客户端发来一个--，当其=4时，认定用户掉线
        /// </summary>
        public int PingCount;

        #region 用户组别颜色

        /// <summary>
        /// 表示用户组别状态的颜色
        /// </summary>
        public SolidColorBrush groupColor { get; set; }
        /// <summary>
        /// 设置用户组别状态的颜色
        /// </summary>
        /// <param name="groupSign">0=无组 1=A组（红 2=B组（蓝</param>
        public void ChangeGroupColor(int groupSign) {
            switch (groupSign) {
                case 0:
                    groupColor = new SolidColorBrush(Color.FromArgb(80, 0, 0, 0));
                    break;
                case 1:
                    groupColor = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0));
                    break;
                case 2:
                    groupColor = new SolidColorBrush(Color.FromArgb(80, 0, 0, 255));
                    break;
            }

        }

        #endregion

        /// <summary>
        /// 计时器
        /// </summary>
        public Timer gameTime;

        /// <summary>
        /// 临时数据
        /// </summary>
        public DataCache dataCache;

    }

}
