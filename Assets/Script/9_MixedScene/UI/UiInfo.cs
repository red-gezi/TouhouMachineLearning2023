using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TouhouMachineLearningSummary.Info
{
    public class UiInfo : MonoBehaviour
    {
        public static UiInfo Instance { get; set; }
        //判断面板是否是需要选择模式
        public static bool IsCardBoardNeedSelect { get; set; } = false;
        public static List<GameObject> ShowCardLIstOnBoard { get; set; } = new List<GameObject>();
        public static GameEnum.CardBoardMode LastCardBoardMode { get; set; } = GameEnum.CardBoardMode.Select;
        [Header("菜单场景")]
        public GameObject loginCanvas_Model;
        [Header("对战场景")]
        public GameObject DownPass;
        public GameObject UpPass;
        public GameObject Arrow;
        public GameObject ArrowEndPoin;
        public GameObject CardBoardCanvas;
        public Transform CardBoardContent;
        public GameObject BoardCard;
        public GameObject NoticePopup;
        [Header("通用场景")]
        public GameObject Notice;
        private void Awake() => Instance = this;
        public static GameObject MyPass => AgainstInfo.IsMyTurn ? Instance.DownPass : Instance.UpPass;
        public static GameObject OpPass => AgainstInfo.IsMyTurn ? Instance.UpPass : Instance.DownPass;

    }
}