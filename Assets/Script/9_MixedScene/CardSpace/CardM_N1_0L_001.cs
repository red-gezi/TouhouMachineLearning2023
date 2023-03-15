using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:鬼人正邪
    /// 卡牌能力:每当己方打出并部署一个单位时，在其部署效果生效前，选择我方场上一个非金单位与其点数互换
    /// </summary>
    public class CardM_N1_0L_001 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.UiSystem.ShowFigure(this);
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.Before, TriggerType.Deploy)
               .AbilityAdd(async (e) =>
               {
                   List<Card> targetCards = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver, CardRank.Copper].CardList;
                   targetCards.Remove(e.TriggerCard);
                   await GameSystem.SelectSystem.SelectUnit(this, targetCards, 1);
                   await GameSystem.PointSystem.Reversal(new Event(e.TriggerCard, GameSystem.InfoSystem.SelectUnits));
               }, Condition.Default, Condition.OnMyRegion)
               .AbilityAppend();
        }
    }
}