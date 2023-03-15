using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:���ٳ�����
    /// ��������:���𣺻�õ���ʱ��2�� �ҷ��غϿ�ʼʱ������ʱ-1������ʱ��0ʱ �����������"ˮ�������"��"���ܵ�����"��λ�������������ӵ��������ޣ�Ȼ�����õ���ʱΪ2
    /// </summary>
    public class CardM_T0_1G_004 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Timer, 2));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
              .AbilityAdd(async (e) =>
              {
                  if (this[CardField.Timer] == 1)
                  {
                      await GameSystem.FieldSystem.ChangeField(new Event(this, this).SetTargetField(CardField.Timer, -1));
                      this.TwoSideCard.ForEach(async card =>
                      {
                          if (card.CardID == "2103001")
                          {
                              await GameSystem.FieldSystem.SetField(new Event(this, card).SetTargetField(CardField.Energy, 3));
                          }
                          if (card.CardID == "2103002")
                          {
                              await GameSystem.FieldSystem.SetField(new Event(this, card).SetTargetField(CardField.Energy, 8));
                          }
                      });
                      await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Timer, 2));
                  }
                  else
                  {
                      await GameSystem.FieldSystem.ChangeField(new Event(this, this).SetTargetField(CardField.Timer, -1));
                  }
              })
              .AbilityAppend();
        }
    }
}