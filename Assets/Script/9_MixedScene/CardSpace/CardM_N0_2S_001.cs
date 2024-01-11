using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:���¾�*��
    /// ��������:����:�����������Ϊ2, ���ٻ����¾�*�£����¾�*�� ���ٻ�ʱ:�����������Ϊ2
    /// </summary>
    public class CardM_N0_2S_001 : Card
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
            //����Ч��
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (e) =>
             {
                 await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Inspire, 2));
                 await GameSystem.TransferSystem.SummonCard(
                     new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].ContainCardList
                     .Where(card => card.CardID == "CardM_N0_2S_002" || card.CardID == "CardM_N0_2S_003")
                     .ToList())
                     );
             }, Condition.Default)
             .AbilityAppend();
            //���ٻ�ʱЧ��
            AbalityRegister(TriggerTime.When, TriggerType.Summon)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Inspire, 2));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}