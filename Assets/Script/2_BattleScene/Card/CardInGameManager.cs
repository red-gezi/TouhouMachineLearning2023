using Sirenix.OdinInspector;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    public class CardInGameManager : MonoBehaviour
    {
        public GameObject gap;
        public GameObject cardTips;
        public GameObject cardIcon;
        public Card ThisCard { get; set; }
        Material CardMaterial { get; set; }
        Material GapMaterial { get; set; }

        float currentMoveTime = 0;
        public Vector3 TargetPosition { get; set; }
        public Vector3 TargetEuler { get; set; }

        //状态相关参数
        /// <summary>
        /// 临时卡牌和无法选择卡牌用灰色表示
        /// </summary>

        //自由移动
        public bool IsFree { get; set; } = false;
        //立刻移动
        public bool IsImmediatelyMove { get; set; } = false;
        //Card是否可见规则
        public bool IsCardVisible { get; set; } = false;
        /// <summary>
        /// 当非系统控制坐标时，根据玩家操作手动更新卡牌坐标与角度
        /// </summary>
        private void Update()
        {
            if (!ThisCard.IsSystemControlMove)
            {
                RefreshTransform();
            }
        }
        public void Init()
        {
            IsImmediatelyMove = true;
            ThisCard = GetComponent<Card>();
            CardMaterial = GetComponent<Renderer>().material;
            GapMaterial = gap.GetComponent<Renderer>().material;
        }
        private void OnMouseEnter()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                AgainstInfo.PlayerFocusCard = ThisCard;
                RefreshFocus();
                //设置待选打出状态
                if (ThisCard.CurrentOrientation == Orientation.Down && (ThisCard.CurrentRegion == GameRegion.Leader || ThisCard.CurrentRegion == GameRegion.Hand) && IsFree)
                {
                    ThisCard.isPrepareToPlay = true;
                }
                RowCommand.RefreshAllRowsCards();
                _ = SoundEffectCommand.PlayAsync(SoundEffectType.CardSelect);
                NetCommand.AsyncInfo(NetAcyncType.FocusCard);
            }

        }
        private void OnMouseExit()
        {
            if (AgainstInfo.PlayerFocusCard == ThisCard)
            {
                AgainstInfo.PlayerFocusCard = null;
                ThisCard.isPrepareToPlay = false;
                RefreshFocus();
                RowCommand.RefreshAllRowsCards();
                NetCommand.AsyncInfo(NetAcyncType.FocusCard);
            }
        }
        private void OnMouseDown()
        {
            if (ThisCard.isPrepareToPlay && !EventSystem.current.IsPointerOverGameObject())
            {
                AgainstInfo.playerPrePlayCard = ThisCard;
            }
        }
        private void OnMouseUp()
        {
            if (AgainstInfo.playerPrePlayCard != null && !EventSystem.current.IsPointerOverGameObject())
            {
                if (AgainstInfo.PlayerFocusRow != null && AgainstInfo.PlayerFocusRow.rowPrefab.name == "下方_墓地")
                {
                    Info.AgainstInfo.playerDisCard = Info.AgainstInfo.playerPrePlayCard;
                }
                //将卡牌放回（不做数据改变处理）
                else if (Info.AgainstInfo.PlayerFocusRow != null && (AgainstInfo.PlayerFocusRow.rowPrefab.name == "下方_领袖" || AgainstInfo.PlayerFocusRow.rowPrefab.name == "下方_手牌"))
                {

                }
                else
                {
                    Info.AgainstInfo.playerPlayCard = Info.AgainstInfo.playerPrePlayCard;
                }
                Info.AgainstInfo.playerPrePlayCard = null;
            }
        }
        private void OnMouseOver()
        {
            //鼠标悬浮于卡牌上右键时可加载对应Card效果
            if (Input.GetMouseButtonUp(1) && IsCardVisible)
            {
                CardAbilityBoardManager.Manager.LoadCardFromGameCard(gameObject);
            }
            //鼠标悬浮于卡牌上左键时可换出Card面板
            if (Input.GetMouseButtonUp(0))
            {
                if (ThisCard.CurrentRegion == GameRegion.Grave)
                {
                    UiCommand.SetCardBoardOpen(CardBoardMode.Temp);
                    UiCommand.SetCardBoardTitle(ThisCard.CurrentOrientation == Orientation.Up ? "敌方墓地" : "我方墓地");
                    CardBoardCommand.ShowCardBoard(GameSystem.InfoSystem.AgainstCardSet[ThisCard.CurrentOrientation][ThisCard.CurrentRegion].ContainCardList, CardBoardMode.Temp, BoardCardVisible.AlwaysShow);
                }
                if (ThisCard.CurrentRegion == GameRegion.Deck && ThisCard.CurrentOrientation == Orientation.Down)
                {
                    UiCommand.SetCardBoardOpen(CardBoardMode.Temp);
                    UiCommand.SetCardBoardTitle("我方卡组");
                    CardBoardCommand.ShowCardBoard(GameSystem.InfoSystem.AgainstCardSet[ThisCard.CurrentOrientation][ThisCard.CurrentRegion].ContainCardList, CardBoardMode.Temp, BoardCardVisible.AlwaysShow);
                    //为了debug暂时以真实顺序排序
                    //.OrderBy(card => card.CardRank)
                    //.ThenBy(card => card.ShowPoint)
                    //.ToList(),
                }
            }
        }
        #region 卡牌刷新
        /// <summary>
        /// 刷新卡牌的可见性,卡牌改变位置和附加状态时触发
        /// </summary>
        public void RefreshCardVisible()
        {
            //窥视状态始终显示
            if (ThisCard[CardState.Pry]) IsCardVisible = true;
            //覆盖状态始终隐藏
            else if (ThisCard[CardState.Cover]) IsCardVisible = false;
            //回放模式下所有Card始终显示
            else if (AgainstInfo.IsReplayMode) IsCardVisible = true;
            //位于领袖,战场,墓地区域时始终显示
            //位于领袖时始终显示
            else if (ThisCard.BelongRow.gameRegion == GameRegion.Leader) IsCardVisible = true;
            else if (ThisCard.BelongRow.gameRegion == GameRegion.Used) IsCardVisible = true;
            //位于战场时始终显示
            else if (ThisCard.BelongRow.gameRegion == GameRegion.Water) IsCardVisible = true;
            else if (ThisCard.BelongRow.gameRegion == GameRegion.Fire) IsCardVisible = true;
            else if (ThisCard.BelongRow.gameRegion == GameRegion.Wind) IsCardVisible = true;
            else if (ThisCard.BelongRow.gameRegion == GameRegion.Soil) IsCardVisible = true;
            //位于墓地时始终显示
            else if (ThisCard.BelongRow.gameRegion == GameRegion.Grave) IsCardVisible = true;
            //位于卡组时始终不显示
            else if (ThisCard.BelongRow.gameRegion == GameRegion.Deck) IsCardVisible = false;
            //位于下方玩家时始终显示
            else if (ThisCard.BelongRow.orientation == Orientation.Down) IsCardVisible = true;
            //其余情况不显示
            else IsCardVisible = false;
        }
        /// <summary>
        /// 刷新卡牌的焦点信息（需要实时）,当焦点变动时触发
        /// </summary>
        public void RefreshFocus()
        {
            if (AgainstInfo.PlayerFocusCard == this)
            {
                CardMaterial.SetFloat("_IsFocus", 1);
                CardMaterial.SetFloat("_IsRed", 0);
            }
            else if (AgainstInfo.OpponentFocusCard == this)
            {
                CardMaterial.SetFloat("_IsFocus", 1);
                CardMaterial.SetFloat("_IsRed", 1);
            }
            else
            {
                CardMaterial.SetFloat("_IsFocus", 0);
            }
        }
        /// <summary>
        /// 刷新卡牌的ui信息,当点数变动时触发
        /// </summary>
        public void RefreshCardUi()
        {
            //数字
            if (ThisCard.ChangePoint > 0)
            {
                ThisCard.PointText.color = Color.green;
            }
            else if (ThisCard.ChangePoint < 0)
            {
                ThisCard.PointText.color = Color.red;
            }
            else
            {
                ThisCard.PointText.color = Color.black;
            }
            ThisCard.PointText.text = ThisCard.Type == CardType.Unit ? ThisCard.ShowPoint.ToString() : "";
            //字段
            for (int i = 0; i < 4; i++)
            {
                if (ThisCard.cardFields.Count > 4 && i == 3)
                {
                    //icon是省略号
                }
                else if (i < ThisCard.cardFields.Count)
                {
                    ThisCard.FieldIconContent.GetChild(i).GetComponent<Image>().sprite = Command.UiCommand.GetFieldAndStateSprite(ThisCard.cardFields.ToList()[i].Key);
                    ThisCard.FieldIconContent.GetChild(i).GetChild(0).GetComponent<Text>().text = ThisCard.cardFields.ToList()[i].Value.ToString();
                    ThisCard.FieldIconContent.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    ThisCard.FieldIconContent.GetChild(i).gameObject.SetActive(false);
                }
            }
            //状态
            for (int i = 0; i < 3; i++)
            {
                if (ThisCard.cardStates.Count > 3 && i == 2)
                {
                    //icon是省略号
                }
                else if (i < ThisCard.cardStates.Count)
                {
                    ThisCard.StateIconContent.GetChild(i).GetComponent<Image>().sprite = Command.UiCommand.GetFieldAndStateSprite(ThisCard.cardStates[i]);
                    ThisCard.StateIconContent.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    ThisCard.StateIconContent.GetChild(i).gameObject.SetActive(false);
                }
            }
            CardMaterial.SetFloat("_IsTemp", ThisCard.IsGray ? 0 : 1);
        }
        /// <summary>
        /// 刷新Card的位置和角度
        /// </summary>
        /// <param name="TargetPosition"></param>
        /// <param name="TargetEulers"></param>
        public void RefreshTransform()
        {
            //计算由系统控制卡牌的位置，还是由玩家手动操作
            if (ThisCard.IsSystemControlMove)
            {
                var rowInfo = ThisCard.BelongRow;
                bool IsSingle = rowInfo.gameRegion == GameRegion.Grave || rowInfo.gameRegion == GameRegion.Deck || rowInfo.gameRegion == GameRegion.Used;
                //根据目标区域中卡牌数量计算卡牌组整体的偏移量，保证所有卡牌居中在区域中间
                float Actual_Bias = IsSingle ? 0 : (Mathf.Min(ThisCard.BelongCardList.Count, 6) - 1) * 0.8f;
                //根据区域中卡牌数量计算每隔卡牌之间的间隔，保证卡牌少时不会太过分散
                float Actual_Interval = Mathf.Min(rowInfo.Range / ThisCard.BelongCardList.Count, 1.6f);
                int cardIndex = ThisCard.BelongCardList.IndexOf(ThisCard);

                Vector3 Actual_Offset_Up = rowInfo.rowPrefab.transform.up * (0.2f + cardIndex * 0.01f) * (ThisCard.isPrepareToPlay ? 1.1f : 1);
                //计算Card的落下前后的偏移距离
                Vector3 moveStepOver_Offset = ThisCard.isMoveStepOver ? Vector3.zero : Vector3.up;
                //计算抽卡的插入牌组前后的偏移距离
                Vector3 drawStepOver_Offset = ThisCard.isDrawStepOver ? Vector3.zero : -rowInfo.rowPrefab.transform.forward;
                //焦点注释在Card时Card回像屏幕移动产生一个放大即将打出效果
                Vector3 Actual_Offset_Forward = ThisCard.isPrepareToPlay ? -rowInfo.rowPrefab.transform.forward * 0.5f : Vector3.zero;
                //计算最终的Card位置的角度
                Vector3 TargetPosition = rowInfo.rowPrefab.transform.position + Vector3.left * (Actual_Interval * cardIndex - Actual_Bias) + Actual_Offset_Up + Actual_Offset_Forward + moveStepOver_Offset + drawStepOver_Offset;
                Vector3 TargetEuler = rowInfo.rowPrefab.transform.eulerAngles + new Vector3(0, 0, IsCardVisible ? 0 : 180);
                //如果位置触发不一致再触发绑定效果
                if (this.TargetPosition != TargetPosition || this.TargetEuler != TargetEuler)
                {
                    //Debug.LogError("重置移动目标");
                    this.TargetPosition = TargetPosition;
                    this.TargetEuler = TargetEuler;
                    currentMoveTime = 0;
                    float moveTotalTime = 0.5f;
                    _ = LerpMove();

                    async Task LerpMove()
                    {
                        while (true)
                        {
                            float process = IsImmediatelyMove ? 1 : currentMoveTime / (moveTotalTime * 1000);
                            IsImmediatelyMove = false;
                            transform.position = Vector3.Lerp(transform.position, this.TargetPosition, process * process);
                            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(this.TargetEuler), process);
                            currentMoveTime += 50;
                            await Task.Delay(50);
                            if (process >= 1) return;
                        }
                    }
                }
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, AgainstInfo.dragToPoint, 0.2f);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, IsCardVisible ? 0 : 180)), 0.2f);
            }
        }
        #endregion
        #region 卡牌UI交互效果
        //弹出点数或状态变动提示
        [Button]
        public async Task ShowTips(string text, Color color, bool isVertical = true)
        {
            if (isVertical)
            {
                text = string.Join("\n", text.ToString().ToCharArray());
            }
            cardTips.GetComponent<Text>().text = text;
            cardTips.GetComponent<Text>().color = color;

            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardTips.GetComponent<CanvasGroup>().alpha = process;
                cardTips.transform.localPosition = new Vector3(0, 0, -1 - process * 30);
            });
            await Task.Delay(800);
            await CustomThread.TimerAsync(0.4f, runAction: (process) =>
            {
                cardTips.GetComponent<CanvasGroup>().alpha = 1 - process;
                cardTips.transform.localPosition = new Vector3(0, 0, -1 - (1 - process) * 30);
            });
        }
        //弹出状态图标附加提示
        [Button]
        public async Task ShowStateIcon(CardState cardState)
        {
            cardIcon.GetComponent<Image>().sprite = UiCommand.GetFieldAndStateSprite(cardState);
            cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1);
            cardIcon.GetComponent<Image>().material.SetFloat("_Bias", 0);

            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - process * 30);
            });
            await Task.Delay(400);
            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = 1 - process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - (1 - process) * 30);
            });
        }
        //弹出状态图标清除提示
        [Button]
        public async Task ShowStateIconBreak(CardState cardState)
        {
            cardIcon.GetComponent<Image>().sprite = UiCommand.GetFieldAndStateSprite(cardState);
            cardIcon.GetComponent<Image>().material.SetFloat("_Bias", 0);
            cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1);

            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - process * 30);
            });
            await Task.Delay(200);
            await CustomThread.TimerAsync(0.1f, runAction: (process) =>
            {
                cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1 - process * 1.5f);
                Debug.Log(cardIcon.GetComponent<Image>().material.GetFloat("_BreakStrength"));
                cardIcon.GetComponent<Image>().material.SetFloat("_Bias", process / 5);
            });
            await Task.Delay(200);
            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = 1 - process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - (1 - process) * 30);
            });
        }
        //弹出字段图标附加提示
        [Button]
        public async Task ShowFieldIcon(CardField cardField)
        {
            cardIcon.GetComponent<Image>().sprite = UiCommand.GetFieldAndStateSprite(cardField);
            cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1);
            cardIcon.GetComponent<Image>().material.SetFloat("_Bias", 0);

            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - process * 30);
            });
            await Task.Delay(800);
            await CustomThread.TimerAsync(0.4f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = 1 - process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - (1 - process) * 30);
            });
        }
        //弹出字段图标清除提示
        [Button]
        public async Task ShowFieldIconBreak(CardField cardField)
        {
            cardIcon.GetComponent<Image>().sprite = UiCommand.GetFieldAndStateSprite(cardField);
            cardIcon.GetComponent<Image>().material.SetFloat("_Bias", 0);
            cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1);

            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - process * 30);
            });
            await Task.Delay(200);
            await CustomThread.TimerAsync(0.1f, runAction: (process) =>
            {
                cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1 - process * 1.5f);
                Debug.Log(cardIcon.GetComponent<Image>().material.GetFloat("_BreakStrength"));
                cardIcon.GetComponent<Image>().material.SetFloat("_Bias", process / 5);
            });
            await Task.Delay(200);
            await CustomThread.TimerAsync(0.4f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = 1 - process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - (1 - process) * 30);
            });
        }
        [Button("除外")]
        public async Task CreatGapAsync()
        {
            gap.SetActive(true);
            await CustomThread.TimerAsync(0.8f, runAction: (process) =>
            {
                GapMaterial.SetFloat("_gapWidth", Mathf.Lerp(10f, 1.5f, process));
            });
            await Task.Delay(1200);
            transform.GetChild(0).gameObject.SetActive(false);
            await CustomThread.TimerAsync(0.6f, runAction: (process) =>
            {
                GapMaterial.SetFloat("_gapWidth", Mathf.Lerp(1.5f, 10f, process));
                CardMaterial.SetFloat("_gapWidth", Mathf.Lerp(1.5f, 10f, process));

            });
            gap.SetActive(false);
            ThisCard.BelongCardList.Remove(ThisCard);
            Destroy(gameObject);
        }
        [Button]
        public async Task Test()
        {
            await ShowTips("+1", Color.green, false);
            await ShowTips("-10", Color.red, false);
            await ShowTips("-3", Color.white, false);
            await ShowTips("支柱", Color.cyan);
            await ShowTips("活力", Color.yellow);
            await ShowTips("封印", new Color(1, 1, 1));
            await ShowTips("中毒", new Color(1, 0, 1));
            await ShowTips("掉SAN", new Color(0, 0.2f, 0));
            await ShowTips("魅了", new Color(1, 0, 0));
        }
        #endregion

    }
}