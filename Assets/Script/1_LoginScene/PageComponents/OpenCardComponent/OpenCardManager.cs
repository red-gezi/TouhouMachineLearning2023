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
        public void BrokenFaith(Info.SingleOpenCardInfo info) => Command.OpenCardCommand.FaithBroken(info);

        public void BrokenAllFaiths() => singleOpenCardInfos.Where(info => info.gameObject.activeSelf).ForEach(Command.OpenCardCommand.FaithBroken);
        //显示开卡界面
        public void ShowOpenCardsCanve() => OpenCardsCanve.SetActive(true);
        //关闭开卡界面
        public void CloseOpenCardsCanve() => OpenCardsCanve.SetActive(false);
        //抽取1张卡
        public void DrawOneCard() => Command.DeckBoardCommand.DeleteDeck();
        //抽取至多5张卡
        public void DrawMoreCard() => Command.DeckBoardCommand.DeleteDeck();
    }
}
