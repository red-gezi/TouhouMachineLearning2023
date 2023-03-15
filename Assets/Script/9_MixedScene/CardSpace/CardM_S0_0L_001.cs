using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:东风谷早苗
    /// 卡牌能力:令场上所有牌至内向外产生位移
    /// </summary>
    public class CardM_S0_0L_001 : Card
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

                  //await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Fire].CardList).SetLocation(Orientation.My, GameRegion.Fire, -1));
                  await GameSystem.TransferSystem.MoveCard(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Soil].CardList).SetLocation(Orientation.My, GameRegion.Soil, -1));
                  await GameSystem.TransferSystem.MoveCard(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Fire].CardList).SetLocation(Orientation.Op, GameRegion.Fire, -1));
                  await GameSystem.TransferSystem.MoveCard(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Soil].CardList).SetLocation(Orientation.Op, GameRegion.Soil, -1));
                  //foreach (var card in GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Soil].CardList)
                  //{
                  //    await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, card).SetLocation(Orientation.My, GameRegion.Soil, -1));
                  //}
                  //foreach (var card in GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Fire].CardList)
                  //{
                  //    await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, card).SetLocation(Orientation.Op, GameRegion.Fire, -1));
                  //}
                  //foreach (var card in GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Soil].CardList)
                  //{
                  //    await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, card).SetLocation(Orientation.Op, GameRegion.Soil, -1));
                  //}
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}