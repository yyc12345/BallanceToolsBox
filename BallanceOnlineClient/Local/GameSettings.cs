using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BallanceOnlineClient.Local {

    public class GameSettings {

        public List<string> ModList;

        public string backgroundName;

        public string BGMName;

        public bool FullScreen;

        public string rootFolder;

        public string playerName;

        public GameSettings() {
            ModList = new List<string>();
            backgroundName = "";
            BGMName = "";
            FullScreen = false;
            rootFolder = "";
            playerName = "";
        }

        /// <summary>
        /// 更新，从命令行读
        /// </summary>
        public void Update() {
            var comd = Environment.GetCommandLineArgs();

            /*第一项是程序->排除不要
             * 第2项是ballance目录
             * 第3项是mod列表，使用,分割
             * 第4项是背景名称
             * 第5项是bgm名称
             * 第6项是否全屏
             * 第7项玩家名称
             * */

            rootFolder = comd[1];

            ModList.Clear();
            var cache1 = comd[2].Split(',');
            foreach (string item in cache1) {
                ModList.Add(item);
            }

            backgroundName = comd[3];
            BGMName = comd[4];

            FullScreen = Convert.ToBoolean(comd[5]);
            playerName = comd[6];

        }

    }
}
