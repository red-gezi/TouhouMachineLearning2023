using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:黄瓜加特林
    /// 卡牌能力:部署：移除我方场上所有单位的能量，对对方场上点数最高的非金单位造成移除值等次数的1点伤害
    /// </summary>
    public class CardM_T0_1G_001 : Card
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
                   var cardList = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardField.Energy].ContainCardList;
                   int num = cardList.Sum(card => card[CardField.Energy]);
                   await GameSystem.FieldSystem.SetField(new Event(this, cardList).SetTargetField(CardField.Energy, 0));
                   for (int i = 0; i < num; i++)
                   {
                       await GameSystem.PointSystem.Hurt(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardRank.Silver][CardRank.Copper][CardFeature.LargestPointUnits].ContainCardList.FirstOrDefault()));
                   }
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}