using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��������a
    /// ��������:�Է��¸�����Ϊͭɫ��λʱ�Զ��ƿ�������Ч�������������ҷ�������С��ͭɫ��λ���ؿ��鶥��
    /// </summary>
    public class CardM_F1_3C_007 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Cover));
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   

               })
               .AbilityAppend();
        }
    }
}