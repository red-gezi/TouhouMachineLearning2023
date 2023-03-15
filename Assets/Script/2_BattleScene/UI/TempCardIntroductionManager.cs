//using I18N.Common;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    public class TempCardIntroductionManager : MonoBehaviour
    {
        public static TempCardIntroductionManager manager;
        public Image cardTexture => transform.GetChild(0).GetComponent<Image>();
        public Text cardAbility => transform.GetChild(1).GetChild(0).GetComponent<Text>();
        Stack<string> introductionCardIds=new Stack<string>();
        bool isShowOver = true;
        private void Awake() => manager = this;
        [Button("´¥·¢")]
        public async void AddCardIntroduciton(string cardID)
        {
            if (introductionCardIds.LastOrDefault() != cardID)
            {
                introductionCardIds.Push(cardID);
            }
            if (isShowOver)
            {
                isShowOver = false;
                await ShowCardIntroduciton();
                isShowOver =true;
            }
        }
        [Button("²âÊÔ")]
        public void Test()
        {
            AddCardIntroduciton("M_S0_1G_002");
            AddCardIntroduciton("M_T1_3C_002");
            AddCardIntroduciton("M_T1_3C_003");
        }
        public async Task ShowCardIntroduciton()
        {
            if (introductionCardIds.TryPop(out var cardID))
            {
                gameObject.SetActive(true);
                var CardStandardInfo = CardAssemblyManager.GetCurrentCardInfos(cardID);
                cardTexture.sprite = CardStandardInfo.GetCardSprite();
                cardAbility.text = CardStandardInfo.TranslateAbility;
                await CustomThread.TimerAsync(0.3f, process =>
                {
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(new Vector3(0, 20, 0)), process);
                    transform.GetComponent<CanvasGroup>().alpha = process;
                });
                await Task.Delay(5000);
                await CustomThread.TimerAsync(0.3f, process =>
                {
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(new Vector3(0, 130, 0)), process);
                    transform.GetComponent<CanvasGroup>().alpha = 1 - process;
                });
                await Task.Delay(1000);
                await ShowCardIntroduciton();
            }
        }
    }
}