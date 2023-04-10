using Sirenix.Utilities;
using System;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

namespace TouhouMachineLearningSummary.control
{
    //用于展示单人多人页面中的卡组面板
    public class GachaControl : MonoBehaviour
    {
        public void InitGachaComponent() => Command.GachaCommand.InitGachaComponent();
        //打开信念背包组件
        public void ShowFaithBag() => Command.GachaCommand.ShowFaithBag();
        //关闭信念背包组件
        public void CloseFaithBag() => Command.GachaCommand.CloseFaithBag();
        //添加选择信念
        public void AddFaith(Transform item) => Command.GachaCommand.AddFaith(item);
        //移除选择信念
        public void RemoveFaith(Transform item) => Command.GachaCommand.RemoveFaith(item);
        //快速选择符合条件的信念
        public void QuickSelectFaith() => Command.GachaCommand.QuickSelectFaith();
        //抽卡,并打开开卡组件
        public void DrawCard() => Command.GachaCommand.DrawCard(Info.GachaInfo.SelectFaiths);
        //再次抽取1张卡
        public void DrawOneCardAgain() => Command.GachaCommand.QuickDrawCard(1);
        //再次抽取至多5张卡
        public void DrawMoreCardAgain() => Command.GachaCommand.QuickDrawCard(5);
        //关闭开卡界面
        public void CloseOpenCardsCanve() => Command.GachaCommand.CloseOpenCardComponent();

        public void ShowCard(Info.GachaCardInfo info) => Command.GachaCommand.ShowCard(info);
        public void TurnCard(Info.GachaCardInfo info) => Command.GachaCommand.TurnCard(info);
        public void ShowAllCards() => Info.GachaInfo.singleOpenCardInfos.Where(info => info.gameObject.activeSelf).ForEach(Command.GachaCommand.ShowCard);
        public void TurnAllCards() => Info.GachaInfo.singleOpenCardInfos.Where(info => info.gameObject.activeSelf).ForEach(Command.GachaCommand.TurnCard);
    }
}
