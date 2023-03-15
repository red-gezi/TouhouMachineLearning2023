using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:火焰猫磷
    /// 卡牌能力:部署：选择将对方墓地一个单位移入己方墓地顶端，同时自身获得与其基数点数等量的强化
    /// </summary>
    public class CardM_F1_2S_003 : Card
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
                 await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Grave][CardType.Unit].CardList);
                 if (GameSystem.InfoSystem.SelectUnit != null)
                 {

                     await GameSystem.TransferSystem.MoveCard(new Event(this, GameSystem.InfoSystem.SelectBoardCard).SetLocation(Orientation.My, GameRegion.Grave, 1));
                     await GameSystem.PointSystem.Strengthen(new Event(this, this).SetPoint(GameSystem.InfoSystem.SelectBoardCard.BasePoint));
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}