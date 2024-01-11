using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����С��
    /// ��������:�ҷ��غϽ���ʱ���ƶ����Է��������λ�Ҳ࣬�����Ƿǽ�λ������������1�㣬��������������������ͬ���˺�
    /// </summary>
    public class CardM_F1_2S_001 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
               .AbilityAdd(async (e) =>
               {
                   var targetCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardFeature.LargestPointUnits].ContainCardList.FirstOrDefault();
                   if (targetCard != null)
                   {
                       await GameSystem.TransferSystem.MoveCard(new Event(this, this).SetLocation(targetCard.CurrentOrientation, targetCard.CurrentRegion, targetCard.CurrentIndex + 1));
                       if (LeftCard.Rank == CardRank.Copper || LeftCard.Rank == CardRank.Silver)
                       {
                           await GameSystem.PointSystem.Gain(new Event(this, this).SetPoint(1));
                           await GameSystem.PointSystem.Hurt(new Event(this, LeftCard).SetPoint(ShowPoint));
                       }
                   }
               }, Condition.Default, Condition.OnMyRegion)
               .AbilityAppend();
        }
    }
}