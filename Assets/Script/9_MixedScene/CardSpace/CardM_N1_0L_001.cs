using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:������а
    /// ��������:ÿ���������������һ����λʱ�����䲿��Ч����Чǰ��ѡ���ҷ�����һ���ǽ�λ�����������
    /// </summary>
    public class CardM_N1_0L_001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.UiSystem.ShowFigure(this);
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.Before, TriggerType.Deploy)
               .AbilityAdd(async (e) =>
               {
                   List<Card> targetCards = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver, CardRank.Copper].CardList;
                   targetCards.Remove(e.TriggerCard);
                   await GameSystem.SelectSystem.SelectUnit(this, targetCards, 1);
                   await GameSystem.PointSystem.Reversal(new Event(e.TriggerCard, GameSystem.InfoSystem.SelectUnits));
               }, Condition.Default, Condition.OnMyRegion)
               .AbilityAppend();
        }
    }
}