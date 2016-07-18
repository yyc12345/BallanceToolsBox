using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceOnlineServer.DataProcess {

    /// <summary>
    /// 全局
    /// </summary>
    public class GlobalPrizeLog {

        public GlobalPrizeLog() {
            Reborn = false;
            Silence = false;
            FirstBlood = false;
            Ace = false;
            Time = false;
        }

        public bool Reborn;
        public bool Silence;
        public bool FirstBlood;
        public bool Ace;
        public bool Time;

    }
}
