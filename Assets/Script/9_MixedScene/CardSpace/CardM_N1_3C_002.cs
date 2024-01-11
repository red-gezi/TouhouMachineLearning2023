using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:万宝槌（仿制品）
    /// 卡牌能力:对一个单位造成（当前点数-1）点的伤害
    /// </summary>
    public class CardM_N1_3C_002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.Silver][CardRank.Copper][CardType.Unit].ContainCardList, 1);
                    if (GameSystem.InfoSystem.SelectUnits.Any())
                    {
                        Card targetCard = GameSystem.InfoSystem.SelectUnit;
                        await GameSystem.PointSystem.Hurt(new Event(this, targetCard).SetPoint(targetCard.ShowPoint - 1));
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