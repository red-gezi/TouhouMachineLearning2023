using System.Linq;
using TMPro;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    public class CardLibraryManager : MonoBehaviour
    {
        /// <summary>
        /// 初始化牌库列表
        /// </summary>
        public static void Init()
        {
            //牌库卡牌列表
            Info.PageComponentInfo.LibraryFilterCardList = CardAssemblyManager.LastMultiCardInfos;
            //如果当前是编辑卡组模式，则只显示指定阵营
            if (Info.PageComponentInfo.isEditDeckMode)
            {
                Info.PageComponentInfo.LibraryFilterCardList = Info.PageComponentInfo.LibraryFilterCardList.Where(card => card.cardCamp == Info.PageComponentInfo.selectCamp || card.cardCamp == GameEnum.Camp.Neutral).ToList();
            }
            int libraryCardNumber = Info.PageComponentInfo.LibraryFilterCardList.Count();
            //如果处于卡组编辑状态，则对卡牌列表做个筛选
            //已生成卡牌列表
            //清空所有已销毁卡牌
            Info.PageComponentInfo.libraryCardModels = Info.PageComponentInfo.libraryCardModels.Where(model => model != null).ToList();
            int libraryModelNumber = Info.PageComponentInfo.libraryCardModels.Count;
            Info.PageComponentInfo.libraryCardModels.ForEach(model => model.SetActive(false));
            if (libraryCardNumber > libraryModelNumber)
            {
                for (int i = 0; i < libraryCardNumber - libraryModelNumber; i++)
                {
                    var newCardModel = Instantiate(Info.PageComponentInfo.cardLibraryCardModel, Info.PageComponentInfo.cardLibraryContent.transform);
                    Info.PageComponentInfo.libraryCardModels.Add(newCardModel);
                }
            }
            for (int i = 0; i < libraryCardNumber; i++)
            {
                //卡牌信息集合
                var info = Info.PageComponentInfo.LibraryFilterCardList[i];
                //卡牌对应场景模型
                var newCardModel = Info.PageComponentInfo.libraryCardModels[i];
                //卡牌id
                string cardId = info.cardID.ToString();
                //该id的卡牌持有数量
                int cardNum = GetHasCardNum(cardId);

                newCardModel.name = cardId;
                newCardModel.transform.localScale = Info.PageComponentInfo.cardLibraryCardModel.transform.localScale;
                Sprite cardTex = info.cardFace.ToSprite();
                newCardModel.GetComponent<Image>().sprite = cardTex;
                newCardModel.transform.GetChild(1).GetComponent<Text>().text = info.TranslateName;
                newCardModel.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "X" + (cardNum > 9 ? "9+" : cardNum + "");
                newCardModel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = info.point == 0 ? "" : info.point.ToString();
                newCardModel.GetComponent<Image>().color = new Color(1, 1, 1, cardNum == 0 ? 0.2f : 1);
                newCardModel.SetActive(true);
            }
        }
        public static int GetHasCardNum(string cardId) => Info.PageComponentInfo.IsAdmin ? 3 : Info.AgainstInfo.OnlineUserInfo.CardLibrary.ContainsKey(cardId) ? Info.AgainstInfo.OnlineUserInfo.CardLibrary[cardId] : 0;
        public static void FocusLibraryCardOnMenu(GameObject cardModel)
        {
            CardAbilityPopupManager.focusCardID = CardAssemblyManager.LastMultiCardInfos[Info.PageComponentInfo.libraryCardModels.IndexOf(cardModel)].cardID;
            if (MenuStateCommand.HasState(MenuState.CardLibrary))
            {
                CardDetailCommand.ChangeFocusCard(CardAbilityPopupManager.focusCardID);
                CardAbilityPopupManager.Manager.ChangeIntroduction(CardAbilityPopupManager.focusCardID);
            }
        }
        public void FocusDeckCardOnMenu(GameObject cardModel) => CardAbilityPopupManager.focusCardID = CardAssemblyManager.LastMultiCardInfos[Info.PageComponentInfo.deckCardModels.IndexOf(cardModel)].cardID;
        public static void LostFocusCardOnMenu() => CardAbilityPopupManager.focusCardID = "";
    }
}