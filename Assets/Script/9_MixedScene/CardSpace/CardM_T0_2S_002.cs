using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:临时储存器
    /// 卡牌能力:部署：选择两个单位，将其能量集中至自身
    /// </summary>
    public class CardM_T0_2S_002 : Card
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
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardField.Energy].ContainCardList, 2);
                   await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Energy, GameSystem.InfoSystem.SelectUnits.Sum(card => card[CardField.Energy])));
                   await GameSystem.FieldSystem.SetField(new Event(this, GameSystem.InfoSystem.SelectUnits).SetTargetField(CardField.Energy, 0));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}