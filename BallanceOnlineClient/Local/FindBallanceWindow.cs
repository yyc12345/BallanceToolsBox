using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceOnlineClient.Local {
    public class FindBallanceWindow {

        /// <summary>
        /// 获取窗口的大小信息
        /// </summary>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void GetWindow(out int xPosition, out int yPosition, out int width, out int height) {

            Win32.RECT r = new Win32.RECT();
            var aaa = Win32.GetWindowRect(Win32.FindWindow(null, "Ballance"), out r);

            xPosition = r.Left;
            yPosition = r.Top;

            width = r.Right - r.Left;
            height = r.Bottom - r.Top;
        }

    }
}
