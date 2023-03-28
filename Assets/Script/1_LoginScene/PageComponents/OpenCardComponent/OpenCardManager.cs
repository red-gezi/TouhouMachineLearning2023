using I18N.Common;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;

namespace TouhouMachineLearningSummary.Manager
{
    //用于展示单人多人页面中的卡组面板
    public class OpenCardManager : MonoBehaviour
    {
        public static OpenCardManager Instance { get; set; }
        public GameObject drawOneCardButton;
        public GameObject drawMoreCardButton;
        public GameObject showAllCardsButton;
        public GameObject turnAllCardsButton;
        public GameObject closeButton;

        public List<SingleOpenCardInfo> singleOpenCardInfos = new List<SingleOpenCardInfo>();
        private void Awake() => Instance = this;
        //抽卡,并打开开卡组件
        public void DrawCard() => Command.OpenCardCommand.DrawCard();
        //再次抽取1张卡
        public void DrawOneCardAgain() => Command.OpenCardCommand.DrawCard();
        //再次抽取至多5张卡
        public void DrawMoreCardAgain() => Command.OpenCardCommand.DrawCard();
        //关闭开卡界面
        public void CloseOpenCardsCanve() => Command.OpenCardCommand.CloseOpenCardComponent();

        public void ShowCard(Info.SingleOpenCardInfo info) => Command.OpenCardCommand.ShowCard(info);
        public void TurnCard(Info.SingleOpenCardInfo info) => Command.OpenCardCommand.TurnCard(info);
        public void ShowAllCards() => singleOpenCardInfos.Where(info => info.gameObject.activeSelf).ForEach(Command.OpenCardCommand.ShowCard);
        public void TurnAllCards() => singleOpenCardInfos.Where(info => info.gameObject.activeSelf).ForEach(Command.OpenCardCommand.TurnCard);
    }
}
