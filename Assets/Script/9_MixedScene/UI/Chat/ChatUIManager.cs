using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static TouhouMachineLearningSummary.Model.ChatMessageInfo;

namespace TouhouMachineLearningSummary.Manager
{
    internal class ChatUIManager : MonoBehaviour
    {
        public static ChatUIManager Instance;
        //聊天列表相关组件
        [Header("聊天列表组件")]
        public GameObject chatTargetCanves;
        public Transform chatTargetContent;
        public GameObject chatTargetPrefab;
        //聊天信息相关组件
        [Header("聊天消息组件")]
        public GameObject chatMessageCanves;
        public Transform chatMessageContent;
        public InputField chatMessageInput;
        [Header("聊天框预制UI")]
        public GameObject leftChatMessagePrefab;
        public GameObject rightChatMessagePrefab;
        public GameObject loadMorePrefab;
        public GameObject messageTimePrefab;
        [Header("消息弹窗组件")]
        public GameObject NotificationCanves;
        [Header("消息通知组件")]
        public GameObject PopupCanves;
        private void Awake() => Instance = this;
        private void Start()
        {
            CloseChatTargetCanves();
            CloseChatMessageCanves();
            PopupShow();
            NotificationCanves.SetActive(true);
            NotificationCanves.GetComponent<CanvasGroup>().alpha = 0;
        }
        ////////////////////////////////玩家数据修改//////////////////////////////////////////
        //改名字
        //public void ChangeUserName(string newName) => ChatCommand.ChangeUserName(newName);
        //查询玩家信息，并在相应中刷新UI
        //public void QueryLocalPlayerData() => ChatCommand.RequestLocalPlayerData();
        ////////////////////////////////好友操作指令//////////////////////////////////////////
        public async void AddFriendInvite(string targetUUID)
        {
            await Command.NetCommand.AddFriend(targetUUID);
        }
        public void DeleteFriend(string targetUUID) => Command.NetCommand.DeleteFriend(targetUUID);
        ////////////////////////////////聊天面板操作指令//////////////////////////////////////////
        //聊天界面
        public async void OpenChatTargetCanves()
        {
            chatTargetCanves.SetActive(true);
            await Command.NetCommand.QueryAllChatTargetInfo();
        }
        public void CloseChatTargetCanves()
        {
            chatTargetCanves.SetActive(false);
            CloseChatMessageCanves();
        }
        ////////////////////////////////聊天记录加载//////////////////////////////////////////
        //当前用户的所有聊天记录
        static Dictionary<string, List<ChatMessage>> allChatMessage = new();
        static List<GameObject> chatMessageItem = new();
        //当前正打开的聊天窗口
        static string currentOpenChatID = "";
        public async void OpenChatMessageCanves(string chatID)
        {
            chatMessageCanves.SetActive(true);
            currentOpenChatID = chatID;
            await Command.NetCommand.QueryChatLog(chatID);
            //读取后去掉红点
            await Command.NetCommand.ClearUnreadCount(chatID);
        }
        public void CloseChatMessageCanves()
        {
            chatMessageCanves.SetActive(false);
            currentOpenChatID = "";
        }
        ////////////////////////////////UI界面刷新//////////////////////////////////////////
        //

