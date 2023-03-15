using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
using UnityEngine.UI;
using static TouhouMachineLearningSummary.Info.CampInfo;

namespace TouhouMachineLearningSummary.Command
{
    class CampSelectCommand
    {
        public static void InitCamp()
        {
            Info.PageComponentInfo.Instance.selectCampOverBtn.SetActive(true);
            Info.PageComponentInfo.Instance.rebackBtn.SetActive(true);
            Info.PageComponentInfo.Instance.selectLeaderOverBtn.SetActive(false);
            Info.PageComponentInfo.Instance.lastStepBtn.SetActive(false);
            Info.PageComponentInfo.selectCardModels.ForEach(Object.Destroy);
            Info.PageComponentInfo.selectCardModels.Clear();
            //初始化信息来源
            Info.CampInfo.campInfos.Clear();
            Info.CampInfo.campInfos.Add(new(Camp.Taoism, "道教", "没有东西的空架子哦", Info.PageComponentInfo.Instance.TaoismTex));
            Info.CampInfo.campInfos.Add(new(Camp.Technology, "科学", "没有东西的空架子哦", Info.PageComponentInfo.Instance.scienceTex));
            Info.CampInfo.campInfos.Add(new(Camp.Buddhism, "佛教", "没有东西的空架子哦", Info.PageComponentInfo.Instance.BuddhismTex));
            Info.CampInfo.campInfos.Add(new(Camp.Shintoism, "神道教", "没有东西的空架子哦", Info.PageComponentInfo.Instance.ShintoismTex));
            Info.CampInfo.campInfos.Add(new(Camp.Neutral, "中立", "请选择一个阵营", Info.PageComponentInfo.Instance.NeutralTex));

            //根据实际阵营数量来生成模型
            for (int i = 0; i < Info.CampInfo.campInfos.Count - 1; i++)
            {
                var newCampModel = Object.Instantiate(Info.PageComponentInfo.Instance.CampModel, Info.PageComponentInfo.Instance.modelContent.transform);
                Info.PageComponentInfo.selectCardModels.Add(newCampModel);
            }
            //设置每个卡牌的属性
            for (int i = 0; i < Info.CampInfo.campInfos.Count - 1; i++)
            {
                //卡牌信息集合
                var info = Info.CampInfo.campInfos[i];
                //卡牌对应场景模型
                var s = Info.PageComponentInfo.selectCardModels;
                var newCardModel = Info.PageComponentInfo.selectCardModels[i];
                newCardModel.name = info.campName;
                newCardModel.transform.localScale = Info.PageComponentInfo.Instance.CampModel.transform.localScale;
                newCardModel.transform.GetChild(0).GetComponent<Image>().sprite = info.campTex;
                newCardModel.transform.GetChild(2).GetComponent<Text>().text = info.campName;
                newCardModel.SetActive(true);
            }
        }
        public static void InitLeader()
        {
            Info.PageComponentInfo.Instance.selectCampOverBtn.SetActive(false);
            Info.PageComponentInfo.Instance.rebackBtn.SetActive(false);
            Info.PageComponentInfo.Instance.selectLeaderOverBtn.SetActive(true);
            Info.PageComponentInfo.Instance.lastStepBtn.SetActive(true);
            Info.PageComponentInfo.selectCardModels.ForEach(Object.Destroy);
            Info.PageComponentInfo.selectCardModels.Clear();
            Info.CampInfo.leaderInfos = Manager.CardAssemblyManager
                    .LastMultiCardInfos
                    .Where(card => card.cardRank == CardRank.Leader)
                    .Where(card => card.cardCamp == Info.PageComponentInfo.selectCamp || card.cardCamp == Camp.Neutral)
                    .ToList();

            //根据实际领袖数量来生成模型
            for (int i = 0; i < leaderInfos.Count(); i++)
            {
                var newCampModel = Object.Instantiate(Info.PageComponentInfo.Instance.LeaderModel, Info.PageComponentInfo.Instance.modelContent.transform);
                Info.PageComponentInfo.selectCardModels.Add(newCampModel);
            }
            //设置每个卡牌的属性
            for (int i = 0; i < leaderInfos.Count(); i++)
            {
                //卡牌信息集合
                var info = leaderInfos[i];
                //卡牌对应场景模型
                var newCardModel = Info.PageComponentInfo.selectCardModels[i];
                newCardModel.name = info.TranslateName;
                newCardModel.transform.localScale = Info.PageComponentInfo.Instance.CampModel.transform.localScale;
                newCardModel.transform.GetChild(0).GetComponent<Image>().sprite = info.GetCardSprite();
                newCardModel.transform.GetChild(2).GetComponent<Text>().text = info.TranslateName;
                newCardModel.SetActive(true);
            }
        }
        //左侧显焦点阵营
        public static void FocusCamp(GameObject campModel)
        {
            Info.PageComponentInfo.isCampIntroduction = true;
            int selectRank = Info.PageComponentInfo.selectCardModels.IndexOf(campModel);
            Info.PageComponentInfo.focusCamp = Info.CampInfo.campInfos[selectRank].camp;
            Command.CardDetailCommand.ChangeFocusCamp();
        }
        //确定所选阵营
        public static void SelectCamp(GameObject campModel)
        {
            int selectRank = Info.PageComponentInfo.selectCardModels.IndexOf(campModel);
            Info.PageComponentInfo.selectCamp = Info.CampInfo.campInfos[selectRank].camp;
        }
        //左侧变更会所选阵营
        public static void LostFocusCamp()
        {
            Info.PageComponentInfo.focusCamp = Info.PageComponentInfo.selectCamp;
            Command.CardDetailCommand.ChangeFocusCamp();
        }

        //左侧显焦点领袖
        public static void FocusLeader(GameObject campModel)
        {
            Info.PageComponentInfo.isCampIntroduction = true;
            int selectRank = Info.PageComponentInfo.selectCardModels.IndexOf(campModel);
            Info.PageComponentInfo.FocusLeaderID = Info.CampInfo.leaderInfos[selectRank].cardID;
            Command.CardDetailCommand.ChangeFocusLeader();
        }
        //选择对应阵营的领袖
        public static void SelectLeader(GameObject campModel)
        {
            int selectRank = Info.PageComponentInfo.selectCardModels.IndexOf(campModel);
            Info.PageComponentInfo.SelectLeaderID = Info.CampInfo.leaderInfos[selectRank].cardID;
        }
        //左侧变更会所选领袖
        public static void LostFocusLeader()
        {
            Info.PageComponentInfo.FocusLeaderID = Info.PageComponentInfo.SelectLeaderID;
            Command.CardDetailCommand.ChangeFocusLeader();
        }
    }
}
