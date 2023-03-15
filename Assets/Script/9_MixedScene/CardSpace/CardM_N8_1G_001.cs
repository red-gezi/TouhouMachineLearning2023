using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:反狱王
    /// 卡牌能力:部署：在我方手牌中生成一张旧地狱怨灵
    /// </summary>
    public class CardM_N8_1G_001 : Card
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
                   await GameSystem.TransferSystem.GenerateCard(new Event(this, targetCard: null).SetTargetCardId("2081002").SetLocation(CurrentOrientation, GameRegion.Hand, -1));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}