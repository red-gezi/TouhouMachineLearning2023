using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:美国小妖精
    /// 卡牌能力:部署:选择两个非金妖精单位并翻倍其鼓舞值
    /// </summary>
    public class CardM_N0_2S_006 : Card
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
                 await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardField.Inspire].CardList, 2, false);
                 GameSystem.InfoSystem.SelectUnits.ForEach(async unit =>
                 {
                     await GameSystem.FieldSystem.SetField(new Event(this, unit).SetTargetField(CardField.Inspire, unit[CardField.Inspire] * 2));
                 });
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}