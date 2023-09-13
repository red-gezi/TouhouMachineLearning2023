using Mirror.Examples.Chat;
using Mirror.Examples.MultipleMatch;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

abstract class DbCommand
{
    static MongoClient client;
    static IMongoDatabase db;

    static IMongoCollection<PlayerData> PlayerDataCollection { get; set; }
    static IMongoCollection<ChatMessageData> ChatDataCollection { get; set; }
    static IMongoCollection<OfflineInviteData> OfflineRequestDataCollection { get; set; }
    static IMongoCollection<ServerConfigData> ServerConfigCollection { get; set; }

    /// <summary>
    /// 初始化数据库配置
    /// </summary>
    public static void Init()
    {
        ServerLog.Init();
        ServerLog.Summary("/////////////////////////////");
        ServerLog.Summary("V2023.5.5.01");
        ServerLog.Summary("/////////////////////////////");
        ServerLog.Summary("链接数据库");
        //读取服务器配置保密文件
        //client = new MongoClient("mongodb://127.0.0.1:28020");
        client = new MongoClient("mongodb://cynthia.ovyno.com:28020");
        db = client.GetDatabase("Metaverse");
        PlayerDataCollection = db.GetCollection<PlayerData>("PlayerData-test");
        OfflineRequestDataCollection = db.GetCollection<OfflineInviteData>("OfflineRequestData");
        ChatDataCollection = db.GetCollection<ChatMessageData>("ChatData");
        ServerConfigCollection = db.GetCollection<ServerConfigData>("ServerConfigData");
        ServerLog.Summary("/////////////////////////////");
        LoadServerConfig();
    }
    //加载服务器的配置信息
    public static void LoadServerConfig()
    {

    }
    //////////////////////////////////////////////////账号系统///////////////////////////////////////////////////////////////////
    public static PlayerData Register(string userData)
    {
        var CheckUserExistQuery = Builders<PlayerData>.Filter.Where(info => info.Account == userData);
        //插入一个初始玩家信息
        PlayerData newPlayer = PlayerData.CreatNewAccout(userData);
        newPlayer.UUID = GetNextUserId();
        PlayerDataCollection.InsertOne(newPlayer);
        Debug.Log("已在服务端创建新用户");
        return newPlayer;
        int GetNextUserId()
        {
            var sort = Builders<PlayerData>.Sort.Descending(u => u.UUID);
            var lastUser = PlayerDataCollection.Find(new BsonDocument())
                                      .Sort(sort)
                                      .Limit(1)
                                      .FirstOrDefault();
            int nextUserId = lastUser != null ? lastUser.UUID + 1 : 10000;
            return nextUserId;
        }
    }

#nullable enable
    /// <summary>
    /// 通过account用户凭证查询所有信息(包括敏感部分)
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static PlayerData? QueryUserInfo(string account)
    {
        try
        {
            Debug.Log("用户查询" + account);
            var CheckUserExistQuery = Builders<PlayerData>.Filter.Where(info => info.Account == account);
            PlayerData? userInfo = PlayerDataCollection.Find(CheckUserExistQuery)?.FirstOrDefault();
            return userInfo;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }
    }
    /// <summary>
    /// 通过UUID查询其他玩家信息，只返回脱敏后的简单消息
    /// </summary>
    /// <param name="UUID"></param>
    /// <returns></returns>
    public static PlayerData? QueryOtherUserInfo(int UUID)
    {
        try
        {
            Debug.Log("用户查询" + UUID);
            var CheckUserExistQuery = Builders<PlayerData>.Filter.Where(info => info.UUID == UUID);
            PlayerData? userInfo = PlayerDataCollection.Find(CheckUserExistQuery)?.FirstOrDefault();
            //此处应该脱敏
            return userInfo?.GetSimpleInfo();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }
    }

