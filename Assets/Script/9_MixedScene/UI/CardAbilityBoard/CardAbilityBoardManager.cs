using System.Collections.Generic;
using TMPro;
using TouhouMachineLearningSummary.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    //右键卡牌时展现的详情面板
    public class CardAbilityBoardManager : MonoBehaviour
    {
        //需求功能
        //切换上下一张卡牌
        //显示当前卡牌的各种信息
        enum LoadType { FromLibrary, FromCardList, FromCard, FromCardBoard }
        private void Awake() => Manager = this;
        int currentRank { get; set; } = 0;
        Card CurrentGameCard { get; set; }
        LoadType CurrentLoadType { get; set; }
        public static CardAbilityBoardManager Manager { get; set; }
        public Image Texture => transform.GetChild(0).GetChild(1).GetComponent<Image>();
        public TextMeshProUGUI TagText => transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        public TextMeshProUGUI Name => transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();
        public TextMeshProUGUI AbilityText;
        public List<GameObject> refCardModels;
        public GameObject CompositionCardsButton;
        public GameObject DecompositionCardButton;
        public TextMeshProUGUI DescribeText => transform.GetChild(0).GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>();
        public void Show() => transform.GetChild(0).gameObject.SetActive(true);
        public void Close() => transform.GetChild(0).gameObject.SetActive(false);

        public void LoadCardsIdsFromCardList(GameObject cardModel)
        {
            CompositionCardsButton.SetActive(true);
            DecompositionCardButton.SetActive(true);
            CurrentLoadType = LoadType.FromCardList;
            currentRank = Info.PageComponentInfo.deckCardModels.IndexOf(cardModel);
            ChangeIntroduction(Info.PageComponentInfo.distinctCardIds[currentRank]);
            Show();
        }
        /// <summary>
        /// 从展示面板中的"实体"卡牌获得展示数据
        /// </summary>
        /// <param name="cardModel"></param>
        public void LoadCardFromBoardCard(Card card, int rank)
        {
            CompositionCardsButton.SetActive(false);
            DecompositionCardButton.SetActive(false);
            CurrentGameCard = card;
            CurrentLoadType = LoadType.FromCardBoard;
            currentRank = rank;
            ChangeIntroduction(CurrentGameCard);
            Show();
        }
        /// <summary>
        /// 从展示面板中的"虚拟"卡牌获得展示数据
        /// </summary>
        /// <param name="cardModel"></param>
        public void LoadCardFromBoardCard(string cardID,int rank)
        {
            CompositionCardsButton.SetActive(false);
            DecompositionCardButton.SetActive(false);
            CurrentLoadType = LoadType.FromCardBoard;
            currentRank = rank;
            ChangeIntroduction(cardID);
            Show();
        }
        public void LoadCardsIdsFromCardLibrary(GameObject cardModel)
        {
            CompositionCardsButton.SetActive(true);
            DecompositionCardButton.SetActive(true);
            CurrentLoadType = LoadType.FromLibrary;
            currentRank = Info.PageComponentInfo.libraryCardModels.IndexOf(cardModel);
            ChangeIntroduction(Info.PageComponentInfo.LibraryFilterCardList[currentRank].cardID);
            Show();
        }
        /// <summary>
        /// 从游戏中的卡牌获得展示数据
        /// </summary>
        /// <param name="cardModel"></param>
        public void LoadCardFromGameCard(GameObject cardModel)
        {
            CompositionCardsButton.SetActive(false);
            DecompositionCardButton.SetActive(false);
            CurrentGameCard = cardModel.GetComponent<Card>();
            CurrentLoadType = LoadType.FromCard;
            currentRank = CurrentGameCard.BelongCardList.IndexOf(CurrentGameCard);
            ChangeIntroduction(CurrentGameCard);
            Show();
        }
        
        public void LoadLastCardInfo()
        {
            switch (CurrentLoadType)
            {
                case LoadType.FromLibrary:
                    currentRank = Mathf.Max(0, currentRank - 1);
                    ChangeIntroduction(Info.PageComponentInfo.LibraryFilterCardList[currentRank].cardID);
                    break;
                case LoadType.FromCardList:
                    currentRank = Mathf.Max(0, currentRank - 1);
                    ChangeIntroduction(Info.PageComponentInfo.distinctCardIds[currentRank]);
                    break;
                case LoadType.FromCard:
                    currentRank = Mathf.Max(0, currentRank - 1);
                    ChangeIntroduction(CurrentGameCard.BelongCardList[currentRank]);
                    break;
                default:
                    break;
            }
        }
        public void LoadNextCardInfo()
        {
            switch (CurrentLoadType)
            {
                case LoadType.FromLibrary:
                    currentRank = Mathf.Min(Info.PageComponentInfo.LibraryFilterCardList.Count - 1, currentRank + 1);
                    ChangeIntroduction(Info.PageComponentInfo.LibraryFilterCardList[currentRank].cardID);
                    break;
                case LoadType.FromCardList:
                    currentRank = Mathf.Min(Info.PageComponentInfo.distinctCardIds.Count - 1, currentRank + 1);
                    ChangeIntroduction(Info.PageComponentInfo.distinctCardIds[currentRank]);
                    break;
                case LoadType.FromCard:
                    currentRank = Mathf.Min(CurrentGameCard.BelongCardList.Count - 1, currentRank + 1);
                    ChangeIntroduction(CurrentGameCard.BelongCardList[currentRank]);
                    break;
                default:
                    break;
            }
        }
        public void ChangeIntroduction<T>(T target)
        {
            //对当前能力的说明
            string ability = "";
            //对当前状态和字段的说明
            string Introduction = "";
            if (typeof(T) == typeof(string))
            {
                var cardInfo = CardAssemblyManager.GetLastCardInfo((string)(object)target);
                Texture.sprite = cardInfo.cardFace.ToSprite();
                Name.text = cardInfo.TranslateName;
                DescribeText.text = cardInfo.TranslateDescribe;
                ability = cardInfo.TranslateAbility;
                ability = KeyWordManager.ReplaceAbilityKeyWord(ability);
                AbilityText.text = ability;
                TagText.text = cardInfo.TranslateTags;
                //补充直接获取的卡牌信息
                //设置卡牌面板中的关联卡牌
                for (int i = 0; i < 3; i++)
                {
                    bool isActive = cardInfo.refCardIDs.Count > i;
                    refCardModels[i].SetActive(isActive);
                    if (isActive)
                    {
                        var refCardInfo = CardAssemblyManager.GetLastCardInfo(cardInfo.refCardIDs[i]);
                        //设置图片
                        refCardModels[i].transform.GetChild(0).GetComponent<Image>().sprite = refCardInfo.cardFace.ToSprite();
                        //设置名字
                        refCardModels[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = refCardInfo.TranslateName;
                        //设置名字
                        refCardModels[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = refCardInfo.TranslateAbility;
                    }
                }
            }
            else
            {
                Card card = (Card)(object)target;
                Texture.sprite = card.CardFace.ToSprite();
                Name.text = card.TranslateName;
                DescribeText.text = card.TranslateDescribe;
                ability = card.TranslateAbility;
                ability = KeyWordManager.ReplaceAbilityKeyWord(ability);
                AbilityText.text = ability;
                TagText.text = card.TranslateTags;
                //设置卡牌面板中的关联卡牌
                for (int i = 0; i < 3; i++)
                {
                    bool isActive = card.refCardIDs.Count > i;
                    refCardModels[i].SetActive(isActive);
                    if (isActive)
                    {
                        var refCardInfo= CardAssemblyManager.GetLastCardInfo( card.refCardIDs[i]) ;
                        //设置图片
                        refCardModels[i].transform.GetChild(0).GetComponent<Image>().sprite = refCardInfo.cardFace.ToSprite();
                        //设置名字
                        refCardModels[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = refCardInfo.TranslateName;
                        //设置名字
                        refCardModels[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = refCardInfo.TranslateAbility;
                    }
                }
                //IntroductionBackground.gameObject.SetActive(true);
                //IntroductionBackground.sizeDelta = new Vector2(300, ability.Length / 13 * 15 + 100);
            }
            //AbilityBackground.sizeDelta = new Vector2(300, ability.Length / 13 * 15 + 100);
            //修改文本为富文本
        }
    }
}