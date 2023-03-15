using UnityEngine;

namespace TouhouMachineLearningSummary.Control
{
    public class CampSelectControl : MonoBehaviour
    {
        public void Reback() => Command.MenuStateCommand.RebackStare();
        public void LastStep() => Command.CampSelectCommand.InitCamp();

        public void FocusCamp(GameObject campModel) => Command.CampSelectCommand.FocusCamp(campModel);
        public void LostFocusCamp() => Command.CampSelectCommand.LostFocusCamp();

        public void FocusLeader(GameObject campModel) => Command.CampSelectCommand.FocusLeader(campModel);
        public void LostFocusLeader() => Command.CampSelectCommand.LostFocusLeader();
        //选择阵营
        public void SelectCamp(GameObject model) => Command.CampSelectCommand.SelectCamp(model);
        //选择领袖
        public void SelectLeader(GameObject model) => Command.CampSelectCommand.SelectLeader(model);
        //选择阵营完毕
        public void SelectCampOver() => Command.CampSelectCommand.InitLeader();
        //选择完毕
        public void SelectLeaderOver() => Command.DeckBoardCommand.CreatDeck("1000001");

    }
}