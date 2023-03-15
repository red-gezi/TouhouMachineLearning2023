using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    //负责管理每个区域的Card位置，区域显示状态等
    public class RowManager : MonoBehaviour
    {
        public Color color;
        public Card TempCard;
        public Orientation orientation;
        public GameRegion gameRegion;
        public bool CanBeSelected;
        public float Range;
        public bool IsMyHandRegion;
        //当前行管理其所管理的的Card列表
        public List<Card> CardList => AgainstInfo.cardSet[RowRank];
        public Material RowMaterial;
        //只有一个Card位
        bool IsSingle => gameRegion == GameRegion.Grave || gameRegion == GameRegion.Deck || gameRegion == GameRegion.Used;
        [ShowInInspector]
        //计算在全局卡组中对应的顺序
        //根据玩家扮演角色（1或者2）分配上方区域和下方区域
        public int RowRank => (int)gameRegion + (AgainstInfo.IsPlayer1 ^ (orientation == Orientation.Down) ? 9 : 0);
        //判断玩家的焦点区域位置次序
        public int Rank
        {
            get
            {
                int Rank = 0;
                float posx = -(AgainstInfo.FocusPoint.x - this.transform.position.x);
                int UnitsNum = CardList.Where(card => !card.IsGray).Count();
                for (int i = 0; i < UnitsNum; i++)
                {
                    if (posx > i * 1.6 - (UnitsNum - 1) * 0.8)
                    {
                        Rank = i + 1;
                    }
                }
                return Rank;
            }
        }
        private void Awake() => AgainstInfo.cardSet.RowManagers.Add(this);
        private void Start() => RowMaterial = transform.GetComponent<Renderer>().material;
        void Update()
        {
            if (AgainstInfo.IsMyTurn)
            {
                //创建临时Card
                if (TempCard == null && CanBeSelected && AgainstInfo.PlayerFocusRegion == this && TempCard == null)
                {
                    Card modelCard = AgainstInfo.cardSet[Orientation.My][GameRegion.Used].CardList.LastOrDefault();
                    TempCard = Command.CardCommand.GenerateTempCard(new Event(null, targetCard: null).SetTargetCardId(modelCard.CardID).SetLocation(orientation, gameRegion, Rank));
                }
                //改变临时Card的位置
                if (TempCard != null && Rank != CardList.IndexOf(TempCard))
                {
                    CardList.Remove(TempCard);
                    CardList.Insert(Rank, TempCard);
                }
                //销毁临时Card
                if (TempCard != null && !(CanBeSelected && AgainstInfo.PlayerFocusRegion == this))
                {
                    CardList.Remove(TempCard);
                    Destroy(TempCard.gameObject);
                    TempCard = null;
                }
            }
            //可部署时区域高亮
            RowMaterial.SetFloat("_Strength", Mathf.PingPong(Time.time * 10, 10) + 10);
        }
        public void RefreshRowCards()
        {
            //设置Card可见性
            CardList.ForEach(card => card.Manager.RefreshCardVisible());
            //设置点数
            CardList.ForEach(card => card.Manager.RefreshCardUi());
            //执行位置、角度移动动画
            CardList.ForEach(card => card.Manager.RefreshTransform());
        }
    }
}