using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��С����
    /// ��������:����:���1����������ӿ����д��һ�ŵ�����͵�����ͭɫ������λ
    /// </summary>
    public class CardM_N0_3C_003 : Card
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
                 await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Inspire, 1));
                 await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardRank.Copper][CardTag.Fairy][CardFeature.LowestPointUnits].CardList, 1, true);
                 await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.SelectUnits));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}