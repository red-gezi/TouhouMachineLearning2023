using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:诹访大舞
    /// 卡牌能力:每有一个单位发生位移，我方奇迹卡牌获得一点祈祷值
    /// </summary>
    public class CardM_S0_2S_001 : Card
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

            AbalityRegister(TriggerTime.After, TriggerType.Move)
             .AbilityAdd(async (e) =>
             {
                 await GameSystem.FieldSystem.ChangeField(new Event(this, Info.AgainstInfo.GameCardsFilter[Orientation.My][CardTag.Miracle].ContainCardList).SetTargetField(CardField.Pary, 1).SetMeanWhile());
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}