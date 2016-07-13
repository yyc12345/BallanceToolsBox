using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
//using System.Windows.Input;
using System.Windows.Forms;

namespace BallanceOnlineClient.Local {

    public class Hook {

        /// <summary>
        /// 键盘监控
        /// </summary>
        private BaseHook kh;

        /// <summary>
        /// 是否屏蔽所有按键
        /// </summary>
        private bool globalHook;
        /// <summary>
        /// 是否屏蔽所有按键
        /// </summary>
        public bool GlobalHook { get { return globalHook; } set { globalHook = value; } }

        /// <summary>
        /// 游戏允许按键
        /// </summary>
        private List<Keys> gameAllowKeys;
        /// <summary>
        /// 交流模式不允许按键
        /// </summary>
        private List<Keys> talkNotAllowKeys;

        private bool hookFlag;

        public Hook() {
            globalHook = false;
            kh = new BaseHook();
            gameAllowKeys = new List<Keys> { Keys.Enter, Keys.Right, Keys.Left, Keys.Up, Keys.Down, Keys.Escape, Keys.Space, Keys.Shift };
            talkNotAllowKeys = new List<Keys> { Keys.Enter, Keys.Right, Keys.Left, Keys.Up, Keys.Down, Keys.Space };
            hookFlag = false;
        }

        /// <summary>
        /// 设定屏蔽标准
        /// </summary>
        /// <param name="flag">true执行游戏标准，false执行交流标准</param>
        public void SetHook(bool flag) {
            kh.InstallHook(this.KeyInput);
            hookFlag = flag;
        }

        public void UnHook() {
            kh.UninstallHook();
        }

        public void KeyInput(Win32.HookStruct hookStruct, out bool handle) {

            if (globalHook == true) { handle = true; return; }

            //分析模式
            if (hookFlag == true) {
                //game

                //默认拦截
                handle = true;

                //检索允许列表
                foreach (Keys item in gameAllowKeys) {
                    if ((int)item == hookStruct.vkCode) { handle = false; return; }
                }
            } else {
                //talk

                //默认不拦截
                handle = false;

                //检索允许列表
                foreach (Keys item in talkNotAllowKeys) {
                    if ((int)item == hookStruct.vkCode) { handle = true; return; }
                }
            }



        }

    }

    public class BaseHook {

        private const int WH_KEYBOARD_LL = 13; //键盘 

        //客户端键盘处理事件 
        public delegate void ProcessKeyHandle(Win32.HookStruct param, out bool handle);

        //接收SetWindowsHookEx返回值 
        private static int _hHookValue = 0;

        //勾子程序处理事件 
        private Win32.HookHandle _KeyBoardHookProcedure;



        private IntPtr _hookWindowPtr = IntPtr.Zero;


        //外部调用的键盘处理事件 
        private static ProcessKeyHandle _clientMethod = null;

        /// <summary> 
        /// 安装勾子 
        /// </summary> 
        /// <param name="hookProcess">外部调用的键盘处理事件</param> 
        public void InstallHook(ProcessKeyHandle clientMethod) {
            _clientMethod = clientMethod;

            // 安装键盘钩子 
            if (_hHookValue == 0) {
                _KeyBoardHookProcedure = new Win32.HookHandle(OnHookProc);

                _hookWindowPtr = Win32.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

                _hHookValue = Win32.SetWindowsHookEx(
                WH_KEYBOARD_LL,
                _KeyBoardHookProcedure,
                _hookWindowPtr,
                0);

                //如果设置钩子失败. 
                if (_hHookValue == 0) UninstallHook();
            }
        }

        //取消钩子事件 
        public void UninstallHook() {
            if (_hHookValue != 0) {
                bool ret = Win32.UnhookWindowsHookEx(_hHookValue);
                if (ret) _hHookValue = 0;
            }
        }

        //钩子事件内部调用,调用_clientMethod方法转发到客户端应用。 
        private static int OnHookProc(int nCode, int wParam, IntPtr lParam) {
            if (nCode >= 0) {
                //转换结构 
                Win32.HookStruct hookStruct = (Win32.HookStruct)Marshal.PtrToStructure(lParam, typeof(Win32.HookStruct));

                if (_clientMethod != null) {
                    bool handle = false;
                    //调用客户提供的事件处理程序。 
                    _clientMethod(hookStruct, out handle);
                    if (handle) return 1; //1:表示拦截键盘,return 退出 
                }
            }
            return Win32.CallNextHookEx(_hHookValue, nCode, wParam, lParam);
        }
    }

    public class HookMouseAndKeyboard {

        Process selectedProcess;

        public HookMouseAndKeyboard() {

            foreach (Process p in Process.GetProcesses()) {
                if (p.ProcessName == "Player") { selectedProcess = p; }
            }

            if (selectedProcess == null) {
                throw new NotImplementedException("Couldn't find process!");
            }

        }

        /// <summary>
        /// 开始封锁
        /// </summary>
        public void Start() {
            Win32.EnableWindow(selectedProcess.Handle, false);
        }

        /// <summary>
        /// 解除封锁
        /// </summary>
        public void Stop() {
            Win32.EnableWindow(selectedProcess.Handle, true);
        }
    }

}
