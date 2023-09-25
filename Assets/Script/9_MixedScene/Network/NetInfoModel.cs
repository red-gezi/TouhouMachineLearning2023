using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using UnityEngine;

namespace TouhouMachineLearningSummary.Model
{
    public class SampleCardModel
    {
        public string CardID { get; set; } = "";
        public string CardBackID { get; set; } = "";
        public int BasePoint { get; set; } = 0;
        public int ChangePoint { get; set; } = 0;
        public Dictionary<string, int> CardFields { get; set; }
        public List<int> State { get; set; } = new List<int>();
        public SampleCardModel() { }
        public SampleCardModel(Card card)
        {
            CardID = card.CardID;
            BasePoint = card.BasePoint;
            ChangePoint = card.ChangePoint;
            CardFields = card.cardFields.ToDictionary(field => field.Key.ToString(), field => field.Value);
            State = Enumerable.Range(0, Enum.GetNames(typeof(GameEnum.CardState)).Length).SelectList(index => card[(GameEnum.CardState)index] ? 1 : 0);
        }
    }
    [Serializable]

    /// <summary>
    /// 卡牌坐标模板
    /// </summary>
    public class Location
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Location(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    //卡牌配置模板
    public class CardConfig
    {
        public string _id { get; set; }
        public DateTime UpdataTime { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }

        public byte[] AssemblyFileData { get; set; }
        public byte[] SingleCardFileData { get; set; }
        public byte[] MultiCardFileData { get; set; }

        public CardConfig() { }

        public CardConfig(string version, FileInfo assemblyFile, FileInfo singleCardFile, FileInfo multiCardFile)
        {
            _id = Guid.NewGuid().ToString();
            this.UpdataTime = DateTime.Now;
            this.Version = version;
            this.AssemblyFileData = File.ReadAllBytes(assemblyFile.FullName);
            this.SingleCardFileData = File.ReadAllBytes(singleCardFile.FullName);
            this.MultiCardFileData = File.ReadAllBytes(multiCardFile.FullName);
        }
    }
    [Serializable]
    public class Deck
    {
        public string DeckName { get; set; }
        public string DeckBackID { get; set; }
        public string LeaderID { get; set; }
        public List<string> CardIDs { get; set; }
        public Deck(string DeckName, string LeaderId, string DeckBackID, List<string> CardIds)
        {
            this.DeckName = DeckName;
            this.LeaderID = LeaderId;
            this.CardIDs = CardIds;
            this.DeckBackID = DeckBackID;
        }
    }
    public class Faith
    {
        public string BelongUserUID { get; set; }
        public int Count { get; set; }
        public bool IsLock { get; set; }
        [NonSerialized]
        public Texture2D faithTexture;
        [NonSerialized]
        public Sprite faithSprite;
        [NonSerialized]
        public string userName;

        public void Init()
        {
            int rank = int.Parse(BelongUserUID);
            userName = GachaInfo.Instance.TempSprites[rank].name;
            faithTexture = GachaInfo.Instance.TempSprites[rank];
            faithSprite = GachaInfo.Instance.TempSprites[rank].ToSprite();
        }
        //一个临时实现方案
        public Texture2D GetFaithIconTexture()
        {
            Init();
            return faithTexture;
        }
        public Sprite GetFaithIconSprite()
        {
            Init();
            return faithSprite;
        }
    }
    /// <summary>
    /// 玩家信息模板,若有更新需要在服务端同步更新
    /// </summary>
    public class PlayerInfo
    {
        public string _id { get; set; }
        public string UID { get; set; }
        //玩家的个人账号，手机或邮箱
        public string Account { get; set; }
        //玩家后期补充的邮箱信息，用于找回密码
        public string E_mail { get; set; }
        //玩家游戏中的名字
        public string Name { get; set; }
        public List<ChatTargetInfo> ChatTargets { get; set; } = new();

        public List<string> UnlockTitleTags { get; set; }
        public string UsePrefixTitleTag { get; set; }
        public string UseSuffixTitleTag { get; set; }
        public string Password { get; set; }
        public int Level { get; set; }
        public int Rank { get; set; }
        public DateTime LastLoginTime { get; set; }
        public Dictionary<string, int> Stage { get; set; } = new();
        public int GetStage(string StageName) => !Stage.ContainsKey(StageName) ? -1 : Stage[StageName];
        public Dictionary<string, int> Resource { get; set; } = new Dictionary<string, int>();
        //自定义个性宣言
        public string WinMessage { get; set; }
        public string LoseMessage { get; set; }
        public string GreetingsMessage { get; set; }
        //信仰相关属性
        public List<Faith> Faiths { get; set; } = new();
        //决定游戏进程
        [ShowInInspector]
        public Dictionary<string, int> CardLibrary { get; set; } = new Dictionary<string, int>();
        public int UseDeckNum { get; set; } = 0;
        public List<Deck> Decks { get; set; }
        [JsonIgnore]
        public Deck UseDeck
        {
            get => Decks.Count > UseDeckNum ? Decks[UseDeckNum] : Decks[0];
            set => Decks[UseDeckNum] = value;
        }
        public PlayerInfo() { }
        public PlayerInfo(string uid, string name, string prefixTitleTag, string suffixTitleTag, string password, List<Deck> decks)
        {
            UID = uid;
            Name = name;
            UsePrefixTitleTag = prefixTitleTag;
            UseSuffixTitleTag = suffixTitleTag;
            UnlockTitleTags = new();
            Decks = decks;
            Password = password;
            Level = 0;
            Rank = 0;
            UseDeckNum = 0;
        }
        /// <summary>
        /// 返回脱敏后的简易用户信息
        /// </summary>
        /// <returns></returns>
        public PlayerInfo GetSampleInfo() => new PlayerInfo(UID, Name, UsePrefixTitleTag, UseSuffixTitleTag, "", Decks)
        {
            Level = Level,
            Rank = Rank,
            UseDeckNum = UseDeckNum,
        };
        public async Task<bool> UpdateName(string name)
        {
            Info.AgainstInfo.OnlineUserInfo.Name = name;
            Manager.UserInfoManager.Refresh();
            return await Command.NetCommand.UpdateInfoAsync(UpdateType.Name, name);
        }
        public async Task<bool> UpdateTitle(string tag)
        {
            if (!Info.AgainstInfo.OnlineUserInfo.UnlockTitleTags.Contains(tag))
            {
                Info.AgainstInfo.OnlineUserInfo.UnlockTitleTags.Add(tag);
                return await Command.NetCommand.UpdateInfoAsync(UpdateType.UnlockTitles, tag);
            }
            return false;
        }
        public async Task<bool> UpdateUserStateAsync(string stageTag, int stageRank)
        {
            Stage[stageTag] = stageRank;
            return await Command.NetCommand.UpdateInfoAsync(UpdateType.Stage, Stage);
        }
        public async Task<bool> UpdateDecksAsync()
        {
            bool isSuccessUpdateDeck = await Command.NetCommand.UpdateInfoAsync(UpdateType.Decks, Decks);
            bool isSuccessUpdateUseDeckNum = await Command.NetCommand.UpdateInfoAsync(UpdateType.UseDeckNum, UseDeckNum);
            return isSuccessUpdateDeck && isSuccessUpdateUseDeckNum;
        }
    }
    /// <summary>
    /// 可离线加好友请求
    /// </summary>
    [Serializable]
    public class OfflineInviteInfo
    {
        public string _id { get; set; }
        public int SenderUUID { get; set; }
        public int ReceiverUUID { get; set; }
        public string SenderName { get; set; }
        public string SeceiverName { get; set; }
        public DateTime CreatTime { get; set; }
        public OfflineInviteInfo() { }
    }
    /// <summary>
    /// 聊天对象信息
    /// </summary>
    public class ChatTargetInfo
    {
        public string ChatID { get; set; }
        public string TargetChaterUID { get; set; }
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
    public class ChatMessageInfo
    {
        public string _id { get; set; }
        //不同日期的聊天日志
        public List<ChatMessage> ChatMessages { get; set; } = new();
        ////聊天者的UUID
        //public List<int> chatterUUID = new List<int>();
        public class ChatMessage
        {
            //消息索引
            public int Index { get; set; }
            public string Date { get; set; }
            //发言者
            public string SpeakerUUID { get; set; }
            public string SpeakerName { get; set; }
            //消息类型
            public ChatMessageType MessageType { get; set; }
            //聊天信息、语音、图片信息
            public string Text { get; set; }
            public ChatMessage(){}
        }
    }
}