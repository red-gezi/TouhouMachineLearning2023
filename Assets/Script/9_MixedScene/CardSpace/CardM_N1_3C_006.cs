using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:狂暴之头
    /// 卡牌能力:每回合开始正序移动至最下一排最右侧并对左侧单位造成1点伤害
    /// </summary>
    public class CardM_N1_3C_006 : Card
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
                  await GameSystem.TransferSystem.MoveCard(new Event(this, this).SetLocation(OppositeOrientation, NextBattleRegion, -1));
                  await GameSystem.PointSystem.Hurt(new Event(this, this.LeftCard).SetPoint(1));
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}