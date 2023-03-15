using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:小野冢小町
    /// 卡牌能力:部署：选择两个单位，依次拉取至自身右侧
    /// </summary>
    public class CardM_N8_2S_001 : Card
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
                  for (int i = 0; i < 2; i++)
                  {
                      await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.NoGold].CardList, 1);
                      await GameSystem.TransferSystem.MoveCard(new Event(this, GameSystem.InfoSystem.SelectUnit).SetLocation(CurrentOrientation, CurrentRegion, -1));
                  }
              })
              .AbilityAppend();
        }
    }
}