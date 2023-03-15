using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public class PracticeManager : MonoBehaviour
    {
        public void SelectLeader(int leader) => Info.PageComponentInfo.SelectLeader = (PracticeLeader)leader;
        public void SelectFirstHandMode(int firstHangMode) => Info.PageComponentInfo.SelectFirstHandMode = firstHangMode;
    }
}