using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:静坐祈福
    /// 卡牌能力:回合结束：我方所有奇迹牌获得一点祈祷值
    /// </summary>
    public class CardM_S0_3C_001 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
              .AbilityAdd(async (e) =>
              {
                  await GameSystem.FieldSystem.ChangeField(new Event(this, Info.AgainstInfo.cardSet[Orientation.My][CardTag.Miracle].CardList).SetTargetField(CardField.Pary, 1).SetMeanWhile());
              }, Condition.Default, Condition.OnMyRegion)
              .AbilityAppend();
        }
    }
}