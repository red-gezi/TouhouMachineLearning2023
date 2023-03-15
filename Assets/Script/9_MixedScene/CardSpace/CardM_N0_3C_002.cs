using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:战略转移
    /// 卡牌能力:部署:将我方场上一个铜色单位移入卡组底端，同时打出卡组中点数最低的首张铜色单位
    /// </summary>
    public class CardM_N0_3C_002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
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
                    //移入墓地，必定触发
                    await GameSystem.TransferSystem.TransferToBelongGrave(this);
                })
               .AbilityAppend();
        }
    }
}