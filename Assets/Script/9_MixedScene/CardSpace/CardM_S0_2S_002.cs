using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:奇迹*圣人苏生
    /// 卡牌能力:从墓地中选择一张小于等于自身祈祷值的非金单位复活至场上
    /// </summary>
    public class CardM_S0_2S_002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    var cardList = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardRank.NoGold].CardList
                         .Where(card => card.ShowPoint <= this[CardField.Pary])
                         .ToList();
                    await GameSystem.SelectSystem.SelectBoardCard(this, cardList);
                    await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.SelectBoardCards));
                }, Condition.NotSeal)
                .AbilityAdd(async (e) =>
                {
                    //移入墓地，必定触发
                    await GameSystem.TransferSystem.TransferToBelongGrave(this);
                })
               .AbilityAppend();
        }
    }
}