using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Config;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary
{
    public class Card : MonoBehaviour
    {
        #region 卡牌基础属性和字段
        /// <summary>
        /// 对卡牌进行相关交互和数据刷新的管理器
        /// </summary>
        public CardInGameManager Manager => GetComponent<CardInGameManager>();
        private void Awake() => Manager.Init();
        ////////////////////////////////////////卡牌基本属性/////////////////////////////////////////////////////////////
        public string CardID { get; set; }

        public int BasePoint { get; set; }
        public int ChangePoint { get; set; }
        public int ShowPoint => Mathf.Max(0, BasePoint + ChangePoint);
        //卡牌类型
        public CardType Type { get; set; }
        //卡牌品质
        public CardRank Rank { get; set; }
        public Texture2D CardFace { get; set; }
        public Texture2D CardBack { get; set; }
        //卡牌默认可部署属性区域
        public BattleRegion CardDeployRegion { get; set; }
        //卡牌默认可部署所属
        public Territory CardDeployTerritory { get; set; }
        [ShowInInspector]
        public string TranslateName => CardAssemblyManager.GetCurrentCardInfos(CardID).TranslateName;
        [ShowInInspector]
        public string TranslateAbility => CardAssemblyManager.GetCurrentCardInfos(CardID).TranslateAbility;
        //经过翻译后的描述
        [ShowInInspector]
        public string TranslateDescribe => CardAssemblyManager.GetCurrentCardInfos(CardID).TranslateDescribe;
        //经过翻译后的标签集合
        [ShowInInspector]
        public string TranslateTags { get; set; }
        //卡牌能力效果
        public Dictionary<TriggerTime, Dictionary<TriggerType, List<Func<Event, Task>>>> CardAbility { get; set; } = new();
        //卡牌附加状态
        [ShowInInspector]
        public List<CardState> cardStates = new ();
        public bool this[CardState cardState]
        {
            get => cardStates.Contains(cardState);
            set
            {
                if (value)
                {
                    if (!cardStates.Contains(cardState))
                    {
                        cardStates.Add(cardState);
                    }
                }
                else
                {
                    cardStates.Remove(cardState);
                }
            }
        }
        //卡牌附加值
        [ShowInInspector]
        public Dictionary<CardField, int> cardFields = new ();
        public int this[CardField cardField]
        {
            get => cardFields.ContainsKey(cardField) ? cardFields[cardField] : 0;
            set
            {
                cardFields[cardField] = value;
                if (value <= 0)
                {
                    cardFields.Remove(cardField);
                }
            }
        }
        ////////////////////////////////////////卡牌状态/////////////////////////////////////////////////////////////

        //是否以灰色状态显示
        public bool IsGray { get; set; } = false;
        public bool IsCardReadyToGrave => ShowPoint == 0 && AgainstInfo.GameCardsFilter[GameRegion.Battle].ContainCardList.Contains(this);
        //决定卡牌落下阶段是否
        public bool isMoveStepOver = true;
        //决定卡牌抽卡阶段是否完成
        public bool isDrawStepOver = true;
        //卡牌准备打出
        public bool isPrepareToPlay = false;
        //处于系统控制移动状态
        public bool IsSystemControlMove => this != AgainstInfo.playerPrePlayCard;
       

        ////////////////////////////////////////卡牌延生信息/////////////////////////////////////////////////////////////
        //卡牌当前所属
        public Territory CardCurrentTerritory => AgainstInfo.GameCardsFilter[Orientation.Down].ContainCardList.Contains(this) ? Territory.My : Territory.Op;
        [ShowInInspector]
        //卡牌默认可部署所属
        public Orientation CurrentOrientation => AgainstInfo.GameCardsFilter[Orientation.Down].ContainCardList.Contains(this) ? Orientation.Down : Orientation.Up;
        public Orientation OppositeOrientation => CurrentOrientation == Orientation.Down ? Orientation.Up : Orientation.Down;
        //获取卡牌所在区域
        public GameRegion CurrentRegion => AgainstInfo.GameCardsFilter.ContainRowInfos.First(row => row.CardList.Contains(this)).gameRegion;
        //卡牌的当前顺序
        public int CurrentIndex => AgainstInfo.GameCardsFilter.ContainRowInfos.First(row => row.CardList.Contains(this)).CardList.IndexOf(this);

        //按水->土->风->火->水的顺序获取下一个区域属性
        public GameRegion LastBattleRegion => CurrentRegion switch
        {
            GameRegion.Water => GameRegion.Soil,
            GameRegion.Fire => GameRegion.Water,
            GameRegion.Wind => GameRegion.Fire,
            GameRegion.Soil => GameRegion.Wind,
            _ => CurrentRegion,
        };
        //按水->火->风->土->水的顺序获取下一个区域属性
        public GameRegion NextBattleRegion => CurrentRegion switch
        {
            GameRegion.Water => GameRegion.Fire,
            GameRegion.Fire => GameRegion.Wind,
            GameRegion.Wind => GameRegion.Soil,
            GameRegion.Soil => GameRegion.Water,
            _ => CurrentRegion,
        };
        
        

        public List<Card> BelongCardList => RowCommand.GetRowInfo(this).CardList;
        public RowInfo BelongRow => RowCommand.GetRowInfo(this);

        public Location Location => RowCommand.GetLocation(this);
        public Card LeftCard => Location.Rank > 0 ? BelongCardList[Location.Rank - 1] : null;
        public Card RightCard => Location.Rank < BelongCardList.Count - 1 ? BelongCardList[Location.Rank + 1] : null;
        public List<Card> TwoSideCard
        {
            get
            {
                var targetList = new List<Card>();
                if (LeftCard != null) targetList.Add(LeftCard);
                if (RightCard != null) targetList.Add(RightCard);
                return targetList;
            }
        }
        /// <summary>
        /// 在卡牌能力面板上显示的卡牌引用
        /// </summary>
        [ShowInInspector]
        public List<string> refCardIDs { get; set; } = new();
        public Text PointText => transform.GetChild(0).GetChild(0).GetComponent<Text>();
        public Transform FieldIconContent => transform.GetChild(0).GetChild(1);
        public Transform StateIconContent => transform.GetChild(0).GetChild(2);
       
        
        #endregion
        #region 卡牌能力注册
        /// <summary>
        /// 注册一个能力
        /// </summary>
        /// <param name="time">触发时机</param>
        /// <param name="type">触发方式</param>
        /// <returns>一个触发效果的配置管理器</returns>
        public CardAbilityConfig AbalityRegister(TriggerTime time, TriggerType type) => new Config.CardAbilityConfig(this, time, type);

        /// <summary>
        /// 初始化和注册默认共通的卡牌效果
        /// </summary>
        public virtual void Init()
        {
            ///////////////////////////////////////////////////////初始化////////////////////////////////////////////////////////////////////////

            //初始化卡牌效果集合并填充空效果
            foreach (TriggerTime tirggerTime in Enum.GetValues(typeof(TriggerTime)))
            {
                CardAbility[tirggerTime] = new Dictionary<TriggerType, List<Func<Event, Task>>>();
                foreach (TriggerType triggerType in Enum.GetValues(typeof(TriggerType)))
                {
                    CardAbility[tirggerTime][triggerType] = new List<Func<Event, Task>>();
                }
            }

            //////////////////////////////////////////////////配置通用响应效果///////////////////////////////////////////////////////////////////
            #region 所属移动
            //当创造时从牌库中构建
            AbalityRegister(TriggerTime.When, TriggerType.Generate).AbilityAdd(CardCommand.GenerateCard).AbilityAppend();
            //当抽牌时从牌库转移至手牌
            AbalityRegister(TriggerTime.When, TriggerType.Draw).AbilityAdd(CardCommand.DrawCard).AbilityAppend();
            //当弃牌时移动至墓地
            AbalityRegister(TriggerTime.When, TriggerType.Discard).AbilityAdd(CardCommand.DisCard).AbilityAppend();
            //当死亡时移至墓地
            AbalityRegister(TriggerTime.When, TriggerType.Dead).AbilityAdd(CardCommand.DeadCard).AbilityAppend();
            //当复活时移至出牌区
            AbalityRegister(TriggerTime.When, TriggerType.Revive).AbilityAdd(CardCommand.ReviveCard).AbilityAppend();
            //当复活时移至出牌区
            AbalityRegister(TriggerTime.When, TriggerType.Reback).AbilityAdd(CardCommand.RebackCard).AbilityAppend();
            //当移动时移到指定位置
            AbalityRegister(TriggerTime.When, TriggerType.Move).AbilityAdd(CardCommand.MoveCard).AbilityAppend();
            //当间隙时从游戏中除外
            AbalityRegister(TriggerTime.When, TriggerType.Banish).AbilityAdd(CardCommand.BanishCard).AbilityAppend();
            //当召唤时从卡组中拉出
            AbalityRegister(TriggerTime.When, TriggerType.Summon).AbilityAdd(CardCommand.SummonCard).AbilityAppend();
            #endregion
            #region 点数变动
            //当设置点数时，修改变更点数值
            AbalityRegister(TriggerTime.When, TriggerType.Set).AbilityAdd(CardCommand.Set).AbilityAppend();
            //当获得增益时获得点数增加
            AbalityRegister(TriggerTime.When, TriggerType.Gain).AbilityAdd(CardCommand.Gain).AbilityAppend();
            //默认受伤效果：当卡牌受到伤害时则会受到伤害，当卡牌死亡时，触发卡牌死亡机制
            AbalityRegister(TriggerTime.When, TriggerType.Hurt).AbilityAdd(CardCommand.Hurt).AbilityAppend();
            //当被治愈时触发被治愈效果
            AbalityRegister(TriggerTime.When, TriggerType.Cure).AbilityAdd(CardCommand.Cure).AbilityAppend();
            //当被摧毁时以自身点数对自己造成伤害
            AbalityRegister(TriggerTime.When, TriggerType.Destory).AbilityAdd(CardCommand.Destory).AbilityAppend();
            //当点数逆转时触发
            AbalityRegister(TriggerTime.When, TriggerType.Reverse).AbilityAdd(CardCommand.Reversal).AbilityAppend();
            #endregion
            #region 附加状态
            //卡牌状态附加时效果
            AbalityRegister(TriggerTime.When, TriggerType.StateAdd).AbilityAdd(CardCommand.StateAdd).AbilityAppend();
            //卡牌状态取消时效果
            AbalityRegister(TriggerTime.When, TriggerType.StateClear).AbilityAdd(CardCommand.StateClear).AbilityAppend();
            #endregion
            #region 附加字段
            //卡牌字段设置时效果
            AbalityRegister(TriggerTime.When, TriggerType.FieldSet).AbilityAdd(CardCommand.FieldSet).AbilityAppend();
            //卡牌字段改变时效果
            AbalityRegister(TriggerTime.When, TriggerType.FieldChange).AbilityAdd(CardCommand.FieldChange).AbilityAppend();
            #endregion
            #region 流程响应
            //结算卡牌的回合开始时触发的自动类型效果
            AbalityRegister(TriggerTime.After, TriggerType.TurnEnd).AbilityAdd(CardCommand.TurnEnd).AbilityAppend();
            //小局结束时，移入所有卡牌至墓地
            AbalityRegister(TriggerTime.After, TriggerType.RoundEnd).AbilityAdd(CardCommand.RoundEnd).AbilityAppend();
            #endregion
        }
        #endregion
        #region 测试
        [Button]
        public void AddStateAndField()
        {
            for (int i = 1; i < 8; i++)
            {
                cardFields.Add((CardField)i, i);
            }
            for (int i = 1; i < 18; i++)
            {
                cardStates.Add((CardState)i);
            }
        }

        //[Button] public void 水() => _ = GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Water));
        //[Button] public void 火() => _ = GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Fire));
        //[Button] public void 风() => _ = GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Wind));
        //[Button] public void 土() => _ = GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Soil));
        #endregion
    }
}