        //刷新玩家本地信息，更新聊天对象列表用户列表数据、修改用户信息、道具使用、增加或删除好友时调用，同时会刷新聊天对象界面
        public  void RefreshChatTargets()
        {
            var chatTargets = Info.AgainstInfo.OnlineUserInfo.ChatTargets;
            int chatCount = chatTargets.Count;
            int currentChatButtonCount = chatTargetContent.childCount;
            for (int i = currentChatButtonCount; i < chatCount; i++)
            {
                Instantiate(chatTargetPrefab, chatTargetContent);
            }
            currentChatButtonCount = chatTargetContent.childCount;
            for (int i = 0; i < currentChatButtonCount; i++)
            {
                if (i < chatCount)
                {
                    GameObject chatTarget = chatTargetContent.GetChild(i).gameObject;
                    chatTarget.SetActive(true);
                    var chatTargetInfo = chatTargets[i];
                    var targetChaterUUID = chatTargetInfo.TargetChaterUID;
                    //移动到服务端
                    //注意UI报null的话会导致服务端自动退出
                    chatTarget.transform.GetChild(0).GetComponent<Text>().text = chatTargetInfo.Name;
                    chatTarget.transform.GetChild(1).GetComponent<Text>().text = chatTargetInfo.Name + ":" + chatTargetInfo.LastMessage;
                    chatTarget.transform.GetChild(2).GetComponent<Text>().text = chatTargetInfo.Name + ":" + chatTargetInfo.LastMessageTime;
                    //设置未读数字
                    chatTarget.transform.GetChild(3).gameObject.SetActive(chatTargetInfo.UnReadCount > 0);
                    chatTarget.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = chatTargetInfo.UnReadCount.ToString();
                    chatTarget.transform.GetChild(4).GetComponent<Text>().text = chatTargetInfo.Signature;
                    chatTarget.GetComponent<Button>().onClick.RemoveAllListeners();
                    chatTarget.GetComponent<Button>().onClick.AddListener(() => OpenChatMessageCanves(chatTargetInfo.ChatID));
                    chatTarget.transform.GetChild(5).GetComponent<Button>().onClick.RemoveAllListeners();
                    chatTarget.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => Command.NetCommand.DeleteFriend(chatTargetInfo.TargetChaterUID));
                }
                else
                {
                    chatTargetContent.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        //刷新聊天信息列表（只有收到消息和用户切换聊天对象时才会调用刷新）
        public  async void RefreshChatMessages(string chatId, List<ChatMessage> chatMessages)
        {
            //这里会拿到指定chatid的指定范围的聊天记录，需要对已有聊天记录做个添加去重，然后刷新聊天对象列表和聊天框内容
            if (!allChatMessage.ContainsKey(chatId))
            {
                allChatMessage[chatId] = chatMessages;
            }
            else
            {
                var Indexs = allChatMessage[chatId].Select(msg => msg.index);
                //添加新消息，去重，填补
                chatMessages.ForEach(message =>
                {
                    if (!Indexs.Contains(message.index))
                    {
                        allChatMessage[chatId].Add(message);
                    }
                });
            }
            //刷新聊天对象ui,暂时先列表所有的一起刷新
            await Command.NetCommand.QueryAllChatTargetInfo();
            //如果改聊天正好打开，则刷新聊天信息窗口
            if (chatId == currentOpenChatID)
            {
                chatMessageItem.ForEach(item => Destroy(item.gameObject));
                chatMessageItem.Clear();
                for (int i = 0; i < allChatMessage[chatId].Count; i++)
                {
                    var message = allChatMessage[chatId][i];
                    bool isLocalPlayerSpeaker = (message.speakerUUID == Info.AgainstInfo.OnlineUserInfo.UID);
                    switch (message.messageType)
                    {
                        case ChatMessageType.Text:
                            var messagePrefab = isLocalPlayerSpeaker ? Instance.rightChatMessagePrefab : Instance.leftChatMessagePrefab;
                            var messageItem = Instantiate(messagePrefab, Instance.chatMessageContent);
                            messageItem.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = message.text;
                            chatMessageItem.Add(messageItem);
                            messageItem.SetActive(true);
                            //修改文字
                            break;
                        case ChatMessageType.Expression:
                            break;
                        default: break;
                    }
                }
            }
        }
        ////////////////////////////////发送消息//////////////////////////////////////////
        public async void SendTextToPlayer()
        {
            //根据对象id在聊天面板中查找指定聊天
            var message = new ChatMessage()
            {
                messageType = ChatMessageType.Text,
                speakerUUID = Info.AgainstInfo.OnlineUserInfo.UID,
                speakerName = Info.AgainstInfo.OnlineUserInfo.Name,
                text = chatMessageInput.text,
            };
            chatMessageInput.text = "";
            var current = Info.AgainstInfo.OnlineUserInfo.ChatTargets.FirstOrDefault(chat => chat.ChatID == currentOpenChatID);
            await NetCommand.SendMessage(current.ChatID, message, Info.AgainstInfo.OnlineUserInfo.UID, current.TargetChaterUID);
            //通知刷新
        }
        public void SendExpressionToPlayer()
        {

        }
        ////////////////////////////////弹窗系统//////////////////////////////////////////
        Queue<OfflineInviteInfo> offlineRequestsQueue = new Queue<OfflineInviteInfo>();
        OfflineInviteInfo currentOfflineRequest;
        bool isShow;
        public async void PopupLoad(List<OfflineInviteInfo> offlineRequests)
        {
            offlineRequestsQueue.Clear();
            offlineRequests.ForEach(offlineRequestsQueue.Enqueue);
            PopupShow();
            //向同频道所有人广播消息
        }
      
        public async void PopupAccept()
        {
            await Command.NetCommand.ResponseOfflineInvite(currentOfflineRequest._id, true);
            PopupCanves.SetActive(false);
            await Task.Delay(1000);
            PopupShow();
            //刷新好友列表
            await Command.NetCommand.QueryAllChatTargetInfo();
        }
        public async void PopupReject()
        {
            Debug.Log("拒绝了" + currentOfflineRequest._id);
            await Command.NetCommand.ResponseOfflineInvite(currentOfflineRequest._id, false);
            PopupCanves.SetActive(false);
            await Task.Delay(1000);
            PopupShow();
        }
        async void PopupShow()
        {
            //如果离线请求队列中存在
            if (offlineRequestsQueue.Any())
            {
                PopupCanves.SetActive(true);
                currentOfflineRequest = offlineRequestsQueue.Dequeue();
            }
            else
            {
                PopupCanves.SetActive(false);
            }
        }
        ////////////////////////////////消息通告系统//////////////////////////////////////////
        public async void NotificeInviteResult(bool isSucceed, string name)
        {
            //这里弹个窗通知就行
            string text = $"对玩家 {name} 的好友邀请";
            text += (isSucceed ? "已被接受" : "已被拒绝");
            Debug.Log(text);
            ChatUIManager.Instance.NotificeShow(text);
            //如果添加成功则触发好友列表刷新
            if (isSucceed)
            {
                await Command.NetCommand.QueryAllChatTargetInfo();
            }
        }

        public async void NotificeShow(string text)
        {
            NotificationCanves.transform.GetChild(0).GetComponent<Text>().text = text;
            for (int i = 0; i < 50; i++)
            {
                NotificationCanves.GetComponent<CanvasGroup>().alpha = i * 1f / 50;
                await Task.Delay(10);
            }
            await Task.Delay(500);
            for (int i = 0; i < 50; i++)
            {
                NotificationCanves.GetComponent<CanvasGroup>().alpha = 1 - i * 1f / 50;
                await Task.Delay(10);
            }
        }
    }
}
