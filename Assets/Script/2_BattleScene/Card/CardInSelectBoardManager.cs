using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    public class CardInSelectBoardManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public int Rank { get; set; }
        public bool IsCanSee { get; set; }

        //指定实际对应的卡牌目标
        public Card ActualCard;
        //指定不存在于对局的虚拟卡牌信息目标
        public string VitualCardId;
        bool isTargetCardActual;
        public Outline selectRect => transform.GetComponent<Outline>();
        public void SetTargetCard(string card)
        {
            VitualCardId = card;
            ActualCard = null;
            isTargetCardActual = false;
        }
        public void SetTargetCard(Card card)
        {
            VitualCardId = "";
            ActualCard = card;
            isTargetCardActual = true;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            AgainstInfo.PlayerFocusCard = ActualCard;
        }

        public void OnPointerExit(PointerEventData eventData) => CardLibraryManager.LostFocusCardOnMenu();
        public void OnPointerClick(PointerEventData eventData)
        {

            //如果右键单击或鼠标左键双击，视为鼠标右键点击
            if (eventData.button == PointerEventData.InputButton.Right || (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2))
            {
                if (Input.GetMouseButtonUp(1) && IsCanSee)
                {
                    if (isTargetCardActual)
                    {
                        CardAbilityBoardManager.Manager.LoadCardFromBoardCard(ActualCard, Rank);
                    }
                    else
                    {
                        CardAbilityBoardManager.Manager.LoadCardFromBoardCard(VitualCardId, Rank);
                    }
                }
            }
            //如果左键单击，视为鼠标左键点击
            else if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 1)
            {
                if (Info.AgainstInfo.cardBoardMode == GameEnum.CardBoardMode.Select || Info.AgainstInfo.cardBoardMode == GameEnum.CardBoardMode.ExchangeCard)
                {
                    if (Info.AgainstInfo.SelectBoardCardRanks.Contains(Rank))//如果已选，则移除
                    {
                        Info.AgainstInfo.SelectBoardCardRanks.Remove(Rank);
                        selectRect.enabled = false;
                        Debug.Log("取消选择" + Rank);
                        //如果是小局开局抽卡，则不同步选择卡牌数据消息，只同步换牌数据
                        //若是卡牌效果换牌，则同步换牌数据
                        if (!Info.AgainstInfo.isRoundStartExchange)
                        {
                            Command.NetCommand.AsyncInfo(GameEnum.NetAcyncType.SelectBoardCard);
                        }
                    }
                    else//否则加入选择列表
                    {
                        Info.AgainstInfo.SelectBoardCardRanks.Add(Rank);
                        selectRect.enabled = true;
                        Debug.Log("选择" + Rank);
                        if (!Info.AgainstInfo.isRoundStartExchange)
                        {
                            Command.NetCommand.AsyncInfo(GameEnum.NetAcyncType.SelectBoardCard);
                        }

                    }
                }
            }
        }
    }
}