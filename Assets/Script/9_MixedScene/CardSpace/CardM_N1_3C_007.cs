using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��˳֮ͷ
    /// ��������:���ҷ�ͬ�����Ҳ�����һ��ͷ��ÿ�غϿ�ʼ����൥����һ������
    /// </summary>
    public class CardM_N1_3C_007 : Card
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
                 await GameSystem.TransferSystem.MoveCard(new Event(this, this).SetLocation(CurrentOrientation, NextBattleRegion, -1));
                 await GameSystem.PointSystem.Gain(new Event(this, this.LeftCard).SetPoint(1));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}