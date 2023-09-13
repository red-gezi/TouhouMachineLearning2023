using System;
using System.Linq;
public class OfflineInviteData
{
    public string _id;
    public RequestType requestType;
    public int senderUUID;
    public int receiverUUID;
    public string senderName;
    public string receiverName;
    public DateTime creatTime;
    public OfflineInviteData(){}
    public OfflineInviteData(RequestType requestType, string account, int receiverUUID)
    {
        var userInfo = DbCommand.QueryUserInfo(account);
        if (userInfo == null) return;
        _id = Guid.NewGuid().ToString();
        this.requestType = requestType;
        creatTime = DateTime.Now;
        this.senderUUID = userInfo.UUID;
        this.senderName = userInfo.Name;
        this.receiverUUID = receiverUUID;
        this.receiverName = DbCommand.QueryOtherUserInfo(receiverUUID).Name;
    }
    //不同日期的聊天日志
    public void Appect()
    {
        DbCommand.AddChatMember(senderUUID, receiverUUID, requestType);
        DbCommand.DeleteOfflineRequest(_id);
    }
    public void Reject() => DbCommand.DeleteOfflineRequest(_id);
}