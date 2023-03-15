using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:古老的崇神
    /// 卡牌能力:回合结束时，若自身不处于单位数最多的一行，则移动过去（优先移动至对面）,若所属发生变化，则自身状态在恩赐和灾厄之间转换，并增加双方所有奇迹牌1祈祷值 恩赐：移动结束后对同排所有非金单位1点增益 灾厄：移动结束后对同排所有非金单位1点伤害 
    /// </summary>
    public class CardM_S0_2S_006 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this,this));
               })
               .AbilityAppend();
        }
    }
}