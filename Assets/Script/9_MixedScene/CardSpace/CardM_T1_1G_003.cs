using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:Զ�ŵ�ƭ��
    /// ��������:�����������Ч���������㷢�������������� �������д��һ���� �ӳ��ϻ���һ�ŷǽ�λ������׶� �ӿ����ȡһ����
    /// </summary>
    public class CardM_T1_1G_003 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this,this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Hand].ContainCardList);
                   await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.SelectBoardCards));
               }, Condition.Default)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.NoGold].ContainCardList);
                   await GameSystem.TransferSystem.TransferCard(new Event(this, GameSystem.InfoSystem.SelectBoardCards).SetLocation(Orientation.My, GameRegion.Hand));
               }, Condition.Default)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.TransferSystem.DrawCard(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].ContainCardList.FirstOrDefault()));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}