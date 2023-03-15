using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:九十九弁弁「高山流水」
    /// 卡牌能力:暴躁：令场上所有高于自身点数的单位点数受到一点伤害 温顺：令所有低于自身点数的单位点数加一
    /// </summary>
    public class CardM_N1_2S_004 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.Increase)
               .AbilityAdd(async (e) =>
               {
                   if (!this[CardState.Furor])//如果不处于狂躁状态
                   {
                       await GameSystem.StateSystem.ClearState(new Event(this, this).SetTargetState(CardState.Docile));
                       await GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Furor));
                   }
               }, Condition.Default)
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Decrease)
               .AbilityAdd(async (e) =>
               {
                   if (!this[CardState.Docile])//如果不处于温顺状态
                   {
                       await GameSystem.StateSystem.ClearState(new Event(this, this).SetTargetState(CardState.Furor));
                       await GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Docile));
                   }
               }, Condition.Default)
                .AbilityAppend();
        }
    }
}