using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
//对卡组列表卡牌模型中进行相关操作
public class UIEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerClickHandler
{
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;
    public UnityEvent onPointerLeftUp;
    public UnityEvent onPointerRightUp;

    public void OnPointerClick(PointerEventData eventData)
    {
        //如果左键单击，视为鼠标左键点击
        if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 1)
        {
            //onPointerLeftUp.Invoke();
        }
        //如果右键单击或鼠标左键双击，视为鼠标右键点击
        if (eventData.button == PointerEventData.InputButton.Right || (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2))
        {
           // onPointerRightUp.Invoke();
        }
    }


    //public UnityEvent onClick;
    public void OnPointerEnter(PointerEventData eventData) => onPointerEnter.Invoke();
    public void OnPointerExit(PointerEventData eventData) => onPointerExit.Invoke();

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
