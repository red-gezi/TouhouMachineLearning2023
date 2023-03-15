using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:阿空
    /// 卡牌能力:部署：从双方墓地中放逐一个单位，选择场上一个非金单位造成与放逐单位基数点数等量的伤害
    /// </summary>
    public class CardM_F1_2S_004 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (e) =>
             {
                 await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Grave].CardList);
                 if (GameSystem.InfoSystem.SelectBoardCard != null)
                 {
                     var point = GameSystem.InfoSystem.SelectBoardCard.BasePoint;
                     await GameSystem.TransferSystem.BanishCard(new Event(this, GameSystem.InfoSystem.SelectBoardCard));

                     await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle].CardList, 1);
                     await GameSystem.PointSystem.Hurt(new Event(this, GameSystem.InfoSystem.SelectUnit).SetPoint(point));
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}