using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:万宝槌
    /// 卡牌能力:对同排所有单位产生2点增益
    /// </summary>
    public class CardM_N1_2S_008 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectRegion(this);
                   await GameSystem.PointSystem.Gain(new Event(this, GameSystem.InfoSystem.SelectRegionCardList).SetPoint(2).SetMeanWhile());
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