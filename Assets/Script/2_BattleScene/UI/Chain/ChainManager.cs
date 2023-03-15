using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public class ChainManager : MonoBehaviour
    {
        public GameObject chain_model;
        public static GameObject chain;
        static int num = 0;
        private void Awake() => chain = chain_model;
        [Button]
        public static void ShowChainCount()
        {
            Info.AgainstInfo.currentMyChainCount++;
            chain.GetComponent<TextMeshProUGUI>().text = $" x{Info.AgainstInfo.currentMyChainCount}\nChain";
            //chain.GetComponent<TextMeshProUGUI>().outlineColor=Info.AgainstInfo.IsMyTurn ? Color.green : Color.red;
            //chain.GetComponent<TextMeshProUGUI>().material.SetColor("Outline", Info.AgainstInfo.IsMyTurn ? Color.green : Color.red);
            chain.GetComponent<TextMeshProUGUI>().material.SetColor("_OutlineColor", Info.AgainstInfo.IsMyTurn ? Color.green : Color.red);
            chain.GetComponent<CanvasGroup>().alpha = 1;
            chain.GetComponent<RectTransform>().localPosition = Vector3.zero;
            _ = CustomThread.TimerAsync(1, runAction: (process) =>
            {
                chain.GetComponent<RectTransform>().localPosition = Vector3.up*50 * process;
                chain.GetComponent<CanvasGroup>().alpha = 1 - process;
            });
        }
    }
}