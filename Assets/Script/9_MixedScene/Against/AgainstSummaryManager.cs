using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Other;
using UnityEngine;

namespace TouhouMachineLearningSummary.Manager
{
    public enum PlayerOperationType
    {
        PlayCard,//出牌
        DisCard,//弃牌
        Pass,//过牌
    }
    public enum SelectOperationType
    {
        SelectProperty,//选择属性
        SelectUnit,//选择单位
        SelectRegion,//选择对战区域
        SelectLocation,//选择位置坐标
        SelectBoardCard,//从面板中选择卡牌
        SelectExchangeOver,//选择换牌完毕
    }
    public class AgainstSummaryManager
    {
        public string _id { get; set; }
        public string AssemblyVerision { get; set; } = "";
        public PlayerInfo Player1Info { get; set; }
        public PlayerInfo Player2Info { get; set; }
        public int Winner { get; set; } = 0;
        public DateTime UpdateTime { get; set; }
        //是否按照流程完成对局
        bool IsFinishAgainst { get; set; }
        [JsonIgnore]
        public TurnOperation TargetJumpTurn { get; set; } = null;

        public List<TurnOperation> TurnOperations { get; set; } = new List<TurnOperation>();
        public class TurnOperation//更新数据模型时须同步更新服务端数据模型
        {
            public int RoundRank { get; set; }//当前小局数
            public int TurnRank { get; set; }//当前回合数
            public int TotalTurnRank { get; set; }//当前总回合数
            public bool IsOnTheOffensive { get; set; }//是否先手
            public bool IsPlayer1Turn { get; set; }//是否处于玩家1的操作回合
            public int RelativeStartPoint { get; set; }//玩家操作前双方的点数差
            public int RelativeEndPoint { get; set; }//玩家操作后双方的点数差  
            //0表示不投降，1表示玩家1投降，2表示玩家2投降
            public int SurrenderState { get; set; } = 0;
            public List<List<SimpleCardModel>> AllCardList { get; set; } = new List<List<SimpleCardModel>>();
            public PlayerOperation TurnPlayerOperation { get; set; }
            public List<SelectOperation> TurnSelectOperations { get; set; } = new List<SelectOperation>();
            public TurnOperation() { }
            public TurnOperation Init()
            {
                this.RoundRank = AgainstInfo.roundRank;
                this.TurnRank = AgainstInfo.turnRank;
                this.TotalTurnRank = AgainstInfo.totalTurnRank;
                this.IsOnTheOffensive = AgainstInfo.isOnTheOffensive;
                this.IsPlayer1Turn = !(AgainstInfo.IsPlayer1 ^ AgainstInfo.IsMyTurn);
                this.AllCardList = CardsFilter.GlobalCardList.SelectList(cardlist => cardlist.SelectList(card => new SimpleCardModel(card)));
                return this;
            }
            //回合开始时的基本操作，共三类
            //1从手牌中选择一张打出
            //2从手牌中选择一张打出
            //3pass
            public class PlayerOperation
            {
                public List<int> Operation { get; set; }
                public List<SimpleCardModel> TargetcardList { get; set; }
                public string SelectCardID { get; set; }         //打出的目标卡牌id
                public int SelectCardIndex { get; set; }     //打出的目标卡牌索引

                public PlayerOperation() { }
                public PlayerOperation(PlayerOperationType operation, List<Card> targetcardList, Card selectCard = null)
                {
                    this.Operation = operation.EnumToOneHot();
                    this.TargetcardList = targetcardList.SelectList(card => new SimpleCardModel(card));
                    this.SelectCardID = selectCard?.CardID??"";
                    this.SelectCardIndex = selectCard == null ? -1 : targetcardList.IndexOf(selectCard);
                }
            }
            public class SelectOperation
            {
                //操作类型 选择场地属性/从战场选择多个单位/从卡牌面板中选择多张牌/从战场中选择一个位置/从战场选择多片对战区域
                public List<int> Operation { get; set; }
                public string TriggerCardID { get; set; }
                //选择面板卡牌
                public List<int> SelectBoardCardRanks { get; set; }
                //换牌时洗入的位置
                public int WashInsertRank { get; internal set; }
                public bool IsPlayer1Select { get; set; }
                //换牌完成,true为玩家1换牌操作，false为玩家2换牌操作
                public bool IsPlay1ExchangeOver { get; set; }

