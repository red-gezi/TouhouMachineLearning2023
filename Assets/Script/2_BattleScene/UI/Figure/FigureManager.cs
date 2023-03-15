using Sirenix.OdinInspector;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    public class FigureManager : MonoBehaviour
    {
        public static FigureManager Instance;
        public GameObject MyFigure;
        Transform myBack => MyFigure.transform.GetChild(0);
        Transform myImage => MyFigure.transform.GetChild(1);
        Transform myText => MyFigure.transform.GetChild(2);
        public GameObject OpFigure;
        private void Awake() => Instance = this;
        [Button]
        public async Task ShowFigureAsync(bool isMy, string text)
        {
            myText.GetComponent<Text>().text = text;
            if (isMy)
            {
                MyFigure.SetActive(true);
                await CustomThread.TimerAsync(0.4f, runAction: (process) =>
                {
                    GetComponent<CanvasGroup>().alpha = process;
                    myBack.transform.localPosition = new Vector3(Mathf.Cos(process * 6.28f), Mathf.Sin(process * 0.628f), 0) * 10;
                    myImage.transform.localPosition = new Vector3(-200 - (1 - process) * 400, 50, 0) + new Vector3(-Mathf.Cos(process * 6.28f), Mathf.Sin(process * 6.28f), 0) * 20;
                    myText.transform.localPosition = new Vector3(400 + (1 - process) * 400, -400, 0) + new Vector3(-Mathf.Cos(process * 6.28f), Mathf.Sin(process * 6.28f), 0) * 20;
                });
                await CustomThread.TimerAsync(1f, runAction: (process) =>
                {
                    myBack.transform.localPosition = new Vector3(Mathf.Cos(process * 6.28f), Mathf.Sin(process * 0.628f), 0) * 10;
                    myImage.transform.localPosition = new Vector3(-200, 50, 0) + new Vector3(-Mathf.Cos(process * 6.28f), Mathf.Sin(process * 0.628f), 0) * 20;
                    myText.transform.localPosition = new Vector3(400, -400, 0) + new Vector3(Mathf.Cos(process * 6.28f), Mathf.Sin(process * 6.28f), 0) * 10;
                });
                await CustomThread.TimerAsync(0.2f, runAction: (process) =>
                {
                    GetComponent<CanvasGroup>().alpha = 1 - process;
                    myBack.transform.localPosition = new Vector3(Mathf.Cos(process * 6.28f), Mathf.Sin(process * 0.628f), 0) * 10;
                    myImage.transform.localPosition = new Vector3(-200 - process * 400, 50, 0) + new Vector3(-Mathf.Cos(process * 6.28f), Mathf.Sin(process * 6.28f), 0) * 20;
                    myText.transform.localPosition = new Vector3(400 + process * 400, -400, 0) + new Vector3(-Mathf.Cos(process * 6.28f), Mathf.Sin(process * 6.28f), 0) * 20;

                });
            }
            else
            {
                OpFigure.SetActive(true);
            }
        }
    }
}
