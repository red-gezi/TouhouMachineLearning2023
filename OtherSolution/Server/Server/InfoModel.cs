using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Drawing;
using TouhouMachineLearningSummary.GameEnum;

namespace Server
{
    public class UserState
    {
        public int Step { get; set; }
        public int Rank { get; set; }
    }
    public class HoldInfo
    {
        public HoldInfo(int firstMode, PlayerInfo playerInfo, PlayerInfo virtualOpponentInfo = null, IClientProxy client = null)
        {
            FirstMode = firstMode;
            UserInfo = playerInfo;
            VirtualOpponentInfo = virtualOpponentInfo;
            Client = client;
            Rank = playerInfo.Rank;
            WinRate = playerInfo.WinRate;
            CollectionRate = 0;
            JoinTime = DateTime.Now;
        }

        public int FirstMode { get; set; }
        public PlayerInfo UserInfo { get; set; }
        public PlayerInfo VirtualOpponentInfo { get; set; }
        public IClientProxy Client { get; set; }
        public int Rank { get; set; }
        public float WinRate { get; set; }
        public float CollectionRate { get; set; }
        public DateTime JoinTime { get; set; }
    }
    public class Faith
    {
        public string BelongUserUID { get; set; }
        public int Count { get; set; }
        public bool IsLock { get; set; }
    }
    public class PlayerInfo
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string UID { get; set; }
        //玩家的个人账号，手机或邮箱
        public string Account { get; set; }
        //玩家后期补充的邮箱信息，用于找回密码
        public string E_mail { get; set; }
        //玩家游戏中的名字
        public string Name { get; set; }
        public List<ChatData> ChatTargets { get; set; } = new();

        public List<string> UnlockTitleTags { get; set; }
        public string UsePrefixTitleTag { get; set; }
        public string UseSuffixTitleTag { get; set; }
        public string Password { get; set; }
        public int Level { get; set; }
        public int Rank { get; set; }
        public float WinRate { get; set; }
        public DateTime LastLoginTime { get; set; }

        public Dictionary<string, int> Resource { get; set; }
        //自定义个性宣言
        public string WinMessage { get; set; }
        public string LoseMessage { get; set; }
        public string GreetingsMessage { get; set; }
        //抽卡信仰相关属性
        public bool IsFirstDraw { get; set; }

        public Color FaithColor { get; set; }
        public List<Faith> Faiths { get; set; } = new();
        //决定关卡进程
        public Dictionary<string, int> Stage { get; set; } = new();

