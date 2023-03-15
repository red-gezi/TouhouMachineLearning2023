using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:四季映姬
    /// 卡牌能力:部署：选择两个单位，依次赋予黑、白状态 回合开始，赋予左侧单位黑状态，赋予右侧单位白状态
    /// </summary>
    public class CardM_N8_1G_003 : Card
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

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.NoGold].CardList, 1);
                   await GameSystem.StateSystem.SetState(new Event(this, GameSystem.InfoSystem.SelectUnit).SetTargetState(CardState.Black));
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.NoGold].CardList, 1);
                   await GameSystem.StateSystem.SetState(new Event(this, GameSystem.InfoSystem.SelectUnit).SetTargetState(CardState.White));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.TurnStart)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.StateSystem.SetState(new Event(this, LeftCard).SetTargetState(CardState.Black));
                   await GameSystem.StateSystem.SetState(new Event(this, RightCard).SetTargetState(CardState.White));
               })
               .AbilityAppend();
        }
    }
}