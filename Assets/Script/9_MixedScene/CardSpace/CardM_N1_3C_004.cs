using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:隙间的折叠伞
    /// 卡牌能力:将一个单位移动至横向的另一排
    /// </summary>
    public class CardM_N1_3C_004 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver][CardRank.Copper][CardType.Unit].CardList, 1);
                    if (GameSystem.InfoSystem.SelectUnits.Any())
                    {
                        Card targetCard = GameSystem.InfoSystem.SelectUnits.First();
                        switch (targetCard.CurrentRegion)
                        {
                            case GameRegion.Water: await GameSystem.TransferSystem.MoveCard(new Event(this, targetCard).SetLocation(targetCard.CurrentOrientation, GameRegion.Fire, -1)); break;
                            case GameRegion.Fire: await GameSystem.TransferSystem.MoveCard(new Event(this, targetCard).SetLocation(targetCard.CurrentOrientation, GameRegion.Water, 0)); break;
                            case GameRegion.Wind: await GameSystem.TransferSystem.MoveCard(new Event(this, targetCard).SetLocation(targetCard.CurrentOrientation, GameRegion.Soil, 0)); break;
                            case GameRegion.Soil: await GameSystem.TransferSystem.MoveCard(new Event(this, targetCard).SetLocation(targetCard.CurrentOrientation, GameRegion.Wind, -1)); break;
                            default: break;
                        }
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