using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:条件反射a
    /// 卡牌能力:对方下个卡牌为铜色单位时自动掀开，在其效果发动后将其与我方点数最小的铜色单位弹回卡组顶端
    /// </summary>
    public class CardM_F1_3C_007 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Cover));
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   

               })
               .AbilityAppend();
        }
    }
}