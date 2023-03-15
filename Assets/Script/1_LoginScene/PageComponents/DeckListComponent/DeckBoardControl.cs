using UnityEngine;

namespace TouhouMachineLearningSummary.Control
{
    //用于展示单人多人页面中的卡组面板
    public class DeckBoardControl : MonoBehaviour
    {
        void Update() => Command.DeckBoardCommand.UpdateDeckPosition();
        public void OnDeckClick(GameObject deck) => Command.DeckBoardCommand.OnDeckClick(deck);
        // public void CreatDeck() => Command.DeckBoardCommand.CreatDeck();
        public void DeleteDeck() => Command.DeckBoardCommand.DeleteDeck();
        public void RenameDeck() => Command.DeckBoardCommand.RenameDeck();
        public void StartAgainst() => _ = Command.DeckBoardCommand.StartAgainstAsync();
    }
}
