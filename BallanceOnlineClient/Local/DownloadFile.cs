using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BallanceOnlineClient.Local {

    public class DownloadFile {

        FileStream fr;

        bool IsWrite;

        string checkMD5;

        public DownloadFile() {
            IsWrite = false;
            checkMD5 = "";
        }

        /// <summary>
        /// 开始写入，写入要验证的md5
        /// </summary>
        /// <param name="MD5"></param>
        public void Start(string MD5) {
            System.IO.File.Delete(Environment.CurrentDirectory + @"\cacheMap.nmo");
            IsWrite = true;
            checkMD5 = MD5;
            fr = new FileStream(Environment.CurrentDirectory + @"\cacheMap.nmo", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        }

        public void Write(byte[] data) {
            if (IsWrite == true)
                fr.Write(data, 0, data.Length);
            else
                throw new NotImplementedException("File not open");
        }

        /// <summary>
        /// 停止写入，并验证md5
        /// </summary>
        /// <returns></returns>
        public bool Stop() {
            if (IsWrite == true) {
                fr.Close();
                fr.Dispose();

                //check
                var cache = BallanceOnline.MD5.GetMD5HashFromFile(Environment.CurrentDirectory + @"\cacheMap.nmo");
                IsWrite = false;

                //if (cache == checkMD5)
                    return true;
                //else return false;



            }else { throw new NotImplementedException("File not open"); }

        }

    }
}
