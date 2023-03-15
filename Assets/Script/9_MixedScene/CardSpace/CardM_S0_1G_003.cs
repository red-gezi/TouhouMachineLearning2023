using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:加倍奉还
    /// 卡牌能力:卡组中一奇迹牌的祈祷值设置为自身两倍并打出
    /// </summary>
    public class CardM_S0_1G_003 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardTag.Miracle].CardList);
                    var s = GameSystem.InfoSystem.SelectBoardCards;
                    if (GameSystem.InfoSystem.SelectBoardCard != null)
                    {
                        await GameSystem.FieldSystem.ChangeField(new Event(this, GameSystem.InfoSystem.SelectBoardCard).SetPoint(GameSystem.InfoSystem.SelectBoardCard[CardField.Pary]));
                        await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.SelectBoardCard));
                    }
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