using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��������
    /// ��������:�ҷ��غϽ������Ƴ����൥λ����ֵ�������������������
    /// </summary>
    public class CardM_T0_2S_005 : Card
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
                  int energyPoint = TwoSideCard.Sum(card => card[CardField.Energy]);
                  await GameSystem.FieldSystem.ChangeField(new Event(this, this).SetTargetField(CardField.Energy, energyPoint));
                  await GameSystem.FieldSystem.SetField(new Event(this, TwoSideCard).SetTargetField(CardField.Energy, 0));
                  await GameSystem.PointSystem.Gain(new Event(this, this).SetPoint(energyPoint));
              }, Condition.Default, Condition.OnMyRegion)
              .AbilityAppend();
        }
    }
}