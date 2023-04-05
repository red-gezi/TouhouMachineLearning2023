﻿using I18N.Common;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;

namespace TouhouMachineLearningSummary.Manager
{
    //用于展示单人多人页面中的卡组面板
    public class GachaManager : MonoBehaviour
    {
        public static GachaManager Instance { get; set; }
        [Header("卡池面板")]
        public GameObject FaithBag;

        public GameObject fastSelectButton;
        public GameObject openFaithBagButton;
        public GameObject closeFaithBagButton;



        [Header("开卡面板")]
        public GameObject drawOneCardButton;
        public GameObject drawMoreCardButton;
        public GameObject showAllCardsButton;
        public GameObject turnAllCardsButton;
        public GameObject closeButton;

        public Transform GachaBoardGroup;

        public List<GachaBoardInfo> singleOpenCardInfos = new List<GachaBoardInfo>();
        private void Awake()
        {
            Instance = this;
            for (int i = 0; i < 5; i++)
            {
                singleOpenCardInfos.Add(GachaBoardGroup.GetChild(i).GetComponent<GachaBoardInfo>());
            }
            Command.GachaCommand.InitFaithBag(FaithBag, closeFaithBagButton);
        }
        //打开信念背包组件
        public  void ShowFaithBag() =>Command.GachaCommand.ShowFaithBag(FaithBag, closeFaithBagButton);
        //关闭信念背包组件
        public  void CloseFaithBag() => Command.GachaCommand.CloseFaithBag(FaithBag, closeFaithBagButton);
        //抽卡,并打开开卡组件
        public void DrawCard() => Command.GachaCommand.QuickDrawCard(1);
        //再次抽取1张卡
        public void DrawOneCardAgain() => Command.GachaCommand.QuickDrawCard(1);
        //再次抽取至多5张卡
        public void DrawMoreCardAgain() => Command.GachaCommand.QuickDrawCard(5);
        //关闭开卡界面
        public void CloseOpenCardsCanve() => Command.GachaCommand.CloseOpenCardComponent();

        public void ShowCard(Info.GachaBoardInfo info) => Command.GachaCommand.ShowCard(info);
        public void TurnCard(Info.GachaBoardInfo info) => Command.GachaCommand.TurnCard(info);
        public void ShowAllCards() => singleOpenCardInfos.Where(info => info.gameObject.activeSelf).ForEach(Command.GachaCommand.ShowCard);
        public void TurnAllCards() => singleOpenCardInfos.Where(info => info.gameObject.activeSelf).ForEach(Command.GachaCommand.TurnCard);
    }
}
