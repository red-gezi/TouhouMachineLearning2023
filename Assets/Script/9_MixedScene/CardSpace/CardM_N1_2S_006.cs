using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:逆符「天下翻覆」
    /// 卡牌能力:部署：自身与场上点数最大的非金单位进行逆转
    /// </summary>
    public class CardM_N1_2S_006 : Card
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
                 await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.Silver][CardRank.Copper][CardFeature.LargestPointUnits].CardList, 1, isAuto: true);
                 await GameSystem.PointSystem.Reversal(new Event(this, GameSystem.InfoSystem.SelectUnit));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}