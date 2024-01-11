using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����ˮ��
    /// ��������:�����Ƴ���൥λ���������ѡ��Է�����һ���ǽ�λ�������������ֵ���˺�
    /// </summary>
    public class CardM_T0_3C_004 : Card
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
                    int num = LeftCard[CardField.Energy];
                    await GameSystem.FieldSystem.SetField(new Event(this, LeftCard).SetTargetField(CardField.Energy, 0));

                    await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardRank.Silver, CardRank.Copper].ContainCardList, 1);
                    await GameSystem.PointSystem.Hurt(new Event(this, GameSystem.InfoSystem.SelectUnits).SetPoint(num));
                }, Condition.Default)
                .AbilityAppend();
        }
    }
}