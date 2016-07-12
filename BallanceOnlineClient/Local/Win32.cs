using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BallanceOnlineClient.Local {

    public class Win32 {

        #region 内存读取

        public struct MEMORY_BASIC_INFORMATION {
            public int BaseAddress;
            public int AllocationBase;
            public int AllocationProtect;
            public int RegionSize;
            public int State;
            public int Protect;
            public int lType;
        }

        public const int MEM_COMMIT = 0x1000;       //已物理分配
        public const int MEM_PRIVATE = 0x20000;
        public const int PAGE_READWRITE = 0x04;     //可读写内存

        [DllImport("kernel32.dll")]     //查询内存块信息
        public static extern int VirtualQueryEx(
            IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(
            IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int size, out int numBytesRead);
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int size, out int numBytesWrite);

        #endregion

        #region Hook 

        //键盘处理事件委托 ,当捕获键盘输入时调用定义该委托的方法. 
        public delegate int HookHandle(int nCode, int wParam, IntPtr lParam);

        //Hook结构 
        [StructLayout(LayoutKind.Sequential)]
        public class HookStruct {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        //设置钩子 
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookHandle lpfn, IntPtr hInstance, int threadId);

        //取消钩子 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //调用下一个钩子 
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        //获取当前线程ID 
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        //Gets the main module for the associated process. 
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        #endregion

        #region 获取窗口位置

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        #endregion

        #region 禁用窗口

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        #endregion
    }

}
