using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��¶ŵ
    /// ��������:���𣬻�����x��:��ӡ1+x���ǽ�λ������2���˺���xΪ���൥λ�Ļ���ֵ�ܺ�
    /// </summary>
    public class CardM_N0_2S_004 : Card
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
                 for (int i = 0; i < 1 + GameSystem.InfoSystem.GetTwoSideField(this, CardField.Inspire) + 1; i++)
                 {
                     await GameSystem.SelectSystem.SelectUnit(this, AgainstInfo.cardSet[GameRegion.Battle].CardList, 1, false);
                     await GameSystem.StateSystem.ChangeState(new Event(this, GameSystem.InfoSystem.SelectUnits).SetTargetState(CardState.Seal));
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}