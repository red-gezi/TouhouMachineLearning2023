using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����ز�
    /// ��������:ѡ����һ��ͭɫ��λ���������´��
    /// </summary>
    public class CardM_N1_3C_001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[CardRank.Silver][CardRank.Copper][CardType.Unit].ContainCardList, 1);
                    await GameSystem.TransferSystem.ReviveCard(new Event(this, GameSystem.InfoSystem.SelectUnit));
                    await GameSystem.TransferSystem.PlayCard(new Event(GameSystem.InfoSystem.SelectUnit, GameSystem.InfoSystem.SelectUnit));
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