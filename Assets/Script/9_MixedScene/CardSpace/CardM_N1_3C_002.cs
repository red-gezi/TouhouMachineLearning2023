using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��鳣�����Ʒ��
    /// ��������:��һ����λ��ɣ���ǰ����-1������˺�
    /// </summary>
    public class CardM_N1_3C_002 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.Silver][CardRank.Copper][CardType.Unit].ContainCardList, 1);
                    if (GameSystem.InfoSystem.SelectUnits.Any())
                    {
                        Card targetCard = GameSystem.InfoSystem.SelectUnit;
                        await GameSystem.PointSystem.Hurt(new Event(this, targetCard).SetPoint(targetCard.ShowPoint - 1));
                    }
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