using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary
{
    //class Event { }
    public class Event
    {
        public TriggerTime TriggerTime { get; set; }
        public TriggerType TriggerType { get; set; }
        public Card TriggerCard { get; set; }
        public List<Card> TargetCards { get; set; }
        public CardState TargetState { get; set; }
        public CardField TargetFiled { get; set; }
        public string TargetCardId { get; set; }
        /// <summary>
        /// 判断多个卡牌目标是否同时触发效果
        /// </summary>
        public bool triggerMeanWhile = false;
        [JsonIgnore]
        public Card TargetCard => TargetCards.FirstOrDefault();
        public int point;
        public Location location = new Location(-1, -1);
        public Orientation orientation = Orientation.My;
        public GameRegion region = GameRegion.None;
        public bool isOrder = true;
        public string carkBackID;
        public BulletModel BulletModel { get; set; }
        [JsonIgnore]
        public Event this[TriggerTime triggerTime] => Clone(triggerTime: triggerTime);
        [JsonIgnore]
        public Event this[TriggerType triggerType] => Clone(triggerType: triggerType);
        [JsonIgnore]
        public Event this[Card targetCard] => Clone(targetCards: new List<Card> { targetCard });
        public Event this[List<Card> targetCards] => Clone(targetCards: targetCards);

        private Event Clone(TriggerTime? triggerTime = null, TriggerType? triggerType = null, List<Card> targetCards = null)
        {
            Event e = new Event(TriggerCard, targetCards);
            e.TriggerTime = triggerTime ?? this.TriggerTime;
            e.TriggerType = triggerType ?? this.TriggerType;
            e.TargetCards = targetCards ?? this.TargetCards;
            e.triggerMeanWhile = triggerMeanWhile;
            e.TriggerCard = TriggerCard;
            e.BulletModel = BulletModel;
            e.location = location;
            e.point = point;
            e.TargetState = TargetState;
            e.TargetFiled = TargetFiled;
            e.TargetCardId = TargetCardId;
            return e;
        }
        //反序列化时使用
        public Event() { }
        /// <summary>
        /// 创建一个卡牌触发信息模板，并设置触发者（某卡牌,若是由系统触发则填null）、触发对象(单个)
        /// </summary>
        public Event(Card triggerCard = null, Card targetCard = null)
        {
            this.TriggerCard = triggerCard;
            TargetCards = new List<Card>();
            if (targetCard != null)
            {
                TargetCards.Add(targetCard);
            }
        }
        /// <summary>
        /// 创建一个卡牌触发信息模板，并设置触发者（某卡牌,若是由系统触发则填null）、触发对象(多个)
        /// </summary>
        public Event(Card triggerCard, List<Card> targetCards)
        {
            this.TriggerCard = triggerCard;
            this.TargetCards = targetCards?.ToList();
        }
        /// <summary>
        /// 设置部署区域（靠所属，区域和次序定位，次序为正代表从左往右，最左侧位置为0，为负代表从右往左，最右侧为-1）
        /// </summary>
        public Event SetLocation(Orientation orientation, GameRegion regionType, int index=0)
        {
            int x = GameSystem.InfoSystem.AgainstCardSet[regionType][orientation].ContainRowInfos.First().RowRank;
            int y = index;
            location = new Location(x, y);
            return this;
        }
        /// <summary>
        /// 设置抽卡效果的插入区域，是否触发手牌排序
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="isOrder"></param>
        /// <returns></returns>
        public Event SetDrawCard(Orientation orientation, bool isOrder)
        {
            this.orientation = orientation;
            this.isOrder = isOrder;
            return this;
        }
        ///// <summary>
        ///// 设置卡牌转移区域，不会触发衍生效果
        ///// </summary>
        ///// <param name="orientation"></param>
        ///// <param name="isOrder"></param>
        ///// <returns></returns>
        //public Event SetTransferRegion(Orientation orientation, GameRegion region)
        //{
        //    this.orientation = orientation;
        //    this.region = region;
        //    return this;
        //}
        /// <summary>
        /// 设置触发点数信息
        /// </summary>
        public Event SetPoint(int point)
        {
            this.point = point;
            return this;
        }
        public Event SetCardBack(string cardBackId)
        {
            this.carkBackID = cardBackId;
            return this;
        }
        public Event SetBullet(BulletModel bulletModel)
        {
            this.BulletModel = bulletModel;
            return this;
        }
        /// <summary>
        /// 以同时的方式触发弹幕和卡牌效果,触发后再进行结算
        /// </summary>
        /// <returns></returns>
        public Event SetMeanWhile()
        {
            triggerMeanWhile = true;
            return this;
        }
        /// <summary>
        /// 设置目标状态
        /// </summary>
        /// <param name="targetState"></param>
        /// <returns></returns>
        public Event SetTargetState(CardState targetState)
        {
            this.TargetState = targetState;
            return this;
        }
        /// <summary>
        ///设置目标字段和目标值
        /// </summary>
        public Event SetTargetField(CardField targetField, int ponit)
        {
            TargetFiled = targetField;
            point = ponit;
            return this;
        }
        /// <summary>
        /// 设置目标ID
        /// </summary>
        /// <param name="targetState"></param>
        /// <returns></returns>
        public Event SetTargetCardId(string cardId)
        {
            TargetCardId = cardId;
            return this;
        }
    }
}