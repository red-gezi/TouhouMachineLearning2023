using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:空气朋友
    /// 卡牌能力:对方先于己方pass时：若自身处于卡组，则自动打出至场上单位最少的随机排
    /// </summary>
    public class CardM_F1_2S_005 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this,this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.After, TriggerType.Pass)
               .AbilityAdd(async (e) =>
               {
                   if (Info.AgainstInfo.IsMyPass&&! Info.AgainstInfo.IsOpPass)
                   {
                       await GameSystem.TransferSystem.SummonCard(new Event(this, this));
                   }
               }, Condition.NotSeal,Condition.NotDead,Condition.OnDeck,Condition.OnOpRegion)
               .AbilityAppend();
        }
    }
}