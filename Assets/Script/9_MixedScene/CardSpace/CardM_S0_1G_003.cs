using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:�ӱ��
    /// ��������:������һ�漣�Ƶ���ֵ����Ϊ�������������
    /// </summary>
    public class CardM_S0_1G_003 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardTag.Miracle].CardList);
                    var s = GameSystem.InfoSystem.SelectBoardCards;
                    if (GameSystem.InfoSystem.SelectBoardCard != null)
                    {
                        await GameSystem.FieldSystem.ChangeField(new Event(this, GameSystem.InfoSystem.SelectBoardCard).SetPoint(GameSystem.InfoSystem.SelectBoardCard[CardField.Pary]));
                        await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.SelectBoardCard));
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