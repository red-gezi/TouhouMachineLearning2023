using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class CardListControl : MonoBehaviour
    {
        public void SaveDeck() => Command.CardListCommand.SaveDeck();
        public void CancelDeck() => Command.CardListCommand.CancelDeck();
        public void RenameDeck() => Command.CardListCommand.RenameDeck();
        public void FocusDeckCardOnMenu(GameObject cardModel) => Command.CardListCommand.FocusDeckCardOnMenu(cardModel);
        public void LostFocusCardOnMenu() => Command.CardListCommand.LostFocusCardOnMenu();
        public void AddCardFromLibrary(GameObject clickCard) => Command.CardListCommand.AddCardFromLibrary(clickCard);
        public void RemoveCardFromDeck(GameObject clickCard) => Command.CardListCommand.RemoveCardFromDeck(clickCard);
    }
}