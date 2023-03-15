using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:�����ؾ�
    /// ��������:����Ϊ�Է����������������Ҳ�Ŀ��Ƹ��衰���ӡ�״̬
    /// </summary>
    public class CardM_F1_2S_002 : Card
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
                 var rowLeftCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Hand].CardList.FirstOrDefault();
                 var rowRightCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Hand].CardList.LastOrDefault();
                 if (rowLeftCard != null)
                 {
                     if (rowLeftCard != rowRightCard)
                     {
                         await GameSystem.StateSystem.SetState(new Event(this, new List<Card>() { rowLeftCard, rowRightCard }).SetTargetState(CardState.Pry).SetMeanWhile());
                     }
                     else
                     {
                         await GameSystem.StateSystem.SetState(new Event(this, rowLeftCard).SetTargetState(CardState.Pry));
                     }
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}