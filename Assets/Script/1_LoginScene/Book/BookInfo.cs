using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class BookInfo : MonoBehaviour
    {
        //组件化
        [Header("书本模型")]
        [Sirenix.OdinInspector.ShowInInspector]
        public static bool isBookOpen;
        public static bool IsSimulateFilpPage { get; set; }
        public static BookInfo instance;

        [Header("书本模型")]
        public GameObject coverModel;
        public GameObject axisModel;
        //翻页过程中的空白书页
        public GameObject voidPageModel;
        //翻页过程中的虚假书页
        public GameObject fakePageModel;

        [Header("UI组件")]
        public GameObject UIComponent;

        public GameObject singleModeSelectComponent;
        public GameObject multiplayerModeSelectComponent;
        public GameObject practiceComponent;
        public GameObject cardListComponent;
        public GameObject cardDeckListComponent;
        public GameObject cardLibraryComponent;
        public GameObject mapComponent;
        public GameObject cardDetailComponent;
        public GameObject campSelectComponent;
        public GameObject scenePageComponent;

        public GameObject shrineComponent;
        public GameObject collectComponent;
        public GameObject configComponent;
        void Awake() => instance = this;
    }
}

