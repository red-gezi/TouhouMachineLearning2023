using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:请神
    /// 卡牌能力:从牌组中打出点数小于自身祈祷值且最接近的铜色单位
    /// </summary>
    public class CardM_S0_3C_003 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    var targetCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardRank.Copper]
                    .ContainCardList
                    .Where(card => card.ShowPoint <= this[CardField.Pary])
                    .OrderBy(card => card.ShowPoint)
                    .FirstOrDefault();
                    await GameSystem.TransferSystem.PlayCard(new Event(this, targetCard));
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