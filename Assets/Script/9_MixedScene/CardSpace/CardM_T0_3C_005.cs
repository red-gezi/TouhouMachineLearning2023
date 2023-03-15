using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��ͯ����
    /// ��������:����ѡ�����������´���һ��ͭɫ��е��λ�Ĳ���Ч��
    /// </summary>
    public class CardM_T0_3C_005 : Card
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
                    await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Copper][CardTag.Machine].CardList, 1);
                    await GameSystem.PointSystem.Cure(new Event(this, GameSystem.InfoSystem.SelectUnits));
                    await GameSystem.TransferSystem.DeployCard(new Event(this, GameSystem.InfoSystem.SelectUnits), true);
                }, Condition.Default)
                .AbilityAppend();
        }
    }
}