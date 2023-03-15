using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    public class ChatManager : MonoBehaviour
    {
        public GameObject openButton;
        public GameObject closeButton;
        public GameObject sendButton;
        public GameObject inputFiled;
        public GameObject textArea;
        public GameObject background;
        Queue PublicMessageQueue { get; set; } = new Queue();
        List<Queue> PrivateMessageQueue { get; set; } = new List<Queue>();
        public static ChatManager MainChat { get; set; }

        void Awake() => Init();
        public void Init()
        {
            MainChat = this;
            openButton.GetComponent<Button>().onClick.AddListener(OpenChat);
            closeButton.GetComponent<Button>().onClick.AddListener(CloseChat);
            sendButton.GetComponent<Button>().onClick.AddListener(SendMessage);
            CloseChat();
        }
        public async void SendMessage()
        {
            string text = inputFiled.GetComponent<InputField>().text;
            await Command.NetCommand.ChatAsync("ÓÃ»§" + Info.AgainstInfo.OnlineUserInfo.Name, text);
        }
        public void OpenChat()
        {
            openButton.SetActive(false);
            closeButton.SetActive(true);
            sendButton.SetActive(true);
            inputFiled.SetActive(true);
            textArea.SetActive(true);
            background.SetActive(true);
        }
        public void CloseChat()
        {
            openButton.SetActive(true);
            closeButton.SetActive(false);
            sendButton.SetActive(false);
            inputFiled.SetActive(false);
            textArea.SetActive(false);
            background.SetActive(false);
        }
        public void ReceiveMessage(string user, string text, string targetUser)
        {
            PublicMessageQueue.Enqueue((user, text));
            while (PublicMessageQueue.Count > 50) { PublicMessageQueue.Dequeue(); }
            textArea.GetComponent<Text>().text = "";
            foreach ((string user, string text) message in PublicMessageQueue)
            {
                textArea.GetComponent<Text>().text += $"<color=yellow><b>{message.user}</b></color>:<color=white>{message.text}</color>\n";
            }
        }
    }
}