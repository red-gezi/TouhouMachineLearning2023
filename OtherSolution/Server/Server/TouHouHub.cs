
using Server;
using MongoDB.Bson;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;

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
    public PlayerInfo? Login(string account, string password) => MongoDbCommand.Login(account, password);
    public List<string> DrawCard(string uid, string password, List<Faith> selectFaiths) => MongoDbCommand.DrawCard(uid, password, selectFaiths);
    //////////////////////////////////////////////等候列表////////////////////////////////////////////////////////////////////
    public void Join(AgainstModeType againstMode, int FirstMode, PlayerInfo userInfo, PlayerInfo virtualOpponentInfo) => HoldListManager.Add(againstMode, FirstMode, userInfo, virtualOpponentInfo, Clients.Caller);
    public void Leave(AgainstModeType againstMode, string uid) => HoldListManager.Remove(againstMode, uid);
    //////////////////////////////////////////////房间////////////////////////////////////////////////////////////////////
    public void Async(NetAcyncType netAcyncType, string roomId, bool isPlayer1, object[] data) => RoomManager.GetRoom(roomId)?.AsyncInfo(netAcyncType, isPlayer1, data);
    public bool AgainstFinish(string roomId, string uid, int P1Score, int P2Score) => RoomManager.DisponseRoom(roomId, uid, P1Score, P2Score);
    //////////////////////////////////////////////用户信息更新操作////////////////////////////////////////////////////////////////////
    public bool UpdateInfo(UpdateType updateType, string uid, string password, object updateValue)
    {
        switch (updateType)
        {
            case UpdateType.Name: return MongoDbCommand.UpdateInfo(uid, password, (x => x.Name), updateValue.To<string>());
            case UpdateType.UnlockTitles: return MongoDbCommand.UpdateInfo(uid, password, (x => x.UnlockTitleTags), updateValue.To<List<string>>());
            case UpdateType.PrefixTitle:return MongoDbCommand.UpdateInfo(uid, password, (x => x.UsePrefixTitleTag), updateValue.To<string>());
            case UpdateType.SuffixTitle: return MongoDbCommand.UpdateInfo(uid, password, (x => x.UseSuffixTitleTag), updateValue.To<string>());
            case UpdateType.Decks: return MongoDbCommand.UpdateInfo(uid, password, (x => x.Decks), updateValue.To<List<Deck>>());
            case UpdateType.UseDeckNum: return MongoDbCommand.UpdateInfo(uid, password, (x => x.UseDeckNum), updateValue.To<int>());
            case UpdateType.Stage: return MongoDbCommand.UpdateInfo(uid, password, (x => x.Stage), updateValue.To<Dictionary<string, int>>());
            case UpdateType.LastLoginTime:
                return MongoDbCommand.UpdateInfo(uid, password, (x => x.LastLoginTime), updateValue.To<DateTime>());
                break;
           
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
    public string UploadCardConfigs(CardConfig cardConfig, List<string> drawAbleList,string CommandPassword) => MongoDbCommand.InsertOrUpdateCardConfig(cardConfig, drawAbleList,  CommandPassword);
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
    public void Chat(string name, string message, string target)
    {
        Console.WriteLine("转发聊天记录" + message);
        if (target == "")
        {
            Clients.All.SendAsync("ChatReceive", (name, message).ToJson());
        }
        else
        {
            Clients.Client("").SendAsync("ChatReceive", (name, message).ToJson());
        }
    }
    public void Test(string text) => Clients.Caller.SendAsync("Test", "服务器向你问候" + text);
}