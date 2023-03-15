using UnityEngine;

namespace TouhouMachineLearningSummary.Control
{
    class CardBoardControl : MonoBehaviour
    {
        public void Hide() => Command.UiCommand.SetCardBoardHide();
        public void Jump() => Command.UiCommand.CardBoardSelectOver();
        public void Show() => Command.UiCommand.SetCardBoardShow();
        public void Close() => Command.UiCommand.SetCardBoardClose();
    }
}
