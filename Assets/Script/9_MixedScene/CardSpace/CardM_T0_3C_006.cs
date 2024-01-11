using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:河童的天才维修法
    /// 卡牌能力:部署：选择并将我方场上一个机械单位直接移入墓地（不视为伤害和死亡），随后从牌组中打出其同名单位
    /// </summary>
    public class CardM_T0_3C_006 : Card
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
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Copper][CardTag.Machine].ContainCardList, 1);
                   await GameSystem.PointSystem.Destory(new Event(this, GameSystem.InfoSystem.SelectUnit));

                   var targetCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].GetCardByID(GameSystem.InfoSystem.SelectUnit.CardID, 1);
                   await GameSystem.TransferSystem.SummonCard(new Event(this, targetCard));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}