    //////////////////////////////////////////////////聊天系统///////////////////////////////////////////////////////////////////
    //离线请求
    public static void CreatOfflineRequest(OfflineInviteData offlineRequest) => OfflineRequestDataCollection.InsertOne(offlineRequest);
    public static void DeleteOfflineRequest(string requestId) => OfflineRequestDataCollection.DeleteOne(request => request._id == requestId);
    /// <summary>
    /// 通过account用户凭证查询所有和自身相关的离线请求
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static List<OfflineInviteData> RequestOfflineInvite(string account)
    {
        var userInfo = QueryUserInfo(account);
        if (userInfo == null) return new List<OfflineInviteData>();
        return OfflineRequestDataCollection
            .AsQueryable()
            .Where(request => request.receiverUUID == userInfo.UUID)
            .ToList();
    }
    /// <summary>
    /// 通过离线请求id查询指定的离线请求
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static OfflineInviteData QueryOfflineRequest(string requestId) => OfflineRequestDataCollection
        .AsQueryable()
        .FirstOrDefault(request => request._id == requestId);
    public static void AddChatMember(int senderUUID, int receiveUUID, RequestType requestType)
    {
        string chatID = Guid.NewGuid().ToString();
        InsertChat(chatID, senderUUID, receiveUUID, requestType);
        InsertChat(chatID, receiveUUID, senderUUID, requestType);

        static void InsertChat(string chatID, int playerA_UUID, int playerB_UUID, RequestType requestType)
        {
            // 使用 UUID 进行查找
            var filter = Builders<PlayerData>.Filter.Eq(x => x.UUID, playerA_UUID);
            var foundPlayer = PlayerDataCollection.Find(filter).FirstOrDefault();
            if (foundPlayer != null)
            {
                // 创建 ChatData 对象
                var chatData = new ChatData
                {
                    ChatID = chatID,
                    TargetChaterUUID = playerB_UUID,
                    chatType = requestType switch
                    {
                        RequestType.AddFriend => ChatType.Friend,
                        RequestType.InviteChat => ChatType.Stranger,
                        _ => ChatType.Delete,
                    }
                };
                // 更新 Chats 属性
                foundPlayer.Chats.Add(chatData);
                // 更新 MongoDB 中的记录
                var update = Builders<PlayerData>.Update.Set(x => x.Chats, foundPlayer.Chats);
                PlayerDataCollection.UpdateOne(filter, update);
            }
        }
    }
    public static void AddMessageToChatLog(string chatID, ChatMessageData.ChatMessage message)
    {
        Debug.Log("#####服务端插入消息#####");

        var targetChat = ChatDataCollection.AsQueryable().FirstOrDefault(chat => chat._id == chatID);
        if (targetChat == null)
        {
            targetChat = new ChatMessageData() { _id = chatID };
            message.date = DateTime.Now.ToShortTimeString();
            message.index = 0;
            targetChat.chatMessages.Add(message);
            ChatDataCollection.InsertOne(targetChat);
        }
        else
        {

            message.date = DateTime.Now.ToShortTimeString();
            var lastMessage = targetChat.chatMessages.LastOrDefault();
            message.index = lastMessage == null ? 0 : lastMessage.index + 1;
            targetChat.chatMessages.Add(message);
            var filter = Builders<ChatMessageData>.Filter.Eq(x => x._id, chatID);
            var update = Builders<ChatMessageData>.Update.Set(x => x.chatMessages, targetChat.chatMessages);
            ChatDataCollection.UpdateOne(filter, update);
        }

    }
    public static int QueryLastChatLogIndex(string chatID)
    {
        var targetChat = ChatDataCollection.AsQueryable().FirstOrDefault(chatData => chatData._id == chatID);
        if (targetChat?.chatMessages?.LastOrDefault() is ChatMessageData.ChatMessage chatMessage)
        {
            return chatMessage.index;
        }
        return -1;
    }
    //查询最后一条消息的内容和时间
    public static (string, string) QueryLastChatLogMessageAndTime(string chatID)
    {
        var targetChat = ChatDataCollection.AsQueryable().FirstOrDefault(chatData => chatData._id == chatID);
        if (targetChat?.chatMessages?.LastOrDefault() is ChatMessageData.ChatMessage chatMessage)
        {
            switch (chatMessage.messageType)
            {
                case ChatMessageType.Image: return ("【图片】", chatMessage.date);
                case ChatMessageType.Text: return (chatMessage.text, chatMessage.date);
                case ChatMessageType.Voice: return ("【声音】", chatMessage.date);
                case ChatMessageType.Expression: return ("【表情】", chatMessage.date);
                default: return ("【未定义类型消息】", chatMessage.date);
            }
        }
        return ("", "");
    }
    //start为-1时，自动替换值为聊天记录中最后一条，range为负数时，代表向上查询
    public static List<ChatMessageData.ChatMessage> QueryChatLog(string chatID, int start, int range)
    {
        var targetChat = ChatDataCollection.AsQueryable().FirstOrDefault(chatData => chatData._id == chatID);
        if (targetChat == null)
        {
            return new List<ChatMessageData.ChatMessage>();
        }
        else
        {
            //判断最后一个聊天记录的索引
            var lastMessage = targetChat.chatMessages.LastOrDefault();
            if (lastMessage == null) return new List<ChatMessageData.ChatMessage>();
            var lastIndex = lastMessage.index;
            int end = lastIndex;
            if (start == -1)
            {
                start = lastIndex;
            }
            if (range > 0)
            {
                end = start + range;
            }
            else
            {
                end = start + 1;
                start = end + range;
            }
            //这里优化下范围，暂时全部返回
            //return targetChat.chatMessages;
            return targetChat.chatMessages.Where(message => message.index >= start && message.index < end).ToList();
        }
    }
#nullable disable
    public static int GetRegisterPlayerCount() => PlayerDataCollection.AsQueryable().Count();
    //////////////////////////////////////////////////用户信息更新///////////////////////////////////////////////////////////////////
    //通过用户凭证修改，修改关键数据时调用这个
    public static bool UpdatePlayerInfo<TField>(string account, Expression<Func<PlayerData, TField>> setOperator, TField field)
    {
        var CheckUserExistQuery = Builders<PlayerData>.Filter.Where(info => info.Account == account);
        var updateUserState = Builders<PlayerData>.Update.Set(setOperator, field);
        IFindFluent<PlayerData, PlayerData> findFluent = PlayerDataCollection.Find(CheckUserExistQuery);
        if (findFluent.CountDocuments() > 0)
        {
            PlayerDataCollection.UpdateOne(CheckUserExistQuery, updateUserState);
            return true;//修改成功
        }
        else
        {
            return false;//修改失败
        }
    }
    //通过UUID修改，修改非关键数据时可使用这个
    public static bool UpdatePlayerInfo<TField>(int UUID, Expression<Func<PlayerData, TField>> setOperator, TField field)
    {
        var CheckUserExistQuery = Builders<PlayerData>.Filter.Where(info => info.UUID == UUID);
        var updateUserState = Builders<PlayerData>.Update.Set(setOperator, field);
        IFindFluent<PlayerData, PlayerData> findFluent = PlayerDataCollection.Find(CheckUserExistQuery);
        if (findFluent.CountDocuments() > 0)
        {
            PlayerDataCollection.UpdateOne(CheckUserExistQuery, updateUserState);
            return true;//修改成功
        }
        else
        {
            return false;//修改失败
        }
    }
    public static bool UpdateChat<TField>(string chatID, Expression<Func<ChatMessageData, TField>> setOperator, TField field)
    {
        var CheckChatExistQuery = Builders<ChatMessageData>.Filter.Where(chat => chat._id == chatID);
        var updateUserState = Builders<ChatMessageData>.Update.Set(setOperator, field);
        IFindFluent<ChatMessageData, ChatMessageData> findFluent = ChatDataCollection.Find(CheckChatExistQuery);
        if (findFluent.CountDocuments() > 0)
        {
            ChatDataCollection.UpdateOne(CheckChatExistQuery, updateUserState);
            return true;//修改成功
        }
        else
        {
            return false;//修改失败
        }
    }
}