using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����
    /// ��������:����ѡ����һ��ͭɫ��λ������ϴ�ؿ��飬�������������Ϸ�2���и��Ǳ�ǩ��ͭɫ����ѡ��һ�Ų����
    /// </summary>
    public class CardM_F1_3C_004 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                   //����Ч������������ʱ�Ŵ���
                }, Condition.NotSeal)
                .AbilityAdd(async (e) =>
                {
                    //����Ĺ�أ��ض�����
                    await GameSystem.TransferSystem.TransferToBelongGrave(this);
                })
               .AbilityAppend();
        }
    }
}