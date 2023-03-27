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
        public GameObject OpenCardsCanve;
        public List<SingleOpenCardInfo> singleOpenCardInfos = new List<SingleOpenCardInfo>();
        public GameObject drawOneCardButton;
        public GameObject drawMoreCardButton;
        public GameObject brokenAllFaithsButton;
        public GameObject turnAllCardButton;
        public GameObject closeButton;
        private void Awake() => Instance = this;
        public void ShowCard(Info.SingleOpenCardInfo info) => Command.OpenCardCommand.ShowCard(info);

        public void ShowAllCards() => singleOpenCardInfos.Where(info => info.gameObject.activeSelf).ForEach(Command.OpenCardCommand.ShowCard);
        public void TurnCard(Info.SingleOpenCardInfo info) => Command.OpenCardCommand.TurnCard(info);

        public void TurnAllCards() => singleOpenCardInfos.Where(info => info.gameObject.activeSelf).ForEach(Command.OpenCardCommand.TurnCard);
        //显示开卡界面
        public void ShowOpenCardsCanve() => OpenCardsCanve.SetActive(true);
        //关闭开卡界面
        public void CloseOpenCardsCanve() => OpenCardsCanve.SetActive(false);
        //选择抽卡
        public void DrawCard() => Command.OpenCardCommand.DrawCard();
        //再次抽取1张卡
        public void DrawOneCardAgain() => Command.DeckBoardCommand.DeleteDeck();
        //再次抽取至多5张卡
        public void DrawMoreCardAgain() => Command.DeckBoardCommand.DeleteDeck();
    }
}
