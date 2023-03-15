using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����è��
    /// ��������:����ѡ�񽫶Է�Ĺ��һ����λ���뼺��Ĺ�ض��ˣ�ͬʱ�����������������������ǿ��
    /// </summary>
    public class CardM_F1_2S_003 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (e) =>
             {
                 await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Grave][CardType.Unit].CardList);
                 if (GameSystem.InfoSystem.SelectUnit != null)
                 {

                     await GameSystem.TransferSystem.MoveCard(new Event(this, GameSystem.InfoSystem.SelectBoardCard).SetLocation(Orientation.My, GameRegion.Grave, 1));
                     await GameSystem.PointSystem.Strengthen(new Event(this, this).SetPoint(GameSystem.InfoSystem.SelectBoardCard.BasePoint));
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}