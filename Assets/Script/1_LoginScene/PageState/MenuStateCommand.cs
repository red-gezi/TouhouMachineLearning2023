using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;
namespace TouhouMachineLearningSummary.Command
{
    public partial class MenuStateCommand
    {
        static List<MenuState> currentState = new() { MenuState.Login };
        /// <summary>
        /// 设置书本路径为登录前或者某个一级目录
        /// </summary>
        /// <param name="state"></param>
        public static void ChangeToMainPage(MenuState state)
        {
            //初始化组件状态
            Info.PageComponentInfo.isEditDeckMode = false;

            if (state == MenuState.Login)
            {
                currentState = new List<MenuState>() { MenuState.Login };
            }
            else
            {
                currentState = new List<MenuState>() { MenuState.Login, state };
            }
            RefreshCurrentState();
        }
        public static MenuState GetCurrentState() => currentState.Last();
        public static int GetCurrentStateRank() => currentState.Count();
        public static void AddState(MenuState state)
        {
            currentState.Add(state);
            RefreshCurrentState();
        }
        public static void RebackStare()
        {
            currentState.Remove(currentState.Last());
            RefreshCurrentState();
        }
        public static bool HasState(MenuState menuState) => currentState.Contains(menuState);
        public static void ShowStare() => Debug.Log(currentState.Select(x => x.ToString()).ToJson());
        //根据最后的状态
        public static void RefreshCurrentState(bool isImmediately = false)
        {
            _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.Page);
            MenuState menuState = GetCurrentState();
            switch (menuState)
            {
                case MenuState.Login://进入登录状态，不显示所有UI组件
                    Command.BookCommand.ActiveCompment(isImmediately);
                    break;
                case MenuState.Single:
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.Single);
                    break;
                case MenuState.LevelSelect://进入关卡选择界面，之后修正卡组列表为故事介绍组件
                    //Command.CardListCommand.Init();
                    //Command.cardDetailCommand
                    Command.StageCommand.Init();
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.Map);
                    break;
                case MenuState.PracticeConfig://进入练习配置界面
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.Practice);
                    break;
                case MenuState.Multiplayer://进入多人模式选择界面
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.Multiplayer);
                    break;
                case MenuState.LevelModeDeckSelect://进入关卡模式卡组选择选择界面
                    Command.DeckBoardCommand.Init();
                    Command.CardListCommand.Init();
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.DeckList, BookCompmentType.CardList);
                    break;
                case MenuState.PracticeModeDeckSelect://进入练习模式卡组选择选择界面
                    Command.DeckBoardCommand.Init();
                    Command.CardListCommand.Init();
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.DeckList, BookCompmentType.CardList);
                    break;
                case MenuState.CasualModeDeckSelect://进入休闲模式卡组选择选择界面
                    Command.DeckBoardCommand.Init();
                    Command.CardListCommand.Init();
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.DeckList, BookCompmentType.CardList);
                    break;
                case MenuState.RankModeDeckSelect://进入天梯模式卡组选择选择界面
                    Command.DeckBoardCommand.Init();
                    Command.CardListCommand.Init();
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.DeckList, BookCompmentType.CardList);
                    break;
                case MenuState.ArenaModeDeckSelect://进入竞技场模式卡组选择选择界面
                    Command.DeckBoardCommand.Init();
                    Command.CardListCommand.Init();
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.DeckList, BookCompmentType.CardList);
                    break;
                case MenuState.CardLibrary:
                    Manager.CardLibraryManager.Init();
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.CardDetial, BookCompmentType.CardLibrary);
                    break;
                case MenuState.Shrine:
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.Shrine);
                    break;
                case MenuState.Collect:
                    CollectionManager.Init();
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.Collect);
                    break;
                case MenuState.Config:
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.Config);
                    break;
                case MenuState.CardListChange:
                    Command.CardListCommand.Init(canChangeCard: true);
                    Manager.CardLibraryManager.Init();
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.CardList, BookCompmentType.CardLibrary);
                    break;
                case MenuState.CardDetail:
                    break;
                case MenuState.CampSelect:
                    Command.CampSelectCommand.InitCamp();
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.CardDetial, BookCompmentType.CampSelect);
                    break;
                case MenuState.WaitForBattle:
                    //设置书页翻页方向，并清除所有组件
                    Command.BookCommand.ActiveCompment(isImmediately);
                    break;
                case MenuState.ScenePage:
                    //场景组件初始化待补充

                    Debug.LogWarning("激活场景页面前" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff"));
                    Command.BookCommand.ActiveCompment(isImmediately, BookCompmentType.ScenePage);
                    Debug.LogWarning("激活场景页面后" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff"));

                    break;

                default:
                    break;
            }
        }
    }
}