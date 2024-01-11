using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:堀川雷鼓
    /// 卡牌能力:部署：使与自身点数相同的所有非金单位与自身点数减半（向下取整），重复三次
    /// </summary>
    public class CardM_N1_1G_002 : Card
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
                  var targetList = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver, CardRank.Copper].ContainCardList.Where(card => card.ShowPoint == ShowPoint).ToList();
                  targetList.Add(this);
                  await GameSystem.PointSystem.Hurt(new Event(this, targetList).SetPoint(ShowPoint / 2));
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}