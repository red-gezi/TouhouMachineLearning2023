using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class GearControl : MonoBehaviour
    {
        public float x;
        public bool isf;
        void Update()
        {
            if (Time.time % 1 < 0.5f)
            {
                transform.Rotate(transform.forward * (isf ? 1 : -1), x);
            }
        }
    }
}