using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:命定之抽
    /// 卡牌能力:从卡组中选择一张小于等于自身祈祷值的卡牌并打出（非单位牌视为0点）
    /// </summary>
    public class CardM_S0_1G_004 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    var targetCards = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].ContainCardList.Where(card => card.ShowPoint <= this[CardField.Pary]).ToList();
                    await GameSystem.SelectSystem.SelectBoardCard(this, targetCards);
                    await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.SelectBoardCard));
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