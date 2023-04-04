using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    internal class UserInfoManager : MonoBehaviour
    {
        public Text userName;
        public Text uid;
        public static UserInfoManager Instance { get; set; }

        public static string Account { get; set; }
        public static string UID { get; set; }

        public static string E_mail { get; set; }

        public static string Password { get; set; }

        private void Awake() => Instance = this;
        public static void Refresh()
        {
            Instance.userName.text = Info.AgainstInfo.OnlineUserInfo.Name;
            Instance.uid.text = "UID:" + Info.AgainstInfo.OnlineUserInfo.UID;
        }
    }
}