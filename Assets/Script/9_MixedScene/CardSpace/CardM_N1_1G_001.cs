using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����������
    /// ��������:���𣺴ݻ�һ��������������3���ķǽ�λ
    /// </summary>
    public class CardM_N1_1G_001 : Card
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
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver, CardRank.Copper].ContainCardList.Where(card => card.ShowPoint > ShowPoint * 3).ToList(), 1);
                   await GameSystem.PointSystem.Destory(new Event(this, GameSystem.InfoSystem.SelectUnits));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}