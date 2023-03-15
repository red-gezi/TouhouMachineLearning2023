using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:������������յ�����
    /// ��������:����ѡ��Է�������һ����λ������"���"״̬
    /// </summary>
    public class CardM_F1_0L_001 : Card
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
                  await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Hand].CardList);
                  await GameSystem.StateSystem.SetState(new Event(this, GameSystem.InfoSystem.SelectBoardCard).SetTargetState(CardState.Close));
              })
              .AbilityAppend();
        }
    }
}