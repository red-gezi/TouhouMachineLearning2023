using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.Config
{
    public class CardAbilityConfig
    {
        private Card card;
        private TriggerTime time;
        private TriggerType type;
        List<Func<Event, Task>> abilitys = new List<Func<Event, Task>>();
        List<AbilityCondition> conditions = new List<AbilityCondition>();
        public CardAbilityConfig(Card card, TriggerTime time, TriggerType type)
        {
            this.card = card;
            this.time = time;
            this.type = type;
        }
        public CardAbilityConfig AbilityAdd(Func<Event, Task> ability, params Condition[] condition)
        {
            abilitys.Add(ability);
            conditions.Add(new AbilityCondition(condition));
            return this;
        }
        class AbilityCondition
        {
            private List<Condition> conditions;
            public AbilityCondition(Condition[] condition) => conditions = condition.ToList();
            //后续更新
            public bool IsAbilityActive(Card card)
            {
                bool isAbilityActive = true;
                if (conditions.Contains(Condition.Default))
                {
                    conditions.AddRange(new Condition[] { Condition.NotDead, Condition.NotSeal, Condition.OnBattle });
                }
                isAbilityActive &= JudgeAbilityActive(card, Condition.Dead, card.ShowPoint == 0);
                isAbilityActive &= JudgeAbilityActive(card, Condition.NotDead, card.ShowPoint > 0);
                isAbilityActive &= JudgeAbilityActive(card, Condition.NotSeal, !card[CardState.Seal]);
                isAbilityActive &= JudgeAbilityActive(card, Condition.OnUse, Info.AgainstInfo.cardSet[GameRegion.Used].CardList.Contains(card));
                isAbilityActive &= JudgeAbilityActive(card, Condition.OnHand, Info.AgainstInfo.cardSet[GameRegion.Hand].CardList.Contains(card));
                isAbilityActive &= JudgeAbilityActive(card, Condition.OnDeck, Info.AgainstInfo.cardSet[GameRegion.Deck].CardList.Contains(card));
                isAbilityActive &= JudgeAbilityActive(card, Condition.OnGrave, Info.AgainstInfo.cardSet[GameRegion.Grave].CardList.Contains(card));
                isAbilityActive &= JudgeAbilityActive(card, Condition.OnBattle, Info.AgainstInfo.cardSet[GameRegion.Battle].CardList.Contains(card));
                isAbilityActive &= JudgeAbilityActive(card, Condition.OnMyRegion, Info.AgainstInfo.cardSet[Orientation.My].CardList.Contains(card));
                isAbilityActive &= JudgeAbilityActive(card, Condition.OnOpRegion, Info.AgainstInfo.cardSet[Orientation.Op].CardList.Contains(card));
                return isAbilityActive;
            }

            /// <summary>
            /// 根据触发条件和卡牌状态返回是否满足该条件
            /// </summary>
            private bool JudgeAbilityActive(Card card, Condition condition, bool abilityActiveCondition) => conditions.Contains(condition) ? abilityActiveCondition : true;
        }
        /// <summary>
        /// 以附加的方式将能力追加
        /// </summary>
        public void AbilityAppend()
        {
            List<AbilityCondition> currentConditions = conditions;
            for (int i = 0; i < abilitys.Count; i++)
            {
                int num = i;
                card.CardAbility[time][type].Add(
                async (e) =>
                {
                    AbilityCondition abilityCondition = currentConditions[num];
                    if (conditions[num].IsAbilityActive(card))
                    {
                        await abilitys[num](e);
                    }
                });
            }
        }
        /// <summary>
        /// 以替换的方式将能力覆盖原来的效果
        /// </summary>
        public void AbilityReplace()
        {
            card.CardAbility[time][type].Clear();
            AbilityAppend();
        }
    }
}