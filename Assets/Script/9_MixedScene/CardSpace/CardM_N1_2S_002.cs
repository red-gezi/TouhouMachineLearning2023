using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:������
    /// ��������:���꣺�ڵз�ͬ�����Ҳ�����һ��ͷ��ÿ�غϿ�ʼ�����ƶ�������һ�����Ҳಢ����൥λ���1���˺� ��˳�����ҷ�ͬ�����Ҳ�����һ��ͷ��ÿ�غϿ�ʼ����൥����һ������
    /// </summary>
    public class CardM_N1_2S_002 : Card
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

                       await GameSystem.TransferSystem.GenerateCard(new Event(this, targetCard: null).SetTargetCardId("2013006").SetLocation(CurrentOrientation, CurrentRegion, -1));
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

                       await GameSystem.TransferSystem.GenerateCard(new Event(this, targetCard: null).SetTargetCardId("2013007").SetLocation(CurrentOrientation, CurrentRegion, -1));

                   }
               }, Condition.Default)
                .AbilityAppend();
        }
    }
}