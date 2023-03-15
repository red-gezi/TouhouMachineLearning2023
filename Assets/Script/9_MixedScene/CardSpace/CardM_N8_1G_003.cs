using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:�ļ�ӳ��
    /// ��������:����ѡ��������λ�����θ���ڡ���״̬ �غϿ�ʼ��������൥λ��״̬�������Ҳ൥λ��״̬
    /// </summary>
    public class CardM_N8_1G_003 : Card
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
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.NoGold].CardList, 1);
                   await GameSystem.StateSystem.SetState(new Event(this, GameSystem.InfoSystem.SelectUnit).SetTargetState(CardState.Black));
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.NoGold].CardList, 1);
                   await GameSystem.StateSystem.SetState(new Event(this, GameSystem.InfoSystem.SelectUnit).SetTargetState(CardState.White));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.TurnStart)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.StateSystem.SetState(new Event(this, LeftCard).SetTargetState(CardState.Black));
                   await GameSystem.StateSystem.SetState(new Event(this, RightCard).SetTargetState(CardState.White));
               })
               .AbilityAppend();
        }
    }
}