using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Config;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Control
{
    public class LoginSceneManager : MonoBehaviour
    {
        public Text AccountText;
        public Text PasswordText;
        public string Account;
        public string Password;
        static bool IsAleardyLogin { get; set; } = false;
        public static bool IsEnterRoom { get; set; } = false;
        async void Start()
        {
            Debug.LogWarning("场景已切换" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff"));
            AccountText.text = Account;
            PasswordText.text = Password;
            await SceneCommand.InitAsync(false);
            Debug.LogWarning("场景初始化" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff"));
            //加载对话
            DialogueCommand.Init();
            DialogueCommand.Load();
            Debug.LogWarning("对话加载完毕" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff"));
            TaskThrowCommand.Init();
            //初始化场景物体状态，如果已登录，则进入到指定页，否则进入初始场景
            await InitAsync(IsAleardyLogin);
            //await BookCommand.InitAsync(IsAleardyLogin);
            if (!IsAleardyLogin)
            {

                UserLogin();//自动登录
                await Task.Delay(1000);
                //await TestReplayAsync();
                //await TestBattleAsync();
            }

            /// <summary>
            /// 初始化场景状态(第一次进入与从对战界面退出时再次进入)
            /// </summary>
            /// <returns></returns>
            static async Task InitAsync(bool isAleardyLogin)
            {
                Debug.LogWarning("初始化场景界面状态");
                //设置登陆窗口可见性
                UiInfo.Instance.loginCanvas_Model.SetActive(!isAleardyLogin);
                //设置摄像机视角
                await CameraViewManager.MoveToViewAsync(isAleardyLogin ? 3 : 0, true);
                //如果已经登陆了，则翻开书本，重置ui状态
                if (isAleardyLogin)
                {
                    Debug.LogWarning("重置ui" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff"));
                    MenuStateCommand.RefreshCurrentState(true);
                    Debug.LogWarning("打开封面" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff"));
                    await BookCommand.SetCoverStateAsync(true, true);
                    //await Task.Delay(1000);
                    await CameraViewManager.MoveToViewAsync(1);
                    Debug.LogWarning("等待返回" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff"));
                    //退回到书本视角
                    BookCommand.SimulateFilpPage(true, false);
                    await Task.Delay(1000);
                    BookCommand.SimulateFilpPage(false);

                    MenuStateCommand.RebackStare();
                    MenuStateCommand.RebackStare();
                }
            }
        }
        private void Update()
        {
            //临时，显示当前的ui路径
            if (Input.GetKeyDown(KeyCode.S))
            {
                MenuStateCommand.ShowStare();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                switch (MenuStateCommand.GetCurrentStateRank())
                {
                    case (1)://如果当前状态为登录前，则关闭程序
                        {
                            _ = NoticeCommand.ShowAsync("退出游戏？",
                            okAction: async () =>
                            {
                                Application.Quit();
                            });
                            break;
                        }
                    case (2)://如果当前状态为第主级页面，则询问并退出登录
                        {
                            _ = NoticeCommand.ShowAsync("退出登录",
                            okAction: async () =>
                            {
                                IsAleardyLogin = false;
                                await Manager.CameraViewManager.MoveToViewAsync(0);
                                MenuStateCommand.RebackStare();
                                MenuStateCommand.ChangeToMainPage(MenuState.Login);
                                await BookCommand.SetCoverStateAsync(false);
                                UiInfo.Instance.loginCanvas_Model.SetActive(true);
                            });
                            break;
                        }
                    default://如果当前状态为多级页面，则返回上级（个别页面需要询问）
                        {
                            //如果是组牌模式，则询问是否返回上一页，否则直接返回上一页
                            if (MenuStateCommand.GetCurrentState() == MenuState.CardListChange)
                            {
                                _ = NoticeCommand.ShowAsync("不保存卡组？",
                                okAction: async () =>
                                {
                                    MenuStateCommand.RebackStare();
                                });
                            }
                            else
                            {
                                MenuStateCommand.RebackStare();
                            }
                            break;
                        }
                }
            }
        }
        public async void UserRegister()
        {
            try
            {
                int result = await NetCommand.RegisterAsync(AccountText.text, PasswordText.text);
                switch (result)
                {
                    case (1): await NoticeCommand.ShowAsync("注册成功", NotifyBoardMode.Ok); break;
                    case (-1): await NoticeCommand.ShowAsync("账号已存在", NotifyBoardMode.Ok); break;
                    default: await NoticeCommand.ShowAsync("注册发生异常", NotifyBoardMode.Ok); break;
                }
            }
            catch (Exception e) { Debug.LogException(e); }
        }

        public async void UserLogin()
        {
            try
            {
                bool isSuccessLogin = await NetCommand.LoginAsync(AccountText.text, PasswordText.text);
                if (isSuccessLogin)
                {
                    //保存静态账号密码
                    Account = AccountText.text;
                    Password = PasswordText.text;
                    IsAleardyLogin = true;
                    var stage = AgainstInfo.OnlineUserInfo.Stage;
                    if (AgainstInfo.OnlineUserInfo.GetStage("0") == 0)
                    {
                        await DialogueCommand.Play("0", 0);
                        await AgainstInfo.OnlineUserInfo.UpdateUserStateAsync("0", 1);
                    }
                    Manager.UserInfoManager.Refresh();
                    await BookCommand.InitToOpenStateAsync();
                    //检测是否已经在对战中
                    _ = NetCommand.CheckRoomAsync(AccountText.text, PasswordText.text);
                }
                else
                {
                    await NoticeCommand.ShowAsync("账号或密码错误，请重试", NotifyBoardMode.Ok);
                }
            }
            catch (Exception e) { Debug.LogException(e); }
        }
        public async Task TestReplayAsync()
        {
            var summarys = await NetCommand.DownloadOwnerAgentSummaryAsync(AgainstInfo.OnlineUserInfo.Account, 0, 20);
            AgainstConfig.ReplayStart(summarys.Last());
        }
        public async Task TestBattleAsync()
        {

            await Manager.CameraViewManager.MoveToViewAsync(2);
            MenuStateCommand.AddState(MenuState.WaitForBattle);
            BookCommand.SimulateFilpPage(true);//开始翻书

            AgainstModeType targetAgainstMode = AgainstModeType.Story;
            PlayerInfo sampleUserInfo = Info.AgainstInfo.OnlineUserInfo.GetSampleInfo();
            PlayerInfo virtualOpponentInfo = DeckConfig.GetOpponentCardDeck("test");

            _ = NoticeCommand.ShowAsync("少女排队中~", NotifyBoardMode.Cancel, cancelAction: async () =>
            {
                BookCommand.SimulateFilpPage(false);//开始翻书
                await Task.Delay(2000);
                await Manager.CameraViewManager.MoveToViewAsync(1);
                MenuStateCommand.RebackStare();
                await NetCommand.LeaveHoldOnList(AgainstModeType.Story, sampleUserInfo.Account);
            });
            //配置对战模式
            AgainstConfig.Init();
            AgainstConfig.SetAgainstMode(targetAgainstMode);
            //开始排队
            await NetCommand.JoinHoldOnList(targetAgainstMode, 0, sampleUserInfo, virtualOpponentInfo);
        }
        public void UserServerSelect() => AgainstInfo.IsHostNetMode = !Info.AgainstInfo.IsHostNetMode;
        private void OnApplicationQuit() => NetCommand.Dispose();
        private void OnGUI()
        {
            //if (GUI.Button(new Rect(0, 0, 100, 50), "回放测试"))
            //{
            //    TestReplayAsync();
            //}
            //if (GUI.Button(new Rect(0, 100, 100, 50), "对战测试"))
            //{
            //    TestBattleAsync();
            //}
            if (GUI.Button(new Rect(0, 200, 100, 50), "资源重载"))
            {

                AssetBundleCommand.AlreadyInit = false;
                SceneManager.LoadScene(0);
            }
        }
    }
}