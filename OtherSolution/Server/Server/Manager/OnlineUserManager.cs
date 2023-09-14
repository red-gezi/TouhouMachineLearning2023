using Microsoft.AspNetCore.SignalR;

namespace Server
{
    public class OnlineUserManager
    {
        public static Dictionary<string, PlayerInfo> OnlineUserList { get; set; } = new();
        public static void Add(string connectId, PlayerInfo playerInfo) => OnlineUserList[connectId] = playerInfo;
        public static void Remove(string connectId)
        {
            Console.WriteLine(OnlineUserList.ContainsKey(connectId));
            OnlineUserList.Remove(connectId);
        }
        //通过UIDp
        public static PlayerInfo GetOnlinePlayerInfo(string connectId)
        {
            if (OnlineUserList.ContainsKey(connectId))
            {
                Console.WriteLine($"查找到用户{connectId}{OnlineUserList[connectId].UID}");
                return OnlineUserList[connectId];
            }
            else
            {
                Console.WriteLine($"用户{connectId}不在线");
                return null;
            }
        }
        public static string GetConnectId(string UID) => OnlineUserList.FirstOrDefault(data => data.Value.UID == UID).Key;
        public static bool hasAgainstRoom(string uid)
        {
            var room = RoomManager.ContainPlayerRoom(uid);
            return room != null;
        }
    }
}
