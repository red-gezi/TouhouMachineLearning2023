using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:八班神奈子
    /// 卡牌能力:场上每有一个非金单位触发一次位移效果后，便给予其1点伤害
    /// </summary>
    public class CardM_S0_1G_001 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)//注册一个 当 卡牌 打出时 生效的效果
               .AbilityAdd(async (e) =>//添加一个子效果
               {
                   //等待该行执行完再运行下一行代码 游戏机制.选择机制.选择坐标（触发者：当前卡牌 部署所属（当前卡牌的部署所属 我方/敌方）， 部署区域（当前卡牌的部署区域 哪一排））
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   //等待该行执行完再运行下一行代码 游戏机制.转移机制.部署（触发者：当前卡牌 生效者 ：当前卡牌））
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();//以追加的方式添加该效果

            AbalityRegister(TriggerTime.After, TriggerType.Move)
               .AbilityAdd(async (e) =>
               {
                   //如果移动的对象在场上非金集合中，则追加1点伤害
                   if (GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.NoGold].ContainCardList.Contains(e.TargetCard))
                   {
                       await GameSystem.PointSystem.Hurt(new Event(this, e.TargetCard).SetPoint(1).AddDanmuEffect(new DanmuModel()));
                   }
               }, Condition.Default, Condition.OnMyRegion)
               .AbilityAppend();
        }
    }
}