using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceOnline {
    public abstract class Player {

        public string playerName;
        public int localPort;
        public int remoteIP;

        public void SendMessage(string MSG_HEAD, byte[] MSG_CONTANT) {

        }

    }
}
