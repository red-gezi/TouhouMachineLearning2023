using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:旧地狱怨灵
    /// 卡牌能力:我方回合结束时:若自身位于手牌，则移动至对方手牌 当我方pass时 召唤至场上 部署、召唤:丢弃我方手牌中最右侧的卡牌
    /// </summary>
    public class CardM_N8_1G_002 : Card
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
                   //await GameSystem.TransferSystem.MoveToOpHand(this);
                   await GameSystem.TransferSystem.TransferCard(new Event(this, this).SetLocation(Orientation.Op, GameRegion.Hand, -1));
               }, Condition.OnHand, Condition.OnMyRegion)
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.TransferSystem.DisCard(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Hand].CardList.LastOrDefault()));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}