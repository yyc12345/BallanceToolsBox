using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceOnline {
    public class MD5 {
        /// <summary>
        /// 获取文件的MD5
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>MD5</returns>
        public static string GetMD5HashFromFile(string fileName) {
            try {
                FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++) {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            } catch (Exception) {
                return "";
            }
        }
    }
}
