using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:宗教传销
    /// 卡牌能力:部署：选择场上一个单位，将其移动至所在半场与该牌属性相同排的最右侧
    /// </summary>
    public class CardM_S0_3C_006 : Card
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
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.NoGold].ContainCardList, 1);
                   if (GameSystem.InfoSystem.SelectUnit != null)
                   {
                       await GameSystem.TransferSystem
                       .MoveCard(new Event(this, GameSystem.InfoSystem.SelectUnit)
                            .SetLocation(GameSystem.InfoSystem.SelectUnit.CurrentOrientation, CurrentRegion, -1));
                   }
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}