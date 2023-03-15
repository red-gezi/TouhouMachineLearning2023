using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Command
{
    public class CardListCommand
    {

        //初始化牌组列表组件
        public static void Init(Model.Deck newTempDeck = null, bool canChangeCard = false)
        {
            Log.Show("配置面板");
            Info.PageComponentInfo.isEditDeckMode = canChangeCard;
            Info.PageComponentInfo.okButton.SetActive(canChangeCard);
            Info.PageComponentInfo.cancelButton.SetActive(canChangeCard);
            Info.PageComponentInfo.changeButton.SetActive(!canChangeCard);
            //如果当前状态是卡组列表改变模式
            if (Command.MenuStateCommand.GetCurrentState() == MenuState.CardListChange)
            {
                Info.PageComponentInfo.cardDeckNameModel.GetComponent<Text>().text = Info.PageComponentInfo.tempDeck.DeckName;
            }
            else
            {
                Info.PageComponentInfo.cardDeckNameModel.GetComponent<Text>().text = Info.AgainstInfo.OnlineUserInfo.UseDeck.DeckName;
            }
            Log.Show("配置卡组名");
            Info.PageComponentInfo.deckCardModels.RemoveAll(x => x == null);
            //if (true)
            //{
            //    //初始化领袖栏?可以舍去？
            //    Info.PageCompnentInfo.deckCardModels.ForEach(model =>
            //    {
            //        if (model != null)
            //        {
            //            Object.Destroy(model);
            //        }
            //    });
            //    Info.PageCompnentInfo.deckCardModels.Clear();
            //}
            //如果领袖存在，载入对应卡图，否则加载空卡图
            if (Info.PageComponentInfo.tempDeck.LeaderID != "")
            {
                var cardTexture = Manager.CardAssemblyManager.GetLastCardInfo(Info.PageComponentInfo.tempDeck.LeaderID).cardFace;
                Info.PageComponentInfo.cardLeaderImageModel.GetComponent<Image>().material.mainTexture = cardTexture;
            }
            else
            {

            }

            Log.Show("配置领袖");
            int deskCardNumber = Info.PageComponentInfo.distinctCardIds.Count();
            int deskModelNumber = Info.PageComponentInfo.deckCardModels.Count;
            Info.PageComponentInfo.deckCardModels.ForEach(model => model.SetActive(false));
            if (deskCardNumber > deskModelNumber)
            {
                for (int i = 0; i < deskCardNumber - deskModelNumber; i++)
                {
                    var newCardModel = Object.Instantiate(Info.PageComponentInfo.cardDeckCardModel, Info.PageComponentInfo.cardDeckContent.transform);
                    Info.PageComponentInfo.deckCardModels.Add(newCardModel);
                }
            }
            Log.Show("新增牌组栏");

            //初始化卡牌栏
            for (int i = 0; i < Info.PageComponentInfo.distinctCardIds.Count(); i++)
            {
                string cardID = Info.PageComponentInfo.distinctCardIds[i];
                GameObject currentCardModel = Info.PageComponentInfo.deckCardModels[i];

                var info = CardAssemblyManager.LastMultiCardInfos.FirstOrDefault(cardInfo => cardInfo.cardID == cardID);
                if (info != null)
                {
                    currentCardModel.transform.GetChild(0).GetComponent<Text>().text = info.TranslateName;
                    Sprite cardTex = info.cardFace.ToSprite();
                    currentCardModel.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = cardTex;
                    //设置数量
                    Color RankColor = Color.white;
                    switch (info.cardRank)
                    {
                        case GameEnum.CardRank.Leader: RankColor = new Color(0.98f, 0.9f, 0.2f); break;
                        case GameEnum.CardRank.Gold: RankColor = new Color(0.98f, 0.9f, 0.2f); break;
                        case GameEnum.CardRank.Silver: RankColor = new Color(0.75f, 0.75f, 0.75f); break;
                        case GameEnum.CardRank.Copper: RankColor = new Color(0.55f, 0.3f, 0.1f); break;
                    }
                    //品质
                    currentCardModel.transform.GetChild(2).GetComponent<Image>().color = RankColor;
                    //点数
                    int point = Manager.CardAssemblyManager.GetLastCardInfo(cardID).point;
                    currentCardModel.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = point == 0 ? " " : point + "";
                    //数量
                    currentCardModel.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "x" + Info.PageComponentInfo.tempDeck.CardIDs.Count(id => id == cardID);

                    currentCardModel.SetActive(true);
                }
                else
                {
                    Debug.Log(cardID + "查找失败");
                }
            }
            Log.Show("配置牌组");
        }
        public static async void SaveDeck()
        {
            Debug.Log("保存卡组");
            Info.AgainstInfo.OnlineUserInfo.UseDeck = Info.PageComponentInfo.tempDeck;
            await Info.AgainstInfo.OnlineUserInfo.UpdateDecksAsync();
            Command.CardListCommand.Init();
            Command.MenuStateCommand.RebackStare();
        }
        public static void CancelDeck()
        {
            Debug.Log("取消卡组修改");
            Info.PageComponentInfo.tempDeck = Info.AgainstInfo.OnlineUserInfo.UseDeck;
            Command.CardListCommand.Init();
            Command.MenuStateCommand.RebackStare();
        }
        public static void RenameDeck()
        {
            if (Command.MenuStateCommand.GetCurrentState() == MenuState.CardListChange)
            {
                _ = NoticeCommand.ShowAsync("重命名卡牌", NotifyBoardMode.Input, inputAction: async (text) =>
                {
                    Debug.Log("重命名卡组为" + text);
                    Info.PageComponentInfo.tempDeck.DeckName = text;
                    Command.CardListCommand.Init();
                    await Task.Delay(100);
                }, inputField: Info.AgainstInfo.OnlineUserInfo.UseDeck.DeckName);
            }
        }

        public static void FocusDeckCardOnMenu(GameObject cardModel)
        {
            int focusCardRank = Info.PageComponentInfo.deckCardModels.IndexOf(cardModel);
            string cardID = Info.PageComponentInfo.distinctCardIds[focusCardRank];
            CardAbilityPopupManager.focusCardID = cardID;
        }
        public static void LostFocusCardOnMenu()
        {
            CardAbilityPopupManager.focusCardID = "";
        }
        public static void AddCardFromLibrary(GameObject clickCard)
        {
            if (Info.PageComponentInfo.isEditDeckMode)
            {
                Debug.Log("添加卡牌" + clickCard.name);
                string clickCardId = clickCard.name;
                int usedCardIdsNum = Info.PageComponentInfo.tempDeck.CardIDs.Count(id => id == clickCardId);
                var cardInfo = Manager.CardAssemblyManager.GetLastCardInfo(clickCardId);
                if (cardInfo.cardRank == CardRank.Leader)
                {
                    Info.PageComponentInfo.tempDeck.LeaderID = cardInfo.cardID;
                    _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeSuccess);
                }
                else
                {
                    int allowAddNum = cardInfo.cardRank == GameEnum.CardRank.Copper ? 3 : 1;
                    if (usedCardIdsNum < Mathf.Min(allowAddNum, Manager.CardLibraryManager.GetHasCardNum(clickCard.name)))
                    {
                        Info.PageComponentInfo.tempDeck.CardIDs.Add(clickCardId);
                        Command.CardListCommand.Init(newTempDeck: Info.PageComponentInfo.tempDeck, canChangeCard: true);
                        _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeSuccess);
                    }
                    else
                    {
                        _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeFailed);
                    }
                }
            }
            else
            {
                Debug.Log("当前为不可编辑模式");
                _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeFailed);
            }
        }
        public static void RemoveCardFromDeck(GameObject clickCard)
        {
            Debug.Log("准备移除卡牌");

            if (Info.PageComponentInfo.isEditDeckMode)
            {
                Debug.Log("移除卡牌");
                _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeSuccess);
                string removeCardId = Info.PageComponentInfo.distinctCardIds[Info.PageComponentInfo.deckCardModels.IndexOf(clickCard)];
                Info.PageComponentInfo.tempDeck.CardIDs.Remove(removeCardId);
                Command.CardListCommand.Init(newTempDeck: Info.PageComponentInfo.tempDeck, canChangeCard: true);
            }
            else
            {
                _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeFailed);
            }
        }
    }
}