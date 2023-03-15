using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:水利发电机
    /// 卡牌能力:容量：（3） 我方回合结束：自身能量加一，并为两侧单位附加水状态，能量值超出容量后触发超载效果 超载：对两侧单位造成2点伤害并摧毁自身
    /// </summary>
    public class CardM_T0_3C_001 : Card
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

            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.FieldSystem.ChangeField(new Event(this, this).SetTargetField(CardField.Energy, 1));
                   await GameSystem.StateSystem.SetState(new Event(this, this.TwoSideCard).SetTargetState(CardState.Water).SetMeanWhile());
                   if (this[CardField.Energy] > 3)
                   {
                       await GameSystem.UiSystem.ShowTips(this, "超载", new Color(1, 0, 0));
                       await GameSystem.PointSystem.Hurt(new Event(this, this.TwoSideCard).SetPoint(2).SetMeanWhile());
                       await GameSystem.PointSystem.Destory(new Event(this, this));
                   }
               }, Condition.Default, Condition.OnMyRegion)
               .AbilityAppend();
        }
    }
}