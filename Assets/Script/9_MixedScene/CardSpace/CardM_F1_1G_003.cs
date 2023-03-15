using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:被厌恶者的哲学
    /// 卡牌能力:令一个单位重新进入覆盖状态
    /// </summary>
    public class CardM_F1_1G_003 : Card
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
                    await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[CardRank.NoGold][GameRegion.Battle].CardList, 1);
                    await GameSystem.StateSystem.ChangeState(new Event(this, GameSystem.InfoSystem.SelectUnit).SetTargetState( CardState.Cover));
              })
              .AbilityAppend();
        }
    }
}