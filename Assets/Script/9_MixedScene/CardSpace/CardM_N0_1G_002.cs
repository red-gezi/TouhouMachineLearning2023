using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:桑尼*缪克
    /// 卡牌能力:部署:摧毁场上非金且点数最大的所有单位
    /// </summary>
    public class CardM_N0_1G_002 : Card
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
                  await GameSystem.PointSystem.Destory(
                      new Event(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.Copper, CardRank.Silver][CardFeature.LargestPointUnits].ContainCardList)
                      );
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}