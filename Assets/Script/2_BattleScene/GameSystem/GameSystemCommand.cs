using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Other;
using UnityEngine;
namespace TouhouMachineLearningSummary.Command
{
    public class GameSystemCommand
    {
        /// <summary>
        /// 系统通知类型效果触发
        /// 会向对战中所有卡牌依次通知触发 xx前、xx时、xx后效果
        /// 一般由系统控制流程时触发，如回合开始时、小局开始时等
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static async Task TriggerNotice(Event e)
        {
            foreach (var card in AgainstInfo.cardSet.CardList)
            {
                await Trigger(card, e[card][TriggerTime.Before]);
            }
            foreach (var card in AgainstInfo.cardSet.CardList)
            {
                await Trigger(card, e[card][TriggerTime.When]);
            }
            foreach (var card in AgainstInfo.cardSet.CardList)
            {
                await Trigger(card, e[card][TriggerTime.After]);
            }
        }
        /// <summary>
        /// 效果连锁触发广播
        /// 在触发xx效果时
        /// 会向其他对战中其他卡牌广播通知"xx牌触发xx效果"卡牌，方便进行连锁判定
        /// 一般由卡牌效果触发，如打出部署时、部署时、死亡时等
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static async Task TriggerBroadcast(Event e)
        {
            //Debug.LogError($"触发目标{TriggerTime.Before}{e.TriggerType}");
            if (e.TriggerCard!=null)
            {
                GameSystem.UiSystem.ShowIntroductionCard(e.TriggerCard.CardID);
            }
            if (e.TargetCards.Any())
            {
                //遍历所有触发对象，对每个对象目标外的全体对象广播xx效果前效果
                foreach (var targetCard in e.TargetCards)
                {
                    foreach (var card in AgainstInfo.cardSet.CardList.Where(card => card != targetCard))
                    {
                        //await Trigger(card, e[card][TriggerTime.Before]);
                        await Trigger(card, e[TriggerTime.Before]);
                    }
                }
                //如果以同时方式触发效果，则进行同时触发，并等待所有连锁效果触发完成后再继续触发xx效果后的效果
                if (e.triggerMeanWhile)
                {
                    List<Task> tasks = new List<Task>();
                    foreach (var card in e.TargetCards)
                    {
                        tasks.Add(Trigger(card, e[card][TriggerTime.When]));
                    }
                    await Task.WhenAll(tasks.ToArray());
                }
                //如果以依次方式触发效果，则目标xx效果执行并等待所有连锁效果触发完成再触发下一个目标的xx效果，全部触发万抽后触发xx效果后的效果
                else
                {
                    foreach (var card in e.TargetCards)
                    {
                        await Trigger(card, e[card][TriggerTime.When]);
                    }
                }
                //Debug.LogError($"触发目标{TriggerTime.After}{e.TriggerType}");
                ////遍历所有触发对象，对每个对象目标外的全体对象广播XX之后效果
                foreach (var targetCard in e.TargetCards)
                {
                    foreach (var card in AgainstInfo.cardSet.CardList.Where(card => card != targetCard))
                    {
                        //Debug.LogError($"触发目标{card.CardID}{card.TranslateName}{TriggerTime.After}{e.TriggerType}");
                        //await Trigger(card, e[targetCard][TriggerTime.After]);
                        await Trigger(card, e[TriggerTime.After]);
                    }
                }
            }
            else
            {
                Debug.LogWarning("无生效目标");
            }
        }
        /// <summary>
        /// 遍历触发卡牌对应类型的所有效果，并统计进序列图日志
        /// </summary>
        /// <param name="targetCard"></param>
        /// 触发的对标卡牌
        /// <param name="parentEnent"></param>
        /// 父级触发信息，用于生成序列图返回父级支干
        /// <param name="e"></param>
        /// 本级触发信息，用与生成序列图新支干和触发对应事件
        /// <returns></returns>
        static async Task Trigger(Card targetCard, Event e)
        {
            foreach (var ability in targetCard.CardAbility[e.TriggerTime][e.TriggerType])
            {
                //暂时不记录afterTurnEnd相关数据
                if (e.TriggerTime != TriggerTime.After && e.TriggerType != TriggerType.TurnEnd) ImageSummary.AddNode(e);
                await ability(e);
                if (e.TriggerTime != TriggerTime.After && e.TriggerType != TriggerType.TurnEnd) ImageSummary.RebackNode();
            }
        }
    }
}