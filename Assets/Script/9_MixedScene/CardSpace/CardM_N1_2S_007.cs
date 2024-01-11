using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:草根妖怪网络
    /// 卡牌能力:召唤一个草根妖怪
    /// </summary>
    public class CardM_N1_2S_007 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][GameEnum.CardTag.Yokai][CardRank.Silver, CardRank.Copper].ContainCardList);
                   await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.SelectUnits));
                   await GameSystem.TransferSystem.TransferToBelongGrave(this);
               })
               .AbilityAppend();
        }
    }
}