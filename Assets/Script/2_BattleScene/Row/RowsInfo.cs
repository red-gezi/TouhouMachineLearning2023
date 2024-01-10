using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
//24.1.10新重构的脚本，管理数据
namespace TouhouMachineLearningSummary.Manager
{
    //负责管理每个区域的Card位置，区域显示状态等
    public class RowsInfo : MonoBehaviour
    {
        public List<GameObject> rowPrefabs;
        List<RowInfo> rowInfos;
        class RowInfo
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
            public List<Card> CardList => AgainstInfo.cardSet[RowRank];
            //牌以单张重叠显示还是展开
            bool IsSingle => gameRegion == GameRegion.Grave || gameRegion == GameRegion.Deck || gameRegion == GameRegion.Used;

            //只有一个Card位
            [ShowInInspector]
            //计算在全局卡组中对应的顺序
            //根据玩家扮演角色（1或者2）分配上方区域和下方区域
            public int RowRank => (int)gameRegion + (AgainstInfo.IsPlayer1 ^ (orientation == Orientation.Down) ? 9 : 0);

            //判断玩家的焦点区域位置次序
            public int GetFocusRank()
            {
                float posx = -(AgainstInfo.FocusPoint.x - rowPrefab.transform.position.x);
                int cardNum = CardList.Where(card => !card.IsGray).Count();
                int rank = Enumerable.Range(0, cardNum).FirstOrDefault(i => posx > i * 1.6 - (cardNum - 1) * 0.8) + 1;
                return rank;
            }
        }
        public void Init()
        {
            for (int i = 0; i < 18; i++)
            {
                rowInfos[i].rowPrefab = rowPrefabs[i];
                rowInfos[i].gameRegion = (GameRegion)(i % 9);
                rowInfos[i].orientation = i < 9 ? Orientation.Down : Orientation.Up;
                //AgainstInfo.cardSet.RowManagers.Add(rowInfos[i]);
            }
        }
        void RefreshTempCard()
        {
            rowInfos.ForEach(rowInfo =>
            {
                if (AgainstInfo.IsMyTurn)
                {
                    Card tempCard = rowInfo.TempCard;
                    int focusRank = rowInfo.GetFocusRank();
                    //创建临时Card
                    if (tempCard == null && rowInfo.CanBeSelected && AgainstInfo.PlayerFocusRegion == this)
                    {
                        Card modelCard = AgainstInfo.cardSet[Orientation.My][GameRegion.Used].CardList.LastOrDefault();
                        tempCard = Command.CardCommand.GenerateTempCard(new Event(null, targetCard: null)
                                .SetTargetCardId(modelCard.CardID)
                                .SetLocation(rowInfo.orientation, rowInfo.gameRegion, focusRank));
                    }
                    //改变临时Card的位置
                    if (tempCard != null && focusRank != rowInfo.CardList.IndexOf(tempCard))
                    {
                        rowInfo.CardList.Remove(tempCard);
                        rowInfo.CardList.Insert(focusRank, tempCard);
                    }
                    //销毁临时Card
                    if (tempCard != null && !(rowInfo.CanBeSelected && AgainstInfo.PlayerFocusRegion == this))
                    {
                        rowInfo.CardList.Remove(tempCard);
                        Destroy(tempCard.gameObject);
                        tempCard = null;
                    }
                }
                rowInfo.RowMaterial.SetFloat("_Strength", Mathf.PingPong(Time.time * 10, 10) + 10);
            });
            //可部署时区域高亮
        }
        public void RefreshRowCards()
        {
            rowInfos.ForEach(rowInfo =>
            {
                //设置Card可见性
                rowInfo.CardList.ForEach(card => card.Manager.RefreshCardVisible());
                //设置点数
                rowInfo.CardList.ForEach(card => card.Manager.RefreshCardUi());
                //执行位置、角度移动动画
                rowInfo.CardList.ForEach(card => card.Manager.RefreshTransform());
            });

        }
    }
}