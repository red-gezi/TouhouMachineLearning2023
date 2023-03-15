using TMPro;
using TouhouMachineLearningSummary.Manager;

namespace TouhouMachineLearningSummary.Command
{
    internal class StageCommand
    {
        public static void Init()
        {
            SelectStage("1");
            //地图渐入

        }
        public static void SelectStage(string Stage)
        {
            //获得标签对应的所有阶段的关卡信息
            Info.PageComponentInfo.CurrentSelectStageInfos = Stage.TranslationStageText();
            Info.PageComponentInfo.CurrentStage = Stage;
            //获得玩家的线上进度
            int rank = Info.AgainstInfo.OnlineUserInfo.GetStage(Stage);
            //控制右侧阶段信息的显示
            var content = Info.PageComponentInfo.Instance.stageModel.transform.parent;
            //当前已有的小关ui数量
            int stageStepCurrentCount = content.childCount;
            //当前应该有的小关ui数量
            int stageStepMaxCount = Info.PageComponentInfo.CurrentSelectStageInfos.Count;
            var stageModel = Info.PageComponentInfo.Instance.stageModel;
            //生成对应数量的小关

            for (int i = stageStepCurrentCount; i < stageStepMaxCount; i++)
            {
                UnityEngine.Object.Instantiate(stageModel, content);
            }
            stageStepCurrentCount = content.childCount;
            //前n位修改名称和可见性和锁定未解锁关卡
            for (int i = 0; i < stageStepCurrentCount; i++)
            {
                content.GetChild(i).name = i.ToString();
                if (i < stageStepMaxCount)
                {
                    content.GetChild(i).gameObject.SetActive(true);
                    content.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = Info.PageComponentInfo.CurrentSelectStageInfos[i].StageName;
                }
                else
                {
                    content.GetChild(i).gameObject.SetActive(false);
                }
            }
            //默认点开初始
            SelectStep(0);
        }
        public static void SelectStep(int step)
        {

            if (Info.PageComponentInfo.CurrentSelectStageInfos.Count > step)
            {
                Info.PageComponentInfo.CurrentStep = step;
                var targetStageInfo = Info.PageComponentInfo.CurrentSelectStageInfos[step];
                //控制左侧领袖信息的显示
                Info.PageComponentInfo.Instance.leaderSprite.sprite = targetStageInfo.LeadSprite;
                Info.PageComponentInfo.Instance.leaderName.text = targetStageInfo.LeaderName;
                Info.PageComponentInfo.Instance.leaderNick.text = targetStageInfo.LeaderNick;
                Info.PageComponentInfo.Instance.leaderIntroduction.text = targetStageInfo.LeaderIntroduction;
                //控制下侧关卡信息的显示
                Info.PageComponentInfo.Instance.stageIntroduction.text = targetStageInfo.StageIntroduction;
            }

        }
    }
}


