using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:今泉影狼
    /// 卡牌能力:暴躁：对对方同排所有目标获得一点伤害 温顺：按正顺序移动至下一排
    /// </summary>
    public class CardM_N1_2S_003 : Card
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

                       await GameSystem.PointSystem.Hurt(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][CurrentRegion].ContainCardList).SetPoint(1).SetMeanWhile());

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

                       await GameSystem.TransferSystem.MoveCard(new Event(this, this).SetLocation(Orientation.My, NextBattleRegion, -1));
                   }
               }, Condition.Default)
                .AbilityAppend();
        }
    }
}