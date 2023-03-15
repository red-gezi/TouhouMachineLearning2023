using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:¶��
    /// ��������:����:�Գ���������͵ķǽ�λ���һ���˺����ظ�x+1��(xΪ���൥λ�Ļ���ֵ�ܺ�)
    /// </summary>
    public class CardM_N0_1G_003 : Card
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
                 for (int i = 0; i < 1 + GameSystem.InfoSystem.GetTwoSideField(this, CardField.Inspire); i++)
                 {
                     await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardRank.Copper, CardRank.Silver][CardFeature.LowestPointUnits].CardList, 1, true);
                     await GameSystem.PointSystem.Hurt(new Event(this, GameSystem.InfoSystem.SelectUnits).SetPoint(1));
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}