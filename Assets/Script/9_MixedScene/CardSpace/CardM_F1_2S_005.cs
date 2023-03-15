using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��������
    /// ��������:�Է����ڼ���passʱ���������ڿ��飬���Զ���������ϵ�λ���ٵ������
    /// </summary>
    public class CardM_F1_2S_005 : Card
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
            AbalityRegister(TriggerTime.After, TriggerType.Pass)
               .AbilityAdd(async (e) =>
               {
                   if (Info.AgainstInfo.IsMyPass&&! Info.AgainstInfo.IsOpPass)
                   {
                       await GameSystem.TransferSystem.SummonCard(new Event(this, this));
                   }
               }, Condition.NotSeal,Condition.NotDead,Condition.OnDeck,Condition.OnOpRegion)
               .AbilityAppend();
        }
    }
}