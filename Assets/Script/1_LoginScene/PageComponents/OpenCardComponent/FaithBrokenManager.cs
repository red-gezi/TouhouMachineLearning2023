using I18N.Common;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;

namespace TouhouMachineLearningSummary.Control
{
    //用于展示单人多人页面中的卡组面板
    public class FaithBrokenManager : MonoBehaviour
    {
        public GameObject OpenCardsCanve;
        public List<OpenCardManager> openCardManagers = new List<OpenCardManager>();
        public void BrokenAllFaiths() => openCardManagers.Where(manager => manager.gameObject.activeSelf).ForEach(manager => manager.FaithBroken());
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
