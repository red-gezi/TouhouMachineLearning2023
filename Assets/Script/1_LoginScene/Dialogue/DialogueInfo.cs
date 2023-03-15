using Sirenix.OdinInspector;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Info
{

    public class DialogueInfo : MonoBehaviour
    {
        public static DialogueInfo instance;
        void Awake() => instance = this;
        public GameObject dialogueCanvas;
        public GameObject selectUi;
        public GameObject models;
        public GameObject left;
        public GameObject right;
        public Text charaName;
        public Text charaText;
        public static bool isLeftCharaActive = false;
        public static bool isRightCharaActive = false;
        public static Transform targetLive2dChara = null;
        //当前加载的剧情对应的标签
        public static string StageTag { get; set; }
        //当前加载的剧情对应的小结
        public static int StageRank { get; set; }

        //跳过对话
        public static bool IsSkip { get; set; } = false;
        public static bool IsFastForward { get; set; } = false;
        public static bool IsSelectOver { get; set; } = true;
        public static bool IsShowNextText { get; set; } = false;
        public static int CurrentPoint { get; set; } = 0;
        public static int SelectBranch { get; set; } = 0;
        /// <summary>
        /// 记录剧本读取记录中的所有指令，方便回退
        /// </summary>
        public static Dictionary< string,Stack<DialogueModel>> DialogueSummary { get; set; } = new ();
        [ShowInInspector]
        public static DialogueModel currnetDialogueModel { get; set; } = new DialogueModel();
        [ShowInInspector]
        public static List<DialogueModel> DialogueModels { get; set; } = new List<DialogueModel>();
    }
}
