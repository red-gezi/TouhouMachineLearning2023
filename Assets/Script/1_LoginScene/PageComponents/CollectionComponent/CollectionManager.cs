using System.Collections.Generic;
using TouhouMachineLearningSummary.Command;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public class CollectionManager : MonoBehaviour
    {
        public GameObject againstSummaryComponent;
        public GameObject againstSummaryItem;
        public GameObject achievementComponent;
        public GameObject musicComponent;
        public GameObject cgComponent;

        static List<GameObject> CreatItem { get; set; } = new List<GameObject>();
        public static CollectionManager Manager { get; set; }
        void Awake() => Manager = this;
        public static void Init()
        {
            CreatItem.ForEach(DestroyImmediate);
            CreatItem.Clear();
            Manager.againstSummaryItem.SetActive(false);
            Manager.againstSummaryComponent.SetActive(false);
            Manager.achievementComponent.SetActive(false);
            Manager.musicComponent.SetActive(false);
            Manager.cgComponent.SetActive(false);
        }
        public static async void InitAgainstSummaryComponent()
        {
            Init();
            Manager.againstSummaryComponent.SetActive(true);
            var summarys = await NetCommand.DownloadOwnerAgentSummaryAsync(Info.AgainstInfo.OnlineUserInfo.Account, 0, 20);
            summarys.ForEach(summary =>
            {
                var item = Instantiate(Manager.againstSummaryItem, Manager.againstSummaryItem.transform.parent);
                item.SetActive(true);
                item.GetComponent<AgainstSummaryItemManager>().Init(summary);
                CreatItem.Add(item);
            });

        }
        public static void InitAchievementComponent()
        {
            Init();
        }
        public static void InitMusicComponent()
        {
            Init();
        }
        public static void InitCGComponent()
        {
            Init();
            Manager.cgComponent.SetActive(true);
        }
    }
}
