using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:1
    /// 卡牌能力:牌技：打出牌库最上方点数与自身连锁值相等且品质最高的单位
    /// </summary>
    public class CardM_T1_1G_001 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (e) =>
               {
                   var targetCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].ContainCardList
                    .Where(card => card.ShowPoint == this[CardField.Chain])
                    .GroupBy(card => card.Rank)
                    .FirstOrDefault()
                    ?.FirstOrDefault();
                   await GameSystem.TransferSystem.PlayCard(new Event(this, targetCard));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}