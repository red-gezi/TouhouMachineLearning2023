using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��ʱ������
    /// ��������:����ѡ��������λ��������������������
    /// </summary>
    public class CardM_T0_2S_002 : Card
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
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardField.Energy].ContainCardList, 2);
                   await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Energy, GameSystem.InfoSystem.SelectUnits.Sum(card => card[CardField.Energy])));
                   await GameSystem.FieldSystem.SetField(new Event(this, GameSystem.InfoSystem.SelectUnits).SetTargetField(CardField.Energy, 0));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}