
using Server;
using MongoDB.Bson;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;
using TouhouMachineLearningSummary.GameEnum;
using System.Data.Common;

public class TouHouHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("一个用户登录了" + Context.ConnectionId);
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("一个用户登出了" + Context.ConnectionId);
        OnlineUserManager.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
    //////////////////////////////////////////////账户////////////////////////////////////////////////////////////////////
    public int Register(string account, string password) => MongoDbCommand.Register(account, password);
    public PlayerInfo? Login(string account, string password)
    {


        PlayerInfo? playerInfo = MongoDbCommand.Login(account, password);
        if (playerInfo != null)
        {
            OnlineUserManager.Add(Context.ConnectionId, playerInfo);
        }
        return playerInfo;
    }
    public PlayerInfo QueryOtherUserInfoin(string UID) => MongoDbCommand.QueryOtherUserInfo(UID);
    public async Task<List<string>> DrawCard(string uid, string password, List<Faith> selectFaiths) => await MongoDbCommand.DrawCard(uid, password, selectFaiths);
    //////////////////////////////////////////////等候列表////////////////////////////////////////////////////////////////////
    public void Join(AgainstModeType againstMode, int FirstMode, PlayerInfo userInfo, PlayerInfo virtualOpponentInfo) => HoldListManager.Add(againstMode, FirstMode, userInfo, virtualOpponentInfo, Clients.Caller);
    public void Leave(AgainstModeType againstMode, string uid) => HoldListManager.Remove(againstMode, uid);
    //////////////////////////////////////////////房间////////////////////////////////////////////////////////////////////
    public void Async(NetAcyncType netAcyncType, string roomId, bool isPlayer1, object[] data) => RoomManager.GetRoom(roomId)?.AsyncInfo(netAcyncType, isPlayer1, data);
    public bool AgainstFinish(string roomId, string uid, int P1Score, int P2Score) => RoomManager.DisponseRoom(roomId, uid, P1Score, P2Score);
    //////////////////////////////////////////////用户信息更新操作////////////////////////////////////////////////////////////////////
    public async Task<bool> UpdateInfo(string uid, string password, UpdateType updateType, object updateValue)
    {
        switch (updateType)
        {
            case UpdateType.Name: return await MongoDbCommand.UpdateInfo(uid, password, (x => x.Name), updateValue.To<string>());
            case UpdateType.UnlockTitles: return await MongoDbCommand.UpdateInfo(uid, password, (x => x.UnlockTitleTags), updateValue.To<List<string>>());
            case UpdateType.PrefixTitle: return await MongoDbCommand.UpdateInfo(uid, password, (x => x.UsePrefixTitleTag), updateValue.To<string>());
            case UpdateType.SuffixTitle: return await MongoDbCommand.UpdateInfo(uid, password, (x => x.UseSuffixTitleTag), updateValue.To<string>());
            case UpdateType.Decks: return await MongoDbCommand.UpdateInfo(uid, password, (x => x.Decks), updateValue.To<List<Deck>>());
            case UpdateType.UseDeckNum: return await MongoDbCommand.UpdateInfo(uid, password, (x => x.UseDeckNum), updateValue.To<int>());
            case UpdateType.Stage: return await MongoDbCommand.UpdateInfo(uid, password, (x => x.Stage), updateValue.To<Dictionary<string, int>>());
            case UpdateType.LastLoginTime: return await MongoDbCommand.UpdateInfo(uid, password, (x => x.LastLoginTime), updateValue.To<DateTime>());
            default: return false;
        }
    }



    //////////////////////////////////////////////日志////////////////////////////////////////////////////////////////////
    //下载自己的记录
    public List<AgainstSummary> DownloadOwnerAgentSummary(string playerName, int skipNum, int takeNum) => MongoDbCommand.QueryAgainstSummary(playerName, skipNum, takeNum);
    //下载所有的记录
    public List<AgainstSummary> DownloadAllAgentSummary(int skipNum, int takeNum) => MongoDbCommand.QueryAgainstSummary(skipNum, takeNum);
    public void UpdateTurnOperation(string roomId, AgainstSummary.TurnOperation turnOperation) => RoomManager.GetRoom(roomId)?.Summary.AddTurnOperation(turnOperation);
    public void UpdatePlayerOperation(string roomId, AgainstSummary.TurnOperation.PlayerOperation playerOperation) => RoomManager.GetRoom(roomId)?.Summary.AddPlayerOperation(playerOperation);
    public void UpdateSelectOperation(string roomId, AgainstSummary.TurnOperation.SelectOperation selectOperation) => RoomManager.GetRoom(roomId)?.Summary.AddSelectOperation(selectOperation);
    public void UploadStartPoint(string roomId, int relativePoint) => RoomManager.GetRoom(roomId)?.Summary.AddStartPoint(relativePoint);
    public void UploadEndPoint(string roomId, int relativePoint) => RoomManager.GetRoom(roomId)?.Summary.AddEndPoint(relativePoint);
    public void UploadSurrender(string roomId, int surrenddrState) => RoomManager.GetRoom(roomId)?.Summary.AddSurrender(surrenddrState);
    //////////////////////////////////////////////卡牌配置////////////////////////////////////////////////////////////////////
    //查询最新版本
    public string GetCardConfigsVersion() => MongoDbCommand.GetLastCardUpdateVersion();
    //更新卡牌配置信息
    public string UploadCardConfigs(CardConfig cardConfig, List<string> drawAbleList, string CommandPassword) => MongoDbCommand.InsertOrUpdateCardConfig(cardConfig, drawAbleList, CommandPassword);
    //下载卡牌配置信息
    public CardConfig DownloadCardConfigs(string date) => MongoDbCommand.GetCardConfig(date);
    //////////////////////////////////////////////上传AB包////////////////////////////////////////////////////////////////////
    public string UploadAssetBundles(string path, byte[] fileData, string commandPassword)
    {
        if (commandPassword.GetSaltHash("514") == ServerConfigManager.CommandPassword)
        {
            Directory.CreateDirectory(new FileInfo(path).DirectoryName);
            Console.WriteLine("接收到" + path + "——开始写入，长度为" + fileData.Length);
            File.WriteAllBytes(path, fileData);
            return "AB包更新成功";
        }
        return "指令密码输入错误，服务器拒绝修改";
    }

    //////////////////////////////////////////////聊天////////////////////////////////////////////////////////////////////
    ///await TouHouHub.SendAsync("SendMessage", PlayerPassWord, chatID, message, speakerUID, targetChaterUID);
    public void SendMessage(string PlayerPassWord, string chatID, ChatMessageInfo.ChatMessage message, string speakerUID, string targetChaterUID)
    {
        Console.WriteLine("转发聊天记录" + message.SpeakerName + ":" + message.Text);
        MongoDbCommand.AddMessageToChatLog(chatID, message);
        //通知双方更新消息
        var targetConnectId = OnlineUserManager.GetConnectId(speakerUID);
        if (targetConnectId != null)
        {
            Console.WriteLine("发送给玩家"+ speakerUID);
            Clients.Client(targetConnectId).SendAsync("ChatMessageReceive", chatID, message);
        }
        targetConnectId = OnlineUserManager.GetConnectId(targetChaterUID);
        if (targetConnectId != null)
        {
            Console.WriteLine("发送给玩家" + targetChaterUID);
            Clients.Client(targetConnectId).SendAsync("ChatMessageReceive", chatID, message);
        }
    }
    public void AddFriend(string password, string senderUID, string recevierUID)
    {
        Console.WriteLine(recevierUID);
        OfflineInviteInfo offlineRequest = new OfflineInviteInfo(password, senderUID, recevierUID);
        if (offlineRequest._id == null)
        {
            Console.WriteLine("离线请求创建失败，无对应UID玩家");
            return;
        }
        if (MongoDbCommand.CreatOfflineRequest(offlineRequest))
        {
            var targetConnectId = OnlineUserManager.GetConnectId(recevierUID);
            Console.WriteLine("尝试邀请对方");
            if (targetConnectId == null) return;
            //对方在线则通知对方触发离线邀请检测
            Console.WriteLine("好友邀请目标存在，直接发起邀请");
            Clients.Client(targetConnectId).SendAsync("QueryOfflineInvite");
        }
        else
        {
            var targetConnectId = OnlineUserManager.GetConnectId(senderUID);
            if (targetConnectId == null) return;
            //对方在线则通知对方触发离线邀请检测
            Console.WriteLine("好友邀请已存在，创建失败");
            Clients.Client(targetConnectId).SendAsync("Notifice", "请勿重复发送好友邀请");
        }
    }
    public List<ChatTargetInfo> DeleteFriend(string password, string senderUID, string targetUID)
    {

        var userInfo = MongoDbCommand.QueryUserInfo(senderUID, password);
        var targetChat = userInfo.ChatTargets.FirstOrDefault(chat => chat.TargetChaterUID == targetUID);
        if (targetChat != null)
        {
            userInfo.ChatTargets.Remove(targetChat);
            MongoDbCommand.UpdateInfo(senderUID, password, info => info.ChatTargets, userInfo.ChatTargets);
        }
        Console.WriteLine(senderUID + "删除好友" + targetUID);
        return userInfo.ChatTargets;
    }
    public List<OfflineInviteInfo> QueryOfflineInvite(string password, string senderUID) => MongoDbCommand.QueryOfflineInvites(password, senderUID);
    public void ResponseOfflineInvite(string password, string requestId, bool inviteResult)
    {
        Console.WriteLine("进行了选择，选择结果为" + requestId + inviteResult);
        var offlineRequests = MongoDbCommand.QueryOfflineInvite(requestId);
        if (inviteResult)
        {
            offlineRequests.Appect();
        }
        else
        {
            offlineRequests.Reject();
        }
        //接受邀请，通知对方创建并打开聊天记录
        var targetConnectId = OnlineUserManager.GetConnectId(offlineRequests.SenderUID);
        if (targetConnectId == "") return;
        //对方在线则触发离线邀请检测
        Clients.Client(targetConnectId).SendAsync("ResponseInvite", inviteResult, offlineRequests.ReceiverName);
    }
    public async Task<List<ChatTargetInfo>> QueryAllChatTargetInfo(string password, string senderUID)
    {
        //打开聊天窗口时，根据聊天列表，挨个更新聊天对象信息，更新到数据库中，并传输给客户端本地
        //更新聊天对象详细信息
        var playerData = MongoDbCommand.QueryUserInfo(senderUID, password);
        for (int i = 0; i < playerData.ChatTargets.Count; i++)
        {
            string targetChaterUID = playerData.ChatTargets[i].TargetChaterUID;
            PlayerInfo targetChatterInfo = MongoDbCommand.QueryOtherUserInfo(targetChaterUID);
            ChatTargetInfo targetChatter = playerData.ChatTargets[i];
            if (targetChatter != null)
            {
                targetChatter.Name = targetChatterInfo.Name;
                targetChatter.Signature = targetChatterInfo.Signature;
                targetChatter.LastMessageIndex = MongoDbCommand.QueryLastChatLogIndex(targetChatter.ChatID);
                targetChatter.UnReadCount = targetChatter.LastMessageIndex - targetChatter.LastReadIndex;
                (string lastMessage, string lastMessageTime) = MongoDbCommand.QueryLastChatLogMessageAndTime(targetChatter.ChatID);
                targetChatter.LastMessage = lastMessage;
                targetChatter.LastMessageTime = lastMessageTime;
            }
        }
        var result = await MongoDbCommand.UpdateInfo(senderUID, password, info => info.ChatTargets, playerData.ChatTargets);
        Console.WriteLine("聊天对象列表更新" + result);
        return playerData.ChatTargets;
    }
    public List<ChatMessageInfo.ChatMessage> QueryChatLog(string chatID) => MongoDbCommand.QueryChatLog(chatID);

    public void Test(string text) => Clients.Caller.SendAsync("Test", "服务器向你问候" + text);
}