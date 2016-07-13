using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BallanceOnline {
    public class StringGroup {
        /// <summary>
        /// 源数据
        /// </summary>
        private string _sourceString;

        /// <summary>
        /// 源数据
        /// </summary>
        public string sourceString { get { return _sourceString; } }

        /// <summary>
        /// 分割文本
        /// </summary>
        private string _SplitString;

        /// <summary>
        /// 分割文本
        /// </summary>
        public string SplitString { get { return _SplitString; } }

        #region 初始化

        /// <summary>
        /// 以指定字符串初始化
        /// </summary>
        /// <param name="value">字符</param>
        /// <param name="splitValue">分割字符</param>
        public StringGroup(string value, string splitValue) {
            _sourceString = value;
            _SplitString = splitValue;
        }

        /// <summary>
        /// 以指定字符组初始化
        /// </summary>
        /// <param name="value">字符</param>
        /// <param name="splitValue">分割字符</param>
        public StringGroup(string[] value, string splitValue) {
            _sourceString = string.Join(splitValue, value);
            _SplitString = splitValue;
        }

        /// <summary>
        /// 以arraylist初始化
        /// </summary>
        /// <param name="value">字符</param>
        /// <param name="splitValue">分割字符</param>
        public StringGroup(ArrayList value, string splitValue) {
            foreach (object a in value) {
                string cache = a.ToString();
                if (_sourceString == "") { _sourceString = cache; } else {
                    _sourceString += splitValue;
                    _sourceString += cache;
                }
            }
            _SplitString = splitValue;
        }

        /// <summary>
        /// 以list初始化
        /// </summary>
        /// <param name="value">字符</param>
        /// <param name="splitValue">分割字符</param>
        public StringGroup(List<string> value, string splitValue) {
            foreach (string a in value) {
                if (_sourceString == "") { _sourceString = a; } else {
                    _sourceString += splitValue;
                    _sourceString += a;
                }
            }
            _SplitString = splitValue;
        }


        ///// <summary>
        ///// 以bigarraylist初始化
        ///// </summary>
        ///// <param name="value">字符</param>
        ///// <param name="splitValue">分割字符</param>
        //public StringGroup(BigArrayList value, string splitValue) {
        //    for (int a = 0; a < value.Count; a++) {
        //        if (_sourceString == "") { _sourceString = value.Item(a).ToString(); } else {
        //            _sourceString += splitValue;
        //            _sourceString += value.Item(a).ToString();
        //        }
        //    }
        //    _SplitString = splitValue;
        //}

        #endregion

        #region 输出

        /// <summary>
        /// 常规输出
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return _sourceString;
        }

        /// <summary>
        /// 输出到字符组
        /// </summary>
        /// <returns></returns>
        public string[] ToStringGroup() {
            if (_sourceString == "") { return null; }
            return Regex.Split(_sourceString, _SplitString, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 输出到arraylist
        /// </summary>
        /// <returns></returns>
        public ArrayList ToArrayList() {
            if (_sourceString == "") { return null; }

            ArrayList sendOut = new ArrayList();
            string[] sp = Regex.Split(_sourceString, _SplitString, RegexOptions.IgnoreCase);
            foreach (string a in sp) {
                sendOut.Add(a);
            }

            return sendOut;
        }

        /// <summary>
        /// 输出到arraylist
        /// </summary>
        /// <returns></returns>
        public List<string> ToList() {
            if (_sourceString == "") { return null; }

            List<string> sendOut = new List<string>();
            string[] sp = Regex.Split(_sourceString, _SplitString, RegexOptions.IgnoreCase);
            foreach (string a in sp) {
                sendOut.Add(a);
            }

            return sendOut;
        }

        ///// <summary>
        ///// 输出到bigarraylist
        ///// </summary>
        ///// <returns></returns>
        //public BigArrayList ToBigArrayList() {
        //    if (_sourceString == "") { return null; }

        //    BigArrayList sendOut = new BigArrayList();
        //    string[] sp = Regex.Split(_sourceString, _SplitString, RegexOptions.IgnoreCase);
        //    foreach (string a in sp) {
        //        sendOut.Add(a);
        //    }

        //    return sendOut;
        //}

        /// <summary>
        /// 输出到一个新的分割符文本
        /// </summary>
        /// <param name="splitValue">新分隔符</param>
        /// <returns></returns>
        public string ToNewSplitWord(string splitValue) {
            if (_sourceString == "") { return null; }
            if (splitValue == _SplitString) { return _sourceString; }

            return _sourceString.Replace(_SplitString, splitValue);
        }

        #endregion


    }


}
