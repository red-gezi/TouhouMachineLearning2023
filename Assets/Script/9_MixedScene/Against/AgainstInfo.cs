using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Info
{
    /// <summary>
    /// 全局对战信息
    /// </summary>
    public static class AgainstInfo
    {
        //选择脚本是从本地加载还是从线上加载，从本地加载更方便测试
        public static bool LoagScriptFromLoacl => !File.Exists("Assets\\Script\\9_MixedScene\\CardSpace\\GameCard.asmdef");
        public static string CurrentCardScriptsVersion { get; set; } = "";
        public static bool IsHostNetMode { get; set; } = true;//连接到本地服务器
        public static bool IsTrainMode { get; set; } = false;//训练加速模式，所有等待设为0
        public static bool IsReplayMode { get; set; } = false;//回放模式，会加载指定对战记录读取操作
        public static bool IsJumpMode { get; set; } = false;//跳转到指定回合模式
        public static VariationType VariationType = VariationType.None;//默认为无异变模式
        public static bool isShouldUploadSummaryOperation => !IsReplayMode && ((IsPVP && IsMyTurn) || IsPVE);//是否处于应该上传对战记录操作状态,回放模式不上传，单人模式客户端双方均上传记录，多人模式由双方在客户端主体方上传记录
        /// <summary>
        /// 玩家线上人物信息
        /// </summary>
        [ShowInInspector]
        public static PlayerInfo OnlineUserInfo { get; set; } = new ();
        /// <summary>
        /// 玩家的用户信息（可能为线上信息或者单机固定卡组信息）
        /// </summary>
        [ShowInInspector]
        public static PlayerInfo CurrentUserInfo { get; set; }
        /// <summary>
        /// 对手的用户信息（可能为线上信息或者单机固定卡组信息）\
        /// </summary>
        [ShowInInspector]
        public static PlayerInfo CurrentOpponentInfo { get; set; }
        //双方用户信息

        public static string UserName => CurrentUserInfo.Name;
        public static string OpponentName => CurrentOpponentInfo.Name;
        public static Deck UserDeck => CurrentUserInfo.UseDeck;
        public static Deck OpponentDeck => CurrentOpponentInfo.UseDeck;
        /// <summary>
        /// 获得当前卡牌使用者的卡背id
        /// </summary>

        public static string myCardBackIndex => IsMyTurn ? UserDeck.DeckBackID : OpponentDeck.DeckBackID;
        public static string opCardBackIndex => IsMyTurn ? OpponentDeck.DeckBackID : UserDeck.DeckBackID;

        public static AgainstSummaryManager summary = new AgainstSummaryManager();

        //网络同步信息
        public static Card TargetCard;
        public static int washInsertRank;

        public static bool IsSelectCardOver;
        public static string RoomID;
        //操作标志位
        public static List<GameObject> ArrowList = new List<GameObject>();

        public static List<int> Player1BlackCardList;
        public static List<int> Player2BlackCardList;

        public static Vector3 dragToPoint;
        //玩家注视的卡牌
        public static Card PlayerFocusCard { get; set; }
        //对手注视的卡牌
        public static Card OpponentFocusCard { get; set; }
        //玩家准备使用的悬空状态下的牌
        public static Card playerPrePlayCard;
        //玩家打出的牌
        public static Card playerPlayCard;
        //玩家放弃的牌
        public static Card playerDisCard;
        //玩家选择过牌
        public static bool IsPlayerPass { get; set; } = false;
        //小局回合信息
        public static int roundRank = 1;
        public static int turnRank = 0;
        public static int totalTurnRank = 0;
        public static bool isOnTheOffensive = true;

        //选择属性
        public static BattleRegion SelectProperty { get; set; }
        public static bool IsWaitForSelectProperty { get; set; }

        //选择的区域
        public static RowManager PlayerFocusRegion { get; set; }
        public static bool IsWaitForSelectRegion { get; set; }
        public static int SelectRowRank { get; set; }
        public static List<Card> SelectRowCardList => cardSet[SelectRowRank];
        //选择的单位
        public static Card ArrowStartCard { get; set; }
        public static Card ArrowEndCard { get; set; }
        public static bool IsWaitForSelectUnits { get; set; }
        public static List<Card> SelectUnits { get; set; } = new List<Card>();//玩家选择的单位
        //选择的次序
        public static Vector3 FocusPoint;
        public static bool IsWaitForSelectLocation;
        public static int SelectRank = -1;
        //选择的卡牌面板卡片
        public static bool isRoundStartExchange = false;
        public static bool isPlayer1RoundStartExchangeOver = false;
        public static bool isPlayer2RoundStartExchangeOver = false;

        public static CardBoardMode cardBoardMode;
        public static List<string> cardBoardIDList;//游戏对战中的展示面板显示的虚拟卡牌列表
        public static List<Card> cardBoardList;//游戏对战中的展示面板显示的真实卡牌列表
        public static List<Card> tempCardBoardList;//玩家临时点开查看的墓地或卡组中的卡牌列表，与游戏对战中的展示面板卡牌面板互相独立

        public static List<int> SelectBoardCardRanks { get; set; }
        public static List<Card> SelectActualCards => SelectBoardCardRanks.Select(rank => cardBoardList[rank]).ToList();
        public static List<string> SelectVirualCardIds => SelectBoardCardRanks.Select(rank => cardBoardIDList[rank]).ToList();

        public static int ExChangeableCardNum = 0;
        //判断是否强制先后手
        public static int FirstMode { get; set; } = 0;
        //判断是否1号玩家
        public static bool IsPlayer1 { get; set; } = false;
        public static bool IsMyTurn { get; set; }
        public static bool IsPVP { get; set; } = false;
        public static bool IsPVE => !IsPVP;
        //判断是用Ai代替玩家操作
        public static bool IsAiAgent { get; set; } = false;
        //PvP我方超时，或者是PVE对方回合或者是Ai代理模式下
        public static bool IsAIControl => IsAiAgent || (IsPVE && !IsMyTurn || (IsPVP && IsMyTurn && CoinManager.isTimeout));
        /// <summary>
        /// 对局中卡牌的集合
        /// </summary>
        public static CardSet cardSet = new CardSet();

        public static List<Card> AllCardList => CardSet.GlobalCardList.SelectMany(x => x).ToList();
        //连锁数
        public static int lastP1ChainCount;
        public static int lastP2ChainCount;
        public static int currentP1ChainCount;
        public static int currentP2ChainCount;

        public static int lastMyChainCount
        {
            get => IsPlayer1! ^ IsMyTurn ? ref lastP1ChainCount : ref lastP2ChainCount;
            set => (IsPlayer1! ^ IsMyTurn ? ref lastP1ChainCount : ref lastP2ChainCount) = value;
        }
        public static int lastOpChainCount
        {
            get => IsPlayer1 ^ IsMyTurn ? ref lastP1ChainCount : ref lastP2ChainCount;
            set => (IsPlayer1 ^ IsMyTurn ? ref lastP1ChainCount : ref lastP2ChainCount) = value;
        }
        public static int currentMyChainCount
        {
            get => IsPlayer1! ^ IsMyTurn ? ref currentP1ChainCount : ref currentP2ChainCount;
            set => (IsPlayer1! ^ IsMyTurn ? ref currentP1ChainCount : ref currentP2ChainCount) = value;
        }
        public static int currentOpChainCount
        {
            get => IsPlayer1 ^ IsMyTurn ? ref currentP1ChainCount : ref currentP2ChainCount;
            set => (IsPlayer1 ^ IsMyTurn ? ref currentP1ChainCount : ref currentP2ChainCount) = value;
        }
        //分数
        public static (int P1Score, int P2Score) PlayerScore;
        public static (int MyScore, int OpScore) ShowScore => IsPlayer1 ? (PlayerScore.P1Score, PlayerScore.P2Score) : (PlayerScore.P2Score, PlayerScore.P1Score);
        public static int TotalUpPoint => cardSet[Orientation.Up][GameRegion.Battle].CardList.Sum(card => card.ShowPoint);
        public static int TotalDownPoint => cardSet[Orientation.Down][GameRegion.Battle].CardList.Sum(card => card.ShowPoint);
        public static int TotalMyPoint => cardSet[Orientation.My][GameRegion.Battle].CardList.Sum(card => card.ShowPoint);
        public static int TotalOpPoint => cardSet[Orientation.Op][GameRegion.Battle].CardList.Sum(card => card.ShowPoint);
        public static int TotalPlayer1Point => IsPlayer1 ? TotalDownPoint : TotalUpPoint;
        public static int TotalPlayer2Point => IsPlayer1 ? TotalUpPoint : TotalDownPoint;
        public static int TurnRelativePoint => TotalMyPoint - TotalOpPoint;
        //pass状态
        public static bool isUpPass = false;
        public static bool isDownPass = false;

        public static bool IsMyPass => IsMyTurn ? isDownPass : isUpPass;
        public static bool IsOpPass => IsMyTurn ? isUpPass : isDownPass;
        public static bool isBoothPass => isUpPass && isDownPass;
    }
}