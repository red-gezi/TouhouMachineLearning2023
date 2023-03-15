using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:手摇发电机
    /// 卡牌能力:位于场上时，我方每部署一张单位，在其部署效果生效前，该牌能量点数+1
    /// </summary>
    public class CardM_T0_2S_001 : Card
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
            AbalityRegister(TriggerTime.Before, TriggerType.Deploy)
              .AbilityAdd(async (e) =>
              {
                  await GameSystem.FieldSystem.ChangeField(new Event(this, this).SetTargetField(CardField.Energy, 1));
              }, Condition.Default, Condition.OnMyRegion)
              .AbilityAppend();
        }
    }
}