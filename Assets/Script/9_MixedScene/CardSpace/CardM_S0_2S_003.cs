using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��ȷ��֮��
    /// ��������:������λ��Ч���󣬻��2������
    /// </summary>
    public class CardM_S0_2S_003 : Card
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

            AbalityRegister(TriggerTime.When, TriggerType.Move)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.PointSystem.Gain(new Event(this, this).SetPoint(2));
               })
               .AbilityAppend();
        }
    }
}