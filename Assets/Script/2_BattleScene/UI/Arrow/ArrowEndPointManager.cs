using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public class ArrowEndPointManager : MonoBehaviour
    {
        public float High;
        public float Distance;
        static Ray SceneRay;
        public GameObject ArrowEndPoint;
        void Update()
        {
            SceneRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Distance = Mathf.Abs((Camera.main.transform.position.y - High) / -SceneRay.direction.normalized.y);
            ArrowEndPoint.transform.position = SceneRay.GetPoint(High);
        }
    }
}

