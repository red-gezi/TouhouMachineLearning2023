﻿using MongoDB.Driver;
using Renci.SshNet;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using TouhouMachineLearningSummary.GameEnum;
using ConnectionInfo = Renci.SshNet.ConnectionInfo;

namespace Server
{
    class MongoDbCommand
    {
        static MongoClient client;
        static IMongoDatabase db;
        static IMongoCollection<PlayerInfo>? PlayerInfoCollection { get; set; }
        static IMongoCollection<ChatMessageInfo> ChatDataCollection { get; set; }
        static IMongoCollection<OfflineInviteInfo> OfflineRequestDataCollection { get; set; }
        static IMongoCollection<CardConfig>? CardConfigCollection { get; set; }
        static IMongoCollection<ServerConfig>? ServerConfigCollection { get; set; }
        static IMongoCollection<AgainstSummary>? SummaryCollection { get; set; }
        public static IMongoCollection<DiyCardInfo> DiyCardCollection { get; set; }

        public static void Init()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Log.Summary("/////////////////////////////");
            Log.Summary("V2023.9.25.V1");
            Log.Summary("/////////////////////////////");
            Log.Summary("链接数据库");
            //读取服务器配置保密文件
            if (!File.Exists("Config.ini"))
            {
                File.WriteAllLines("Config.ini", new List<string> { "你的ssh ip", "你的ssh密码" });
                Console.WriteLine("检测不到配置文件，开始创建");
            }
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (isWindows)
            {
                string SSH_HOST = File.ReadAllLines("Config.ini")[0]; ;
                string SSH_PASSWORD = File.ReadAllLines("Config.ini")[1];
                // 创建 SSH 连接
                var sshClient = new SshClient(SSH_HOST, 22, "root", SSH_PASSWORD);
                sshClient.Connect();
                // 创建 SSH 端口转发
                var forwardedPort = new ForwardedPortLocal("localhost", "127.0.0.1", 28020);
                sshClient.AddForwardedPort(forwardedPort);
                forwardedPort.Start();
                // 创建 MongoDB 连接字符串
                string mongodbUri = $"mongodb://localhost:{forwardedPort.BoundPort}";
                // 创建 MongoDB 客户端
                client = new MongoClient(mongodbUri);
                Console.WriteLine("当前数据库转发地址为" + mongodbUri);
            }
            else
            {
                client = new MongoClient("mongodb://127.0.0.1:28020");
            }
            db = client.GetDatabase("Gezi");
            PlayerInfoCollection = db.GetCollection<PlayerInfo>("PlayerInfo");
            ChatDataCollection = db.GetCollection<ChatMessageInfo>("ChatInfo");
            OfflineRequestDataCollection = db.GetCollection<OfflineInviteInfo>("OfflineRequestData");
            CardConfigCollection = db.GetCollection<CardConfig>("CardConfig");
            ServerConfigCollection = db.GetCollection<ServerConfig>("ServerConfig");
            SummaryCollection = db.GetCollection<AgainstSummary>("AgainstSummary");
            DiyCardCollection = db.GetCollection<DiyCardInfo>("DiyCards");

