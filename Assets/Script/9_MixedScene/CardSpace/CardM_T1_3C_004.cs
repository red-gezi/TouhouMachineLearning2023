using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:������ǲ��
    /// ��������:Ǳ�� ���𣺼������ҷ��������Ϸ������Ų�ͬ��ͭɫ��λ�ƣ�ѡ������һ�Ų����
    /// </summary>
    public class CardM_T1_3C_004 : Card
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
        }
    }
}