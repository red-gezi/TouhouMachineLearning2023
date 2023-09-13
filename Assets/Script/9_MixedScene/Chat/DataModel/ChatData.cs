//交流对象的信息
using TouhouMachineLearningSummary.GameEnum;

public class ChatData
{
    public string ChatID { get; set; }
    public int TargetChaterUUID { get; set; }
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