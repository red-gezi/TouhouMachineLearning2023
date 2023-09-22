using Microsoft.AspNetCore.SignalR.Client;
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
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    public static class NetCommand
    {
        static string ip => Info.AgainstInfo.IsHostNetMode ? "localhost:495" : "106.15.38.165:495";
        static HubConnection TouHouHub { get; set; } = null;
        public static async Task Init(bool isHotFixedLoad = false)
        {
            try
            {
                if (TouHouHub == null)
                {
                    TouHouHub = new HubConnectionBuilder().WithUrl($"http://{ip}/TouHouHub").Build();
                    await TouHouHub.StartAsync();

                    TouHouHub.On<string>("test", message => Debug.Log(message));
                    TouHouHub.On("QueryOfflineInvite", () => _ = QueryOfflineInvite());
                    TouHouHub.On<string>("Notifice", message => ChatUIManager.Instance.NotificeShow(message));
                    TouHouHub.On<string, ChatMessageInfo.ChatMessage>("ChatMessageReceive", (chatID, chatMessage) =>
                        ChatUIManager.Instance.RefreshChatMessages(chatID, new List<ChatMessageInfo.ChatMessage>() { chatMessage }));
                    TouHouHub.On<bool, string>("ResponseInvite", async (bool isSucceed, string name) =>
                    {
                        //这里弹个窗通知就行
                        string text = $"对玩家 {name} 的好友邀请";
                        text += (isSucceed ? "已被接受" : "已被拒绝");
                        ChatUIManager.Instance.NotificeShow(text);
                        //如果添加成功则触发好友列表刷新
                        if (isSucceed)
                        {
                            await QueryAllChatTargetInfo();
                        }
                    });
                    TouHouHub.On<object[]>("StartAgainst", ReceiveInfo =>
                    {
                        Info.AgainstInfo.RoomID = ReceiveInfo[0].ToType<string>();
                        bool isPlayer1 = ReceiveInfo[1].ToType<bool>();
                        bool isMyTurn = ReceiveInfo[2].ToType<bool>();
                        PlayerInfo playerInfo = ReceiveInfo[3].ToType<PlayerInfo>();
                        PlayerInfo opponentInfo = ReceiveInfo[4].ToType<PlayerInfo>();

                        _ = NoticeCommand.CloseAsync();//关闭ui
                        BookCommand.SimulateFilpPage(false);//停止翻书
                        MenuStateCommand.AddState(MenuState.ScenePage);//增加书页路径
                        //Manager.LoadingManager.manager?.OpenAsync();
                        Debug.Log("进入对战配置模式");
                        _ = AgainstConfig.OnlineStart(isPlayer1, isMyTurn, playerInfo, opponentInfo);
                    });
                    TouHouHub.On<NetAcyncType, object[]>("Async", (type, receiveInfo) =>
                    {
                        switch (type)
                        {
                            case NetAcyncType.FocusCard:
                                {
                                    int X = receiveInfo[0].ToType<int>();
                                    int Y = receiveInfo[1].ToType<int>();
                                    AgainstInfo.OpponentFocusCard = Command.RowCommand.GetCard(X, Y);
                                    break;
                                }
                            case NetAcyncType.PlayCard:
                                {
                                    int X = receiveInfo[0].ToType<int>();
                                    int Y = receiveInfo[1].ToType<int>();
                                    Card targetCard = Command.RowCommand.GetCard(X, Y);
                                    Info.AgainstInfo.playerPlayCard = targetCard;
                                    break;
                                }
                            case NetAcyncType.SelectRegion:
                                {
                                    Debug.Log("触发区域同步");
                                    AgainstInfo.SelectRowRank = receiveInfo[0].ToType<int>();
                                    break;
                                }
                            case NetAcyncType.SelectUnits:
                                {
                                    Debug.Log("收到同步单位信息");
                                    List<Location> Locations = receiveInfo[0].ToType<List<Location>>();
                                    AgainstInfo.SelectUnits.AddRange(Locations.Select(location => Command.RowCommand.GetCard(location.X, location.Y)));
                                    break;
                                }
                            case NetAcyncType.SelectLocation:
                                {
                                    Debug.Log("触发坐标同步");
                                    int X = receiveInfo[0].ToType<int>();
                                    int Y = receiveInfo[1].ToType<int>();
                                    Info.AgainstInfo.SelectRowRank = X;
                                    Info.AgainstInfo.SelectRank = Y;
                                    Debug.Log($"坐标为：{X}:{Y}");
                                    break;
                                }
                            case NetAcyncType.Pass:
                                {
                                    Info.AgainstInfo.IsPlayerPass = true;
                                    break;
                                }
                            case NetAcyncType.Surrender:
                                {
                                    Debug.Log("收到结束指令");
                                    _ = StateCommand.AgainstEnd(true, true);
                                    break;
                                }
                            case NetAcyncType.ExchangeCard:
                                {
                                    Debug.Log("交换卡牌信息");
                                    Location location = receiveInfo[0].ToType<Location>();
                                    int washInsertRank = receiveInfo[1].ToType<int>();
                                    _ = CardCommand.ExchangeCard(Command.RowCommand.GetCard(location), IsPlayerExchange: false, insertRank: washInsertRank);
                                    break;
                                }
                            case NetAcyncType.RoundStartExchangeOver:
                                Debug.LogError("交换卡牌完毕");
                                if (AgainstInfo.IsPlayer1)
                                {
                                    AgainstInfo.isPlayer2RoundStartExchangeOver = true;
                                }
                                else
                                {
                                    AgainstInfo.isPlayer1RoundStartExchangeOver = true;
                                }
                                break;
                            case NetAcyncType.SelectProperty:
                                {
                                    AgainstInfo.SelectProperty = receiveInfo[0].ToType<BattleRegion>();
                                    Debug.Log("通过网络同步当前属性为" + Info.AgainstInfo.SelectProperty);
                                    break;
                                }
                            case NetAcyncType.SelectBoardCard:
                                {
                                    AgainstInfo.SelectBoardCardRanks = receiveInfo[0].ToType<List<int>>();
                                    AgainstInfo.IsSelectCardOver = receiveInfo[1].ToType<bool>();
                                    break;
                                }
                            default:
                                break;
                        }
                    });
                }
                else
                {
                    //日后补充断线重连
                    Debug.Log("服务器已有初始化实例");
                }

            }
            catch (Exception e)
            {
                //热更界面使用独立的警告框素材单独处理
                if (isHotFixedLoad)
                {
                    Debug.LogError("无法连接至服务器");
                }
                else
                {
                    await NoticeCommand.ShowAsync("无法链接到服务器,请点击重连\n" + e.Message, NotifyBoardMode.Ok, okAction: async () => { await Init(); });
                }
            }
        }
        public static async Task CheckHubState()
        {
            if (TouHouHub == null)
            {
                await Init();
            }
            if (TouHouHub.State == HubConnectionState.Disconnected)
            {
                await TouHouHub.StartAsync();
            }
        }
        public static async void Dispose()
        {
            Debug.Log("释放网络资源");
            await TouHouHub.StopAsync();
        }
        public static async Task<int> RegisterAsync(string account, string password)
        {
            try
            {
                Debug.Log("注册请求");
                await CheckHubState();
                return await TouHouHub.InvokeAsync<int>("Register", account, password);
            }
            catch (Exception e) { Debug.LogException(e); }
            return -1;
        }
        public static async Task<PlayerInfo> LoginAsync(string account, string password)
        {
            try
            {
                Debug.Log("登陆请求");
                await CheckHubState();
                return await TouHouHub.InvokeAsync<PlayerInfo>("Login", account, password);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("账号登录失败");
                return null;
            }
        }
        public static async Task<PlayerInfo> QueryOtherUserInfo(string UID) => await TouHouHub.InvokeAsync<PlayerInfo>("QueryOtherUserInfoin", UID);

        public static async Task<List<string>> DrawCardAsync(string uid, string password, List<Faith> selectFaiths)
        {
            List<string> DrawCardList = new List<string>();
            try
            {
                Debug.Log("抽卡请求");
                await CheckHubState();
                DrawCardList = await TouHouHub.InvokeAsync<List<string>>("DrawCard", uid, password, selectFaiths);
                //抽完卡重新拉取用户信息
                Info.AgainstInfo.OnlineUserInfo = await LoginAsync(uid, password);
            }
            catch (Exception e) { Debug.LogException(e); }
            return DrawCardList;
        }
        ///////////////////////////////////////对战记录///////////////////////////////////////////////////////////////////////
        public static async Task UpdateTurnOperationAsync(AgainstSummaryManager.TurnOperation turnOperation)
        {
            await CheckHubState();
            await TouHouHub.SendAsync("UpdateTurnOperation", Info.AgainstInfo.RoomID, turnOperation);
        }
        public static async Task UpdateTurnPlayerOperationAsync(AgainstSummaryManager.TurnOperation.PlayerOperation playerOperation)
        {
            await CheckHubState();
            await TouHouHub.SendAsync("UpdatePlayerOperation", Info.AgainstInfo.RoomID, playerOperation);
        }
        public static async Task UpdateTurnSelectOperationAsync(AgainstSummaryManager.TurnOperation.SelectOperation selectOperation)
        {
            await CheckHubState();
            await TouHouHub.SendAsync("UpdateSelectOperation", Info.AgainstInfo.RoomID, selectOperation);
        }
        public static async Task UploadStartPointAsync()
        {
            await CheckHubState();
            await TouHouHub.SendAsync("UploadStartPoint", Info.AgainstInfo.RoomID, AgainstInfo.TurnRelativePoint);
        }
        public static async Task UploadEndPointAsync()
        {
            await CheckHubState();
            await TouHouHub.SendAsync("UploadEndPoint", Info.AgainstInfo.RoomID, AgainstInfo.TurnRelativePoint);
        }
        public static async Task UploadSurrenderAsync(int surrenddrState)
        {
            await CheckHubState();
            await TouHouHub.SendAsync("UploadSurrender", Info.AgainstInfo.RoomID, surrenddrState);
        }
        public static async Task<List<AgainstSummaryManager>> DownloadOwnerAgentSummaryAsync(string uid, int skipCount, int takeCount)
        {
            await CheckHubState();
            return await TouHouHub.InvokeAsync<List<AgainstSummaryManager>>("DownloadOwnerAgentSummary", uid, skipCount, takeCount);
        }
        public static async Task<List<AgainstSummaryManager>> DownloadAllAgentSummaryAsync(int skipCount, int takeCount)
        {
            await CheckHubState();
            return await TouHouHub.InvokeAsync<List<AgainstSummaryManager>>("DownloadAllAgentSummary", skipCount, takeCount);
        }
        ///////////////////////////////////////卡牌配置///////////////////////////////////////////////////////////////////////

        internal static async Task<string> GetCardConfigsVersionAsync()
        {
            Debug.Log("查询卡牌版本");
            await CheckHubState();
            string version = await TouHouHub.InvokeAsync<string>("GetCardConfigsVersion");
            Debug.Log("最新卡牌版本为" + version);
            return version;
        }

        public static async Task UploadCardConfigsAsync(CardConfig cardConfig, List<string> drawAbleList, string commandPassword)
        {
            try
            {
                Debug.Log("上传卡牌配置");
                await CheckHubState();
                string result = await TouHouHub.InvokeAsync<string>("UploadCardConfigs", cardConfig, drawAbleList, commandPassword);
                Debug.Log("新卡牌配置上传结果: " + result);
            }
            catch (Exception e) { Debug.LogException(e); }
        }
        /// <summary>
        /// 根据输入日期下载指定版本卡牌数据
        /// </summary>
        /// <param name="date"></param>
        public static async Task<CardConfig> DownloadCardConfigsAsync(string date)
        {
            try
            {
                Debug.Log("下载卡牌配置");
                await CheckHubState();
                return await TouHouHub.InvokeAsync<CardConfig>("DownloadCardConfigs", date);
            }
            catch (Exception e) { Debug.LogException(e); }
            return null;
        }
        ///////////////////////////////////////////////////用户操作////////////////////////////////////////////////////////////////
        //所有操作均需附带玩家密码，用于服务器校验操作合法性
        //static string PlayerPassWord => AgainstInfo.OnlineUserInfo.Password;
        //static string PlayerUID => AgainstInfo.OnlineUserInfo.UID;
        static string PlayerPassWord => "";

        static string PlayerUID => "1000";
        public static async Task<bool> UpdateInfoAsync(UpdateType updateType, object updateValue)
        {
            try
            {
                Log.Show("更新");
                await CheckHubState();
                return await TouHouHub.InvokeAsync<bool>("UpdateInfo", PlayerUID, PlayerPassWord, updateType, updateValue);
            }
            catch (Exception e) { Debug.LogException(e); }
            return false;
        }
        public static async Task ChatAsync(string name, string text, string target = "")
        {
            await CheckHubState();
            await TouHouHub.SendAsync("Chat", name, text, target);
        }
        //聊天
        ////////////////////////////////////用户信息操作/////////////////////////////////////
        //public static void ChangeUserName(string newName) => GameData.LocalPlayerManager.CmdChangeUserName(GameData.UserAccount, newName);
        //public static void RequestLocalPlayerData() => GameData.LocalPlayerManager.CmdQueryLocalPlayerData(GameData.UserAccount);
        //internal static void ResponseQueryLocalPlayerData(string playerData) => ChatUiMagnager.Instance.RefreshChatTargets(playerData);
        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public static async Task AddFriend(string targetUID)
        {
            await CheckHubState();
            await TouHouHub.SendAsync("AddFriend", PlayerPassWord, PlayerUID, targetUID);
        }
        ///////////////////请求同步离线请求///////////
        //玩家登录或被服务器通知时查询有无离线请求，如加好友等
        public static async Task QueryOfflineInvite()
        {
            Debug.Log("查询离线好友请求");
            await CheckHubState();
            var offlineInvites = await TouHouHub.InvokeAsync<List<OfflineInviteInfo>>("QueryOfflineInvite", PlayerPassWord, PlayerUID);
            //嵌套弹窗
            ChatUIManager.Instance.PopupLoad(offlineInvites);
        }
        //对好友请求做出回应并
        public static async Task ResponseOfflineInvite(string requestId, bool inviteResult)
        {
            await CheckHubState();
            await TouHouHub.SendAsync("ResponseOfflineInvite", PlayerPassWord, requestId, inviteResult);
        }
        ///////////////////删除好友////////////////////
        //主动删除者隐藏该聊天，对方则是锁定该聊天不再能发送消息
        public static async void DeleteFriend(string targetUID)
        {
            await CheckHubState();
            Info.AgainstInfo.OnlineUserInfo.ChatTargets = await TouHouHub.InvokeAsync<List<ChatTargetInfo>>("DeleteFriend", PlayerPassWord, PlayerUID, targetUID);
            ChatUIManager.Instance.RefreshChatTargets();
        }
        ///////////////////打开聊天界面////////////////
        //刷新好友列表中指定用户的状态信息,并更新本地
        public static async Task QueryChatTargetInfo(string targetUID)
        {
            await CheckHubState();
            await TouHouHub.InvokeAsync<ChatTargetInfo>("QueryChatTargetInfo", PlayerPassWord, PlayerUID, targetUID);
        }
        //刷新好友列表中每个用户的状态信息,并更新本地
        public static async Task QueryAllChatTargetInfo()
        {
            await CheckHubState();
            Info.AgainstInfo.OnlineUserInfo.ChatTargets = await TouHouHub.InvokeAsync<List<ChatTargetInfo>>("QueryAllChatTargetInfo", PlayerPassWord, PlayerUID);
            ChatUIManager.Instance.RefreshChatTargets();
        }
        ///////////////////聊天记录///////////////
        //start为-1时，自动替换值为聊天记录中最后一条，range为负数时，代表向上查询
        public static async Task QueryChatLog(string chatID)
        {
            await CheckHubState();
            var chatMessages = await TouHouHub.InvokeAsync<List<ChatMessageInfo.ChatMessage>>("QueryChatLog", chatID);
        }
        //清除指定聊天未读消息数量
        public static async Task ClearUnreadCount(string chatID)
        {
            await CheckHubState();
            await TouHouHub.SendAsync("ClearUnreadCount", PlayerPassWord, chatID);
        }
        ///////////////////发送聊天信息///////////////
        //请求发送消息
        public static async Task SendMessage(string chatID, ChatMessageInfo.ChatMessage message, string speakerUID, string targetChaterUID)
        {
            await CheckHubState();
            await TouHouHub.SendAsync("SendMessage", PlayerPassWord, chatID, message, speakerUID, targetChaterUID);
        }
        ///////////////////////////////////////////////////房间操作////////////////////////////////////////////////////////////////
        public static async Task JoinHoldOnList(AgainstModeType modeType, int firstMode, PlayerInfo userInfo, PlayerInfo virtualOpponentInfo)
        {
            await CheckHubState();
            try
            {
                Debug.Log($"发送数据 我方牌组数：{userInfo.UseDeck.CardIDs.Count} 敌方牌组数{virtualOpponentInfo?.UseDeck.CardIDs.Count}");
                await TouHouHub.SendAsync("Join", modeType, firstMode, userInfo, virtualOpponentInfo);
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }
        public static async Task<bool> LeaveHoldOnList(AgainstModeType modeType, string uid)
        {
            await CheckHubState();
            return await TouHouHub.InvokeAsync<bool>("Leave", modeType, uid);
        }
        public static async Task<bool> AgainstFinish()
        {
            await CheckHubState();
            return await TouHouHub.InvokeAsync<bool>("AgainstFinish", Info.AgainstInfo.RoomID, AgainstInfo.OnlineUserInfo.UID, AgainstInfo.PlayerScore.P1Score, AgainstInfo.PlayerScore.P2Score);
        }
        //判断是否存在正在对战中的房间
        internal static async Task CheckRoomAsync(string text1, string text2)
        {
            await CheckHubState();
            int roomId = await TouHouHub.InvokeAsync<int>("CheckRoom");
            if (true)
            {

            }
        }
        //数据同步类型
        public static async void AsyncInfo(NetAcyncType AcyncType)
        {
            if (Info.AgainstInfo.IsPVP && (Info.AgainstInfo.IsMyTurn || AcyncType == NetAcyncType.FocusCard || AcyncType == NetAcyncType.ExchangeCard || AcyncType == NetAcyncType.RoundStartExchangeOver))
            {
                await CheckHubState();
                switch (AcyncType)
                {
                    case NetAcyncType.Init:
                        break;
                    case NetAcyncType.FocusCard:
                        {
                            Location TargetCardLocation = Info.AgainstInfo.PlayerFocusCard != null ? Info.AgainstInfo.PlayerFocusCard.Location : new Location(-1, -1);
                            await TouHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { TargetCardLocation.X, TargetCardLocation.Y });
                            break;
                        }
                    case NetAcyncType.PlayCard:
                        {
                            Debug.Log("同步打出卡牌");
                            Location TargetCardLocation = Info.AgainstInfo.playerPlayCard.Location;
                            await TouHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { TargetCardLocation.X, TargetCardLocation.Y });
                            break;
                        }
                    case NetAcyncType.DisCard:
                        Debug.Log("同步弃掉卡牌");

                        break;
                    case NetAcyncType.SelectRegion:
                        {
                            int RowRank = Info.AgainstInfo.SelectRowRank;
                            Debug.Log("同步焦点区域为" + RowRank);
                            await TouHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { RowRank });
                            break;
                        }
                    case NetAcyncType.SelectUnits:
                        {
                            List<Location> Locations = Info.AgainstInfo.SelectUnits.SelectList(unit => unit.Location);
                            Debug.LogError("选择单位完成：" + Locations.ToJson());
                            await TouHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { Locations });
                            break;
                        }
                    case NetAcyncType.SelectLocation:
                        {
                            int RowRank = Info.AgainstInfo.SelectRowRank;
                            int LocationRank = Info.AgainstInfo.SelectRank;
                            Debug.Log("同步焦点坐标给对面：" + RowRank);
                            await TouHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { RowRank, LocationRank });
                            break;
                        }
                    case NetAcyncType.ExchangeCard:
                        {
                            Debug.Log("触发交换卡牌信息");
                            Location Locat = Info.AgainstInfo.TargetCard.Location;
                            int RandomRank = Info.AgainstInfo.washInsertRank;
                            await TouHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { Locat, RandomRank });
                            break;
                        }
                    case NetAcyncType.RoundStartExchangeOver:
                        Debug.Log("触发回合开始换牌完成信息");
                        await TouHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { });
                        break;
                    case NetAcyncType.Pass:
                        {
                            Debug.Log("pass");
                            await TouHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { });
                            break;
                        }
                    case NetAcyncType.Surrender:
                        {
                            Debug.Log("投降");
                            await TouHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { });
                            break;
                        }
                    case NetAcyncType.SelectProperty:
                        {
                            Debug.Log("选择场地属性");
                            await TouHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, AgainstInfo.SelectProperty);
                            break;
                        }
                    case NetAcyncType.SelectBoardCard:
                        Debug.Log("同步面板卡牌数据选择");
                        await TouHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { Info.AgainstInfo.SelectBoardCardRanks, Info.AgainstInfo.IsSelectCardOver });
                        break;


                    default:
                        {
                            Debug.Log("异常同步指令");
                            break;
                        }
                }
            }
        }


    }
}