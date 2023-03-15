using Sirenix.OdinInspector;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class PlayerDataControl : MonoBehaviour
    {
        public void ShowPlayerDataCanve() => Command.PlayerDataCommand.ShowPlayerDataCanve(Info.AgainstInfo.OnlineUserInfo.UID);
        public void ClosePlayerDataCanve() => Command.PlayerDataCommand.ClosePlayerDataCanve();
        public void ShowCanve(GameObject targetCanves) => Command.PlayerDataCommand.ShowCanve(targetCanves);
        public void CloseCanve() => Command.PlayerDataCommand.CloseCanve();
        public void SaveEdit(GameObject targetCanves) => Command.PlayerDataCommand.SaveEdit( targetCanves);
        public void SaveTitle(GameObject item) => Command.PlayerDataCommand.SaveTitle(item);
        [Button("¼¤»î³ÆºÅ")]
        public void Unlock(string tag) => Command.TitleCommand.Unlock(tag);
    }
}

