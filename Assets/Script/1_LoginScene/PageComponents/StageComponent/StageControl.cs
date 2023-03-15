using UnityEngine;

namespace TouhouMachineLearningSummary.Control
{
    public class StageControl : MonoBehaviour
    {
        //选择欲加载关卡
        public void SelectStage(string tag) => Command.StageCommand.SelectStage(tag);
        //选择欲加载关卡阶段
        public void SelectStep(GameObject stageButton)
        {
            //名字按0，1，2.。。取名
            Command.StageCommand.SelectStep(int.Parse(stageButton.name));
            //Info.PageCompnentInfo.CurrentSelectStageTag = stageButton.name;
        }
    }
}
