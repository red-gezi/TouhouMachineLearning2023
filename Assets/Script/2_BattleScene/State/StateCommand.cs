using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Config;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Other;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TouhouMachineLearningSummary.Command
{
    public static class StateCommand
    {
        //初始化双方状态并判断当前是否第初始回合换牌阶段
        internal static bool AgainstStateInit()
        {
            Info.CardInfo.CreatCardRank = 0;
            //加载目标回合，同步下状态
            AgainstSummaryManager.TurnOperation targetJumpTurn = AgainstInfo.summary.TargetJumpTurn;
            AgainstInfo.roundRank = targetJumpTurn.RoundRank;
            AgainstInfo.turnRank = targetJumpTurn.TurnRank;
            AgainstInfo.totalTurnRank = targetJumpTurn.TotalTurnRank;
            //判断该回合是否为预处理换牌阶段
            bool isExchangeTurn = targetJumpTurn.TurnRank == 0;
            //根据对战中的回合数据初始化场上卡牌
            AgainstInfo.GameCardsFilter = new CardsFilter();
            //此处根据对战记录塞卡牌数据
            RowCommand.SetCardListFromSummary(targetJumpTurn.AllCardList);
            AgainstInfo.IsJumpMode = false;
            return isExchangeTurn;
        }
        ////////////////////////////////////////////////////对局流程指令////////////////////////////////////////////////////////////////////////////////
        public static async Task AgainstStart()
        {
            Info.CardInfo.CreatCardRank = 0;
            TaskThrowCommand.Init();
            //如果不是通过配置文件启动的场景,而是在编辑器中做卡组测试，则赋予默认测试卡组信息
            if (AgainstInfo.CurrentUserInfo == null)
            {
                //如果在编辑器停止播放游戏则中断接下来的步骤
                TaskThrowCommand.Throw();
                AgainstInfo.IsMyTurn = true;
                AgainstInfo.CurrentUserInfo = DeckConfig.GetPlayerCardDeck("test");
                AgainstInfo.CurrentOpponentInfo = DeckConfig.GetOpponentCardDeck("test");
            }
            //初始化对局集合信息
            AgainstInfo.GameCardsFilter = new CardsFilter();
            RowCommand.SetRegionSelectable(GameRegion.None);
            await CustomThread.Delay(1500);
            Manager.LoadingManager.manager?.CloseAsync();
            //Debug.LogError("初始双方信息");
            //await Task.Delay(500);
            await UiCommand.NoticeBoardShow("BattleStart".TranslationGameText());
            Location GenerateLocation;
            //初始化我方领袖卡
            await GameSystem.TransferSystem.GenerateCard(new Event()
                .SetTargetCardId(AgainstInfo.UserDeck.LeaderID)
                .SetLocation(Orientation.Down, GameRegion.Leader, 0)
                .SetCardBack(AgainstInfo.myCardBackIndex));
            //初始化敌方领袖卡
            await GameSystem.TransferSystem.GenerateCard(new Event()
                .SetTargetCardId(AgainstInfo.OpponentDeck.LeaderID)
                .SetLocation(Orientation.Up, GameRegion.Leader, 0)
                .SetCardBack(AgainstInfo.opCardBackIndex));
            //初始双方化牌组
            AgainstInfo.UserDeck.CardIDs.ForEach(async cardId =>
            await GameSystem.TransferSystem.GenerateCard(new Event()
                .SetTargetCardId(cardId)
                .SetLocation(Orientation.Down, GameRegion.Deck, -1)
                .SetCardBack(AgainstInfo.myCardBackIndex)));
            AgainstInfo.OpponentDeck.CardIDs.ForEach(async cardId =>
            await GameSystem.TransferSystem.GenerateCard(new Event()
                .SetTargetCardId(cardId)
                .SetLocation(Orientation.Up, GameRegion.Deck, -1)
                .SetCardBack(AgainstInfo.opCardBackIndex)));
            await CustomThread.Delay(000);
        }
        public static async Task AgainstEnd(bool IsSurrender = false, bool IsWin = true)
        {
            await UiCommand.NoticeBoardShow($"对战终止\n{AgainstInfo.ShowScore.MyScore}:{AgainstInfo.ShowScore.OpScore}");
            if (AgainstInfo.isShouldUploadSummaryOperation)
            {
                await Command.NetCommand.AgainstFinish();
            }
            await Task.Delay(2000);
            //Debug.Log("释放线程资源");
            TaskThrowCommand.Token.Cancel();
            //如果是故事模式，假如胜利则更新玩家进度
            if (Info.PageComponentInfo.currentAgainstMode == AgainstModeType.Story && IsWin)
            {
                Debug.LogWarning("解锁" + Info.PageComponentInfo.CurrentStage + "--" + Info.PageComponentInfo.CurrentStep);
                await Command.DialogueCommand.UnlockAsync(Info.PageComponentInfo.CurrentStage, Info.PageComponentInfo.CurrentStep);
            }
            SceneManager.LoadScene("1_LoginScene");
            //await Manager.CameraViewManager.MoveToViewAsync(2);

        }
        public static async Task RoundStart()
        {
            ReSetPassState();
            await UiCommand.NoticeBoardShow($"第{AgainstInfo.roundRank}小局开始");
            //await GameSystem.SelectSystem.SelectProperty();
            //await Task.Delay(2000);
            switch (AgainstInfo.roundRank)
            {
                case (1):
                    {
                        Info.AgainstInfo.ExChangeableCardNum = 3;
                        UiCommand.SetCardBoardTitle("Remaining".TranslationGameText() + Info.AgainstInfo.ExChangeableCardNum);
                        for (int i = 0; i < 10; i++)
                        {
                            await CustomThread.Delay(100);
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            //_ = CardCommand.DrawCard(isPlayerDraw: true, isOrder: false);
                            //_ = CardCommand.DrawCard(isPlayerDraw: false, isOrder: false);
                            _ = CardCommand.DrawCard(new Event(null, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].ContainCardList.FirstOrDefault()).SetDrawCard(Orientation.My, false));
                            _ = CardCommand.DrawCard(new Event(null, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Deck].ContainCardList.FirstOrDefault()).SetDrawCard(Orientation.Op, false));
                            await CustomThread.Delay(100);
                        }
                        await CustomThread.Delay(3000);
                        CardCommand.OrderHandCard();
                        break;
                    }
                case (2):
                    {
                        Info.AgainstInfo.ExChangeableCardNum += 1;
                        UiCommand.SetCardBoardTitle("Remaining".TranslationGameText() + Info.AgainstInfo.ExChangeableCardNum);
                        _ = CardCommand.DrawCard(new Event(null, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].ContainCardList.FirstOrDefault()).SetDrawCard(Orientation.My, true));
                        _ = CardCommand.DrawCard(new Event(null, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Deck].ContainCardList.FirstOrDefault()).SetDrawCard(Orientation.Op, true));

                        //await CardCommand.DrawCard();
                        //await CardCommand.DrawCard(false);
                        break;
                    }
                case (3):
                    {
                        Info.AgainstInfo.ExChangeableCardNum += 1;
                        UiCommand.SetCardBoardTitle("Remaining".TranslationGameText() + Info.AgainstInfo.ExChangeableCardNum);
                        _ = CardCommand.DrawCard(new Event(null, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].ContainCardList.FirstOrDefault()).SetDrawCard(Orientation.My, true));
                        _ = CardCommand.DrawCard(new Event(null, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Deck].ContainCardList.FirstOrDefault()).SetDrawCard(Orientation.Op, true));
                        //await CardCommand.DrawCard();
                        //await CardCommand.DrawCard(false);
                        break;
                    }
                default:
                    break;

            }
            await CustomThread.Delay(3000);
            Command.RowCommand.RefreshAllRowsCards();
            //Debug.LogWarning("等待换牌选择");

        }
        public static async Task RoundEnd()
        {
            await UiCommand.NoticeBoardShow($"第{AgainstInfo.roundRank}小局结束\n{AgainstInfo.TotalDownPoint}:{AgainstInfo.TotalUpPoint}\n{((AgainstInfo.TotalDownPoint > AgainstInfo.TotalUpPoint) ? "Win" : "Lose")}");
            //await Task.Delay(2000);
            int result = 0;
            if (AgainstInfo.TotalPlayer1Point > AgainstInfo.TotalPlayer2Point)
            {
                result = 1;
            }
            else if (AgainstInfo.TotalPlayer1Point < AgainstInfo.TotalPlayer2Point)
            {
                result = 2;
            }
            AgainstInfo.PlayerScore.P1Score += result == 0 || result == 1 ? 1 : 0;
            AgainstInfo.PlayerScore.P2Score += result == 0 || result == 2 ? 1 : 0;
            await Task.Delay(3500);
            await GameSystem.ProcessSystem.WhenRoundEnd();
        }
        public static async Task TurnStart()
        {
            AgainstInfo.lastMyChainCount = AgainstInfo.currentMyChainCount;
            AgainstInfo.lastOpChainCount = AgainstInfo.currentOpChainCount;
            AgainstInfo.currentMyChainCount = 0;
            AgainstInfo.currentOpChainCount = 0;

            Manager.AgainstSummaryManager.UploadTurn();
            await UiCommand.NoticeBoardShow((AgainstInfo.IsMyTurn ? "MyTurnStart" : "OpTurnStart").TranslationGameText());
            await GameSystem.ProcessSystem.WhenTurnStart();
            RowCommand.SetPlayCardMoveFree(AgainstInfo.IsMyTurn);
            await CustomThread.Delay(1000);

        }
        public static async Task TurnEnd()
        {
            RowCommand.SetPlayCardMoveFree(false);
            await UiCommand.NoticeBoardShow((AgainstInfo.IsMyTurn ? "MyTurnEnd" : "OpTurnEnd").TranslationGameText());
            await GameSystem.ProcessSystem.WhenTurnEnd();
            await CustomThread.Delay(1000);
            AgainstInfo.IsMyTurn = !AgainstInfo.IsMyTurn;
        }
        ////////////////////////////////////////////////////玩家操作指令////////////////////////////////////////////////////////////////////////////////
        public static async void SetCurrentPass()
        {
            UiInfo.MyPass.SetActive(true);
            (AgainstInfo.IsMyTurn ? ref AgainstInfo.isDownPass : ref AgainstInfo.isUpPass) = true;
            await GameSystem.ProcessSystem.Pass();
        }
        public static void ReSetPassState()
        {
            UiInfo.MyPass.SetActive(false);
            UiInfo.OpPass.SetActive(false);
            Info.AgainstInfo.isUpPass = false;
            Info.AgainstInfo.isDownPass = false;
        }
        public static async Task Surrender()
        {
            Debug.Log("投降");
            Command.NetCommand.AsyncInfo(NetAcyncType.Surrender);
            Manager.AgainstSummaryManager.UploadSurrender(AgainstInfo.IsPlayer1);
            await AgainstEnd(true, false);
        }
        ////////////////////////////////////////////////////等待操作指令////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 等待玩家进行操作
        /// 1：玩家主动
        /// 2：由网络同步的来自对方的操作
        /// 3：Ai自动判断的操作
        /// 4：玩家主动操作超时导致的AI自动操作
        /// 5：由记录复现操作
        /// </summary>
        /// <returns></returns>
        public static async Task WaitForPlayerOperation()
        {
            //先开始计时，然后等待
            CoinManager.SetCoinTimerStart(6000);
            Debug.LogWarning("等待玩家操作");
            //初始化要打出/放弃/过牌操作
            while (true)
            {
                //如果是录播模式，则通过指定的对局数据获取操作记录
                if (AgainstInfo.IsReplayMode)
                {
                    var operation = AgainstInfo.summary.GetCurrentPlayerOperation();
                    if (operation != null)//如果拥有指令则执行，否则为pass跳过
                    {
                        if (operation.Operation.OneHotToEnum<PlayerOperationType>() == PlayerOperationType.PlayCard)
                        {
                            Info.AgainstInfo.playerPlayCard = Info.AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Leader, GameRegion.Hand].ContainCardList[operation.SelectCardIndex];
                            Debug.LogWarning("打出卡牌" + Info.AgainstInfo.playerPlayCard.CardID);

                        }
                        else if (operation.Operation.OneHotToEnum<PlayerOperationType>() == PlayerOperationType.DisCard)
                        {
                            Info.AgainstInfo.playerDisCard = Info.AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Leader, GameRegion.Hand].ContainCardList[operation.SelectCardIndex];
                        }
                        else if (operation.Operation.OneHotToEnum<PlayerOperationType>() == PlayerOperationType.Pass)
                        {
                            AgainstInfo.IsPlayerPass = true;
                        }
                        else
                        {
                            Debug.LogError("对战记录识别出现严重bug");
                            throw new Exception("");
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    //若果当前模式满足ai操作，则交由ai处理
                    if (AgainstInfo.IsAIControl)
                    {
                        await AiCommand.TempPlayOperation();
                    }
                    else
                    {
                        //如果是我的回合则等待玩家操作，否则等待网络同步对方操作
                        if (AgainstInfo.IsMyTurn)
                        {
                            //如果无牌则自动pass
                            if (!AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Leader, GameRegion.Hand].ContainCardList.Any())
                            {
                                SetCurrentPass();
                            }
                        }
                        else
                        {

                        }
                    }
                }
                //如果当前回合出牌
                if (Info.AgainstInfo.playerPlayCard != null)
                {
                    //Debug.Log("当前打出了牌");
                    await AgainstSummaryManager.UploadPlayerOperationAsync(PlayerOperationType.PlayCard, AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Leader, GameRegion.Hand].ContainCardList, AgainstInfo.playerPlayCard);
                    await GameSystem.TransferSystem.PlayCard(new Event(null, AgainstInfo.playerPlayCard));
                    //Debug.Log("打出效果执行完毕");

                    break;
                }
                //如果当前回合弃牌
                if (Info.AgainstInfo.playerDisCard != null)
                {
                    await AgainstSummaryManager.UploadPlayerOperationAsync(PlayerOperationType.DisCard, AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Leader, GameRegion.Hand].ContainCardList, AgainstInfo.playerDisCard);
                    await GameSystem.TransferSystem.DisCard(new Event(null, AgainstInfo.playerDisCard));
                    break;
                }
                if (AgainstInfo.IsMyPass)//如果当前pass则结束回合
                {
                    break;
                }
                else
                {
                    if (AgainstInfo.IsPlayerPass)
                    {
                        await AgainstSummaryManager.UploadPlayerOperationAsync(PlayerOperationType.Pass, AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Leader, GameRegion.Hand].ContainCardList, null);
                        SetCurrentPass();
                        AgainstInfo.IsPlayerPass = false;
                        break;
                    }
                }
                await Task.Delay(10);
                TaskThrowCommand.Throw();
            }
            CoinManager.SetCoinTimerClose();
        }
        public static async Task WaitForSelectProperty()
        {
            //放大硬币
            CoinManager.ScaleUp();
            await CustomThread.Delay(1000);
            CoinManager.Unfold();
            AgainstInfo.IsWaitForSelectProperty = true;
            AgainstInfo.SelectProperty = BattleRegion.None;
            //暂时设为1秒，之后还原成10秒
            CoinManager.SetCoinTimerStart(1);
            // Debug.Log("等待选择属性");
            while (AgainstInfo.SelectProperty == BattleRegion.None)
            {
                TaskThrowCommand.Throw();
                if (AgainstInfo.IsAIControl)
                {
                    //Debug.Log("自动选择属性");
                    int rowRank = AiCommand.GetRandom(0, 4);
                    await CoinManager.ChangePropertyAsync((BattleRegion)rowRank);
                }
                await Task.Delay(1);
            }
            Command.NetCommand.AsyncInfo(NetAcyncType.SelectProperty);
            CoinManager.SetCoinTimerClose();
            AgainstInfo.IsWaitForSelectProperty = false;
            await CustomThread.Delay(1000);
            CoinManager.ScaleDown();
        }
        public static async Task WaitForPlayerExchange()
        {
            AgainstInfo.isRoundStartExchange = true;
            await WaitForSelectBoardCard(null, AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Hand].ContainCardList, CardBoardMode.ExchangeCard);
            AgainstInfo.isRoundStartExchange = false;
        }
        public static async Task WaitForSelectRegion(Card triggerCard, Territory territory, GameRegion regionTypes)
        {
            AgainstInfo.IsWaitForSelectRegion = true;
            AgainstInfo.SelectRowRank = -1;
            RowCommand.SetRegionSelectable(regionTypes, territory);
            while (AgainstInfo.SelectRowRank == -1)
            {
                TaskThrowCommand.Throw();
                if (AgainstInfo.IsReplayMode)
                {
                    var operation = AgainstInfo.summary.GetCurrentSelectOperation();
                    if (operation.Operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectRegion)
                    {
                        AgainstInfo.SelectRowRank = operation.SelectRegionRank;
                    }
                    else
                    {
                        Debug.LogError("对战记录识别出现严重bug");
                        throw new Exception("");
                    }
                }
                else if (AgainstInfo.IsAIControl)
                {
                    await CustomThread.Delay(1000);
                    AgainstInfo.SelectRowRank = AgainstInfo.GameCardsFilter.ContainRowInfos.Where(row => row.CanBeSelected).OrderBy(x => AiCommand.GetRandom()).FirstOrDefault().RowRank;
                }
                await Task.Delay(1);
            }
            NetCommand.AsyncInfo(NetAcyncType.SelectRegion);
            AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectRegion, triggerCard);
            RowCommand.SetRegionSelectable(GameRegion.None);
            AgainstInfo.IsWaitForSelectRegion = false;
        }
        public static async Task WaitForSelectLocation(Card triggerCard, Territory territory, BattleRegion region)
        {
            AgainstInfo.IsWaitForSelectLocation = true;
            //设置指定坐标可选
            RowCommand.SetRegionSelectable((GameRegion)region, territory);
            AgainstInfo.SelectRank = -1;
            while (AgainstInfo.SelectRank < 0)
            {
                TaskThrowCommand.Throw();
                if (AgainstInfo.IsReplayMode)
                {
                    var operation = AgainstInfo.summary.GetCurrentSelectOperation();
                    if (operation.Operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectLocation)
                    {
                        AgainstInfo.SelectRowRank = operation.SelectRegionRank;
                        AgainstInfo.SelectRank = operation.SelectLocation;
                    }
                    else
                    {
                        Debug.LogError("对战记录识别出现严重bug");
                        throw new Exception("");
                    }
                }
                else if (AgainstInfo.IsAIControl)
                {
                    await CustomThread.Delay(1000);
                    AgainstInfo.SelectRowRank = AgainstInfo.GameCardsFilter.ContainRowInfos.Where(row => row.CanBeSelected).OrderBy(x => AiCommand.GetRandom()).FirstOrDefault().RowRank;
                    AgainstInfo.SelectRank = 0;//设置部署次序
                }
                await Task.Delay(10);
            }
            NetCommand.AsyncInfo(NetAcyncType.SelectLocation);
            AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectLocation, triggerCard);
            RowCommand.SetRegionSelectable(GameRegion.None);
            AgainstInfo.IsWaitForSelectLocation = false;
        }
        public static async Task WaitForSelecUnit(Card triggerCard, List<Card> filterCards, int num, bool isAuto)
        {
            //可选列表中移除自身
            filterCards.Remove(triggerCard);
            AgainstInfo.ArrowStartCard = triggerCard;
            AgainstInfo.IsWaitForSelectUnits = true;
            AgainstInfo.AllCardList.ForEach(card => card.IsGray = true);
            filterCards.ForEach(card => card.IsGray = false);
            RowCommand.RefreshAllRowsCards();
            AgainstInfo.SelectUnits.Clear();
            //await Task.Delay(500);
            //若为回放模式
            if (AgainstInfo.IsReplayMode)
            {
                var operation = AgainstInfo.summary.GetCurrentSelectOperation();
                if (operation.Operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectUnit)
                {
                    AgainstInfo.SelectUnits = operation.SelectCardRank.SelectList(index => filterCards[index]);
                }
                else
                {
                    Debug.LogError("对战记录识别出现严重bug");
                    throw new Exception("");
                }
            }
            else
            {
                if (Info.AgainstInfo.IsMyTurn && !isAuto)
                {
                    UiCommand.CreatFreeArrow();
                }
                int selectableNum = Math.Min(filterCards.Count, num);
                while (AgainstInfo.SelectUnits.Count < selectableNum)
                {
                    TaskThrowCommand.Throw();
                    //AI操作或者我方回合自动选择模式时 ，用自身随机决定，否则等待网络同步

                    if (AgainstInfo.IsAIControl || (isAuto && AgainstInfo.IsMyTurn))
                    {
                        Debug.Log("自动选择场上单位");
                        IEnumerable<Card> autoSelectTarget = filterCards.OrderBy(x => AiCommand.GetRandom(0, 514)).Take(selectableNum);
                        AgainstInfo.SelectUnits = autoSelectTarget.ToList();
                    }
                    await Task.Delay(10);
                }
                //Debug.Log("选择单位完毕" + Math.Min(Cards.Count, num));

            }
            NetCommand.AsyncInfo(NetAcyncType.SelectUnits);
            AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectUnit, triggerCard, filterCards, num);
            UiCommand.DestoryAllArrow();
            await CustomThread.Delay(250);
            //Debug.Log("同步选择单位完毕");
            AgainstInfo.AllCardList.ForEach(card => card.IsGray = false);
            RowCommand.RefreshAllRowsCards();
            AgainstInfo.IsWaitForSelectUnits = false;
            //Debug.Log("结束选择单位");
        }
        public static async Task WaitForSelectBoardCard<T>(Card triggerCard, List<T> cardInfos, CardBoardMode mode = CardBoardMode.Select, BoardCardVisible cardVisable = BoardCardVisible.FromCard, int num = 1)
        {
            AgainstInfo.SelectBoardCardRanks = new List<int>();
            AgainstInfo.IsSelectCardOver = false;
            AgainstInfo.cardBoardMode = mode;
            UiCommand.SetCardBoardOpen(mode);
            CardBoardCommand.ShowCardBoard(cardInfos, mode, cardVisable);
            //Debug.Log("进入选择模式");
            switch (mode)
            {
                case CardBoardMode.Select:
                    int maxSelectableCount = Mathf.Min(cardInfos.Count, num);
                    while (AgainstInfo.SelectBoardCardRanks.Count < maxSelectableCount && !AgainstInfo.IsSelectCardOver)
                    {
                        if (AgainstInfo.IsAIControl)
                        {
                            await Task.Delay(2000);
                            Debug.Log("自动选择场面板单位");
                            var autoSelectRank = Enumerable.Range(0, maxSelectableCount).OrderBy(x => AiCommand.GetRandom(0, 514)).Take(maxSelectableCount);
                            AgainstInfo.SelectBoardCardRanks.AddRange(autoSelectRank);
                        }
                        await Task.Delay(10);
                    }
                    break;
                case CardBoardMode.ExchangeCard:
                    if (AgainstInfo.IsReplayMode)
                    {
                        Debug.LogWarning("以记录方式读取操作");
                        List<AgainstSummaryManager.TurnOperation.SelectOperation> selectOperations = AgainstInfo.summary.GetCurrentSelectOperations();
                        for (int i = 0; i < selectOperations.Count; i++)
                        {
                            var operation = selectOperations[i];
                            if (operation.Operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectBoardCard)
                            {
                                //如果是我方换牌，则记录，如果是对方换牌，直接生效
                                Info.AgainstInfo.SelectBoardCardRanks = operation.SelectBoardCardRanks;
                                Info.AgainstInfo.washInsertRank = operation.WashInsertRank;
                                bool isPlayer1Select = operation.IsPlayer1Select;
                                List<Card> CardLists = AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Hand].ContainCardList;
                                int selectRank = AgainstInfo.SelectBoardCardRanks[0];
                                //交换对象需要重新考究
                                await CardCommand.ExchangeCard(CardLists[selectRank], IsPlayerExchange: !(isPlayer1Select ^ Info.AgainstInfo.IsPlayer1), isRoundStartExchange: true, insertRank: AgainstInfo.washInsertRank);
                                Info.AgainstInfo.ExChangeableCardNum--;
                                Info.AgainstInfo.SelectBoardCardRanks.Clear();
                                UiCommand.SetCardBoardTitle("Remaining".TranslationGameText() + Info.AgainstInfo.ExChangeableCardNum);
                            }
                            else if (operation.Operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectExchangeOver)
                            {

                                if (operation.IsPlay1ExchangeOver)
                                {
                                    AgainstInfo.isPlayer1RoundStartExchangeOver = true;
                                }
                                else
                                {
                                    AgainstInfo.isPlayer2RoundStartExchangeOver = true;
                                }
                            }
                            else
                            {
                                Debug.LogError("对战记录识别出现严重bug");
                                throw new Exception("");
                            }
                        }
                    }
                    else
                    {
                        //我方超时或者打ai时对方自动结束换牌
                        AiCommand.RoundStartExchange(false);
                        //如果满足换牌条件则持续换牌状态
                        while (Info.AgainstInfo.ExChangeableCardNum != 0 && !Info.AgainstInfo.IsSelectCardOver)
                        {
                            TaskThrowCommand.Throw();
                            //通过对战记录换牌

                            //有牌要被换
                            if (Info.AgainstInfo.SelectBoardCardRanks.Count > 0)
                            {
                                List<Card> CardLists = AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Hand].ContainCardList;
                                int selectRank = AgainstInfo.SelectBoardCardRanks[0];
                                AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectBoardCard, triggerCard, CardLists, 1, isPlayer1Select: Info.AgainstInfo.IsPlayer1);
                                await CardCommand.ExchangeCard(CardLists[selectRank], IsPlayerExchange: true, isRoundStartExchange: true, insertRank: AgainstInfo.washInsertRank);
                                AgainstInfo.ExChangeableCardNum--;
                                AgainstInfo.SelectBoardCardRanks.Clear();
                                UiCommand.SetCardBoardTitle("Remaining".TranslationGameText() + Info.AgainstInfo.ExChangeableCardNum);
                            }
                            await Task.Delay(10);
                        }
                        //跳出换牌循环，并告知对面
                        if (AgainstInfo.IsPlayer1)
                        {
                            AgainstInfo.isPlayer1RoundStartExchangeOver = true;
                        }
                        else
                        {
                            AgainstInfo.isPlayer2RoundStartExchangeOver = true;
                        }
                        NetCommand.AsyncInfo(NetAcyncType.RoundStartExchangeOver);
                    }
                    break;
                case CardBoardMode.ShowOnly:
                    maxSelectableCount = Mathf.Min(cardInfos.Count, num);
                    while (AgainstInfo.SelectBoardCardRanks.Count < maxSelectableCount && !AgainstInfo.IsSelectCardOver)
                    {
                        if (AgainstInfo.IsAIControl)
                        {
                            await Task.Delay(2000);
                            Debug.Log("自动选择场面板单位");
                            var autoSelectRank = Enumerable.Range(0, maxSelectableCount).OrderBy(x => AiCommand.GetRandom(0, 514)).Take(maxSelectableCount);
                            AgainstInfo.SelectBoardCardRanks.AddRange(autoSelectRank);
                        }
                        await Task.Delay(10);
                    }
                    break;
                default:
                    break;
            }
            //设置卡牌面板为需要选择模式
            UiInfo.IsCardBoardNeedSelect = false;
            UiCommand.SetCardBoardClose();
            if (mode == CardBoardMode.ExchangeCard)
            {
                //等待双方退出
                await CustomThread.UnitllAcync(
                    () => AgainstInfo.isPlayer1RoundStartExchangeOver,
                    () => AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectExchangeOver, isPlayer1ExchangeOver: true)
                );
                await CustomThread.UnitllAcync(
                    () => AgainstInfo.isPlayer2RoundStartExchangeOver,
                    () => AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectExchangeOver, isPlayer1ExchangeOver: false)
                );

                AgainstInfo.isPlayer1RoundStartExchangeOver = false;
                AgainstInfo.isPlayer2RoundStartExchangeOver = false;
                AgainstInfo.IsSelectCardOver = false;
            }
            //复位
            AgainstInfo.cardBoardMode = CardBoardMode.Temp;
        }
    }
}