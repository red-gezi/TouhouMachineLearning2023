using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����С����
    /// ��������:����:���1����裬ѡ������һ���ǽ�������λ
    /// </summary>
    public class CardM_N0_3C_001 : Card
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
                 await GameSystem.FieldSystem.ChangeField(new Event(this, this).SetTargetField(CardField.Inspire, 1));
                 await GameSystem.SelectSystem.SelectUnit(this, AgainstInfo.cardSet[Orientation.My][GameRegion.Battle][CardRank.Silver, CardRank.Copper].CardList, 1);
                 await GameSystem.PointSystem.Cure(new Event(this, GameSystem.InfoSystem.SelectUnits));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}