        public Dictionary<string, int> CardLibrary { get; set; }
        public int UseDeckNum { get; set; }
        public List<Deck> Decks { get; set; }
        [JsonIgnore]
        public Deck UseDeck => Decks[UseDeckNum];
        public PlayerInfo ShufflePlayerDeck()
        {
            Decks[UseDeckNum].CardIDs = UseDeck.CardIDs.OrderBy(i => new Random(DateTime.Now.GetHashCode()).Next()).ToList();
            return this;
        }
        public PlayerInfo() { }
        public PlayerInfo Creat(string account, string password, string title, List<Deck> deck, Dictionary<string, int> cardLibrary)
        {
            _id = Guid.NewGuid().ToString();
            UID = (MongoDbCommand.GetRegisterPlayerCount() + 1000).ToString();
            Account = account;
            Name = "村中人";
            UnlockTitleTags = new() { "1-0", "2-0" };
            UsePrefixTitleTag = "1-0";
            UseSuffixTitleTag = "2-0";
            Decks = deck;
            Password = password.GetSaltHash(UID);
            CardLibrary = cardLibrary;
            Level = 0;
            Rank = 0;
            UseDeckNum = 0;
            Stage = new Dictionary<string, int>() { { "0", 1 } };
            Resource = new Dictionary<string, int>() { { "FaithFragment", 0 }, { "FaithPowder", 0 }, { "GeziCoin", 0 } };
            FaithColor = Color.White;
            IsFirstDraw = true;
            return this;
        }
    }
    //卡牌不同版本的配置文件类型
    public class CardConfig
    {
        [BsonId]
        public string _id { get; set; }
        public DateTime UpdataTime { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public byte[] AssemblyFileData { get; set; }
        public byte[] SingleCardFileData { get; set; }
        public byte[] MultiCardFileData { get; set; }
        public CardConfig() { }
    }
    public class Deck
    {
        public string DeckName { get; set; }
        public string DeckBackID { get; set; }
        public string LeaderID { get; set; }
        public List<string> CardIDs { get; set; }
        public Deck() { }
        public Deck(string DeckName, string LeaderId, string DeckBackID, List<string> CardIds)
        {
            this.DeckName = DeckName;
            this.LeaderID = LeaderId;
            this.CardIDs = CardIds;
            this.DeckBackID = DeckBackID;
        }
    }
    //简易的数字卡牌量化模型
    public class SampleCardModel
    {
        public string CardID { get; set; } = "";
        public int BasePoint { get; set; } = 0;
        public int ChangePoint { get; set; } = 0;
        public List<int> State { get; set; } = new List<int>();
        public Dictionary<string, int> CardFields { get; set; }
        public Dictionary<string, bool> CardStates { get; set; }
        public SampleCardModel() { }
    }
    //对战记录模型
    public class AgainstSummary
    {
        [BsonId]
        public string _id { get; set; }
        public string AssemblyVerision { get; set; } = "";
        public PlayerInfo Player1Info { get; set; }
        public PlayerInfo Player2Info { get; set; }
        public int Winner { get; set; } = 0;
        public DateTime UpdateTime { get; set; }
        public List<TurnOperation> TurnOperations { get; set; } = new List<TurnOperation>();
        public class TurnOperation
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
            public List<List<SampleCardModel>> AllCardList { get; set; } = new List<List<SampleCardModel>>();
            public PlayerOperation TurnPlayerOperation { get; set; }
            public List<SelectOperation> TurnSelectOperations { get; set; } = new List<SelectOperation>();
            public TurnOperation() { }
            public class PlayerOperation
            {
                public List<int> Operation { get; set; }
                public List<SampleCardModel> TargetcardList { get; set; }
                public string SelectCardID { get; set; }
                public int SelectCardIndex { get; set; }     //打出的目标卡牌索引

                public PlayerOperation() { }
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
                public List<SampleCardModel> TargetCardList { get; set; }
                public List<int> SelectCardRank { get; set; }
                public int SelectMaxNum { get; set; }
                //区域
                public int SelectRegionRank { get; set; }
                public int SelectLocation { get; set; }
                public SelectOperation() { }
            }
        }
        /// <summary>
        /// 增加一个回合记录
        /// </summary>
        /// <param name="turnOperation"></param>
        public void AddTurnOperation(TurnOperation turnOperation)
        {
            Console.WriteLine("新增回合记录");
            TurnOperations.Add(turnOperation);
        }
        /// <summary>
        /// 增加一个回合玩家操作记录
        /// </summary>
        /// <param name="turnOperation"></param>
        public void AddPlayerOperation(TurnOperation.PlayerOperation playerOperation)
        {
            Console.WriteLine("新增回合玩家操作记录");
            TurnOperations.Last().TurnPlayerOperation = playerOperation;
        }

        /// <summary>
        /// 增加一个回合玩家选择记录
        /// </summary>
        /// <param name="turnOperation"></param>
        public void AddSelectOperation(TurnOperation.SelectOperation selectOperation)
        {
            Console.WriteLine("新增回合选择操作记录");
            TurnOperations.Last().TurnSelectOperations.Add(selectOperation);
        }

        public void AddStartPoint(int relativePoint)
        {
            Console.WriteLine("新增开始点数" + relativePoint);
            TurnOperations.Last().RelativeStartPoint = relativePoint;
        }

        public void AddEndPoint(int relativePoint)
        {
            Console.WriteLine("新增结束点数" + relativePoint);
            TurnOperations.Last().RelativeEndPoint = relativePoint;
        }

        public void AddSurrender(int surrendrState)
        {
            Console.WriteLine("新增投降事件");
            TurnOperations.Last().SurrenderState = surrendrState;
        }

        public void UploadAgentSummary(int p1Score, int p2Score)
        {
            UpdateTime = DateTime.Now;
            if (p1Score > p2Score) { Winner = 1; }
            if (p1Score < p2Score) { Winner = 2; }
            if (p1Score == p2Score) { Winner = 3; }
            MongoDbCommand.InsertAgainstSummary(this);
        }
    }
    /// <summary>
    /// 聊天对象信息
    /// </summary>
    public class ChatData
    {
        public string ChatID { get; set; }
        public string TargetChaterUUID { get; set; }
        public string Signature { get; set; }
        public string Name { get; set; }
        public bool Online { get; set; }
        public ChatType CurrentChatType { get; set; }
        public StateType PlayerStateType { get; set; }
        public int LastReadIndex { get; set; }
        public int LastMessageIndex { get; set; }
        public int UnReadCount { get; set; }
        public string LastMessage { get; set; }
        public string LastMessageTime { get; set; }
    }
    public class ChatMessageData
    {
        public string _id { get; set; }
        //不同日期的聊天日志
        public List<ChatMessage> chatMessages = new();
        ////聊天者的UUID
        //public List<int> chatterUUID = new List<int>();
        public class ChatMessage
        {
            //消息索引
            public int Index { get; set; }
            public string SendTime { get; set; }
            //发言者
            public string SpeakerUUID { get; set; }
            public string SpeakerName { get; set; }
            //消息类型
            public ChatMessageType messageType;
            //聊天信息、语音、图片信息
            public string Text;
            public ChatMessage(){}
        }
        public void AppendMessage(string speakerUUID, string speakerName, string Text)
        {
            string date = DateTime.Today.ToShortDateString();
            chatMessages.Add(new ChatMessage() { SpeakerUUID = speakerUUID, SpeakerName = speakerName, date = date, Text = Text });
        }
        //固定时段定时操作
        public void DeleteLog(int userUUID, string Text)
        {
            string date = DateTime.Today.ToShortDateString();
        }
    }
    [Serializable]
    public class OfflineInviteInfo
    {
        public string _id { get; set; }
        public string SenderUID { get; set; }
        public string ReceiverUID { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public DateTime CreatTime { get; set; }
        public OfflineInviteInfo() { }
        public OfflineInviteInfo(string password, string senderUID, string receiverUID)
        {
            var userInfo = MongoDbCommand.QueryUserInfo(senderUID, password);
            var otherUserInfo = MongoDbCommand.QueryOtherUserInfo(receiverUID);
            if (userInfo == null || otherUserInfo == null) return;
            _id = Guid.NewGuid().ToString();
            CreatTime = DateTime.Now;
            this.SenderUID = userInfo.UID;
            this.SenderName = userInfo.Name;
            this.ReceiverUID = receiverUID;
            this.ReceiverName = otherUserInfo.Name;
        }
        //不同日期的聊天日志
        public void Appect()
        {
            MongoDbCommand.AddChatMember(SenderUID, ReceiverUID);
            MongoDbCommand.DeleteOfflineRequest(_id);
        }
        public void Reject() => MongoDbCommand.DeleteOfflineRequest(_id);
    }
}