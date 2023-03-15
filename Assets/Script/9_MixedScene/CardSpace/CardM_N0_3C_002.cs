using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:ս��ת��
    /// ��������:����:���ҷ�����һ��ͭɫ��λ���뿨��׶ˣ�ͬʱ��������е�����͵�����ͭɫ��λ
    /// </summary>
    public class CardM_N0_3C_002 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Copper].CardList, 1);
                    await GameSystem.TransferSystem.TransferCard(new Event( this,GameSystem.InfoSystem.SelectUnit).SetLocation( Orientation.My, GameRegion.Deck,-1));
                    await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardRank.Copper][CardFeature.LowestPointUnits].CardList));
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