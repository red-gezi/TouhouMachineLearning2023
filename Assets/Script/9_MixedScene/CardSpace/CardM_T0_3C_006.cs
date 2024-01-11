using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��ͯ�����ά�޷�
    /// ��������:����ѡ�񲢽��ҷ�����һ����е��λֱ������Ĺ�أ�����Ϊ�˺��������������������д����ͬ����λ
    /// </summary>
    public class CardM_T0_3C_006 : Card
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
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Copper][CardTag.Machine].ContainCardList, 1);
                   await GameSystem.PointSystem.Destory(new Event(this, GameSystem.InfoSystem.SelectUnit));

                   var targetCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].GetCardByID(GameSystem.InfoSystem.SelectUnit.CardID, 1);
                   await GameSystem.TransferSystem.SummonCard(new Event(this, targetCard));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}