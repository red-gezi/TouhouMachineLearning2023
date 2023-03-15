using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:�ɵ���Թ��
    /// ��������:�ҷ��غϽ���ʱ:������λ�����ƣ����ƶ����Է����� ���ҷ�passʱ �ٻ������� �����ٻ�:�����ҷ����������Ҳ�Ŀ���
    /// </summary>
    public class CardM_N8_1G_002 : Card
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
                   //await GameSystem.TransferSystem.MoveToOpHand(this);
                   await GameSystem.TransferSystem.TransferCard(new Event(this, this).SetLocation(Orientation.Op, GameRegion.Hand, -1));
               }, Condition.OnHand, Condition.OnMyRegion)
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.TransferSystem.DisCard(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Hand].CardList.LastOrDefault()));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}