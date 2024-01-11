using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class RowInfo
    {
        public Color color;
        public Card TempCard;
        public Orientation orientation;
        public GameRegion gameRegion;
        public bool CanBeSelected;
        public float Range;
        public bool IsMyHandRegion;
        public GameObject rowPrefab;
        public Material RowMaterial => rowPrefab.GetComponent<Renderer>().material;
        //当前行管理其所管理的的Card列表
        public List<Card> CardList = new List<Card>();
        //牌以单张重叠显示还是展开
        bool IsSingle => gameRegion == GameRegion.Grave || gameRegion == GameRegion.Deck || gameRegion == GameRegion.Used;
        //计算玩家区域编号，会根据玩家扮演角色不同（1或者2）而变化
        public int RowRank => (int)gameRegion + (AgainstInfo.IsPlayer1 ^ (orientation == Orientation.Down) ? 9 : 0);
        //鼠标当前焦点位置
        public int FocusRank => RowCommand.GetFocusRank(this);


    }
}