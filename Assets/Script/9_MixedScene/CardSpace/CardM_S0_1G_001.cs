using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:�˰�������
    /// ��������:����ÿ��һ���ǽ�λ����һ��λ��Ч���󣬱������1���˺�
    /// </summary>
    public class CardM_S0_1G_001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)//ע��һ�� �� ���� ���ʱ ��Ч��Ч��
               .AbilityAdd(async (e) =>//���һ����Ч��
               {
                   //�ȴ�����ִ������������һ�д��� ��Ϸ����.ѡ�����.ѡ�����꣨�����ߣ���ǰ���� ������������ǰ���ƵĲ������� �ҷ�/�з����� �������򣨵�ǰ���ƵĲ������� ��һ�ţ���
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   //�ȴ�����ִ������������һ�д��� ��Ϸ����.ת�ƻ���.���𣨴����ߣ���ǰ���� ��Ч�� ����ǰ���ƣ���
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();//��׷�ӵķ�ʽ��Ӹ�Ч��

            AbalityRegister(TriggerTime.After, TriggerType.Move)
               .AbilityAdd(async (e) =>
               {
                   //����ƶ��Ķ����ڳ��Ϸǽ𼯺��У���׷��1���˺�
                   if (GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.NoGold].ContainCardList.Contains(e.TargetCard))
                   {
                       await GameSystem.PointSystem.Hurt(new Event(this, e.TargetCard).SetPoint(1).AddDanmuEffect(new DanmuModel()));
                   }
               }, Condition.Default, Condition.OnMyRegion)
               .AbilityAppend();
        }
    }
}