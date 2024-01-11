using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����֮��
    /// ��������:�ӿ�����ѡ��һ��С�ڵ���������ֵ�Ŀ��Ʋ�������ǵ�λ����Ϊ0�㣩
    /// </summary>
    public class CardM_S0_1G_004 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    var targetCards = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].ContainCardList.Where(card => card.ShowPoint <= this[CardField.Pary]).ToList();
                    await GameSystem.SelectSystem.SelectBoardCard(this, targetCards);
                    await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.SelectBoardCard));
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