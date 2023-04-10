using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Info
{
    //抽卡ui组件的相关信息
    public class GachaInfo : MonoBehaviour
    {
        [Header("抽卡相关组件")]
        //卡池界面组件
        public GameObject cardPoolComponent;
        //信念背包组件
        public GameObject faithBagComponent;
        //开卡界面组件
        public GameObject openCardComponent;
        

        [Header("信念选择器")]
        public GameObject fastSelectButton;
        public GameObject openFaithBagButton;
        public Transform faithSelectGroup;
        public static List<Faith> SelectFaiths { get; set; } = new();
        [Header("信念背包")]
        public Transform faithBagGroup;
        public GameObject faithBagItem;

        [Header("开卡面板")]
        public Transform GachaBoardGroup;

        public GameObject drawOneCardButton;
        public GameObject closeButton;
        public GameObject showAllCardsButton;
        public GameObject turnAllCardsButton;
        public GameObject drawMoreCardButton;
        

        public static List<GachaCardInfo> singleOpenCardInfos = new List<GachaCardInfo>();

        public static GachaInfo Instance { get; set; }

        private void Awake() => Instance = this;

        public List<Texture2D> TempSprites;

    }
}
