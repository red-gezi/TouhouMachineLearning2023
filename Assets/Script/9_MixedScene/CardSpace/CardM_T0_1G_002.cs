using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:压力测试
    /// 卡牌能力:选择一个机械单位，将其移动至对方同排，并根据其能量值对同排所有单位造成伤害
    /// </summary>
    public class CardM_T0_1G_002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardTag.Machine].CardList, 1);
                    await GameSystem.TransferSystem.MoveCard(new Event(this, GameSystem.InfoSystem.SelectUnit).SetLocation(Orientation.Op, GameSystem.InfoSystem.SelectUnit.CurrentRegion, -1));
                })
               .AbilityAdd(async (e) =>
               {
                   if (GameSystem.InfoSystem.SelectUnit != null)
                   {
                       int num = GameSystem.InfoSystem.SelectUnit[CardField.Energy];
                       await GameSystem.FieldSystem.SetField(new Event(this, GameSystem.InfoSystem.SelectUnit).SetTargetField(CardField.Energy, 0));
                       await GameSystem.PointSystem.Hurt(new Event(GameSystem.InfoSystem.SelectUnit, GameSystem.InfoSystem.SelectUnit.BelongCardList).SetPoint(num).SetMeanWhile());
                   }
               })
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.TransferSystem.TransferToBelongGrave(this);
               })
               .AbilityAppend();
        }
    }
}