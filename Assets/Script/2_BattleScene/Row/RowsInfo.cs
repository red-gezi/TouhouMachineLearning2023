using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
//24.1.10新重构的脚本，管理数据
namespace TouhouMachineLearningSummary.Info
{
    //负责管理每个区域的Card位置，区域显示状态等
    public partial class RowsInfo : MonoBehaviour
    {
        public List<GameObject> rowPrefabList;
        public static List<RowInfo> rowInfoList=new List<RowInfo>();
        public List<Color> RegionColors;
        public List<float> Range;
        private void Awake() => Init();
        public void Init()
        {
            for (int i = 0; i < 18; i++)
            {
                RowInfo rowInfo = new RowInfo();
                rowInfo.rowPrefab = rowPrefabList[i];
                rowInfo.gameRegion = (GameRegion)(i % 9);
                rowInfo.orientation = i < 9 ? Orientation.Up : Orientation.Down;
                rowInfo.color = RegionColors[i%9];
                rowInfo.Range = Range[i];
                rowInfoList.Add(rowInfo);
            }
            rowInfoList.OrderBy(rowInfo => rowInfo.RowRank);
        }
    }
}