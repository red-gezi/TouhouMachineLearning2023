using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��ʮ�Ű���
    /// ��������:���꣺������е�λ����˳״̬ ��˳��������е�λ�ı���״̬
    /// </summary>
    public class CardM_N1_2S_005 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.Increase)
               .AbilityAdd(async (e) =>
               {
                   if (!this[CardState.Furor])//��������ڿ���״̬
                   {
                       await GameSystem.StateSystem.ClearState(new Event(this, this).SetTargetState(CardState.Docile));
                       await GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Furor));
                   }
               }, Condition.Default)
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Decrease)
               .AbilityAdd(async (e) =>
               {
                   if (!this[CardState.Docile])//�����������˳״̬
                   {
                       await GameSystem.StateSystem.ClearState(new Event(this, this).SetTargetState(CardState.Furor));
                       await GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Docile));
                   }
               }, Condition.Default)
                .AbilityAppend();
        }
    }
}