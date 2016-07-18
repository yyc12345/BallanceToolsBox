using BallanceOnline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceOnlineServer.DataProcess {
    //处理获得的数据时候的所需要的临时数据
    public class DataCache {

        public DataCache() {

            previousLife = "";
            previousTime = "";
            previousUnit = "";
            previousState = PlayerState.Playing;
            dataCompareNotChangeCount = 0;

            previousUnitData = new List<PlayerUnitData>();
        }

        public string previousLife;

        public string previousTime;

        public string previousUnit;

        public string previousState;

        public int dataCompareNotChangeCount;

        /// <summary>
        /// 刚进入此小节的数据
        /// </summary>
        private List<PlayerUnitData> previousUnitData;

        /// <summary>
        /// 添加小节数据
        /// </summary>
        public void AddUnitData(string mark, string life) {
            previousUnitData.Add(new PlayerUnitData { Mark = mark, Life = life });
        }

        /// <summary>
        /// 返回记录的每节
        /// </summary>
        /// <returns></returns>
        public List<PlayerUnitData> ReturnData() {

            var returnData = new List<PlayerUnitData>();

            for (int i = 0; i < previousUnitData.Count - 1; i++) {
                returnData.Add(new PlayerUnitData {
                    Unit = (i + 1).ToString(),
                    Life = (int.Parse(previousUnitData[i + 1].Life) - int.Parse(previousUnitData[i].Life)).ToString(),
                    Mark = (int.Parse(previousUnitData[i + 1].Mark) - int.Parse(previousUnitData[i].Mark)).ToString(),
                    PerfomancePoint = (((int.Parse(previousUnitData[i + 1].Life) - int.Parse(previousUnitData[i].Life)) * 100 + (int.Parse(previousUnitData[i + 1].Mark) - int.Parse(previousUnitData[i].Mark))) / 50).ToString()
                });
            }



            return returnData;
        }


    }

}
