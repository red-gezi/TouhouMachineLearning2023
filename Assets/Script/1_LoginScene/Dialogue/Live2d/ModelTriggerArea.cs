using UnityEngine;
namespace TouhouMachineLearningSummary.Dialogue
{
    public class ModelTriggerArea : MonoBehaviour
    {
        public bool isClickDown = false;
        public bool isClickUp = false;
        public float mouseX = 0;
        public float mouseY = 0;
        private void OnMouseDown() => isClickDown = true;
        private void OnMouseUp() => isClickDown = false;
        private void OnMouseDrag()
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }
    }
}