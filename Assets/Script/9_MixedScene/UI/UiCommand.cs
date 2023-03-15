using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Command
{
    public class UiCommand
    {
        //////////////////////////////////////////////////////////状态与字段UI//////////////////////////////////////////////////////////////
        public static Sprite GetFieldAndStateSprite<T>(T cardField)
        {
            Sprite targetSprite = AssetBundleCommand.Load<Sprite>("FieldAndState", cardField.ToString());
            targetSprite ??= AssetBundleCommand.Load<Sprite>("FieldAndState", "None");
            return targetSprite;
        }
        //////////////////////////////////////////////////////////对战中游戏卡牌面板//////////////////////////////////////////////////////////////
        static Text UiText => UiInfo.Instance.CardBoardCanvas.transform.GetChild(0).GetChild(1).GetComponent<Text>();
        static GameObject HideButton => UiInfo.Instance.CardBoardCanvas.transform.GetChild(1).GetChild(0).gameObject;
        static GameObject JumpButton => UiInfo.Instance.CardBoardCanvas.transform.GetChild(1).GetChild(1).gameObject;
        static GameObject ShowButton => UiInfo.Instance.CardBoardCanvas.transform.GetChild(1).GetChild(2).gameObject;
        static GameObject CloseButton => UiInfo.Instance.CardBoardCanvas.transform.GetChild(1).GetChild(3).gameObject;
        static GameObject BackImage => UiInfo.Instance.CardBoardCanvas.transform.GetChild(0).gameObject;
        /// <summary>
        /// 修改展示板标题
        /// </summary>
        /// <param name="Title"></param>
        public static void SetCardBoardTitle(string Title) => UiText.text = Title;
        /// <summary>
        /// 打开展示板，并根据模式显示相应按钮
        /// </summary>
        /// <param name="mode"></param>
        public static void SetCardBoardOpen(CardBoardMode mode)
        {
            UiInfo.Instance.CardBoardCanvas.SetActive(true);
            BackImage.SetActive(true);
            HideButton.SetActive(false);
            JumpButton.SetActive(false);
            ShowButton.SetActive(false);
            CloseButton.SetActive(false);
            switch (mode)
            {
                case CardBoardMode.Temp:
                    CloseButton.SetActive(true);
                    break;
                //选择或换牌模式下
                case CardBoardMode.Select:
                case CardBoardMode.ExchangeCard:
                    HideButton.SetActive(true);
                    JumpButton.SetActive(true);
                    //设置卡牌面板为等待选择模式
                    UiInfo.IsCardBoardNeedSelect = true;
                    UiInfo.LastCardBoardMode = mode;
                    break;
                case CardBoardMode.ShowOnly:
                    break;
            }
        }
        public static void SetCardBoardClose()
        {
            UiInfo.Instance.CardBoardCanvas.SetActive(false);
            if (UiInfo.IsCardBoardNeedSelect)
            {
                SetCardBoardHide();
            }
        }
        public static void SetCardBoardHide()
        {
            UiInfo.Instance.CardBoardCanvas.SetActive(true);
            BackImage.SetActive(false);
            HideButton.SetActive(false);
            JumpButton.SetActive(false);
            ShowButton.SetActive(true);
            CloseButton.SetActive(false);

        }
        public static void SetCardBoardShow()
        {
            string title = UiInfo.LastCardBoardMode == CardBoardMode.Select ? "Remaining".TranslationGameText() + Info.AgainstInfo.ExChangeableCardNum : "";
            UiCommand.SetCardBoardTitle(title);
            SetCardBoardOpen(UiInfo.LastCardBoardMode);
            CardBoardCommand.ShowCardBoard<Card>(null, UiInfo.LastCardBoardMode );
        }
        public static void CardBoardSelectOver() => AgainstInfo.IsSelectCardOver = true;
        //////////////////////////////////////////////////////////回合阶段提示UI//////////////////////////////////////////////////////////////
        public static async Task NoticeBoardShow(string Title)
        {
            GameObject NoticeBoard = UiInfo.Instance.NoticePopup;
            NoticeBoard.transform.GetChild(0).GetComponent<Text>().text = Title;
            NoticeBoard.GetComponent<Image>().color = AgainstInfo.IsMyTurn ? new Color(0.2f, 0.5f, 1, 0.5f) : new Color(1, 0.2f, 0.2f, 0.5f);
            NoticeBoard.transform.localScale = new Vector3(1, 0, 1);
            NoticeBoard.SetActive(true);
            await CustomThread.TimerAsync(0.3f, runAction: process =>
            {
                NoticeBoard.transform.localScale = new Vector3(1, process * process, 1);
            });
            await Task.Delay(500);
            await CustomThread.TimerAsync(0.3f, runAction: process =>
            {
                NoticeBoard.transform.localScale = new Vector3(1, 1 - process * process, 1);
            });
            NoticeBoard.SetActive(false);
        }

        //////////////////////////////////////////////////////////箭头//////////////////////////////////////////////////////////////
        public static void CreatFreeArrow()
        {
            GameObject newArrow = GameObject.Instantiate(UiInfo.Instance.Arrow);
            newArrow.name = "Arrow-null";
            newArrow.GetComponent<ArrowManager>().InitArrow(AgainstInfo.ArrowStartCard, UiInfo.Instance.ArrowEndPoin);
            AgainstInfo.ArrowList.Add(newArrow);
        }
        public static void DestoryFreeArrow()
        {
            GameObject targetArrow = AgainstInfo.ArrowList.First(arrow => arrow.GetComponent<ArrowManager>().targetCard == null);
            AgainstInfo.ArrowList.Remove(targetArrow);
            GameObject.Destroy(targetArrow);
        }
        public static void CreatFixedArrow(Card card)
        {
            GameObject newArrow = GameObject.Instantiate(UiInfo.Instance.Arrow);
            newArrow.name = "Arrow-" + card.name;
            newArrow.GetComponent<ArrowManager>().InitArrow(AgainstInfo.ArrowStartCard, AgainstInfo.PlayerFocusCard);
            AgainstInfo.ArrowList.Add(newArrow);
        }
        public static void DestoryFixedArrow(Card card)
        {
            GameObject targetArrow = AgainstInfo.ArrowList.First(arrow => arrow.GetComponent<ArrowManager>().targetCard == card);
            AgainstInfo.ArrowList.Remove(targetArrow);
            GameObject.Destroy(targetArrow);
        }
        public static void DestoryAllArrow()
        {
            AgainstInfo.ArrowList.ForEach(GameObject.Destroy);
            AgainstInfo.ArrowList.Clear();
        }
    }
}