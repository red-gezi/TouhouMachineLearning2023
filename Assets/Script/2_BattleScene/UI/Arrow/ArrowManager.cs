using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public class ArrowManager : MonoBehaviour
    {
        Transform StartPos;
        Transform EndPos;
        //箭头指向的卡片
        public Card targetCard;
        void Update() => UpdateState();

        public void InitArrow(Card startCard, GameObject endPoint)
        {
            StartPos = startCard.transform;
            EndPos = endPoint.transform;
            UpdateState();
        }
        public void InitArrow(Card startCard, Card endCard)
        {
            StartPos = startCard.transform;
            EndPos = endCard.transform;
            targetCard = endCard;
            UpdateState();
        }
        private void UpdateState()
        {
            transform.position = (EndPos.position + StartPos.position) / 2;
            if ((StartPos.position - EndPos.position).magnitude > 0)
            {
                transform.forward = StartPos.position - EndPos.position;
                transform.localScale = new Vector3(0.2f, 1, Vector3.Distance(EndPos.position, StartPos.position) / 10);
            }
        }
    }
}