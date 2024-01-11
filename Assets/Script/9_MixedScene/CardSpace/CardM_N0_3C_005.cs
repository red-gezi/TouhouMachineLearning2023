using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:樱石
    /// 卡牌能力:复活一个铜色妖精单位
    /// </summary>
    public class CardM_N0_3C_005 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectBoardCard(this, AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Grave][CardRank.Copper][GameEnum.CardTag.Fairy][CardType.Unit].ContainCardList);
                    await GameSystem.TransferSystem.ReviveCard(new Event(this, AgainstInfo.SelectActualCards));
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