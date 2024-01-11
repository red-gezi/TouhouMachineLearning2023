using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����
    /// ��������:�������д������С��������ֵ����ӽ���ͭɫ��λ
    /// </summary>
    public class CardM_S0_3C_003 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    var targetCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardRank.Copper]
                    .ContainCardList
                    .Where(card => card.ShowPoint <= this[CardField.Pary])
                    .OrderBy(card => card.ShowPoint)
                    .FirstOrDefault();
                    await GameSystem.TransferSystem.PlayCard(new Event(this, targetCard));
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