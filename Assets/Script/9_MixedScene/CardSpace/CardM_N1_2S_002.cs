using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:赤蛮奇
    /// 卡牌能力:暴躁：在敌方同排最右侧生成一个头，每回合开始正序移动至最下一排最右侧并对左侧单位造成1点伤害 温顺：在我方同排最右侧生成一个头，每回合开始对左侧单生成一点增益
    /// </summary>
    public class CardM_N1_2S_002 : Card
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

                       await GameSystem.TransferSystem.GenerateCard(new Event(this, targetCard: null).SetTargetCardId("2013006").SetLocation(CurrentOrientation, CurrentRegion, -1));
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

                       await GameSystem.TransferSystem.GenerateCard(new Event(this, targetCard: null).SetTargetCardId("2013007").SetLocation(CurrentOrientation, CurrentRegion, -1));

                   }
               }, Condition.Default)
                .AbilityAppend();
        }
    }
}