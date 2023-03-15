using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����С����
    /// ��������:����:ѡ�������ǽ�������λ�����������ֵ
    /// </summary>
    public class CardM_N0_2S_006 : Card
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
                 await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardField.Inspire].CardList, 2, false);
                 GameSystem.InfoSystem.SelectUnits.ForEach(async unit =>
                 {
                     await GameSystem.FieldSystem.SetField(new Event(this, unit).SetTargetField(CardField.Inspire, unit[CardField.Inspire] * 2));
                 });
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}