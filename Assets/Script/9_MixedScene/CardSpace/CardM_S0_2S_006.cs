using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:���ϵĳ���
    /// ��������:�غϽ���ʱ�����������ڵ�λ������һ�У����ƶ���ȥ�������ƶ������棩,�����������仯��������״̬�ڶ��ͺ��ֶ�֮��ת����������˫�������漣��1��ֵ ���ͣ��ƶ��������ͬ�����зǽ�λ1������ �ֶ��ƶ��������ͬ�����зǽ�λ1���˺� 
    /// </summary>
    public class CardM_S0_2S_006 : Card
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