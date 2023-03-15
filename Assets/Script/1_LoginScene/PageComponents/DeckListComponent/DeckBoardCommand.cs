using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Config;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Command
{
    class DeckBoardCommand
    {
        public static void Init()
        {
            var s = Info.AgainstInfo.OnlineUserInfo;
            Info.PageComponentInfo.selectDeckRank = Info.AgainstInfo.OnlineUserInfo.UseDeckNum;
            Info.PageComponentInfo.tempDeck = Info.AgainstInfo.OnlineUserInfo.UseDeck.Clone();
            Info.PageComponentInfo.Instance.deckModel.SetActive(false);

            var decks = Info.AgainstInfo.OnlineUserInfo.Decks;
            var deckModel = Info.PageComponentInfo.Instance.deckModel;
            var deckModels = Info.PageComponentInfo.Instance.deckModels;
            deckModels.ForEach(model => model.SetActive(false));

            Debug.LogWarning(deckModels.Count + "-" + decks.Count);
            if (decks.Count > deckModels.Count - 1)
            {
                int num = decks.Count - (deckModels.Count - 1);
                for (int i = 0; i < num; i++)
                {
                    deckModels.Insert(deckModels.Count, UnityEngine.Object.Instantiate(deckModel, deckModel.transform.parent));
                }
            }
            for (int i = 0; i < decks.Count; i++)
            {
                deckModels[i].SetActive(true);
                //更新卡组信息
                deckModels[i].transform.GetChild(1).GetComponent<Text>().text = decks[i].DeckName;
                var cardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(decks[i].LeaderID);
                Sprite cardTex = cardInfo.cardFace.ToSprite();
                deckModels[i].transform.GetComponent<Image>().sprite = cardTex;
            }
            Info.PageComponentInfo.values.Clear();
            for (int i = 0; i < decks.Count; i++)
            {
                Info.PageComponentInfo.values.Add(Info.PageComponentInfo.bias + i * Info.PageComponentInfo.fre);
            }
        }
        public static async void OnDeckClick(GameObject deck)
        {
            int selectRank = Info.PageComponentInfo.Instance.deckModels.IndexOf(deck);
            if (Info.PageComponentInfo.selectDeckRank != selectRank)
            {
                Info.PageComponentInfo.selectDeckRank = selectRank;
                Info.PageComponentInfo.isCardClick = true;
                Info.AgainstInfo.OnlineUserInfo.UseDeckNum = Info.PageComponentInfo.selectDeckRank;
                await Info.AgainstInfo.OnlineUserInfo.UpdateDecksAsync();
                Command.DeckBoardCommand.Init();
                Command.CardListCommand.Init();
                Debug.LogWarning("点击修改为" + Info.PageComponentInfo.selectDeckRank);
            }
        }
        public static void UpdateDeckPosition()
        {
            if (Info.PageComponentInfo.Instance.content.gameObject.activeInHierarchy)
            {
                Info.PageComponentInfo.show = Info.PageComponentInfo.Instance.content.GetComponent<RectTransform>().localPosition.x;
                for (int i = 0; i < Info.PageComponentInfo.values.Count; i++)
                {
                    Info.PageComponentInfo.values[i] = Info.PageComponentInfo.bias + i * Info.PageComponentInfo.fre;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Info.PageComponentInfo.isDragMode = true;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (!Info.PageComponentInfo.isCardClick)
                    {
                        float selectValue = Info.PageComponentInfo.values.OrderBy(value => Mathf.Abs(value - Info.PageComponentInfo.Instance.content.GetComponent<RectTransform>().localPosition.x)).First();

                        GameObject deck = Info.PageComponentInfo.Instance.deckModels[Info.PageComponentInfo.values.IndexOf(selectValue)];
                        OnDeckClick(deck);
                    }
                    else
                    {
                        Info.PageComponentInfo.isCardClick = false;
                    }
                    Info.PageComponentInfo.isDragMode = false;
                }
                if (!Info.PageComponentInfo.isDragMode)
                {
                    Vector3 end = new Vector3(Info.PageComponentInfo.values[Info.PageComponentInfo.selectDeckRank], 120, 0);
                    Info.PageComponentInfo.Instance.content.GetComponent<RectTransform>().localPosition = Vector3.Lerp(Info.PageComponentInfo.Instance.content.GetComponent<RectTransform>().localPosition, end, Time.deltaTime * 3);
                }
            }
        }
        public static async void CreatDeck(string LeaderId)
        {
            Info.AgainstInfo.OnlineUserInfo.Decks.Add(new Model.Deck("新卡组", LeaderId,"", new List<string> { }));
            Info.AgainstInfo.OnlineUserInfo.UseDeckNum = Info.AgainstInfo.OnlineUserInfo.Decks.Count - 1;
            //将牌库设为可编辑模式
            Info.PageComponentInfo.isEditDeckMode = true;
            Debug.Log("切换到deck" + Info.AgainstInfo.OnlineUserInfo.UseDeckNum);
            await Info.AgainstInfo.OnlineUserInfo.UpdateDecksAsync();
            Command.DeckBoardCommand.Init();
            Command.CardListCommand.Init();
            //切换状态至牌库
            Command.MenuStateCommand.RebackStare();
            Command.MenuStateCommand.AddState(MenuState.CardListChange);
        }
        public static void DeleteDeck()
        {
            _ = NoticeCommand.ShowAsync("删除卡组", NotifyBoardMode.Ok_Cancel, okAction: async () =>
            {
                if (Info.AgainstInfo.OnlineUserInfo.Decks.Count > 1)
                {
                    Debug.Log("删除卡组成功");
                    Info.AgainstInfo.OnlineUserInfo.Decks.Remove(Info.AgainstInfo.OnlineUserInfo.UseDeck);
                    Info.AgainstInfo.OnlineUserInfo.UseDeckNum = Math.Max(0, Info.AgainstInfo.OnlineUserInfo.UseDeckNum - 1);
                    await Info.AgainstInfo.OnlineUserInfo.UpdateDecksAsync();
                    Command.DeckBoardCommand.Init();
                    Command.CardListCommand.Init();
                }
                else
                {
                    await NoticeCommand.ShowAsync("请至少保留一个卡组", NotifyBoardMode.Ok);
                }
            });
        }
        public static void RenameDeck()
        {
            _ = NoticeCommand.ShowAsync("重命名卡牌", NotifyBoardMode.Input, inputAction: async (text) =>
            {
                Debug.Log("重命名卡组为" + text);
                Info.AgainstInfo.OnlineUserInfo.UseDeck.DeckName = text;
                await Task.Delay(100);
                await Info.AgainstInfo.OnlineUserInfo.UpdateDecksAsync();
                Command.DeckBoardCommand.Init();
                Command.CardListCommand.Init();

            }, inputField: Info.AgainstInfo.OnlineUserInfo.UseDeck.DeckName);
        }
        public static async Task StartAgainstAsync()
        {
            await Manager.CameraViewManager.MoveToViewAsync(2);
            Command.MenuStateCommand.AddState(MenuState.WaitForBattle);
            Command.BookCommand.SimulateFilpPage(true);//开始翻书
            Info.PageComponentInfo.currentAgainstMode = AgainstModeType.Practice;
            Info.AgainstInfo.FirstMode = 0;
            PlayerInfo sampleUserInfo = null;
            PlayerInfo virtualOpponentInfo = null;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (Command.MenuStateCommand.HasState(MenuState.LevelSelect))//单人关卡选择模式
            {
                Info.PageComponentInfo.currentAgainstMode = AgainstModeType.Story;
                string targetStage = Info.PageComponentInfo.CurrentStage + Info.PageComponentInfo.CurrentStep;
                sampleUserInfo = DeckConfig.GetPlayerCardDeck(targetStage);
                virtualOpponentInfo = DeckConfig.GetPlayerCardDeck(targetStage);
                AgainstConfig.SetFirstMode(2);
                await DialogueCommand.Play(Info.PageComponentInfo.CurrentStage, Info.PageComponentInfo.CurrentStep);
                //播放剧情

            }
            if (Command.MenuStateCommand.HasState(MenuState.PracticeConfig))//单人练习模式
            {
                Info.PageComponentInfo.currentAgainstMode = AgainstModeType.Practice;
                sampleUserInfo = Info.AgainstInfo.OnlineUserInfo.GetSampleInfo();
                virtualOpponentInfo = DeckConfig.GetPracticeCardDeck(Info.PageComponentInfo.SelectLeader);
                AgainstConfig.SetFirstMode(Info.PageComponentInfo.SelectFirstHandMode);

            }
            if (Command.MenuStateCommand.HasState(MenuState.CasualModeDeckSelect))//多人休闲模式
            {
                Info.PageComponentInfo.currentAgainstMode = AgainstModeType.Casual;
                sampleUserInfo = Info.AgainstInfo.OnlineUserInfo.GetSampleInfo();
            }
            if (Command.MenuStateCommand.HasState(MenuState.RankModeDeckSelect))//多人天梯模式
            {
                Info.PageComponentInfo.currentAgainstMode = AgainstModeType.Rank;
                sampleUserInfo = Info.AgainstInfo.OnlineUserInfo.GetSampleInfo();
            }
            if (Command.MenuStateCommand.HasState(MenuState.ArenaModeDeckSelect))//多人竞技场模式
            {
                Info.PageComponentInfo.currentAgainstMode = AgainstModeType.Arena;
                sampleUserInfo = Info.AgainstInfo.OnlineUserInfo.GetSampleInfo();
            }

            _ = NoticeCommand.ShowAsync("少女祈祷中~", NotifyBoardMode.Cancel, cancelAction: async () =>
            {
                Command.BookCommand.SimulateFilpPage(false);//开始翻书
                await Task.Delay(2000);
                await Manager.CameraViewManager.MoveToViewAsync(1);
                Command.MenuStateCommand.RebackStare();
                await Command.NetCommand.LeaveHoldOnList(Info.PageComponentInfo.currentAgainstMode, sampleUserInfo.Account);
            });
            //配置对战模式
            AgainstConfig.Init();
            AgainstConfig.SetAgainstMode(Info.PageComponentInfo.currentAgainstMode);
            //开始排队
            await Command.NetCommand.JoinHoldOnList(Info.PageComponentInfo.currentAgainstMode, Info.AgainstInfo.FirstMode, sampleUserInfo, virtualOpponentInfo);
        }
    }
}
