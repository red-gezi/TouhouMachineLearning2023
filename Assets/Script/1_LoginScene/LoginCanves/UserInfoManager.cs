using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    internal class UserInfoManager : MonoBehaviour
    {
        public Text userName;
        public Text uid;
        static UserInfoManager manager;
        private void Awake() => manager = this;
        public static void Refresh()
        {
            manager.userName.text = Info.AgainstInfo.OnlineUserInfo.Name;
            manager.uid.text = "UID:" + Info.AgainstInfo.OnlineUserInfo.UID;
        }
    }
}