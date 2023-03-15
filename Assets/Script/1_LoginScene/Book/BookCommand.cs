using Sirenix.OdinInspector;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    public partial class BookCommand : MonoBehaviour
    {
        /// <summary>
        /// 场景载入状态(第一次进入与从对战界面退出时再次进入)
        /// </summary>
        /// <returns></returns>
        public static async Task InitAsync(bool isAleardyLogin)
        {
            UiInfo.Instance.loginCanvas_Model.SetActive(!isAleardyLogin);
            BookInfo.instance.coverModel.transform.position = new Vector3(0.5f, 0.08f, 0);
            BookInfo.instance.coverModel.transform.eulerAngles = Vector3.zero;
            ActiveCompment();
            //初始状态待补充
            //Command.MenuStateCommand.ChangeToMainPage(MenuState.Single);
            //await Manager.CameraViewManager.MoveToViewAsync(1);
        }
        /// <summary>
        /// 进入书本翻开状态
        /// </summary>
        /// <returns></returns>
        public static async Task InitToOpenStateAsync()
        {
            UiInfo.Instance.loginCanvas_Model.SetActive(false);
            await SetCoverStateAsync(true);
            ActiveCompment();
            Command.MenuStateCommand.ChangeToMainPage(MenuState.Single);
            await Manager.CameraViewManager.MoveToViewAsync(1);
        }
        [Button]
        public static async Task SetCoverStateAsync(bool isBookOpen, bool isImmediately = false) =>
          await CustomThread.TimerAsync(isImmediately ? 0 : 0.3f, runAction: (process) =>
                {
                    Info.BookInfo.instance.coverModel.transform.eulerAngles = Vector3.zero;
                    float length = (Info.BookInfo.instance.coverModel.transform.position - Info.BookInfo.instance.axisModel.transform.position).magnitude;
                    float angle = isBookOpen ? Mathf.Lerp(0, 180, process) : Mathf.Lerp(180, 0, process);
                    Info.BookInfo.instance.coverModel.transform.localPosition = new Vector3(0, 0.08f, 0) + new Vector3(length * Mathf.Cos(Mathf.PI / 180 * angle), length * Mathf.Sin(Mathf.PI / 180 * angle));
                    Info.BookInfo.instance.coverModel.transform.eulerAngles = new Vector3(0, 0, angle);
                });

        public static async void ActiveCompment(bool isImmediately = false, params BookCompmentType[] types)
        {
            await CustomThread.TimerAsync(isImmediately ? 0 : 0.2f, runAction: (process) => //在0.2秒内不断移动并降低透明度
            {
                Info.BookInfo.instance.UIComponent.GetComponent<CanvasGroup>().alpha = 1 - process;
                Info.BookInfo.instance.UIComponent.transform.localPosition = new Vector3(50, 0, 0) * (process);
            });
            Info.BookInfo.instance.singleModeSelectComponent.SetActive(false);
            Info.BookInfo.instance.multiplayerModeSelectComponent.SetActive(false);
            Info.BookInfo.instance.practiceComponent.SetActive(false);
            Info.BookInfo.instance.cardDetailComponent.SetActive(false);
            Info.BookInfo.instance.cardListComponent.SetActive(false);
            Info.BookInfo.instance.mapComponent.SetActive(false);
            Info.BookInfo.instance.cardDeckListComponent.SetActive(false);
            Info.BookInfo.instance.cardLibraryComponent.SetActive(false);
            Info.BookInfo.instance.campSelectComponent.SetActive(false);
            Info.BookInfo.instance.campSelectComponent.SetActive(false);
            Info.BookInfo.instance.scenePageComponent.SetActive(false);
            Info.BookInfo.instance.shrineComponent.SetActive(false);
            Info.BookInfo.instance.collectComponent.SetActive(false);
            Info.BookInfo.instance.configComponent.SetActive(false);
            types.ToList().ForEach(type =>
            {
                GameObject targetUiComoinent;
                switch (type)
                {
                    case BookCompmentType.Single: targetUiComoinent = Info.BookInfo.instance.singleModeSelectComponent; break;
                    case BookCompmentType.Multiplayer: targetUiComoinent = Info.BookInfo.instance.multiplayerModeSelectComponent; break;
                    case BookCompmentType.Practice: targetUiComoinent = Info.BookInfo.instance.practiceComponent; break;
                    case BookCompmentType.CardDetial: targetUiComoinent = Info.BookInfo.instance.cardDetailComponent; break;
                    case BookCompmentType.CardList: targetUiComoinent = Info.BookInfo.instance.cardListComponent; break;
                    case BookCompmentType.DeckList: targetUiComoinent = Info.BookInfo.instance.cardDeckListComponent; break;
                    case BookCompmentType.CardLibrary: targetUiComoinent = Info.BookInfo.instance.cardLibraryComponent; break;
                    case BookCompmentType.Map: targetUiComoinent = Info.BookInfo.instance.mapComponent; break;
                    case BookCompmentType.CampSelect: targetUiComoinent = Info.BookInfo.instance.campSelectComponent; break;
                    case BookCompmentType.ScenePage: targetUiComoinent = Info.BookInfo.instance.scenePageComponent; break;
                    case BookCompmentType.Shrine: targetUiComoinent = Info.BookInfo.instance.shrineComponent; break;
                    case BookCompmentType.Collect: targetUiComoinent = Info.BookInfo.instance.collectComponent; break;
                    case BookCompmentType.Config: targetUiComoinent = Info.BookInfo.instance.configComponent; break;
                    default: targetUiComoinent = null; break;
                }
                targetUiComoinent?.SetActive(true);
            });
            await CustomThread.TimerAsync(isImmediately ? 0 : 0.2f, runAction: (process) => //在0.2秒内不断移动并降低透明度
            {
                Info.BookInfo.instance.UIComponent.GetComponent<CanvasGroup>().alpha = process;
                Info.BookInfo.instance.UIComponent.transform.localPosition = new Vector3(-50, 0, 0) * (1 - process);
            });
        }
        public static async void SimulateFilpPage(bool IsSimulateFilpPage, bool isRightToLeft = true)
        {
            Info.BookInfo.IsSimulateFilpPage = IsSimulateFilpPage;
            if (IsSimulateFilpPage)
            {
                while (Info.BookInfo.IsSimulateFilpPage)
                {
                    await Task.Delay(2000);
                    if (!IsSimulateFilpPage) break;

                    TaskThrowCommand.Throw();
                    GameObject voidPageModel = Info.BookInfo.instance.voidPageModel;
                    GameObject fakePageModel = Info.BookInfo.instance.fakePageModel;
                    GameObject startPage = Instantiate(voidPageModel, voidPageModel.transform.position, voidPageModel.transform.rotation, voidPageModel.transform.parent);
                    GameObject endPage = Instantiate(voidPageModel, voidPageModel.transform.position, voidPageModel.transform.rotation, voidPageModel.transform.parent);
                    GameObject fakePage1 = Instantiate(fakePageModel, fakePageModel.transform.position, fakePageModel.transform.rotation, fakePageModel.transform.parent);
                    GameObject fakePage2 = Instantiate(fakePageModel, fakePageModel.transform.position, fakePageModel.transform.rotation, fakePageModel.transform.parent);
                    _ = FileSinglePage(isRightToLeft, startPage);
                    await Task.Delay(200);
                    if (!IsSimulateFilpPage) break;

                    _ = FileSinglePage(isRightToLeft, fakePage1);
                    await Task.Delay(200);
                    if (!IsSimulateFilpPage) break;

                    _ = FileSinglePage(isRightToLeft, fakePage2);
                    await Task.Delay(200);
                    if (!IsSimulateFilpPage) break;

                    await FileSinglePage(isRightToLeft, endPage);
                    if (!IsSimulateFilpPage) break;

                }
            }
            static async Task FileSinglePage(bool isRightToLeft, GameObject page)
            {
                if (page != null)
                {
                    page.SetActive(true);
                    await CustomThread.TimerAsync(0.5f, runAction: (process) =>
                    {
                        if (page == null) return;
                        page.transform.eulerAngles = Vector3.zero;
                        float length = (page.transform.position - Info.BookInfo.instance.axisModel.transform.position).magnitude;
                        float angle = isRightToLeft ? Mathf.Lerp(0, 180, process) : Mathf.Lerp(180, 0, process);
                        page.transform.localPosition = new Vector3(0, 0.08f, 0) + new Vector3(length * Mathf.Cos(Mathf.PI / 180 * angle), length * Mathf.Sin(Mathf.PI / 180 * angle));
                        page.transform.eulerAngles = new Vector3(0, 0, angle);
                    });
                    DestroyImmediate(page);
                }
            }
        }
    }
}