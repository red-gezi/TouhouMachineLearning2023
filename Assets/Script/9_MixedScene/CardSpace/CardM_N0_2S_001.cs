using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:三月精*日
    /// 卡牌能力:部署:设置自身活力为2, 并召唤三月精*月，三月精*星 被召唤时:设置自身活力为2
    /// </summary>
    public class CardM_N0_2S_001 : Card
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
            //部署效果
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (e) =>
             {
                 await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Inspire, 2));
                 await GameSystem.TransferSystem.SummonCard(
                     new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].ContainCardList
                     .Where(card => card.CardID == "CardM_N0_2S_002" || card.CardID == "CardM_N0_2S_003")
                     .ToList())
                     );
             }, Condition.Default)
             .AbilityAppend();
            //被召唤时效果
            AbalityRegister(TriggerTime.When, TriggerType.Summon)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Inspire, 2));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}