                //选择单位
                public List<SimpleCardModel> TargetCardList { get; set; }
                public List<int> SelectCardRank { get; set; }
                public int SelectMaxNum { get; set; }
                //区域
                public int SelectRegionRank { get; set; }
                public int SelectLocation { get; set; }


                public SelectOperation() { }
            }

        }
        /// <summary>
        /// 上传一个回合玩家操作记录
        /// </summary>
        public static async Task UploadPlayerOperationAsync(PlayerOperationType operation, List<Card> targetcardList, Card selectCard)
        {
            if (AgainstInfo.isShouldUploadSummaryOperation)
            {
                await Command.NetCommand.UpdateTurnPlayerOperationAsync(new TurnOperation.PlayerOperation(operation, targetcardList, selectCard));
            }
        }
        /// <summary>
        /// 上传一个回合玩家选择记录
        /// </summary>
        public static async void UploadSelectOperation(SelectOperationType operationType, Card triggerCard = null, List<Card> targetCardList = null, int selectMaxNum = 0, bool isPlayer1Select = false, bool isPlayer1ExchangeOver = false)//是否玩家1操作完成
        {
            if (AgainstInfo.isShouldUploadSummaryOperation)
            {
                TurnOperation.SelectOperation operation = new TurnOperation.SelectOperation();
                switch (operationType)
                {
                    case SelectOperationType.SelectProperty:
                        break;
                    case SelectOperationType.SelectUnit:
                        operation.TriggerCardID = triggerCard.CardID;
                        operation.SelectCardRank = AgainstInfo.SelectUnits.SelectList(selectUnit => targetCardList.IndexOf(selectUnit));
                        operation.TargetCardList = targetCardList.SelectList(card => new SimpleCardModel(card));
                        operation.SelectMaxNum = selectMaxNum;
                        break;
                    case SelectOperationType.SelectBoardCard:
                        operation.TriggerCardID = triggerCard != null ? triggerCard.CardID : "";
                        operation.IsPlayer1Select = isPlayer1Select;
                        operation.SelectBoardCardRanks = AgainstInfo.SelectBoardCardRanks;
                        operation.WashInsertRank = AgainstInfo.washInsertRank;
                        break;
                    case SelectOperationType.SelectRegion:
                        operation.TriggerCardID = triggerCard.CardID;
                        operation.SelectRegionRank = AgainstInfo.SelectRowRank;
                        break;
                    case SelectOperationType.SelectLocation:
                        //上传选择的次序
                        operation.TriggerCardID = triggerCard.CardID;
                        operation.SelectRegionRank = AgainstInfo.SelectRowRank;
                        operation.SelectLocation = AgainstInfo.SelectRank;
                        break;
                    case SelectOperationType.SelectExchangeOver:
                        operation.IsPlay1ExchangeOver = isPlayer1ExchangeOver;
                        break;
                    default:
                        break;
                }
                operation.Operation = operationType.EnumToOneHot();
                await Command.NetCommand.UpdateTurnSelectOperationAsync(operation);
            }
            else
            {
                currentSelectOperationsRank++;
                Debug.Log("当前操作指针" + currentSelectOperationsRank);
            }
        }
        /// <summary>
        /// 上传一个小局记录
        /// </summary>
        public static async void UploadRound()
        {
            AgainstInfo.turnRank = 0;
            AgainstInfo.isOnTheOffensive = true;
            //添加换牌阶段回合操作，回合0代表换牌操作
            if (AgainstInfo.isShouldUploadSummaryOperation)
            {
                await Command.NetCommand.UpdateTurnOperationAsync(new TurnOperation().Init());
            }
            else
            {
                //对战模式下，每次回合指针跟着增一
                currentTurnOperationsRank++;
            }
        }
        /// <summary>
        /// 上传一个回合记录
        /// </summary>
        public static async void UploadTurn()
        {
            if (AgainstInfo.isShouldUploadSummaryOperation)
            {
                if (!AgainstInfo.isOnTheOffensive)
                {
                    AgainstInfo.turnRank++;
                    AgainstInfo.totalTurnRank++;
                }
                await Command.NetCommand.UpdateTurnOperationAsync(new TurnOperation().Init());
                AgainstInfo.isOnTheOffensive = !AgainstInfo.isOnTheOffensive;
            }
            else
            {
                //回放模式下每回合指针加一
                if (AgainstInfo.IsReplayMode)
                {
                    currentTurnOperationsRank++;
                }
            }
            currentSelectOperationsRank = 0;
            Debug.Log("当前回合操作指针" + currentTurnOperationsRank);
        }
        /// <summary>
        /// 上传回合开始后的点数
        /// </summary>
        public static async void UploadStartPoint()
        {
            if (AgainstInfo.isShouldUploadSummaryOperation)
            {
                await Command.NetCommand.UploadStartPointAsync();
            }
        }
        /// <summary>
        /// 上传回合结束前的点数
        /// </summary>
        public static async void UploadEndPoint()
        {
            if (AgainstInfo.isShouldUploadSummaryOperation)
            {
                await Command.NetCommand.UploadEndPointAsync();
            }
        }
        /// <summary>
        /// 上传投降记录
        /// </summary>
        public static async void UploadSurrender(bool isPlayer1Surrenddr)
        {
            if (AgainstInfo.isShouldUploadSummaryOperation)
            {
                await Command.NetCommand.UploadSurrenderAsync(isPlayer1Surrenddr ? 1 : 2);
            }
        }

