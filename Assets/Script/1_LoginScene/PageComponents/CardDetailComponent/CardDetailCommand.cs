using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Command
{
    class CardDetailCommand
    {
        public static void ChangeFocusCard(string cardID)
        {
            if (cardID == "")
            {
                Info.PageComponentInfo.targetCardName.GetComponent<Text>().text = "";
                Info.PageComponentInfo.targetCardTag.GetComponent<Text>().text = "";
                Info.PageComponentInfo.targetCardAbility.GetComponent<Text>().text = "";
            }
            else
            {
                var cardInfo = Manager.CardAssemblyManager.GetLastCardInfo(cardID);
                Info.PageComponentInfo.targetCardTexture.GetComponent<Image>().sprite = cardInfo.GetCardSprite();
                Info.PageComponentInfo.targetCardName.GetComponent<Text>().text = cardInfo.TranslateName;
                Info.PageComponentInfo.targetCardTag.GetComponent<Text>().text = cardInfo.TranslateTags;
                Info.PageComponentInfo.targetCardAbility.GetComponent<Text>().text = cardInfo.TranslateAbility;
            }
        }
        public static void ChangeFocusCamp()
        {
            var campInfo = Info.CampInfo.GetCampInfo(Info.PageComponentInfo.focusCamp);
            Info.PageComponentInfo.targetCardTexture.GetComponent<Image>().sprite = campInfo.campTex;
            Info.PageComponentInfo.targetCardName.GetComponent<Text>().text = campInfo.campName;
            Info.PageComponentInfo.targetCardTag.GetComponent<Text>().text = "强化 反制";
            Info.PageComponentInfo.targetCardAbility.GetComponent<Text>().text = campInfo.campIntroduction;
        }
        public static void ChangeFocusLeader()
        {
            var leaderInfo = Info.CampInfo.GetLeaderInfo(Info.PageComponentInfo.FocusLeaderID);
            Info.PageComponentInfo.targetCardTexture.GetComponent<Image>().sprite = leaderInfo.GetCardSprite();
            Info.PageComponentInfo.targetCardName.GetComponent<Text>().text = leaderInfo.TranslateName;
            Info.PageComponentInfo.targetCardTag.GetComponent<Text>().text = leaderInfo.TranslateTags;
            Info.PageComponentInfo.targetCardAbility.GetComponent<Text>().text = leaderInfo.TranslateAbility;
        }
    }
}