using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:���ô���
    /// ��������:ÿ��һ����λ����λ�ƣ��ҷ��漣���ƻ��һ����ֵ
    /// </summary>
    public class CardM_S0_2S_001 : Card
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

            AbalityRegister(TriggerTime.After, TriggerType.Move)
             .AbilityAdd(async (e) =>
             {
                 await GameSystem.FieldSystem.ChangeField(new Event(this, Info.AgainstInfo.GameCardsFilter[Orientation.My][CardTag.Miracle].ContainCardList).SetTargetField(CardField.Pary, 1).SetMeanWhile());
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}