using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Control
{
    public class BookTagControl : MonoBehaviour
    {
        public Text TagText;
        public MenuState toMenuState;
        static bool isCoolDown = true;
        public void Init(string tagText)
        {
            TagText.text = string.Join("\n", tagText.ToCharArray());
        }
        private async void OnMouseDown()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (isCoolDown)
                {
                    isCoolDown = false;
                    _ = Task.Run(async () =>
                        {
                            await Task.Delay(1000);
                            isCoolDown = true;
                        });
                    Command.MenuStateCommand.ChangeToMainPage(toMenuState);
                }
            }
        }
    }
}