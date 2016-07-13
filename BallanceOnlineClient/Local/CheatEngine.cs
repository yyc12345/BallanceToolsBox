using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BallanceOnlineClient.Local {

    public class CheatEngine {

        public CheatEngine() {
            selectedProcess = null;
            address = new List<int>();
            IsFinsihSearch = false;
        }

        /// <summary>
        /// 选择的进程
        /// </summary>
        private Process selectedProcess;
        /// <summary>
        /// 基地址
        /// </summary>
        private int baseAddress;
        /// <summary>
        /// 搜索字节数
        /// </summary>
        private const int searchBytes = 1 << 2;

        /// <summary>
        /// 搜索的数据
        /// </summary>
        private long searchedData;
        /// <summary>
        /// 搜索的数据
        /// </summary>
        public long SearchedData { get { return searchedData; } set { searchedData = value; } }

        /// <summary>
        /// 写入的数据
        /// </summary>
        private long writeData;
        /// <summary>
        /// 写入的数据
        /// </summary>
        public long WriteData { get { return writeData; } set { writeData = value; } }

        /// <summary>
        /// 搜索到的地址列表
        /// </summary>
        private List<int> address;

        /// <summary>
        /// 当前搜索到的地址的个数
        /// </summary>
        public int ResultCount { get { return address.Count; } }

        /// <summary>
        /// 是否完成搜索了
        /// </summary>
        private bool IsFinsihSearch;

        #region 找寻地址阶段

        /// <summary>
        /// 初始化找址，选择线程，找到返回t，没找到返回f
        /// </summary>
        public bool StartSearching() {

            foreach (Process p in Process.GetProcesses()) {
                if (p.ProcessName == "Player") { selectedProcess = p; return true; }
            }

            return false;
            //if (selectedProcess == null) {
                //throw new NotImplementedException("Couldn't find process!");

            //}

        }

        /// <summary>
        /// 第一次搜索
        /// </summary>
        /// <returns></returns>
        public Task FirstSearchAsync() {
            return Task.Run(() =>
            {
                //检测线程存在
                if (CheckProcessExist() == false) { return; }

                Win32.MEMORY_BASIC_INFORMATION stMBI = new Win32.MEMORY_BASIC_INFORMATION();
                int searchLen = Marshal.SizeOf(stMBI);
                baseAddress = 0x000000;
                int nReadSize = 0;      //实际读取字节数
                while (baseAddress >= 0 && baseAddress <= 0x7fffffff && stMBI.RegionSize >= 0) {    //nBaseAddr >= 0 在这期间nBaseAddr 可能溢出int范围变成负数
                    searchLen = Win32.VirtualQueryEx(selectedProcess.Handle, (IntPtr)baseAddress, out stMBI, Marshal.SizeOf(stMBI));  //扫描内存信息 
                    if (searchLen == Marshal.SizeOf(typeof(Win32.MEMORY_BASIC_INFORMATION))) {
                        if (stMBI.State == Win32.MEM_COMMIT && stMBI.Protect == Win32.PAGE_READWRITE) {     //如果是已物理分配 并且是 可读写内存 那么读取内存
                            byte[] byData = new byte[stMBI.RegionSize];
                            if (Win32.ReadProcessMemory(selectedProcess.Handle, (IntPtr)baseAddress, byData, stMBI.RegionSize, out nReadSize))
                                if (nReadSize == stMBI.RegionSize)    //如果和实际读取数相符 那么搜索数据
                                    FirstDataCompareAndInput(byData, searchBytes);
                        }
                    } else {
                        break;
                    }
                    baseAddress += stMBI.RegionSize;      //设置基地址偏移
                }

            });
        }

        /// <summary>
        /// 第二次搜索
        /// </summary>
        /// <returns></returns>
        public Task SearchAgainAsync() {
            return Task.Run(() =>
            {
                //检测线程存在
                if (CheckProcessExist() == false) { return; }

                long num = 0;                   //读取出来的值
                byte[] byRead = new byte[searchBytes];      //相应字节数组
                int numBytesRead = 0;           //实际读取字节数
                int index = 0;                              //循环的索引
                int loopLen = address.Count;    //循环的长度

                //开始在上次搜索数据中 进行符合条件的筛选
                for (index = address.Count - 1; index >= 0; index--) {
                    if (Win32.ReadProcessMemory(selectedProcess.Handle, (IntPtr)address[index], byRead, searchBytes, out numBytesRead)) {
                        num = byRead[searchBytes - 1];
                        for (int j = searchBytes, k = 2; j > 1; j--, k++) {
                            num = num << 8;
                            num = num | byRead[searchBytes - k];
                        }
                        if (num != searchedData)
                            address.RemoveAt(index);
                    }
                }

            });
        }

        /// <summary>
        /// 首次搜索中的数据比较
        /// </summary>
        /// <param name="byData"></param>
        /// <param name="typeOfByte"></param>
        private void FirstDataCompareAndInput(byte[] byData, int typeOfByte) {

            long num = 0;       //内存中读取上来的相应字节数据的值
            for (int i = 0, len = byData.Length - typeOfByte; i < len; i++) {
                num = byData[i + typeOfByte - 1];
                for (int j = typeOfByte, k = 2; j > 1; j--, k++) {
                    num = num << 8;
                    num = num | byData[i + typeOfByte - k];
                }

                //满足相应条件把地址保存起来
                if (num == searchedData)
                    address.Add(baseAddress + i);

            }
        }

        /// <summary>
        /// 搜索完成
        /// </summary>
        public void StopSearching() {
            IsFinsihSearch = true;
        }

        #endregion

        #region 获取数据和写入数据段

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <returns></returns>
        public Task WriteDataAsync() {
            return Task.Run(() =>
            {
                if (IsFinsihSearch == false) { return; }

                //循环写入每个地址
                foreach (int item in address) {

                    byte[] byWrite = new byte[searchBytes];
                    long numWrite = writeData;
                    int numAddr = item;
                    int numWriteSize = 0;
                    //将数据写入byte数组中
                    for (int i = 0; i < searchBytes; i++) {
                        byWrite[i] = (byte)((numWrite & (0x00000000000000FF << i * 8)) >> i * 8);
                    }
                    if (Win32.WriteProcessMemory(selectedProcess.Handle, (IntPtr)numAddr, byWrite, searchBytes, out numWriteSize)) {
                        if (numWriteSize == searchBytes) { //如果和实际写入字节数一样,写入成功
                        }
                    } else {
                        throw new NotImplementedException("写入失败");
                    }

                }
            });
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        public Task<List<long>> ReadDataAsync() {
            Task<List<long>> t = new Task<List<long>>(() =>
            {
                if (IsFinsihSearch == false) { return new List<long>(); }

                //返回的数据
                var returnData = new List<long>();

                long num = 0;                   //读取出来的值
                byte[] byRead = new byte[searchBytes];      //相应字节数组
                int numBytesRead = 0;           //实际读取字节数
                int index = 0;                              //循环的索引
                int loopLen = address.Count;    //循环的长度

                //开始在上次搜索数据中 进行符合条件的筛选
                for (index = 0; index < loopLen; index++) {
                    if (Win32.ReadProcessMemory(selectedProcess.Handle, (IntPtr)address[index], byRead, searchBytes, out numBytesRead)) {
                        num = byRead[searchBytes - 1];
                        for (int j = searchBytes, k = 2; j > 1; j--, k++) {
                            num = num << 8;
                            num = num | byRead[searchBytes - k];
                        }

                        //写入数据
                        returnData.Add(num);
                    }
                }

                return returnData;
            });
            t.Start();
            return t;
        }

        #endregion

        #region 杂项

        /// <summary>
        /// 取一组数的众数
        /// </summary>
        /// <param name="list">数列表</param>
        /// <returns></returns>
        public long Mode(List<long> list) {
            if (list.Count == 0) { return 0; }

            //number
            var cache1 = new List<long>();
            //vote
            var cache2 = new List<int>();
            foreach (long item in list) {
                if (cache1.IndexOf(item) < 0) {
                    cache1.Add(item);
                    cache2.Add(1);
                } else {
                    cache2[cache1.IndexOf(item)] += 1;
                }
            }

            return cache1[cache2.IndexOf(cache2.Max())];
        }

        /// <summary>
        /// 检查进程是否可以读取
        /// </summary>
        /// <returns></returns>
        private bool CheckProcessExist() {
            if (selectedProcess != null && selectedProcess.HasExited == false) return true;
            else return false;
        }

        #endregion

    }


}
