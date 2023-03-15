using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:[����]�����ľ���
    /// ��������:���Ʋ�����Է���һ�δ���ķǽ���
    /// </summary>
    public class CardM_F1_1G_002 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.After, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    refCardIDs = new() { e.TargetCard.CardID };
                }, Condition.OnOpRegion)
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.TransferSystem.GenerateCard(new Event(this,targetCard: null).SetTargetCardId(refCardIDs.FirstOrDefault()).SetLocation(CurrentOrientation, CurrentRegion, -1));
                }, Condition.NotSeal)
                .AbilityAdd(async (e) =>
                {
                    //����Ĺ�أ��ض�����
                    await GameSystem.TransferSystem.TransferToBelongGrave(this);
                })
               .AbilityAppend();
        }
    }
}