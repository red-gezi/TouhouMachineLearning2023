using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:远古的骗术
    /// 卡牌能力:依次完成以下效果（不满足发动条件则跳过） 从手牌中打出一张牌 从场上回收一张非金单位至卡组底端 从卡组抽取一张牌
    /// </summary>
    public class CardM_T1_1G_003 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Hand].ContainCardList);
                   await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.SelectBoardCards));
               }, Condition.Default)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.NoGold].ContainCardList);
                   await GameSystem.TransferSystem.TransferCard(new Event(this, GameSystem.InfoSystem.SelectBoardCards).SetLocation(Orientation.My, GameRegion.Hand));
               }, Condition.Default)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.TransferSystem.DrawCard(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].ContainCardList.FirstOrDefault()));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}