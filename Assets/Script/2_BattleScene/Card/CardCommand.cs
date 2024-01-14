using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    /// <summary>
    /// 各种卡牌效果的实现底层
    /// </summary>
    public static class CardCommand
    {

        #region 点数变化
        public static async Task Set(Event e)
        {
            var targetShowPoint = e.TargetCard.ShowPoint;

            await BulletCommand.InitBulletAsync(e);
            int actualChangePoint = e.point - e.TargetCard.ShowPoint;

            await e.TargetCard.Manager.ShowTips((actualChangePoint > 0 ? "+" : "") + actualChangePoint, Color.gray, false);
            e.TargetCard.ChangePoint = e.point - e.TargetCard.BasePoint;
            e.TargetCard.Manager.RefreshCardUi();
            //如果原本点数与设置点数不同，则触发相应
            if (targetShowPoint < e.point)
            {
                await GameSystem.PointSystem.Increase(e);
            }
            if (targetShowPoint > e.point)
            {
                await GameSystem.PointSystem.Decrease(e);
            }
        }
        public static async Task Gain(Event e)
        {
            await BulletCommand.InitBulletAsync(e);
            //await Task.Delay(1000);
            int actualChangePoint = e.point;
            await e.TargetCard.Manager.ShowTips("+" + actualChangePoint, Color.green, false);
            e.TargetCard.ChangePoint += e.point;
            e.TargetCard.Manager.RefreshCardUi();
            await Task.Delay(1000);
            if (e.point > 0)//如果不处于死亡状态，且点数确实增加，则触发点数增加事件
            {
                await GameSystem.PointSystem.Increase(e);
            }
        }
        public static async Task Cure(Event e)
        {
            int gainPoint = -Math.Min(0, e.TargetCard.ChangePoint);
            await GameSystem.PointSystem.Gain(e.SetPoint(gainPoint));
        }
        public static async Task Hurt(Event e)
        {
            var targetCard = e.TargetCard;
            await BulletCommand.InitBulletAsync(e);
            _ = ShakeCard(e.TargetCard);
            if (targetCard[CardState.Congealbounds])
            {
                await GameSystem.StateSystem.ClearState(new Event(e.TriggerCard, targetCard));
            }
            else
            {
                //抵消护盾
                if (targetCard[CardField.Shield] > 0)
                {
                    //计算剩余盾量
                    var shieldPoint = targetCard[CardField.Shield] - e.point;
                    //计算剩余伤害
                    e.point = e.point - targetCard[CardField.Shield];
                    //调整护盾值
                    await GameSystem.FieldSystem.SetField(new Event(e.TriggerCard, targetCard).SetPoint(shieldPoint));
                }
                await Task.Delay(1000);
                //悬浮伤害数字
                int actualChangePoint = Math.Min(e.point, e.TargetCard.ShowPoint);
                await e.TargetCard.Manager.ShowTips("-" + actualChangePoint, Color.red, false);
                e.TargetCard.ChangePoint -= actualChangePoint;
                e.TargetCard.ChangePoint = Math.Max(e.TargetCard.ChangePoint, -e.TargetCard.BasePoint);
                e.TargetCard.Manager.RefreshCardUi();
                await Task.Delay(1000);
                //如果点数确实减少了则同时触发点数减少事件
                if (e.point > 0)
                {
                    await GameSystem.PointSystem.Decrease(e);
                }
                //我死啦
                if (targetCard.ShowPoint == 0)
                {
                    //延命
                    if (targetCard[CardState.Apothanasia])
                    {
                        await GameSystem.StateSystem.ClearState(new Event(targetCard, GameSystem.InfoSystem.SelectUnits).SetTargetState(CardState.Apothanasia));
                        await GameSystem.PointSystem.Gain(new Event(targetCard, targetCard).SetPoint(1));
                    }
                }
            }
        }
        public static async Task Destory(Event e)
        {
            int hurtPoint = (e.TargetCard.ShowPoint + e.TargetCard[CardField.Shield]);
            await GameSystem.PointSystem.Hurt(e.SetPoint(hurtPoint));
        }
        //逆转-组合出发效果
        public static async Task Reversal(Event e)
        {
            int triggerCardPoint = e.TriggerCard.ShowPoint;
            int targetCardPoint = e.TargetCard.ShowPoint;
            _ = GameSystem.PointSystem.Set(new Event(e.TriggerCard, e.TargetCard).SetPoint(triggerCardPoint));
            _ = GameSystem.PointSystem.Set(new Event(e.TargetCard, e.TriggerCard).SetPoint(targetCardPoint));
            await Task.Delay(1000);
        }
        #endregion
        #region 位置变化
        /// <summary>
        /// 在对局中创造出一张卡牌
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Card CreatCard(Event e)
        {
            GameObject newCard = GameObject.Instantiate(Info.CardInfo.cardModel, new Vector3(0, 100, 0), Info.CardInfo.cardModel.transform.rotation);
            newCard.transform.SetParent(GameObject.FindGameObjectWithTag("Card").transform);
            newCard.SetActive(true);
            newCard.name = "Card" + Info.CardInfo.CreatCardRank++;
            //若编辑器下则直接加载本地GameCard的dll卡牌脚本
            //否则从数据库下载卡牌脚本
            if (Application.isEditor)
            {
                //直接加载
                Type componentType;
                if (AgainstInfo.LoagScriptFromLoacl)
                {
                    //直接加载
                    componentType = Type.GetType("TouhouMachineLearningSummary.CardSpace.Card" + e.TargetCardId);
                }
                else
                {
                    //通过反射加载
                    componentType = Assembly.Load(File.ReadAllBytes(@"Library\ScriptAssemblies\GameCard.dll")).GetType("TouhouMachineLearningSummary.CardSpace.Card" + e.TargetCardId);
                }
                newCard.AddComponent(componentType);
            }
            else
            {
                newCard.AddComponent(Manager.CardAssemblyManager.GetCardScript(e.TargetCardId));
            }
            Card card = newCard.GetComponent<Card>();
            var CardStandardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(e.TargetCardId);
            card.CardID = CardStandardInfo.cardID;
            card.BasePoint = CardStandardInfo.point;
            card.CardFace = CardStandardInfo.cardFace;
            if (e.carkBackID == "") e.carkBackID = AgainstInfo.myCardBackIndex;
            card.CardBack = CardStandardInfo.GetCardBack(e.carkBackID);
            card.CardDeployRegion = CardStandardInfo.cardDeployRegion;
            card.CardDeployTerritory = CardStandardInfo.cardDeployTerritory;
            card.TranslateTags = CardStandardInfo.TranslateTags;
            card.Rank = CardStandardInfo.cardRank;
            card.Type = CardStandardInfo.cardType;
            card.refCardIDs = CardStandardInfo.refCardIDs;
            card.GetComponent<Renderer>().material.SetTexture("_Front", card.CardFace);
            card.GetComponent<Renderer>().material.SetTexture("_Back", card.CardBack);
            switch (card.Rank)
            {
                case CardRank.Leader: card.GetComponent<Renderer>().material.SetColor("_side", new Color(0.43f, 0.6f, 1f)); break;
                case CardRank.Gold: card.GetComponent<Renderer>().material.SetColor("_side", new Color(0.8f, 0.8f, 0f)); break;
                case CardRank.Silver: card.GetComponent<Renderer>().material.SetColor("_side", new Color(0.75f, 0.75f, 0.75f)); break;
                case CardRank.Copper: card.GetComponent<Renderer>().material.SetColor("_side", new Color(1, 0.42f, 0.37f)); break;
                default: break;
            }
            return card;
        }
        /// <summary>
        /// 创建一个卡牌，可指定生成位置，若生成在出牌区则自动触发打出效果
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<Card> GenerateCard(Event e)
        {
            var card = e.TargetCard;

            var location = e.location;
            //加入指定区域，若生成在出牌区则自动触发打出效果
            if (location != null)
            {
                int rank = location.Rank >= 0 ? location.Rank : Math.Max(0, AgainstInfo.GameCardsFilter[location.Row].Count + 1 + location.Rank);
                AgainstInfo.GameCardsFilter[location.Row].Insert(rank, card);
                if (AgainstInfo.GameCardsFilter[GameRegion.Used].ContainCardList.Contains(card))
                {
                    await GameSystem.TransferSystem.PlayCard(new Event(null, card));
                }
            }
            //如果目标区域在非战场区域，或者在战场区域且单位数少于6才触发生成效果
            if (GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle].ContainRowInfos.Select(x => x.RowRank).Contains(e.location.Row) && GameSystem.InfoSystem.AgainstCardSet[e.location.Row].Count >= 6)
            {
                //不满足生成条件
                Debug.LogWarning("溢出后摧毁自身");
                await GameSystem.PointSystem.Destory(new Event(null, card));
            }
            RowCommand.RefreshAllRowsCards();
            return card;
        }
        /// <summary>
        /// 从对战记录的简易卡牌模型复现原卡牌,不会触发生成连锁效果
        /// </summary>
        /// <param name="simpleCard"></param>
        /// <returns></returns>
        public static Card GenerateCard(SimpleCardModel simpleCard)
        {
            var card = CreatCard(new Event(null, targetCard: null).SetTargetCardId(simpleCard.CardID).SetCardBack(simpleCard.CardBackID));
            card.Init();
            RowCommand.RefreshAllRowsCards();
            return card;
        }
        /// <summary>
        /// 待部署卡牌时创造的灰色标记卡牌，无实际效果
        /// </summary>
        /// <param name="sampleCard"></param>
        /// <returns></returns>
        public static Card GenerateTempCard(Event e)
        {
            var card = CreatCard(e);
            card.IsGray = true;
            AgainstInfo.GameCardsFilter[e.location.Row].Insert(e.location.Rank, card);
            RowCommand.RefreshAllRowsCards();
            return card;
        }
        public static async Task BanishCard(Event e)
        {
            Card card = e.TargetCard;
            await card.Manager.CreatGapAsync();
            RowCommand.RefreshAllRowsCards();
        }
        public static void OrderHandCard()
        {
            AgainstInfo.GameCardsFilter[GameRegion.Hand].ContainRowInfos.ForEach(singleRowInfo =>
            {
                AgainstInfo.GameCardsFilter[singleRowInfo.RowRank] = AgainstInfo.GameCardsFilter[singleRowInfo.RowRank].OrderByDescending(card => card.Rank).ThenBy(card => card.BasePoint).ThenBy(card => card.CardID).ToList();
            });
            RowCommand.RefreshAllRowsCards();
        }
        //卡牌的通用移动效果
        public static async Task MoveCard(Event e)
        {
            if (e.TargetCard == null) return;
            await Task.Delay(500);
            RowCommand.SetBelongRow(e.TargetCard, e.location);
            //移动至不同区域做不同效果
            switch (e.region)
            {
                case GameRegion.Hand:
                    OrderHandCard();
                    break;
                case GameRegion.Used:
                    break;
                case GameRegion.Deck:
                    break;
                case GameRegion.Grave:
                    //重置卡牌状态
                    e.TargetCard.cardFields.Clear();
                    e.TargetCard.cardStates.Clear();
                    e.TargetCard.ChangePoint = 0;
                    break;
                default:
                    break;
            }
            e.TargetCard.isMoveStepOver = false;
            await Task.Delay(100);
            e.TargetCard.isMoveStepOver = true;
            await SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
            RowCommand.RefreshAllRowsCards();
        }
        public static async Task SummonCard(Event e)
        {
            await MoveCard(e.SetLocation(e.TargetCard.CurrentOrientation, (GameRegion)e.TargetCard.CardDeployRegion, -1));
            //Card card = e.TargetCard;
            //Debug.LogWarning("召唤卡牌于" + card.CurrentOrientation);
            //card.BelongCardList.Remove(card);
            //AgainstInfo.cardSet[(GameRegion)card.CardDeployRegion][card.CurrentOrientation].RowManagers.First().CardList.Add(card);
            //card.isMoveStepOver = false;
            //await Task.Delay(200);
            //card.isMoveStepOver = true;
            //await SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
            //RowCommand.RefreshAllRowsCards();
        }

        public static async Task DeadCard(Event e)
        {
            //撕碎特效
            await MoveToGrave(e);
            //撕碎特效
        }
        public static async Task MoveToGrave(Event e)
        {
            Card card = e.TargetCard;
            if (card == null) return;
            await Task.Delay(100);
            Orientation targetOrientation = card.CurrentOrientation;
            card.BelongCardList.Remove(card);
            AgainstInfo.GameCardsFilter[card.CurrentOrientation][GameRegion.Grave].ContainRowInfos[0].CardList.Insert(0, card);
            //重置卡牌状态
            card.cardFields.Clear();
            card.cardStates.Clear();
            card.ChangePoint = 0;
            card.isMoveStepOver = false;
            await Task.Delay(50);
            card.isMoveStepOver = true;
            await SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
            RowCommand.RefreshAllRowsCards();
        }

        public static async Task DeployCard(Card card, bool reTrigger)
        {
            if (!reTrigger)
            {
                card.BelongCardList.Remove(card);
                AgainstInfo.SelectRowCardList.Insert(AgainstInfo.SelectRank, card);
            }
            card.isMoveStepOver = false;
            await Task.Delay(200);
            card.isMoveStepOver = true;
            _ = SoundEffectCommand.PlayAsync(SoundEffectType.Deploy);
            RowCommand.RefreshAllRowsCards();
        }
        /// <summary>
        /// 洗牌
        /// </summary>
        /// <param name="card">要洗的卡牌</param>
        /// <param name="IsPlayerExchange">是否操控当前玩家洗牌</param>
        /// <param name="isRoundStartExchange">是否回合开始洗牌</param>
        /// <param name="insertRank">洗入位置</param>
        /// <returns></returns>
        public static async Task ExchangeCard(Card card, bool IsPlayerExchange = true, bool isRoundStartExchange = false, int insertRank = 0)
        {
            //Debug.Log("交换卡牌");
            await WashCard(card, IsPlayerExchange, insertRank);
            await CardCommand.DrawCard(new Event(null, GameSystem.InfoSystem.AgainstCardSet[IsPlayerExchange ? Orientation.My : Orientation.Op][GameRegion.Deck].ContainCardList.FirstOrDefault()).SetDrawCard(IsPlayerExchange ? Orientation.My : Orientation.Op, true));
            //await DrawCard(IsPlayerExchange);
            if (IsPlayerExchange)
            {
                CardBoardCommand.ShowCardBoard(AgainstInfo.GameCardsFilter[isRoundStartExchange ? Orientation.Down : Orientation.My][GameRegion.Hand].ContainCardList, CardBoardMode.ExchangeCard);
            }
            static async Task WashCard(Card card, bool IsPlayerWash = true, int InsertRank = 0)
            {
                Debug.Log("洗回卡牌");
                if (IsPlayerWash)
                {
                    AgainstInfo.TargetCard = card;
                    int MaxCardRank = AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Deck].ContainCardList.Count;
                    AgainstInfo.washInsertRank = AiCommand.GetRandom(0, MaxCardRank);
                    NetCommand.AsyncInfo(NetAcyncType.ExchangeCard);
                    AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Hand].Remove(card);
                    AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Deck].Add(card, AgainstInfo.washInsertRank);
                }
                else
                {
                    AgainstInfo.GameCardsFilter[Orientation.Up][GameRegion.Hand].Remove(card);
                    AgainstInfo.GameCardsFilter[Orientation.Up][GameRegion.Deck].Add(card, InsertRank);
                }
                RowCommand.RefreshAllRowsCards();
            }
        }

        public static async Task PlayCard(Card card)
        {
            await Task.Delay(0);//之后实装卡牌特效需要时间延迟配合
            card.isPrepareToPlay = false;
            _ = SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
            RowCommand.SetPlayCardMoveFree(false);
            NetCommand.AsyncInfo(NetAcyncType.PlayCard);
            ChainManager.ShowChainCount();
            card.BelongCardList.Remove(card);
            AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Used].Add(card);
            AgainstInfo.playerPlayCard = null;
            RowCommand.RefreshAllRowsCards();
        }
        public static async Task DrawCard(Event e)
        {
            if (e.TargetCard != null)
            {
                e.TargetCard.BelongCardList.Remove(e.TargetCard);
                AgainstInfo.GameCardsFilter[e.orientation][GameRegion.Hand].Add(e.TargetCard);
            }
            await Task.Delay(800);
            _ = SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
            //抽完后重新洗牌
            if (e.isOrder)
            {
                OrderHandCard();
            }
            //制作一个卡牌抽完后插入牌组的动画效果
            e.TargetCard.isDrawStepOver = false;
            e.TargetCard.Manager.RefreshTransform();
            e.TargetCard.Manager.RefreshCardVisible();
            await Task.Delay(1000);
            e.TargetCard.isDrawStepOver = true;
            e.TargetCard.Manager.RefreshTransform();
        }
        public static async Task DisCard(Event e)
        {
            var card = e.TargetCard;
            await Task.Delay(0);//之后实装卡牌特效需要时间延迟配合
            _ = SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
            RowCommand.SetPlayCardMoveFree(false);
            NetCommand.AsyncInfo(NetAcyncType.DisCard);
            card.isPrepareToPlay = false;
            card.BelongCardList.Remove(card);
            AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Grave].Add(card);
            AgainstInfo.playerDisCard = null;
            RowCommand.RefreshAllRowsCards();
        }
        public static async Task RebackCard(Event e)
        {
            //还清空卡牌状态
            await PlayCard(e.TargetCard);
        }
        public static async Task ReviveCard(Event e)
        {
            Card card = e.TargetCard;
            //墓地复活动画
            await PlayCard(card);
        }
        #endregion
        #region 附加状态与附加值变化
        //执行后需要刷新卡牌ui
        public static async Task StateAdd(Event e)
        {
            var card = e.TargetCard;
            _ = GameSystem.UiSystem.ShowIcon(card, e.TargetState);
            _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.CardSelect);
            switch (e.TargetState)
            {
                case CardState.Lurk:; break;
                case CardState.Seal: card.transform.GetChild(2).gameObject.SetActive(true); break;
                case CardState.None:
                    break;
                case CardState.Invisibility:
                    break;
                case CardState.Pry:
                    break;
                case CardState.Close:
                    break;
                case CardState.Fate:
                    break;
                case CardState.Furor:
                    await GameSystem.UiSystem.ShowTips(card, "狂躁", new Color(0.6f, 0.2f, 0));
                    break;
                case CardState.Docile:
                    await GameSystem.UiSystem.ShowTips(card, "温顺", new Color(0, 0, 0.5f));
                    break;
                case CardState.Poisoning:
                    break;
                case CardState.Rely:
                    break;
                case CardState.Water:
                    if (card.cardStates.Contains(CardState.Water))
                    {
                        card.cardStates.Remove(CardState.Water);
                        await GameSystem.UiSystem.ShowTips(card, "治愈", new Color(0, 1, 0));
                    }
                    else if (card.cardStates.Contains(CardState.Fire))
                    {
                        card.cardStates.Remove(CardState.Fire);
                        await GameSystem.UiSystem.ShowTips(card, "中和", new Color(0, 1, 0));
                    }
                    else if (card.cardStates.Contains(CardState.Wind))
                    {
                        card.cardStates.Remove(CardState.Wind);
                        await GameSystem.UiSystem.ShowTips(card, "扩散", new Color(0, 1, 0));

                    }
                    else if (card.cardStates.Contains(CardState.Soil))
                    {
                        card.cardStates.Remove(CardState.Soil);
                        await GameSystem.UiSystem.ShowTips(card, "泥泞", new Color(1, 1, 0));
                    }
                    else
                    {
                        card.cardStates.Add(CardState.Water);
                    }
                    return;
                case CardState.Fire:
                    if (card.cardStates.Contains(CardState.Water))
                    {
                        card.cardStates.Remove(CardState.Water);
                        await GameSystem.UiSystem.ShowTips(card, "中和", new Color(0, 1, 0));
                    }
                    else if (card.cardStates.Contains(CardState.Fire))
                    {
                        card.cardStates.Remove(CardState.Fire);
                        await GameSystem.UiSystem.ShowTips(card, "烧灼", new Color(0, 1, 0));
                    }
                    else if (card.cardStates.Contains(CardState.Wind))
                    {
                        card.cardStates.Remove(CardState.Wind);
                        await GameSystem.UiSystem.ShowTips(card, "扩散", new Color(0, 1, 0));

                    }
                    else if (card.cardStates.Contains(CardState.Soil))
                    {
                        card.cardStates.Remove(CardState.Soil);
                        await GameSystem.UiSystem.ShowTips(card, "熔岩", new Color(1, 1, 0));
                    }
                    else
                    {
                        card.cardStates.Add(CardState.Fire);
                    }
                    return;
                case CardState.Wind:
                    if (card.cardStates.Contains(CardState.Water))
                    {
                        card.cardStates.Remove(CardState.Water);
                        await GameSystem.UiSystem.ShowTips(card, "扩散", new Color(0, 1, 0));
                    }
                    else if (card.cardStates.Contains(CardState.Fire))
                    {
                        card.cardStates.Remove(CardState.Fire);
                        await GameSystem.UiSystem.ShowTips(card, "扩散", new Color(0, 1, 0));
                    }
                    else if (card.cardStates.Contains(CardState.Wind))
                    {
                        card.cardStates.Remove(CardState.Wind);
                        await GameSystem.UiSystem.ShowTips(card, "烈风", new Color(0, 1, 0));

                    }
                    else if (card.cardStates.Contains(CardState.Soil))
                    {
                        card.cardStates.Remove(CardState.Soil);
                        await GameSystem.UiSystem.ShowTips(card, "扩散", new Color(1, 1, 0));
                    }
                    else
                    {
                        card.cardStates.Add(CardState.Wind);
                    }
                    return;
                case CardState.Soil:
                    if (card.cardStates.Contains(CardState.Water))
                    {
                        card.cardStates.Remove(CardState.Water);
                        await GameSystem.UiSystem.ShowTips(card, "泥泞", new Color(0, 1, 0));
                    }
                    else if (card.cardStates.Contains(CardState.Fire))
                    {
                        card.cardStates.Remove(CardState.Fire);
                        await GameSystem.UiSystem.ShowTips(card, "熔岩", new Color(0, 1, 0));
                    }
                    else if (card.cardStates.Contains(CardState.Wind))
                    {
                        card.cardStates.Remove(CardState.Wind);
                        await GameSystem.UiSystem.ShowTips(card, "扩散", new Color(0, 1, 0));

                    }
                    else if (card.cardStates.Contains(CardState.Soil))
                    {
                        card.cardStates.Remove(CardState.Soil);
                        await GameSystem.UiSystem.ShowTips(card, "固化", new Color(1, 1, 0));
                    }
                    else
                    {
                        card.cardStates.Add(CardState.Soil);
                    }
                    return;
                case CardState.Hold:
                    break;
                case CardState.Congealbounds:
                    break;
                case CardState.Forbidden:
                    break;
                case CardState.Black:
                    if (card.cardStates.Contains(CardState.White))
                    {
                        await GameSystem.SelectSystem.SelectUnit(card, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardRank.NoGold].ContainCardList, 1, true);
                        await GameSystem.PointSystem.Hurt(new Event(card, GameSystem.InfoSystem.SelectUnits).SetPoint(1));
                        card.cardStates.Remove(CardState.White);
                    }
                    break;
                case CardState.White:
                    if (card.cardStates.Contains(CardState.Black))
                    {
                        await GameSystem.SelectSystem.SelectUnit(card, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.NoGold].ContainCardList, 1, true);
                        await GameSystem.PointSystem.Gain(new Event(card, GameSystem.InfoSystem.SelectUnits).SetPoint(1));
                        card.cardStates.Remove(CardState.Black);
                    }
                    break;
                case CardState.Apothanasia:
                    await GameSystem.UiSystem.ShowTips(card, "续命", new Color(1, 0, 0));
                    break;
                    break;
                case CardState.Cover:
                    break;
                default: break;
            }
            card[e.TargetState] = true;
            card.Manager.RefreshCardUi();
        }
        public static async Task StateClear(Event e)
        {
            var card = e.TargetCard;
            await GameSystem.UiSystem.ShowIconBreak(card, e.TargetState);
            card[e.TargetState] = false;
            //动画效果
            switch (e.TargetState)
            {
                case CardState.Lurk:; break;
                case CardState.Seal: card.transform.GetChild(2).gameObject.SetActive(false); break;
                default: break;
            }
            card.Manager.RefreshCardUi();
        }
        public static async Task FieldSet(Event e)
        {
            var card = e.TargetCard;
            if (e.point > card[e.TargetFiled])
            {
                await GameSystem.UiSystem.ShowIcon(card, e.TargetFiled);
            }
            else if (e.point < card[e.TargetFiled])
            {
                await GameSystem.UiSystem.ShowIconBreak(card, e.TargetFiled);
            }
            else
            {
                return;
            }
            Debug.Log($"触发类型：{e.TargetFiled}当字段设置，对象卡牌{card.CardID}原始值{card[e.TargetFiled]},设置值{e.point}");
            card[e.TargetFiled] = e.point;
            Debug.Log($"触发结果：{card[e.TargetFiled]}");
            //移除掉为0的字段
            if (card[e.TargetFiled] > 0)
            {
                switch (e.TargetFiled)
                {
                    case CardField.Timer: break;
                    case CardField.Inspire: break;

                    default: break;
                }
            }
            else
            {
                card.cardFields.Remove(e.TargetFiled);
            }
            card.Manager.RefreshCardUi();
        }
        public static async Task FieldChange(Event e)
        {
            var card = e.TargetCard;
            if (e.point > 0)
            {
                await GameSystem.UiSystem.ShowIcon(card, e.TargetFiled);
            }
            else if (e.point < 0)
            {
                await GameSystem.UiSystem.ShowIconBreak(card, e.TargetFiled);
            }
            else
            {
                return;
            }

            Debug.Log($"触发类型：{e.TargetFiled}当字段变化，对象卡牌{card.CardID}原始值{card[e.TargetFiled]},变化值{e.point}");
            card[e.TargetFiled] += e.point;
            Debug.Log($"触发结果：{card[e.TargetFiled]}");
            switch (e.TargetFiled)
            {
                case CardField.Timer: break;
                case CardField.Inspire: break;
                default: break;
            }
            card.Manager.RefreshCardUi();
        }
        #endregion
        #region 流程
        public static async Task TurnEnd(Event e)
        {
            var card = e.TargetCard;

            //将判定为死掉卡牌移入墓地，触发遗愿联锁效果
            //我死啦
            if (card.IsCardReadyToGrave)
            {
                //摧毁自身同时触发咒术
                Debug.LogError(card.CardID + card.BasePoint + "=" + card.ChangePoint);
                await GameSystem.TransferSystem.DeadCard(e);
            }
            card.Manager.RefreshCardUi();
        }
        public static async Task RoundEnd(Event e)
        {
            var card = e.TargetCard;
            if (AgainstInfo.GameCardsFilter[GameRegion.Battle].ContainCardList.Contains(card))
            {
                //如果有驻守则清掉状态保持原位，不然移入墓地
                if (card[CardState.Hold])
                {
                    await GameSystem.StateSystem.ClearState(new Event(card, card).SetTargetState(CardState.Hold));
                }
                else
                {
                    await GameSystem.TransferSystem.DeadCard(e);
                    //_ = Command.CardCommand.MoveToGrave(e);
                }
            }
            card.Manager.RefreshCardUi();
        }
        #endregion
        #region 其他
        //卡牌震动效果
        public static async Task ShakeCard(Card card)
        {
            var rigiBody = card.GetComponent<Rigidbody>();
            rigiBody.isKinematic = false;
            rigiBody.AddForce(Vector3.up * 3 + UnityEngine.Random.onUnitSphere, ForceMode.Impulse);
            rigiBody.AddRelativeTorque(UnityEngine.Random.onUnitSphere, ForceMode.Impulse);
            await Task.Delay(2000);
            rigiBody.isKinematic = true;
            //重置当前牌目标位置，保证能触发位置刷新
            card.Manager.TargetPosition = Vector3.zero;
            RowCommand.RefreshAllRowsCards();
        }
        #endregion
    }
}