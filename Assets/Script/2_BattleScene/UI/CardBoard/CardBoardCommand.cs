using System.Collections.Generic;
using System.Linq;
using TMPro;
using TouhouMachineLearningSummary.Control;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Command
{
    public class CardBoardCommand
    {

        public static void ShowCardBoard<T>(List<T> cardInfos, CardBoardMode mode, BoardCardVisible cardVisable= BoardCardVisible.FromCard)
        {
            UiInfo.ShowCardLIstOnBoard.ForEach(Object.Destroy);
            if (typeof(T) == typeof(Card))
            {
                List<Card> Cards=null;
                //若为临时展开玩家的牌组墓地面板，则不记录相关展开数据,关闭后直接清空，否则进行保留，下次再次打开时恢复
                if (mode== CardBoardMode.Temp)
                {
                    if (cardInfos!=null)
                    {
                        Info.AgainstInfo.tempCardBoardList = cardInfos.Cast<Card>().ToList();
                    }
                    Cards = Info.AgainstInfo.tempCardBoardList;
                }
                else
                {
                    if (cardInfos != null)
                    {
                        Info.AgainstInfo.cardBoardList = cardInfos.Cast<Card>().ToList();
                    }
                    Cards = Info.AgainstInfo.cardBoardList;
                }
                for (int i = 0; i < Cards.Count; i++)
                {
                    var CardStandardInfo = CardAssemblyManager.GetCurrentCardInfos(Cards[i].CardID);

                    GameObject boardCard = Object.Instantiate(UiInfo.Instance.BoardCard, UiInfo.Instance.CardBoardContent);
                    boardCard.SetActive(true);
                    //设置面板卡牌对应立绘或卡背
                    //如果是临时对墓地和牌库浏览模式，则全部正面显示
                    //如果是对方回合展示模式，则根据卡牌可见性进行正背面区分
                    //如果是我方回合选择/换牌模式,则默认正面，可以特殊配置正背面
                    Texture2D texture = null;
                    if (mode == CardBoardMode.Temp || cardVisable == BoardCardVisible.AlwaysShow)
                    {
                        texture = CardStandardInfo.cardFace;
                    }
                    if (cardVisable == BoardCardVisible.AlwaysHide)
                    {
                        texture = CardStandardInfo.cardBack;
                    }
                    if (cardVisable == BoardCardVisible.FromCard)
                    {
                        texture = Cards[i].Manager.IsCardVisible ? CardStandardInfo.cardFace : CardStandardInfo.cardBack;
                    }
                    boardCard.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    //设置效果文本
                    boardCard.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = Cards[i].TranslateAbility;
                    //设置点数
                    boardCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Cards[i].ShowPoint == 0 ? "" : Cards[i].ShowPoint.ToString();
                    //设置名字
                    boardCard.name = CardStandardInfo.TranslateName;
                    //设置相关信息
                    boardCard.GetComponent<CardInSelectBoardManager>().Rank = i;
                    boardCard.GetComponent<CardInSelectBoardManager>().IsCanSee = Cards[i].Manager.IsCardVisible;
                    boardCard.GetComponent<CardInSelectBoardManager>().SetTargetCard(Cards[i]);
                    //设置品质
                    boardCard.GetComponent<Image>().color = CardStandardInfo.cardRank switch
                    {
                        CardRank.Leader => Color.cyan,
                        CardRank.Gold => new Color(1, 1, 0, 1),
                        CardRank.Silver => new Color(1, 1, 1, 1),
                        CardRank.Copper => new Color(1, 0.5f, 0, 1),
                        _ => Color.cyan,
                    };
                    UiInfo.ShowCardLIstOnBoard.Add(boardCard);
                }
                UiInfo.Instance.CardBoardContent.GetComponent<RectTransform>().sizeDelta = new Vector2(Cards.Count * 275, UiInfo.Instance.CardBoardContent.GetComponent<RectTransform>().sizeDelta.y);
            }
            else
            {
                Info.AgainstInfo.cardBoardIDList = cardInfos.Cast<string>().ToList();
                List<string> CardIds = Info.AgainstInfo.cardBoardIDList;
                for (int i = 0; i < CardIds.Count; i++)
                {
                    var CardStandardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(CardIds[i]);
                    GameObject boardCard = Object.Instantiate(UiInfo.Instance.BoardCard, UiInfo.Instance.CardBoardContent);
                    boardCard.SetActive(true);
                    //设置对应立绘
                    Texture2D texture = CardStandardInfo.cardFace;
                    boardCard.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    //设置效果文本
                    boardCard.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = CardStandardInfo.TranslateAbility;
                    //设置点数
                    boardCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = CardStandardInfo.point == 0 ? "" : CardStandardInfo.point.ToString();
                    //设置名字
                    boardCard.name = CardStandardInfo.TranslateName;
                    //设置相关信息
                    boardCard.GetComponent<CardInSelectBoardManager>().Rank = i;
                    //boardCard.GetComponent<CardInSelectBoardManager>().IsCanSee = CardStandardInfo.IsCanSee;
                    boardCard.GetComponent<CardInSelectBoardManager>().SetTargetCard(CardIds[i]);
                    //设置品质
                    boardCard.GetComponent<Image>().color = CardStandardInfo.cardRank switch
                    {
                        CardRank.Leader => Color.cyan,
                        CardRank.Gold => Color.yellow,
                        CardRank.Silver => Color.gray,
                        CardRank.Copper => Color.magenta,
                        _ => Color.cyan,
                    };
                    UiInfo.ShowCardLIstOnBoard.Add(boardCard);
                    UiInfo.Instance.CardBoardContent.GetComponent<RectTransform>().sizeDelta = new Vector2(CardIds.Count * 275, UiInfo.Instance.CardBoardContent.GetComponent<RectTransform>().sizeDelta.y);
                }
            }
        }







        //public static void LoadBoardCardList(List<string> cardIds, CardBoardMode mode, BoardCardVisible cardVisable)
        //{
        //    Info.AgainstInfo.cardBoardIDList = cardIds;
        //    CreatBoardCardVitual(mode);
        //}
        //public static void LoadBoardCardList(List<Card> cards, CardBoardMode mode, BoardCardVisible cardVisable)
        //{
        //    Info.AgainstInfo.cardBoardList = cards;
        //    CreatBoardCardActual(mode, false);
        //}
        /// <summary>
        /// 临时加载是单独区分存储的列表，不与之前的加载互相干涉
        /// </summary>
        /// <param name="cards"></param>
        //public static void LoadTempBoardCardList(List<Card> cards)
        //{
        //    Info.AgainstInfo.tempCardBoardList = cards;
        //    CreatBoardCardActual(CardBoardMode.Temp, true);
        //}
        ////生成对局存在的卡牌
        //public static void CreatBoardCardActual(GameEnum.CardBoardMode mode, bool isTemp = false)
        //{
        //    UiInfo.ShowCardLIstOnBoard.ForEach(Object.Destroy);
        //    //若为临时展开玩家的牌组墓地面板，则不记录相关展开数据,关闭后直接清空，否则进行保留，下次再次打开时恢复
        //    List<Card> Cards = isTemp ? Info.AgainstInfo.tempCardBoardList : Info.AgainstInfo.cardBoardList;
        //    for (int i = 0; i < Cards.Count; i++)
        //    {
        //        var CardStandardInfo = CardAssemblyManager.GetCurrentCardInfos(Cards[i].CardID);

        //        GameObject boardCard = Object.Instantiate(UiInfo.Instance.BoardCard, UiInfo.Instance.CardBoardContent);
        //        boardCard.SetActive(true);
        //        //设置面板卡牌对应立绘或卡背
        //        //如果是临时对墓地和牌库浏览模式，则全部正面显示
        //        //如果是对方回合展示模式，则根据卡牌可见性进行正背面区分
        //        //如果是我方回合选择/换牌模式,则默认正面，可以特殊配置正背面
        //        Texture2D texture = mode switch
        //        {
        //            CardBoardMode.Temp => CardStandardInfo.cardFace,
        //            _ => Cards[i].Manager.IsCardVisible ? CardStandardInfo.cardFace : CardStandardInfo.cardFace,

        //        };
        //        boardCard.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        //        //设置效果文本
        //        boardCard.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = Cards[i].TranslateAbility;
        //        //设置点数
        //        boardCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Cards[i].ShowPoint == 0 ? "" : Cards[i].ShowPoint.ToString();
        //        //设置名字
        //        boardCard.name = CardStandardInfo.TranslateName;
        //        //设置相关信息
        //        boardCard.GetComponent<CardInSelectBoardManager>().Rank = i;
        //        boardCard.GetComponent<CardInSelectBoardManager>().IsCanSee = Cards[i].Manager.IsCardVisible;
        //        boardCard.GetComponent<CardInSelectBoardManager>().SetTargetCard(Cards[i]);
        //        //设置品质
        //        boardCard.GetComponent<Image>().color = CardStandardInfo.cardRank switch
        //        {
        //            CardRank.Leader => Color.cyan,
        //            CardRank.Gold => new Color(1, 1, 0, 1),
        //            CardRank.Silver => new Color(1, 1, 1, 1),
        //            CardRank.Copper => new Color(1, 0.5f, 0, 1),
        //            _ => Color.cyan,
        //        };
        //        UiInfo.ShowCardLIstOnBoard.Add(boardCard);
        //    }
        //    UiInfo.Instance.CardBoardContent.GetComponent<RectTransform>().sizeDelta = new Vector2(Cards.Count * 275, UiInfo.Instance.CardBoardContent.GetComponent<RectTransform>().sizeDelta.y);
        //}
        //生成对局不存在的卡牌
        //private static void CreatBoardCardVitual(CardBoardMode mode)
        //{
        //    UiInfo.ShowCardLIstOnBoard.ForEach(Object.Destroy);
        //    List<string> CardIds = Info.AgainstInfo.cardBoardIDList;
        //    for (int i = 0; i < CardIds.Count; i++)
        //    {
        //        var CardStandardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(CardIds[i]);

        //        GameObject boardCard = Object.Instantiate(UiInfo.Instance.BoardCard, UiInfo.Instance.CardBoardContent);
        //        boardCard.SetActive(true);
        //        //设置对应立绘
        //        Texture2D texture = CardStandardInfo.cardFace;
        //        boardCard.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        //        //设置效果文本
        //        boardCard.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = CardStandardInfo.TranslateAbility;
        //        //设置点数
        //        boardCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = CardStandardInfo.point == 0 ? "" : CardStandardInfo.point.ToString();
        //        //设置名字
        //        boardCard.name = CardStandardInfo.TranslateName;
        //        //设置相关信息
        //        boardCard.GetComponent<CardInSelectBoardManager>().Rank = i;
        //        //NewCard.GetComponent<CardInSelectBoardManager>().IsCanSee = CardStandardInfo.IsCanSee;
        //        boardCard.GetComponent<CardInSelectBoardManager>().SetTargetCard(CardIds[i]);
        //        //设置品质
        //        boardCard.GetComponent<Image>().color = CardStandardInfo.cardRank switch
        //        {
        //            CardRank.Leader => Color.cyan,
        //            CardRank.Gold => Color.yellow,
        //            CardRank.Silver => Color.gray,
        //            CardRank.Copper => Color.magenta,
        //            _ => Color.cyan,
        //        };
        //        UiInfo.ShowCardLIstOnBoard.Add(boardCard);
        //    }
        //    UiInfo.Instance.CardBoardContent.GetComponent<RectTransform>().sizeDelta = new Vector2(Cards.Count * 275, UiInfo.Instance.CardBoardContent.GetComponent<RectTransform>().sizeDelta.y);
        //}
    }
}