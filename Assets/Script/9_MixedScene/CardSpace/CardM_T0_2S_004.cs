using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��������
    /// ��������:��Ĺ����������е��λ���յ�������
    /// </summary>
    public class CardM_T0_2S_004 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[CardTag.Machine][Orientation.My][GameRegion.Grave].CardList, num: 3);
                   ;
                   //GameSystem.InfoSystem.SelectBoardCards.ForEach(async card => await GameSystem.TransferSystem.TransferCard(new Event(this, card).SetLocation(Orientation.My, GameRegion.Deck, -1)));
                   await GameSystem.TransferSystem.TransferCard(new Event(this, GameSystem.InfoSystem.SelectBoardCards).SetLocation(Orientation.My, GameRegion.Deck, -1));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}