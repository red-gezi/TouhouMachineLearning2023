using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:ӣʯ
    /// ��������:����һ��ͭɫ������λ
    /// </summary>
    public class CardM_N0_3C_005 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectBoardCard(this, AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Grave][CardRank.Copper][GameEnum.CardTag.Fairy][CardType.Unit].ContainCardList);
                    await GameSystem.TransferSystem.ReviveCard(new Event(this, AgainstInfo.SelectActualCards));
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