using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using System;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:˹��
    /// ��������:����:��һ�������ķǽ�λ���x+1���˺��������Ҳࣨ�����𿨣����˵ݼ�(xΪ���൥λ�Ļ���ֵ�ܺ�)
    /// </summary>
    public class CardM_N0_1G_004 : Card
    {
        public override void Init()
        {
            base.Init();

            //��ʼ��ͨ�ÿ���Ч��
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
                 await GameSystem.SelectSystem.SelectRegion(this, Territory.Op, GameRegion.Battle);
                 List<Card> targetCardList = GameSystem.InfoSystem.AgainstCardSet[GameSystem.InfoSystem.SelectRegionRank];
                 int hurtMaxValue = GameSystem.InfoSystem.GetTwoSideField(this, CardField.Inspire) + 1;
                 for (int i = 0; i < Math.Min(targetCardList.Count, hurtMaxValue); i++)
                 {
                     await GameSystem.PointSystem.Hurt(new Event(this, targetCardList[i]).SetPoint(hurtMaxValue - i));
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}