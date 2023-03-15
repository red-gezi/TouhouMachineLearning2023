using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:地底的太阳
    /// 卡牌能力:计数（3）：回合开始时弱计数不为0，则减少1，并摧毁双方场上所有最强的单位
    /// </summary>
    public class CardM_F1_1G_001 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this,this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
              .AbilityAdd(async (e) =>
              {
                  await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField( CardField.Timer,3));
              })
              .AbilityAppend();
        }
    }
}