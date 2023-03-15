using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:五谷丰登之术
    /// 卡牌能力:自身触发位移效果后，获得2点增益
    /// </summary>
    public class CardM_S0_2S_003 : Card
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

            AbalityRegister(TriggerTime.When, TriggerType.Move)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.PointSystem.Gain(new Event(this, this).SetPoint(2));
               })
               .AbilityAppend();
        }
    }
}