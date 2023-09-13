using System;
using System.Collections;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEditor;

public class ChatMessageData
{
    public string _id { get; set; }
    //��ͬ���ڵ�������־
    public List<ChatMessage> chatMessages = new();
    ////�����ߵ�UUID
    //public List<int> chatterUUID = new List<int>();
    public class ChatMessage
    {
        //��Ϣ����
        public int index;
        public string date;
        //������
        public int speakerUUID;
        public string speakerName;
        //��Ϣ����
        public ChatMessageType messageType;
        //������Ϣ��������ͼƬ��Ϣ
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
    //�̶�ʱ�ζ�ʱ����
    public void DeleteLog(int userUUID, string Text)
    {
        string date = DateTime.Today.ToShortDateString();
    }
}

