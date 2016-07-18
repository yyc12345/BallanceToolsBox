using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BallanceOnlineServer.DataProcess {
    /// <summary>
    /// 计时器
    /// </summary>
    public class Timer {

        private long secound;

        private System.Windows.Threading.DispatcherTimer changeTimeTimer;

        public Timer() {
            secound = 0;
            changeTimeTimer = new System.Windows.Threading.DispatcherTimer();
            changeTimeTimer.Interval = TimeSpan.FromMilliseconds(100);
            changeTimeTimer.Tick += new EventHandler((object sender, EventArgs e) =>
            {
                secound++;
            });
        }

        public void Start() { changeTimeTimer.Start(); }

        public void Stop() { changeTimeTimer.Stop(); }

        public string NowTime {
            get {
                if ((secound / 600) <= 0) {
                    //没上分钟
                    return (secound / 10).ToString() + "." + (secound % 10).ToString();
                } else {
                    return (secound / 600).ToString() + ":" + (secound / 10).ToString() + "." + (secound % 10).ToString();
                }
            }
        }

        public void Reset() {
            secound = 0;
        }
    }
}
