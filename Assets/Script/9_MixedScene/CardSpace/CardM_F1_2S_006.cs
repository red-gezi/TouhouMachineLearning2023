using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��������
    /// ��������:����һ�Ÿ���״̬�Ŀ��ƣ�Ч����һ�� �Է��¸���λ����ʱ�ƿ������䲿����Чǰ����4���˺� �Է��¸����⿨���ʱ�ƿ���������Ч������ǰ�����ӡ״̬
    /// </summary>
    public class CardM_F1_2S_006 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectBoardCard(this, new List<string>() { "M_F1_3C_007", "M_F1_3C_008" });
                    await GameSystem.TransferSystem.GenerateCard(new Event(this, targetCard:null).SetTargetCardId(GameSystem.InfoSystem.SelectBoardCardId).SetLocation( Orientation.My,GameRegion.Used,1));
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