using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��ҡ�����
    /// ��������:λ�ڳ���ʱ���ҷ�ÿ����һ�ŵ�λ�����䲿��Ч����Чǰ��������������+1
    /// </summary>
    public class CardM_T0_2S_001 : Card
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
            AbalityRegister(TriggerTime.Before, TriggerType.Deploy)
              .AbilityAdd(async (e) =>
              {
                  await GameSystem.FieldSystem.ChangeField(new Event(this, this).SetTargetField(CardField.Energy, 1));
              }, Condition.Default, Condition.OnMyRegion)
              .AbilityAppend();
        }
    }
}