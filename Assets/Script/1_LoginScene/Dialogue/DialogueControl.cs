using TouhouMachineLearningSummary.Command;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    /// <summary>
    /// 剧情演出控制器
    /// </summary>
    public class DialogueControl : MonoBehaviour
    {
        public void Log() => DialogueCommand.Log();
        public void Skip() => DialogueCommand.Skip();
        public void FastForward() => DialogueCommand.FastForward();
        public void SetBranch(int index) => DialogueCommand.SetBranch(index);
        public async void ShowLastText() => await DialogueCommand.LastDialogue();
        public async void ShowNextText() => await DialogueCommand.NextDialogue();
        [Sirenix.OdinInspector.Button("播放剧情")]
        public async void Play(string tag) => await DialogueCommand.Play(tag, 0);

        private void OnGUI()
        {
            if (GUI.Button(new Rect(100, 100, 100, 100), "播放剧情")) _ = DialogueCommand.Play("test", 0);
            if (GUI.Button(new Rect(200, 100, 100, 100), "上一句话")) _ = DialogueCommand.LastDialogue();
            if (GUI.Button(new Rect(300, 100, 100, 100), "解锁")) TitleCommand.Unlock("1-1");

        }
    }
}