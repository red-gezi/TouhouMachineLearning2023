using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:山童
    /// 卡牌能力:部署：选择一个属性区域，对该牌所有单位赋予土元素状态
    /// </summary>
    public class CardM_T0_2S_006 : Card
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
                   await GameSystem.SelectSystem.SelectRegion(this);
                   await GameSystem.StateSystem.SetState(new Event(this, GameSystem.InfoSystem.SelectRegionCardList).SetTargetState(CardState.Soil));
               })
               .AbilityAppend();
        }
    }
}