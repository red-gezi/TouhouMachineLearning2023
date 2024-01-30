using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:������
    /// ��������:����:����һ���ǽ�������λ�����´����䲿��Ч��
    /// </summary>
    public class CardM_N0_1G_001 : Card
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
                   await GameSystem.SelectSystem.SelectUnit(this, AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Battle][CardRank.Copper, CardRank.Silver][GameEnum.CardTag.Fairy].ContainCardList, 1);
                   await GameSystem.PointSystem.Cure
                   (
                       new Event(this, GameSystem.InfoSystem.SelectUnits)
                       .AddDanmuEffect(new DanmuModel(DanmuType.BigBall, DanmuColor.Green, DanmuTrack.Line))
                   );
                   if (AgainstInfo.SelectUnits.Any())
                   {
                       AgainstInfo.SelectRowRank = AgainstInfo.SelectUnits[0].Location.Row;
                       AgainstInfo.SelectRank = AgainstInfo.SelectUnits[0].Location.Rank;
                   }
                   await GameSystem.TransferSystem.DeployCard(new Event(this, GameSystem.InfoSystem.SelectUnits));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}