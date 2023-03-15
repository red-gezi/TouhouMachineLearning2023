using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:ˮ�������
    /// ��������:��������3�� �ҷ��غϽ���������������һ����Ϊ���൥λ����ˮ״̬������ֵ���������󴥷�����Ч�� ���أ������൥λ���2���˺����ݻ�����
    /// </summary>
    public class CardM_T0_3C_001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
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
                       await GameSystem.UiSystem.ShowTips(this, "����", new Color(1, 0, 0));
                       await GameSystem.PointSystem.Hurt(new Event(this, this.TwoSideCard).SetPoint(2).SetMeanWhile());
                       await GameSystem.PointSystem.Destory(new Event(this, this));
                   }
               }, Condition.Default, Condition.OnMyRegion)
               .AbilityAppend();
        }
    }
}