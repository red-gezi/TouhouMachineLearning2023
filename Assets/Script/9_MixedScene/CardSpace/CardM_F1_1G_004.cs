using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:被隐匿的弱点
    /// 卡牌能力:我方回合开始时：赋予对方手牌中品质最低且点数最小的单位窥视状态
    /// </summary>
    public class CardM_F1_1G_004 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.TurnStart)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectUnit(this,GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Hand][CardFeature.LowestRankUnits, CardFeature.LowestPointUnits].CardList,1,true);
                   await GameSystem.StateSystem.SetState(new Event(this, GameSystem.InfoSystem.SelectUnit).SetTargetState( CardState.Pry));
               }, Condition.Default, Condition.OnMyRegion)
               .AbilityAppend();
        }
    }
}