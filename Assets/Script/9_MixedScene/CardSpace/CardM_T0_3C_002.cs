using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:���ܵ�����
    /// ��������:��������8�� �ҷ��غϽ������Ƴ����൥λ����ֵ���ӵ��������ϣ�����ֵ���������󴥷�����Ч�� ���أ���ȫ�����е�λ���1���˺����ݻ�����
    /// </summary>
    public class CardM_T0_3C_002 : Card
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
                  if (this[CardField.Energy] > 8)
                  {
                      await GameSystem.UiSystem.ShowTips(this, "����", new Color(1, 0, 0));
                      await GameSystem.PointSystem.Hurt(new Event(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle].ContainCardList).SetPoint(1).SetMeanWhile());
                      await GameSystem.PointSystem.Destory(new Event(this, this));
                  }
              }, Condition.Default, Condition.OnMyRegion)
              .AbilityAppend();
        }
    }
}