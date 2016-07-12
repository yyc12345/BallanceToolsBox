using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceOnline {
    /// <summary>
    /// 组合拆分标识符的类
    /// </summary>
    public class CombineAndSplitSign {

        //long 数据占用8byte

        public static byte[] Combine(string clientOrServerSign, string sign, string data) {
            if (data != "") {
                byte[] arraySign = Encoding.UTF8.GetBytes(clientOrServerSign + sign);
                byte[] arrayData = Encoding.UTF8.GetBytes(data);

                byte[] signLength = BitConverter.GetBytes(arraySign.LongLength);
                byte[] dataLength = BitConverter.GetBytes(arrayData.LongLength);

                //combine
                var result = new byte[arraySign.LongLength + arrayData.LongLength + signLength.LongLength + dataLength.LongLength];
                signLength.CopyTo(result, 0);
                arraySign.CopyTo(result, signLength.LongLength);
                dataLength.CopyTo(result, signLength.LongLength + arraySign.LongLength);
                arrayData.CopyTo(result, signLength.LongLength + arraySign.LongLength + dataLength.LongLength);

                return result;
            } else {
                byte[] arraySign = Encoding.UTF8.GetBytes(clientOrServerSign + sign);
                byte[] signLength = BitConverter.GetBytes(arraySign.LongLength);

                //combine
                var result = new byte[arraySign.LongLength + signLength.LongLength];
                signLength.CopyTo(result, 0);
                arraySign.CopyTo(result, signLength.LongLength);

                return result;
            }

        }

        public static byte[] Combine(string clientOrServerSign, string sign, byte[] data) {
            if (data.LongLength != 0) {
                byte[] arraySign = Encoding.UTF8.GetBytes(clientOrServerSign + sign);

                byte[] signLength = BitConverter.GetBytes(arraySign.LongLength);
                byte[] dataLength = BitConverter.GetBytes(data.LongLength);

                //combine
                var result = new byte[arraySign.LongLength + data.LongLength + signLength.LongLength + dataLength.LongLength];
                signLength.CopyTo(result, 0);
                arraySign.CopyTo(result, signLength.LongLength);
                dataLength.CopyTo(result, signLength.LongLength + arraySign.LongLength);
                data.CopyTo(result, signLength.LongLength + arraySign.LongLength + dataLength.LongLength);

                return result;
            } else {
                byte[] arraySign = Encoding.UTF8.GetBytes(clientOrServerSign + sign);
                byte[] signLength = BitConverter.GetBytes(arraySign.LongLength);

                //combine
                var result = new byte[arraySign.LongLength + signLength.LongLength];
                signLength.CopyTo(result, 0);
                arraySign.CopyTo(result, signLength.LongLength);

                return result;
            }

        }

        public static void Split(byte[] inputData, out string sign, out byte[] data) {
            //sign
            var signLength = new byte[8];
            Array.Copy(inputData, 0, signLength, 0, 8);

            long readLength = BitConverter.ToInt64(signLength, 0);
            var readBuffer = new byte[readLength];
            Array.Copy(inputData, 8, readBuffer, 0, readLength);

            sign = Encoding.UTF8.GetString(readBuffer);


            if (inputData.LongLength == 8 + readLength) {
                //没有数据区
                data = null;
                return;
            }

            //data
            var dataLength = new byte[8];
            Array.Copy(inputData, 8 + readLength, dataLength, 0, 8);

            long dataReadLength = BitConverter.ToInt64(dataLength, 0);
            data = new byte[dataReadLength];
            Array.Copy(inputData, 8 + readLength + 8, data, 0, dataReadLength);

        }

        public static string ConvertToString(byte[] list) {
            return Encoding.UTF8.GetString(list);
        }

    }
}
