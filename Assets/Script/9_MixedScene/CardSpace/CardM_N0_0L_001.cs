using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:板烧琪露诺
    /// 卡牌能力:部署，力竭:我方场上每存在一个自身外的妖精单位便对对方场上点数最高目标造成一次1点伤害
    /// </summary>
    public class CardM_N0_0L_001 : Card
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
                   int targetCount = AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Battle][CardTag.Fairy].ContainCardList.Count - 1;
                   for (int i = 0; i < AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Battle][CardTag.Fairy].ContainCardList.Count; i++)
                   {

                       List<Card> cardlist = AgainstInfo.GameCardsFilter[Orientation.Op][GameRegion.Battle][CardFeature.LargestPointUnits].ContainCardList.ToList();
                       if (cardlist.Any())
                       {
                           await GameSystem.SelectSystem.SelectUnit(this, cardlist, 1, isAuto: true);
                           await GameSystem.PointSystem.Hurt(new Event(this, AgainstInfo.SelectUnits).SetPoint(1));
                           if (BasePoint > 1)
                           {
                               await GameSystem.PointSystem.Weak(new Event(this, this).SetPoint(1));
                           }
                       }
                   }
               }, Condition.Default)
               .AbilityAppend();

            AbalityRegister(TriggerTime.After, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   GameSystem.InfoSystem.SetRefCardIDs(this, e.TargetCard.CardID);
               })
               .AbilityAppend();
            //AbalityRegister(TriggerTime.Before, TriggerType.Play)
            //   .AbilityAdd(async (e) =>
            //   {
            //       Debug.Log("修改自身引用id为" + e.targetCard.CardID);
            //       GameSystem.InfoSystem.SetRefCardIDs(this, e.targetCard.CardID);
            //   })
            //   .AbilityAppend();
        }
    }
}