using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Info
{
    /// <summary>
    /// 与书页ui组件相关的列表
    /// </summary>
    public class PageComponentInfo : MonoBehaviour
    {
        //全卡牌解锁的管理员账户
        public static bool IsAdmin = true;
        //public static bool cardListCanChange = false;
        //当前所选对战模式
        public static AgainstModeType currentAgainstMode = AgainstModeType.Story;

        [Header("卡组组件")]
        public GameObject _cardDeckNameModel;
        public GameObject _cardDeckContent;
        public GameObject _cardDeckCardModel;
        public GameObject _cardLeaderImageModel;

        public GameObject _changeButton;
        public GameObject _okButton;
        public GameObject _cancelButton;

        public static GameObject cardDeckNameModel;
        public static GameObject cardDeckContent;
        public static GameObject cardDeckCardModel;
        public static GameObject cardLeaderImageModel;


        public static GameObject changeButton;
        public static GameObject okButton;
        public static GameObject cancelButton;

        public static List<GameObject> deckCardModels = new List<GameObject>();

        public static Model.Deck tempDeck;
        //List<GameObject> ShowCardList;
        //获得指定卡组的去重并按品质排序后的列表
        public static List<string> distinctCardIds => tempDeck.CardIDs
            .Distinct()
            .OrderBy(id => Manager.CardAssemblyManager.GetLastCardInfo(id).cardRank)
            .ThenByDescending(id => Manager.CardAssemblyManager.GetLastCardInfo(id).point)
            .ThenByDescending(id => id)
            .ToList();
        ///////////////////////////////////////////////////////////关卡信息/////////////////////////////////////

        public GameObject stageModel;
        public Image leaderSprite;
        public Text leaderName;
        public Text leaderNick;
        public Text leaderIntroduction;
        public Text stageIntroduction;

        /// <summary>
        /// 所选大关的所有小关信息
        /// </summary>
        public static List<StageInfoModel> CurrentSelectStageInfos { get; set; }
        public static string CurrentStage { get; set; }
        public static int CurrentStep { get; set; }
        ///////////////////////////////////////////////////////////练习选项信息/////////////////////////////////////
        public static PracticeLeader SelectLeader { get; set; }
        public static int SelectFirstHandMode { get; set; } = 0;

        ///////////////////////////////////////////////////////////牌库信息/////////////////////////////////////
        [Header("牌库组件")]
        public GameObject _cardLibraryContent;
        public GameObject _cardLibraryCardModel;

        public static GameObject cardLibraryContent;
        public static GameObject cardLibraryCardModel;

        public static List<GameObject> libraryCardModels = new List<GameObject>();
        public static bool isEditDeckMode = true;
        public static List<Model.CardModel> LibraryFilterCardList { get; set; }
        ///////////////////////////////////////////////////////////卡牌能力详情组件/////////////////////////////
        [Header("卡牌能力详情组件")]
        public GameObject _targetCardTexture;
        public GameObject _targetCardName;
        public GameObject _targetCardTag;
        public GameObject _targetCardAbility;

        public static GameObject targetCardTexture;
        public static GameObject targetCardName;
        public static GameObject targetCardTag;
        public static GameObject targetCardAbility;

        ///////////////////////////////////////////////////////////阵营选择信息信息/////////////////////////////

        [Header("阵营组件")]
        public GameObject modelContent;

        public GameObject CampModel;
        public GameObject LeaderModel;

        public Sprite NeutralTex;
        public Sprite TaoismTex;
        public Sprite ShintoismTex;
        public Sprite BuddhismTex;
        public Sprite scienceTex;

        public GameObject selectCampOverBtn;
        public GameObject selectLeaderOverBtn;
        public GameObject rebackBtn;
        public GameObject lastStepBtn;


        public static List<GameObject> selectCardModels = new List<GameObject>();
        public static bool isCampIntroduction = false;
        public static Camp focusCamp = Camp.Neutral;
        public static Camp selectCamp = Camp.Neutral;

        public static string FocusLeaderID { get; set; } = "";
        public static string SelectLeaderID { get; set; } = "";



        ///////////////////////////////////////////////////////////牌组信息/////////////////////////////
        [Header("牌组信息组件")]
        [ShowInInspector]
        public static int selectDeckRank = 0;
        public GameObject deckModel;
        public List<GameObject> deckModels;
        public static float fre = -310f;
        public static float bias = 0.13f;
        public static float show = -0.66f;
        public static bool isDragMode;//是否处于手动拖拽模式
        public static bool isCardClick;
        [ShowInInspector]
        public static List<float> values = new List<float>();
        public Transform content;

        ///////////////////////////////////////////////////////////用户信息/////////////////////////////

        [Header("用户信息组件")]
        public GameObject playerDataCanve;
        public GameObject chart;
        public GameObject title;
        public GameObject editCanve;
        public GameObject editNameCanve;
        public GameObject editSignatureCanve;
        public GameObject editPrefixTitleCanve;
        public GameObject editSuffixTitleCanve;

        public GameObject prefixTitleModel;
        public GameObject suffixTitleModel;
        ///////////////////////////////////////////////////////////抽卡信息/////////////////////////////
        public static List<Faith> SelectFaiths { get; set; } = new();

        //抽卡界面组件
        public GameObject DrawCardComponent;
        //信念选择组件
        public GameObject FaithComponent;
        //开卡界面组件
        public GameObject OpenCardComponent;
        



        //单例
        public static PageComponentInfo Instance { get; set; }

        //有时间优化掉
        private void Awake()
        {
            Instance = this;
            //牌组组件
            cardDeckNameModel = _cardDeckNameModel;
            cardDeckContent = _cardDeckContent;
            cardDeckCardModel = _cardDeckCardModel;
            cardLeaderImageModel = _cardLeaderImageModel;
            //牌库组件
            cardLibraryContent = _cardLibraryContent;
            cardLibraryCardModel = _cardLibraryCardModel;
            cardLibraryCardModel.SetActive(false);
            //卡牌能力详情组件
            targetCardTexture = _targetCardTexture;
            targetCardName = _targetCardName;
            targetCardTag = _targetCardTag;
            targetCardAbility = _targetCardAbility;
            changeButton = _changeButton;
            okButton = _okButton;
            cancelButton = _cancelButton;
        }
    }
}