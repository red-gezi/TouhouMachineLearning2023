using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:ѹ������
    /// ��������:ѡ��һ����е��λ�������ƶ����Է�ͬ�ţ�������������ֵ��ͬ�����е�λ����˺�
    /// </summary>
    public class CardM_T0_1G_002 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardTag.Machine].CardList, 1);
                    await GameSystem.TransferSystem.MoveCard(new Event(this, GameSystem.InfoSystem.SelectUnit).SetLocation(Orientation.Op, GameSystem.InfoSystem.SelectUnit.CurrentRegion, -1));
                })
               .AbilityAdd(async (e) =>
               {
                   if (GameSystem.InfoSystem.SelectUnit != null)
                   {
                       int num = GameSystem.InfoSystem.SelectUnit[CardField.Energy];
                       await GameSystem.FieldSystem.SetField(new Event(this, GameSystem.InfoSystem.SelectUnit).SetTargetField(CardField.Energy, 0));
                       await GameSystem.PointSystem.Hurt(new Event(GameSystem.InfoSystem.SelectUnit, GameSystem.InfoSystem.SelectUnit.BelongCardList).SetPoint(num).SetMeanWhile());
                   }
               })
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.TransferSystem.TransferToBelongGrave(this);
               })
               .AbilityAppend();
        }
    }
}