        //////////////////////////////////对战指令解析/////////////////////////////////////////////    
        static int currentTurnOperationsRank = -1;//当前指向的玩家回合操作命令编号（因为在换牌阶段也要增一所以默认为-1，这样第一次换牌时会变成0）
        static int currentSelectOperationsRank = 0;//当前指向的玩家回合选择指令编号
        public TurnOperation.PlayerOperation GetCurrentPlayerOperation()
        {
            TurnOperation.PlayerOperation turnPlayerOperation = TurnOperations[currentTurnOperationsRank].TurnPlayerOperation;
            Debug.Log($"读取到指令{currentTurnOperationsRank} 类型：{turnPlayerOperation?.Operation.OneHotToEnum<PlayerOperationType>()} 目标卡片为：{turnPlayerOperation?.SelectCardID}");
            return turnPlayerOperation;
        }

        public TurnOperation.SelectOperation GetCurrentSelectOperation()
        {
            int maxRank = TurnOperations[currentTurnOperationsRank].TurnSelectOperations.Count;
            if (currentSelectOperationsRank >= maxRank)
            {
                //执行到最后检查一下状态，看看是否投降  检查是否投降
                //return null;
            }
            TurnOperation.SelectOperation selectOperation = TurnOperations[currentTurnOperationsRank].TurnSelectOperations[currentSelectOperationsRank];
            Debug.Log($"读取到指令类型{selectOperation.Operation.OneHotToEnum<SelectOperationType>()} {selectOperation.SelectMaxNum}");
            return selectOperation;
        }
        public List<TurnOperation.SelectOperation> GetCurrentSelectOperations() => TurnOperations[currentTurnOperationsRank].TurnSelectOperations;
        //////////////////////////////////对战记录读取////////////////////////////////////////////
        public static AgainstSummaryManager Load(string summaryID) => File.ReadAllText("summary.json").ToObject<AgainstSummaryManager>();
        public void Replay(int TotalRank)
        {
            TaskThrowCommand.Token.Cancel();
            //设置回合初始状态
        }
        public async Task JumpToTurnAsync(int totalTurnRank, bool isOnTheOffensive)
        {
            TaskThrowCommand.Token.Cancel();
            //设置回合初始状态
            TargetJumpTurn = TurnOperations.FirstOrDefault(turn => turn.IsOnTheOffensive == isOnTheOffensive && turn.TotalTurnRank == totalTurnRank);
            if (TargetJumpTurn == null)
            {
                Debug.LogError("回合跳转溢出，重置跳转到最后");
                TargetJumpTurn = TurnOperations.Last();
            }
            //清空所有卡牌
            CardsFilter.GlobalCardList.ForEach(cardlist => cardlist.ForEach(card => UnityEngine.Object.Destroy(card.gameObject)));
            CardsFilter.GlobalCardList.ForEach(cardlist => cardlist.Clear());
            //await Task.Delay(1000);

            //设置当前为跳转模式
            AgainstInfo.IsJumpMode = true;
            await AgainstManager.CreatAgainstProcess();
            //设置当前指定回合
            //重新读取
        }
        //////////////////////////////////对战记录输出////////////////////////////////////////////
        public void Show() => UnityEngine.Debug.LogWarning(this.ToJson());
        public void Explort() => File.WriteAllText("summary.json", this.ToJson());
    }
}