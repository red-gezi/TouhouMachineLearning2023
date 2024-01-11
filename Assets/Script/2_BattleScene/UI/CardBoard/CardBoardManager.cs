using UnityEngine;

namespace TouhouMachineLearningSummary.Manager
{
    class CardBoardManager : MonoBehaviour
    {
        public void Hide() => Command.UiCommand.SetCardBoardHide();
        public void Jump() => Command.UiCommand.CardBoardSelectOver();
        public void Show() => Command.UiCommand.SetCardBoardShow();
        public void Close() => Command.UiCommand.SetCardBoardClose();
    }
}
