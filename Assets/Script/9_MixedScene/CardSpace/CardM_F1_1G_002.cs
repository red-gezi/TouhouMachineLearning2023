using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:[回忆]过往的经历
    /// 卡牌能力:复制并打出对方上一次打出的非金卡牌
    /// </summary>
    public class CardM_F1_1G_002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.After, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    refCardIDs = new() { e.TargetCard.CardID };
                }, Condition.OnOpRegion)
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.TransferSystem.GenerateCard(new Event(this,targetCard: null).SetTargetCardId(refCardIDs.FirstOrDefault()).SetLocation(CurrentOrientation, CurrentRegion, -1));
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