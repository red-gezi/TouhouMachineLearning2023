using TouhouMachineLearningSummary.Command;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class NoticeControl : MonoBehaviour
    {
        public void Ok() => _ = NoticeCommand.OkAsync();
        public void Cancel() => _ = NoticeCommand.CancaelAsync();
        public void Input() => _ = NoticeCommand.InputAsync();
        private void Start() => _ = NoticeCommand.CloseAsync();
    }
}