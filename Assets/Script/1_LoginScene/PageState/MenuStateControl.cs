using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.Control
{
    class MenuStateControl : MonoBehaviour
    {
        public MenuState state;
        public void JumpToNewMenuState() => Command.MenuStateCommand.AddState(state);
    }
}