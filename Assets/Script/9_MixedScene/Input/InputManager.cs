using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using UnityEngine;

namespace TouhouMachineLearningSummary.Manager
{
    public class InputManager : MonoBehaviour
    {
        public float height;
        Ray ray;
        public float PassPressTime;
        Camera camera;
        private void Awake()
        {
            camera = Camera.main;
        }
        void Update()
        {
            GetFocusTarget();
            MouseEvent();
            KeyBoardEvent();
        }
        private void GetFocusTarget()
        {
            ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] infos = Physics.RaycastAll(ray);
            bool isFocusRow = false;
            foreach (RaycastHit hit in infos)
            {
                GameObject hitObject = hit.transform.gameObject;
                RowInfo rowInfo = RowCommand.GetRowInfo(hitObject);
                if (rowInfo != null)
                {
                    AgainstInfo.PlayerFocusRow = rowInfo;
                    AgainstInfo.FocusPoint = hit.point;
                    isFocusRow = true;
                    break;
                }
            }
            RowCommand.RefreshTempCard();
            AgainstInfo.PlayerFocusRow = isFocusRow ? AgainstInfo.PlayerFocusRow : null;
            float distance = (height - ray.origin.y) / ray.direction.y;
            AgainstInfo.dragToPoint = ray.GetPoint(distance);
#if UNITY_EDITOR
            Debug.DrawLine(ray.origin, AgainstInfo.dragToPoint, Color.red);
            Debug.DrawRay(ray.origin, ray.direction, Color.white);
#endif
        }
        private void KeyBoardEvent()
        {

            if (Input.GetKey(KeyCode.Space) && AgainstInfo.IsMyTurn)
            {
                PassPressTime += Time.deltaTime;
                if (PassPressTime > 2)
                {
                    NetCommand.AsyncInfo(NetAcyncType.Pass);
                    AgainstInfo.IsPlayerPass = true;
                    PassPressTime = 0;
                }
            }
            if (Input.GetKeyUp(KeyCode.Space) && AgainstInfo.IsMyTurn)
            {
                PassPressTime = 0;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _ = NoticeCommand.ShowAsync("确认投降？", okAction: StateCommand.Surrender);
            }
        }
        private void MouseEvent()
        {
            if (Input.GetMouseButtonDown(0) && AgainstInfo.IsMyTurn)
            {
                if (AgainstInfo.IsWaitForSelectRegion)
                {
                    AgainstInfo.SelectRowRank = AgainstInfo.PlayerFocusRow.RowRank;
                }
                //处理选择单位的箭头
                if (AgainstInfo.IsWaitForSelectUnits && AgainstInfo.PlayerFocusCard != null && !AgainstInfo.PlayerFocusCard.IsTemp)
                {
                    Card playerFocusCard = AgainstInfo.PlayerFocusCard;
                    if (!AgainstInfo.SelectUnits.Contains(playerFocusCard))
                    {
                        AgainstInfo.SelectUnits.Add(playerFocusCard);
                        UiCommand.CreatFixedArrow(playerFocusCard);
                    }
                    else
                    {
                        AgainstInfo.SelectUnits.Remove(playerFocusCard);
                        UiCommand.DestoryFixedArrow(playerFocusCard);
                    }
                }
                if (AgainstInfo.IsWaitForSelectLocation)
                {
                    if (AgainstInfo.PlayerFocusRow != null && AgainstInfo.PlayerFocusRow.CanBeSelected)
                    {
                        AgainstInfo.SelectRowRank = AgainstInfo.PlayerFocusRow.RowRank;
                        AgainstInfo.SelectRank = AgainstInfo.PlayerFocusRow.FocusRank;
                    }
                }
            }
        }
    }
}