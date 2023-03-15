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
        void Update()
        {
            GetFocusTarget();
            MouseEvent();
            KeyBoardEvent();
        }
        private void GetFocusTarget()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] Infos = Physics.RaycastAll(ray);
            if (Infos.Length > 0)
            {
                for (int i = 0; i < Infos.Length; i++)
                {
                    if (Infos[i].transform.GetComponent<RowManager>() != null)
                    {
                        AgainstInfo.PlayerFocusRegion = Infos[i].transform.GetComponent<RowManager>();
                        AgainstInfo.FocusPoint = Infos[i].point;
                        break;
                    }
                    AgainstInfo.PlayerFocusRegion = null;
                }
            }
            float distance = (height - ray.origin.y) / ray.direction.y;
            AgainstInfo.dragToPoint = ray.GetPoint(distance);
            Debug.DrawLine(ray.origin, AgainstInfo.dragToPoint, Color.red);
            Debug.DrawRay(ray.origin, ray.direction, Color.white);
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
                    AgainstInfo.SelectRowRank = AgainstInfo.PlayerFocusRegion.RowRank;
                }
                //处理选择单位的箭头
                if (AgainstInfo.IsWaitForSelectUnits && AgainstInfo.PlayerFocusCard != null && !AgainstInfo.PlayerFocusCard.IsGray)
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
                    if (AgainstInfo.PlayerFocusRegion != null && AgainstInfo.PlayerFocusRegion.CanBeSelected)
                    {
                        AgainstInfo.SelectRowRank = AgainstInfo.PlayerFocusRegion.RowRank;
                        AgainstInfo.SelectRank = AgainstInfo.PlayerFocusRegion.Rank;
                    }
                }
            }
        }
    }
}