using System;
using System.Collections;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEditor;

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
        public int index;
        public string date;
        //发言者
        public int speakerUUID;
        public string speakerName;
        //消息类型
        public ChatMessageType messageType;
        //聊天信息、语音、图片信息
        public string text;

        public ChatMessage()
        {
        }
    }
    public void AppendMessage(int speakerUUID, string speakerName, string Text)
    {
        string date = DateTime.Today.ToShortDateString();
        chatMessages.Add(new ChatMessage() { speakerUUID = speakerUUID, speakerName = speakerName, date = date, text = Text });
    }
    //固定时段定时操作
    public void DeleteLog(int userUUID, string Text)
    {
        string date = DateTime.Today.ToShortDateString();
    }
}

