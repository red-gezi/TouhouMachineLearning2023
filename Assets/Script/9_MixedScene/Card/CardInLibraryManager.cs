using UnityEngine;
using UnityEngine.EventSystems;

namespace TouhouMachineLearningSummary.Manager
{
    public class CardInLibraryManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,  IPointerClickHandler
    {
        public void OnPointerEnter(PointerEventData eventData) => CardLibraryManager.FocusLibraryCardOnMenu(gameObject);
        public void OnPointerExit(PointerEventData eventData) => CardLibraryManager.LostFocusCardOnMenu();
        public void OnPointerClick(PointerEventData eventData)
        {
            //onPointerLeftUp.Invoke();
            //如果左键单击，视为鼠标左键点击
            if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 1)
            {
                Command.CardListCommand.AddCardFromLibrary(gameObject);
            }
            //如果右键单击或鼠标左键双击，视为鼠标右键点击
            if (eventData.button == PointerEventData.InputButton.Right || (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2))
            {
                CardAbilityBoardManager.Manager.LoadCardsIdsFromCardLibrary(gameObject);
            }
        }
    }
}