            Log.Summary($"查询到用户信息{PlayerInfoCollection.AsQueryable().Count()}条");
            Log.Summary($"查询到卡牌配置信息{CardConfigCollection.AsQueryable().Count()}条");
            Log.Summary($"查询到对局记录信息{SummaryCollection.AsQueryable().Count()}条");
            Log.Summary($"查询到diy卡牌信息{DiyCardCollection.AsQueryable().Count()}条");
            Log.Summary("/////////////////////////////");
            LoadServerConfig();
        }
        //加载服务器的配置信息
        public static void LoadServerConfig()
        {
            ServerConfigManager.config = ServerConfigCollection?.AsQueryable().FirstOrDefault();
            if (ServerConfigManager.config == null)
            {
                Console.WriteLine("检索不到数据库中的服务器配置文件，请重新生成新配置文件");
                ServerConfigManager.config = new ServerConfig() { CommandPassword = "" };
                ServerConfigCollection?.InsertOne(ServerConfigManager.config);
            }
        }
        //////////////////////////////////////////////////账号系统///////////////////////////////////////////////////////////////////
        public static int Register(string account, string password)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account);
            if (PlayerInfoCollection.Find(CheckUserExistQuery).CountDocuments() > 0)
            {
                return -1;//已存在
            }
            else
            {
                PlayerInfoCollection.InsertOne(
                    new PlayerInfo().Creat(account, password, "萌新",
                    new List<Deck>()
                    {
                         new Deck("萌新的第一套卡组", "M_N0_0L_001","N0_0", new List<string>
                         {
                             "M_N0_1G_001","M_N0_1G_002","M_N0_1G_003","M_N0_1G_004",
                             "M_N0_2S_001","M_N0_2S_002","M_N0_2S_003","M_N0_2S_004","M_N0_2S_005","M_N0_2S_006",
                             "M_N0_3C_001","M_N0_3C_002","M_N0_3C_003","M_N0_3C_004","M_N0_3C_005",
                             "M_N0_3C_001","M_N0_3C_002","M_N0_3C_003","M_N0_3C_004","M_N0_3C_005",
                             "M_N0_3C_001","M_N0_3C_002","M_N0_3C_003","M_N0_3C_004","M_N0_3C_005",
                         })
                    },
                    new Dictionary<string, int>
                    {
                        {  "M_N0_1G_001",1  },
                        {  "M_N0_1G_002",1  },
                    }));
                return 1;//成功
            }
        }
        //之后会根据登录方式进行扩展
        public static PlayerInfo? Login(string accountOrUID, string password)
        {
            var userInfo = QueryUserInfo(accountOrUID, password);
            if (userInfo.Password == password.GetSaltHash(userInfo.UID))
            {
                var result = UpdateInfo(userInfo.UID, userInfo.Password, (x => x.LastLoginTime), DateTime.Now);
            }
            return userInfo;
        }

        /// <summary>
        /// 查询玩家自身用户信息
        /// </summary>
        /// <param name="accountOrUID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static PlayerInfo QueryUserInfo(string accountOrUID, string password)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == accountOrUID || info.UID == accountOrUID);
            PlayerInfo? userInfo = PlayerInfoCollection.Find(CheckUserExistQuery).FirstOrDefault();
            if (userInfo != null && userInfo.Password == password.GetSaltHash(userInfo.UID))
            {
                return userInfo;
            }
            //1正确,-1密码错误,-2无此账号
            //return (UserInfo != null ? 1 : playerInfoCollection.Find(CheckUserExistQuery).CountDocuments() > 0 ? -1 : -2, UserInfo);
            return null;
        }
        // <summary>
        /// 查询脱敏后的其余用户信息
        /// </summary>
        /// <param name="accountOrUID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static PlayerInfo QueryOtherUserInfo(string UID)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.UID == UID);
            PlayerInfo userInfo = PlayerInfoCollection.Find(CheckUserExistQuery).FirstOrDefault();
            return userInfo;
        }
        //////////////////////////////////////////////////抽卡系统///////////////////////////////////////////////////////////////////
        public static async Task<string> AddFaiths(string uid, string password, Faith newFaith)
        {
            PlayerInfo? userInfo = QueryUserInfo(uid, password);
            if (userInfo == null)
            {
                return "账号验证失败";
            }
            var targetFaith = userInfo.Faiths.FirstOrDefault(userFaith => userFaith.BelongUserUID == newFaith.BelongUserUID);
            if (targetFaith == null)
            {
                userInfo.Faiths.Add(newFaith);
            }
            else
            {
                targetFaith.Count += newFaith.Count;
            }
            var result = await UpdateInfo(uid, userInfo.Password, (x => x.Faiths), userInfo.Faiths);
            Console.WriteLine("信念增添结果" + result);
            return "信念增添成功";
        }
        public static string RemoveFaiths(string uid, string password, Faith newFaith)
        {
            PlayerInfo? userInfo = PlayerInfoCollection.AsQueryable().FirstOrDefault(info => info.UID == uid);
            if (userInfo != null)
            {
                if (userInfo.Password == password.GetSaltHash(userInfo.UID))
                {
                    var result = UpdateInfo(uid, userInfo.Password, (x => x.LastLoginTime), DateTime.Now);
                }
                else
                {
                    userInfo = null;
                    return "账号验证失败";
                }
            }
            var targetFaith = userInfo.Faiths.FirstOrDefault(userFaith => userFaith.BelongUserUID == newFaith.BelongUserUID);
            if (targetFaith == null)
            {
                return "无移除目标";
            }
            else
            {
                //如果数量足够，减去，否则
                if (targetFaith.Count > newFaith.Count)
                {
                    targetFaith.Count -= newFaith.Count;
                }
                else if (targetFaith.Count == newFaith.Count)
                {
                    userInfo.Faiths.Remove(targetFaith);
                }
                else
                {
                    return "信念移除数量不足";
                }
            }
            PlayerInfoCollection.UpdateOne(info => info.UID == uid, Builders<PlayerInfo>.Update.Set(info => info.Faiths, userInfo.Faiths));
            return "信念移除成功";
        }
        public static async Task<List<string>> DrawCard(string uid, string password, List<Faith> selectFaiths)
        {

            PlayerInfo? userInfo = QueryUserInfo(uid, password);
            if (userInfo != null)
            {
                bool HaveEnoughFaith = true;
                //判断能否扣除仓库中的信念
                selectFaiths.GroupBy(x => x.BelongUserUID).ToList().ForEach(group =>
                {
                    //如果有一项不足，则跳过
                    if (userInfo.Faiths.Where(faith => faith.BelongUserUID == group.Key).Count() >= group.Count())
                    {
                        HaveEnoughFaith = false;
                    }
                });
                //满足抽卡条件
                if (HaveEnoughFaith || true)
                {
                    //如果是第一次，固定返回
                    if (userInfo.IsFirstDraw)
                    {
                        var result = await UpdateInfo(uid, password, (x => x.IsFirstDraw), false);
                        return new List<string> { "M_N0_1G_001" };
                    }
                    //否则平等的随机抽卡
                    else
                    {
                        //卡池计算公式
                        Random rand = new();
                        return ServerConfigManager.DrawAbleCardIDs_Test
                                    .OrderBy(x => rand.Next())
                                    .Take(selectFaiths.Count)
                                    .ToList();
                    }
                }
                else
                {
                    //扣除失败
                    return new List<string> { "0" };
                }
            }
            //账号验证失败
            return new List<string> { "1" };
        }
        public static int GetRegisterPlayerCount() => PlayerInfoCollection.AsQueryable().Count();
        //////////////////////////////////////////////////用户信息更新///////////////////////////////////////////////////////////////////
        public static async Task<bool> UpdateInfo<TField>(string uid, string password, Expression<Func<PlayerInfo, TField>> setOperator, TField field)
        {
            //获取加密后的真实密码
            string realPassword = password.GetSaltHash(uid);
            var filter = Builders<PlayerInfo>.Filter.Where(info => info.UID == uid && info.Password == realPassword);
            var update = Builders<PlayerInfo>.Update.Set(setOperator, field);
            var result = await PlayerInfoCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        //////////////////////////////////////////////////对战记录///////////////////////////////////////////////////////////////////
        public static void InsertAgainstSummary(AgainstSummary againstSummary) => SummaryCollection.InsertOne(againstSummary);
        public static List<AgainstSummary> QueryAgainstSummary(int skipCount, int takeCount) => SummaryCollection.AsQueryable().Skip(skipCount).Take(takeCount).ToList();
        public static List<AgainstSummary> QueryAgainstSummary(string playerUID, int skipCount, int takeCount)
        {
            return SummaryCollection.AsQueryable().Where(summary => summary.Player1Info.UID == playerUID || summary.Player1Info.UID == playerUID).Skip(skipCount).Take(takeCount).ToList();
        }

        //////////////////////////////////////////////////卡牌配置///////////////////////////////////////////////////////////////////
        public static string InsertOrUpdateCardConfig(CardConfig newConfig, List<string> drawAbleList, string commandPassword)
        {
            if (commandPassword.GetSaltHash("514") == ServerConfigManager.CommandPassword)
            {
                if (newConfig.Type == "Test")
                {
                    ServerConfigCollection.UpdateOne(x => true, Builders<ServerConfig>.Update.Set(x => x.DrawAbleCardIDs_Test, drawAbleList));
                    Log.Summary(DateTime.Now + "更新测试版可抽取卡牌列表");
                }
                if (newConfig.Type == "Release")
                {
                    ServerConfigCollection.UpdateOne(x => true, Builders<ServerConfig>.Update.Set(x => x.DrawAbleCardIDs_Release, drawAbleList));
                    Log.Summary(DateTime.Now + "更新正式版可抽取卡牌列表");
                }
                var CheckConfigExistQuery = Builders<CardConfig>.Filter.Where(config => config.Version == newConfig.Version && config.Type == newConfig.Type);
                IFindFluent<CardConfig, CardConfig> findFluent = CardConfigCollection.Find(CheckConfigExistQuery);
                if (findFluent.CountDocuments() > 0)
                {
                    Console.WriteLine("修改卡组配置文件");
                    CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.UpdataTime, newConfig.UpdataTime));
                    CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.AssemblyFileData, newConfig.AssemblyFileData));
                    CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.SingleCardFileData, newConfig.SingleCardFileData));
                    CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.MultiCardFileData, newConfig.MultiCardFileData));
                    return "已修改配置";//修改成功
                }
                else
                {
                    Console.WriteLine("增加卡组配置文件");
                    CardConfigCollection.InsertOne(newConfig);
                    return "已新增配置";//修改失败
                }

            }
            else
            {
                return "指令密码输入错误，服务器拒绝修改";
            }
        }
        public static CardConfig GetCardConfig(string version)
        {
            if (version == "")
            {
                version = CardConfigCollection.AsQueryable().Max(x => x.Version);
            }
            var target = CardConfigCollection.Find(x => x.Version == version).FirstOrDefault();
            return target;
        }
        public static string GetLastCardUpdateTime() => CardConfigCollection.AsQueryable().Max(x => x.UpdataTime).ToString();
        public static string GetLastCardUpdateVersion() => CardConfigCollection.AsQueryable().Max(x => x.Version).ToString();
        //////////////////////////////////////////////////聊天系统///////////////////////////////////////////////////////////////////
        //离线请求
        public static bool CreatOfflineRequest(OfflineInviteInfo offlineRequest)
        {
            //是否已存在重复邀请
            var checkOfflineRequest = OfflineRequestDataCollection.AsQueryable().FirstOrDefault(invite =>
                invite.SenderUID == offlineRequest.SenderUID &&
                invite.ReceiverUID == offlineRequest.ReceiverUID
                );
            if (checkOfflineRequest != null)
            {
                Console.WriteLine("已存在好友请求，不重复申请");
                return false;
            }
            else
            {
                OfflineRequestDataCollection.InsertOne(offlineRequest);
                return true;
            }
        }

        public static void DeleteOfflineRequest(string requestId) => OfflineRequestDataCollection.DeleteOne(request => request._id == requestId);
        /// <summary>
        /// 通过account用户凭证查询所有和自身相关的离线请求
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static List<OfflineInviteInfo> QueryOfflineInvites(string password, string senderUID)
        {
            var userInfo = QueryUserInfo(senderUID, password);
            if (userInfo == null) return new List<OfflineInviteInfo>();
            return OfflineRequestDataCollection
                .AsQueryable()
                .Where(request => request.ReceiverUID == userInfo.UID)
                .ToList();
        }
        /// <summary>
        /// 通过离线请求id查询指定的离线请求
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static OfflineInviteInfo QueryOfflineInvite(string requestId) => OfflineRequestDataCollection
            .AsQueryable()
            .FirstOrDefault(request => request._id == requestId);
        public static void AddChatMember(string senderUUID, string receiveUUID)
        {
            string chatID = Guid.NewGuid().ToString();
            InsertChat(chatID, senderUUID, receiveUUID);
            InsertChat(chatID, receiveUUID, senderUUID);

            static void InsertChat(string chatID, string playerA_UID, string playerB_UID)
            {
                // 使用 UUID 进行查找
                var filter = Builders<PlayerInfo>.Filter.Eq(x => x.UID, playerA_UID);
                var foundPlayer = PlayerInfoCollection.Find(filter).FirstOrDefault();
                if (foundPlayer != null)
                {
                    // 创建 ChatData 对象
                    var chatData = new ChatTargetInfo
                    {
                        ChatID = chatID,
                        TargetChaterUID = playerB_UID,
                    };
                    // 更新 Chats 属性
                    foundPlayer.ChatTargets.Add(chatData);
                    // 更新 MongoDB 中的记录
                    var update = Builders<PlayerInfo>.Update.Set(x => x.ChatTargets, foundPlayer.ChatTargets);
                    PlayerInfoCollection.UpdateOne(filter, update);
                }
            }
        }
        public static void AddMessageToChatLog(string chatID, ChatMessageInfo.ChatMessage message)
        {

            var targetChat = ChatDataCollection.AsQueryable().FirstOrDefault(chat => chat._id == chatID);
            if (targetChat == null)
            {
                targetChat = new ChatMessageInfo() { _id = chatID };
                message.Date = DateTime.Now.ToShortTimeString();
                message.Index = 0;
                targetChat.ChatMessages.Add(message);
                ChatDataCollection.InsertOne(targetChat);
            }
            else
            {

                message.Date = DateTime.Now.ToShortTimeString();
                var lastMessage = targetChat.ChatMessages.LastOrDefault();
                message.Index = lastMessage == null ? 0 : lastMessage.Index + 1;
                targetChat.ChatMessages.Add(message);
                var filter = Builders<ChatMessageInfo>.Filter.Eq(x => x._id, chatID);
                var update = Builders<ChatMessageInfo>.Update.Set(x => x.ChatMessages, targetChat.ChatMessages);
                ChatDataCollection.UpdateOne(filter, update);
            }

        }
        public static int QueryLastChatLogIndex(string chatID)
        {
            var targetChat = ChatDataCollection.AsQueryable().FirstOrDefault(chatData => chatData._id == chatID);
            if (targetChat?.ChatMessages?.LastOrDefault() is ChatMessageInfo.ChatMessage chatMessage)
            {
                return chatMessage.Index;
            }
            return -1;
        }
        //查询最后一条消息的内容和时间
        public static (string, string) QueryLastChatLogMessageAndTime(string chatID)
        {
            var targetChat = ChatDataCollection.AsQueryable().FirstOrDefault(chatData => chatData._id == chatID);
            if (targetChat?.ChatMessages?.LastOrDefault() is ChatMessageInfo.ChatMessage chatMessage)
            {
                return chatMessage.MessageType switch
                {
                    ChatMessageType.Text => (chatMessage.Text, chatMessage.Date),
                    ChatMessageType.Expression => ("【表情】", chatMessage.Date),
                    _ => ("【未定义类型消息】", chatMessage.Date),
                };
            }
            return ("", "");
        }
        //查询对话日志
        public static List<ChatMessageInfo.ChatMessage> QueryChatLog(string chatID)
        {
            var targetChat = ChatDataCollection.AsQueryable().FirstOrDefault(chatData => chatData._id == chatID);
            if (targetChat == null)
            {
                return new List<ChatMessageInfo.ChatMessage>();
            }
            return targetChat.ChatMessages;
        }
    }

}