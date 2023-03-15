using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    public class PointManager : MonoBehaviour
    {
        public int DownShowPoint = 0;
        public int UpShowPoint = 0;
        public Text MyPoint;
        public Text OpPoint;
        void Update()
        {
            if (DownShowPoint != Info.AgainstInfo.TotalDownPoint)
            {
                DownShowPoint = Info.AgainstInfo.TotalDownPoint;

                MyPoint.text = $"<color=yellow>{DownShowPoint}</color>";
                MyPoint.transform.localScale = Vector3.one * 1.5f;
                Invoke("Reset", 1);
            }
            if (UpShowPoint != Info.AgainstInfo.TotalUpPoint)
            {
                UpShowPoint = Info.AgainstInfo.TotalUpPoint;
                OpPoint.text = $"<color=yellow>{UpShowPoint}</color>";
                OpPoint.transform.localScale = Vector3.one * 1.5f;
                Invoke("Reset", 1);
            }
        }
        private void Reset()
        {
            MyPoint.transform.localScale = Vector3.one;
            OpPoint.transform.localScale = Vector3.one;
        }
    }
}