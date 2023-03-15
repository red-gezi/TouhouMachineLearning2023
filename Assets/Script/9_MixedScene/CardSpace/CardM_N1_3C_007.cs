using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:温顺之头
    /// 卡牌能力:在我方同排最右侧生成一个头，每回合开始对左侧单生成一点增益
    /// </summary>
    public class CardM_N1_3C_007 : Card
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
                 await GameSystem.TransferSystem.MoveCard(new Event(this, this).SetLocation(CurrentOrientation, NextBattleRegion, -1));
                 await GameSystem.PointSystem.Gain(new Event(this, this.LeftCard).SetPoint(1));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}