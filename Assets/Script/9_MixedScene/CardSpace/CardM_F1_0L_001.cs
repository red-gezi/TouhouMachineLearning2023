using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:古明地恋：封闭的内心
    /// 卡牌能力:部署，选择对方卡组中一个单位，赋予"封闭"状态
    /// </summary>
    public class CardM_F1_0L_001 : Card
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
                  await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Hand].CardList);
                  await GameSystem.StateSystem.SetState(new Event(this, GameSystem.InfoSystem.SelectBoardCard).SetTargetState(CardState.Close));
              })
              .AbilityAppend();
        }
    }
}