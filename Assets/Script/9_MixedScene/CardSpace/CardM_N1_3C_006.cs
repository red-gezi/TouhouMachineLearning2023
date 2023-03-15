using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��֮ͷ
    /// ��������:ÿ�غϿ�ʼ�����ƶ�������һ�����Ҳಢ����൥λ���1���˺�
    /// </summary>
    public class CardM_N1_3C_006 : Card
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
                  await GameSystem.TransferSystem.MoveCard(new Event(this, this).SetLocation(OppositeOrientation, NextBattleRegion, -1));
                  await GameSystem.PointSystem.Hurt(new Event(this, this.LeftCard).SetPoint(1));
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}