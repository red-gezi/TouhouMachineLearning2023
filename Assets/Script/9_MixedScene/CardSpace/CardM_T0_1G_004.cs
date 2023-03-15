using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:急速充能器
    /// 卡牌能力:部署：获得倒计时（2） 我方回合开始时，倒计时-1，倒计时归0时 将卡牌两侧的"水利发电机"或"储能电容器"单位的能量点数增加到容量上限，然后重置倒计时为2
    /// </summary>
    public class CardM_T0_1G_004 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
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
                   await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Timer, 2));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
              .AbilityAdd(async (e) =>
              {
                  if (this[CardField.Timer] == 1)
                  {
                      await GameSystem.FieldSystem.ChangeField(new Event(this, this).SetTargetField(CardField.Timer, -1));
                      this.TwoSideCard.ForEach(async card =>
                      {
                          if (card.CardID == "2103001")
                          {
                              await GameSystem.FieldSystem.SetField(new Event(this, card).SetTargetField(CardField.Energy, 3));
                          }
                          if (card.CardID == "2103002")
                          {
                              await GameSystem.FieldSystem.SetField(new Event(this, card).SetTargetField(CardField.Energy, 8));
                          }
                      });
                      await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Timer, 2));
                  }
                  else
                  {
                      await GameSystem.FieldSystem.ChangeField(new Event(this, this).SetTargetField(CardField.Timer, -1));
                  }
              })
              .AbilityAppend();
        }
    }
}