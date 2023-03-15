using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:条件反射
    /// 卡牌能力:生成一张覆盖状态的卡牌，效果择一： 对方下个单位部署时掀开并在其部署生效前给予4点伤害 对方下个特殊卡打出时掀开并在其打出效果发动前给与封印状态
    /// </summary>
    public class CardM_F1_2S_006 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectBoardCard(this, new List<string>() { "M_F1_3C_007", "M_F1_3C_008" });
                    await GameSystem.TransferSystem.GenerateCard(new Event(this, targetCard:null).SetTargetCardId(GameSystem.InfoSystem.SelectBoardCardId).SetLocation( Orientation.My,GameRegion.Used,1));
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