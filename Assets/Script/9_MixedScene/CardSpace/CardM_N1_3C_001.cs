using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:替身地藏
    /// 卡牌能力:选择场上一个铜色单位，将其重新打出
    /// </summary>
    public class CardM_N1_3C_001 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[CardRank.Silver][CardRank.Copper][CardType.Unit].ContainCardList, 1);
                    await GameSystem.TransferSystem.ReviveCard(new Event(this, GameSystem.InfoSystem.SelectUnit));
                    await GameSystem.TransferSystem.PlayCard(new Event(GameSystem.InfoSystem.SelectUnit, GameSystem.InfoSystem.SelectUnit));
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