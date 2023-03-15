using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:若鹭姬
    /// 卡牌能力:暴躁：两侧单位受到伤害 温顺：两侧单位受到增益
    /// </summary>
    public class CardM_N1_2S_001 : Card
    {
        public override void Init()
        {
            //初始化通用响应效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Increase)
                .AbilityAdd(async (e) =>
                {
                    if (!this[CardState.Furor])
                    {
                        await GameSystem.StateSystem.ClearState(new Event(this, this).SetTargetState(CardState.Docile));
                        await GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Furor));
                    }
                }, Condition.Default)
                .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Decrease)
               .AbilityAdd(async (e) =>
               {
                   if (!this[CardState.Docile])
                   {
                       await GameSystem.StateSystem.ClearState(new Event(this, this).SetTargetState(CardState.Furor));
                       await GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Docile));
                   }
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}