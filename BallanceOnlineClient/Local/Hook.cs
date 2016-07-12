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
        /// 允许的按键
        /// </summary>
        private List<System.Windows.Forms.Keys> allowKeys;

        /// <summary>
        /// 是否屏蔽所有按键
        /// </summary>
        private bool globalHook;
        /// <summary>
        /// 是否屏蔽所有按键
        /// </summary>
        public bool GlobalHook { get { return globalHook; } set { globalHook = value; } }

        public Hook() {
            globalHook = false;
            kh = new BaseHook();
            allowKeys = new List<Keys> { Keys.Enter, Keys.Right, Keys.Left, Keys.Up, Keys.Down, Keys.Q, Keys.Escape, Keys.Space };
        }

        public void SetHook() {
            kh.InstallHook(this.KeyInput);
        }

        public void UnHook() {
            kh.UninstallHook();
        }

        public void KeyInput(Win32.HookStruct hookStruct, out bool handle) {

            if (globalHook == true) { handle = true; return; }

            //默认拦截
            handle = true;

            //检索允许列表
            foreach (Keys item in allowKeys) {
                if ((int)item == hookStruct.vkCode) { handle = false; return; }
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
