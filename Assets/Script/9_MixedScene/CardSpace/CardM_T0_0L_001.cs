using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:�ɳǺ�ȡ
    /// ��������:���𣺸�����������е��ǩ�ĵ�λ ���� ���� ��ʱ ��ӡ
    /// </summary>
    public class CardM_T0_0L_001 : Card
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
                   await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[CardTag.Machine][Orientation.My][GameRegion.Grave].ContainCardList, num: 2);
                   await GameSystem.TransferSystem.ReviveCard(new Event(this, GameSystem.InfoSystem.SelectBoardCards));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}