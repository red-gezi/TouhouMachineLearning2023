using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Command
{
    class GachaCommand
    {
        //初始化抽卡面板状态
        public static async void InitGachaComponent()
        {
            Info.GachaInfo.Instance.cardPoolComponent.SetActive(true);
            Info.GachaInfo.Instance.openCardComponent.SetActive(false);
            Info.GachaInfo.Instance.faithBagComponent.SetActive(false);
            RefreshSelectFaith();
            Info.GachaInfo.singleOpenCardInfos.Clear();
            for (int i = 0; i < 5; i++)
            {
                Info.GachaInfo.singleOpenCardInfos.Add(Info.GachaInfo.Instance.GachaBoardGroup.GetChild(i).GetComponent<Info.GachaCardInfo>());
            }
            //清空选择槽
            await CustomThread.TimerAsync(0.5f, process =>
            {
                Info.GachaInfo.Instance.cardPoolComponent.transform.localPosition = new Vector3(50 * (1 - process), 0, 0);
                Info.GachaInfo.Instance.cardPoolComponent.GetComponent<CanvasGroup>().alpha = process;
            });
        }
        /// ///////////////////////////////////////////////信念选择相关操作//////////////////////////////////////////
        //打开信念背包组件
        public static void ShowFaithBag()
        {
            Info.GachaInfo.Instance.faithBagComponent.SetActive(true);
            //Info.GachaComponentInfo.Instance.closeFaithBagButton.SetActive(true);
            var currentFaiths = Info.AgainstInfo.OnlineUserInfo.Faiths;
            if (!currentFaiths.Any())
            {
                currentFaiths.Add(new Faith() { BelongUserUID = "0", Count = 5 });
                currentFaiths.Add(new Faith() { BelongUserUID = "1", Count = 45 });
                currentFaiths.Add(new Faith() { BelongUserUID = "2", Count = 225 });
                currentFaiths.Add(new Faith() { BelongUserUID = "3", Count = 215 });
                currentFaiths.Add(new Faith() { BelongUserUID = "4", Count = 3 });
            }

            //创建不足的背包物体
            int creatItemCount = currentFaiths.Count - Info.GachaInfo.Instance.faithBagGroup.childCount;
            for (int i = 0; i < creatItemCount; i++)
            {
                var newItem = GameObject.Instantiate(Info.GachaInfo.Instance.faithBagItem, Info.GachaInfo.Instance.faithBagGroup);
                newItem.GetComponent<Image>().material = new(newItem.GetComponent<Image>().material);
            }
            for (int i = 0; i < Info.GachaInfo.Instance.faithBagGroup.childCount; i++)
            {
                Transform targetItem = Info.GachaInfo.Instance.faithBagGroup.GetChild(i);

                if (i < currentFaiths.Count)
                {
                    targetItem.gameObject.SetActive(true);
                    targetItem.GetComponent<Image>().material.mainTexture = currentFaiths[i].GetFaithIconTexture();
                    targetItem.GetChild(2).GetComponent<TextMeshProUGUI>().text = currentFaiths[i].Count.ToString();
                }
                else
                {
                    targetItem.gameObject.SetActive(false);
                }
            }
            if (true)
            {

            }

        }

        //关闭信念背包组件
        public static void CloseFaithBag()
        {
            Info.GachaInfo.Instance.faithBagComponent.SetActive(false);
        }

        //添加信念
        public static void AddFaith(Transform item)
        {
            int index = 0;
            for (int i = 0; i < Info.GachaInfo.Instance.faithBagGroup.childCount; i++)
            {
                if (Info.GachaInfo.Instance.faithBagGroup.GetChild(i) == item)
                {
                    index = i;
                    break;
                }
            }
            if (Info.GachaInfo.SelectFaiths.Count < 5)
            {
                var targetFaiths = Info.AgainstInfo.OnlineUserInfo.Faiths[index];
                //当点击的信念剩余数量小于同类型已选的信念数量才会被添加
                if (Info.GachaInfo.SelectFaiths.Count(faith => faith.BelongUserUID == targetFaiths.BelongUserUID) < targetFaiths.Count)
                {
                    Info.GachaInfo.SelectFaiths.Add(targetFaiths);
                }
                else
                {
                    //播放无效音效
                }
            }
            RefreshSelectFaith();
        }
        //移除信念
        public static void RemoveFaith(Transform item)
        {
            int index = 0;
            for (int i = 0; i < 5; i++)
            {
                if (Info.GachaInfo.Instance.faithSelectGroup.GetChild(i) == item)
                {
                    index = i;
                    break;
                }
            }
            if (Info.GachaInfo.SelectFaiths.Count>index)
            {
                Info.GachaInfo.SelectFaiths.RemoveAt(index);
            }
            
            RefreshSelectFaith();
        }
        //刷新信念选择器
        public static void RefreshSelectFaith()
        {
            for (int i = 0; i < 5; i++)
            {
                var IconUI = Info.GachaInfo.Instance.faithSelectGroup.GetChild(i).GetChild(0);
                if (i < Info.GachaInfo.SelectFaiths.Count)
                {
                    //设置图片
                    IconUI.gameObject.SetActive(true);
                    IconUI.GetComponent<Image>().sprite = Info.GachaInfo.SelectFaiths[i].GetFaithIconSprite();
                }
                else
                {
                    IconUI.gameObject.SetActive(false);
                }
            }
        }
        //锁定信念
        public static void ChangeLockFaith(int index)
        {
            Info.AgainstInfo.OnlineUserInfo.Faiths[index].IsLock = !Info.AgainstInfo.OnlineUserInfo.Faiths[index].IsLock;
            //更新面板的锁
        }
        //快速选择信念
        public static void QuickSelectFaith(int drawCardCount)
        {
            //已经满足了则直接跳过
            if (Info.GachaInfo.SelectFaiths.Count == drawCardCount) return;
            for (int i = 0; i < Info.AgainstInfo.OnlineUserInfo.Faiths.Count; i++)
            {
                var targetFaiths = Info.AgainstInfo.OnlineUserInfo.Faiths[i];
                if (!targetFaiths.IsLock)
                {
                    //计算该信仰未被使用的数量
                    var unusedNum = targetFaiths.Count - Info.GachaInfo.SelectFaiths.Count(faith => faith.BelongUserUID == targetFaiths.BelongUserUID);
                    Debug.Log("第" + i + "项未被使用的数量为" + unusedNum);
                    for (int j = 0; j < unusedNum; i++)
                    {
                        Info.GachaInfo.SelectFaiths.Add(targetFaiths);
                        if (Info.GachaInfo.SelectFaiths.Count == drawCardCount) return;
                    }
                }
            }
        }
        public static void QuickDrawCard(int drawCardCount)
        {
            //List<Faith> currentFaiths = new();
            //currentFaiths.Add(new Faith() { BelongUserUID = "0", Count = 5 });
            //currentFaiths.Add(new Faith() { BelongUserUID = "1", Count = 45 });
            //currentFaiths.Add(new Faith() { BelongUserUID = "2", Count = 225 });
            //currentFaiths.Add(new Faith() { BelongUserUID = "3", Count = 215 });
            //currentFaiths.Add(new Faith() { BelongUserUID = "4", Count = 3 });
            //DrawCard(currentFaiths.Take(drawCardCount).ToList()); 
            QuickSelectFaith(drawCardCount);
            DrawCard(); 
        }
        //抽卡
        public static async void DrawCard()
        {

            //如果没有选择信念，弹窗提示
            if (!Info.GachaInfo.SelectFaiths.Any())
            {

                return;
            }
            //向服务器发送请求，等待结果
            List<string> drawCardId = await Command.NetCommand.DrawCardAsync(UserInfoManager.UID, UserInfoManager.Password, Info.GachaInfo.SelectFaiths);
            Debug.Log(drawCardId.ToJson());
            //如果服务器扣除失败,弹窗提示失败
            if (false)
            {

                return;
            }
            //初始化卡牌展示界面
            InitOpenCardComponent(drawCardId);
            //展示抽卡结果
            ShowOpenCardComponent();

        }
        /// ///////////////////////////////////////////////开卡选择相关操作//////////////////////////////////////////
        //初始化配置每个开卡组件
        public static void InitOpenCardComponent(List<string> drawCardId)
        {
            for (int i = 0; i < 5; i++)
            {
                var singleOpenCardInfo = Info.GachaInfo.singleOpenCardInfos[i];
                bool isActive = i < drawCardId.Count;
                singleOpenCardInfo.gameObject.SetActive(isActive);
                //激活的卡牌替换卡图
                if (isActive)
                {
                    var cardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(drawCardId[i]);
                    //第一次触发时初始化材质球
                    if (singleOpenCardInfo.cardMaterial == null)
                    {
                        singleOpenCardInfo.cardMaterial = new Material(singleOpenCardInfo.card.GetComponent<Image>().material);
                        singleOpenCardInfo.card.GetComponent<Image>().material = singleOpenCardInfo.cardMaterial;
                        singleOpenCardInfo.cardNameUi.GetComponent<Image>().material = new Material(singleOpenCardInfo.cardNameUi.GetComponent<Image>().material);
                        singleOpenCardInfo.cardCountUi.GetComponent<Image>().material = new Material(singleOpenCardInfo.cardCountUi.GetComponent<Image>().material);

                    }
                    singleOpenCardInfo.faithUserIcon.GetComponent<Image>().sprite = Info.GachaInfo.SelectFaiths[i].GetFaithIconSprite();
                    singleOpenCardInfo.faithUserUi.GetComponent<TextMeshProUGUI>().text = Info.GachaInfo.SelectFaiths[i].userName;
                    singleOpenCardInfo.state = 1;
                    singleOpenCardInfo.cardMaterial.SetTexture("_Face", cardInfo.cardFace);
                    //singleOpenCardInfo.cardMaterial.SetTexture("_Back", cardInfo.cardBack);
                    singleOpenCardInfo.card.transform.localEulerAngles = new Vector3(0, 180, 0);
                    singleOpenCardInfo.cardMaterial.SetFloat("_process", 345);
                    singleOpenCardInfo.cardNameUi.GetComponent<Image>().material.SetFloat("_progress", 2);
                    singleOpenCardInfo.cardCountUi.GetComponent<Image>().material.SetFloat("_progress", 2);
                    var s = singleOpenCardInfo.cardNameUi.transform.GetChild(0);
                    singleOpenCardInfo.cardNameUi.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = false;
                    singleOpenCardInfo.cardNameUi.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cardInfo.TranslateName;
                    singleOpenCardInfo.cardCountUi.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = false;
                    singleOpenCardInfo.cardCountUi.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "New";

                }
                else
                {
                    singleOpenCardInfo.state = 0;
                }
            }
            Info.GachaInfo.SelectFaiths.Clear();
            RefreshOpenCardComponent();
        }
        //显示开卡组件
        public static void ShowOpenCardComponent() => Info.GachaInfo.Instance.openCardComponent.SetActive(true);
        //关闭开卡组件
        public static void CloseOpenCardComponent() => Info.GachaInfo.Instance.openCardComponent.SetActive(false);
        //刷新组件状态
        public static void RefreshOpenCardComponent()
        {
            if (Info.GachaInfo.singleOpenCardInfos.Any(info => info.state == 1))
            {
                //进入等待开卡状态
                Info.GachaInfo.Instance.showAllCardsButton.SetActive(true);
                Info.GachaInfo.Instance.turnAllCardsButton.SetActive(false);
                Info.GachaInfo.Instance.closeButton.SetActive(false);
                Info.GachaInfo.Instance.drawOneCardButton.SetActive(false);
                Info.GachaInfo.Instance.drawMoreCardButton.SetActive(false);
                return;
            }
            if (Info.GachaInfo.singleOpenCardInfos.Any(info => info.state == 2))
            {
                //进入等待翻转状态
                Info.GachaInfo.Instance.showAllCardsButton.SetActive(false);
                Info.GachaInfo.Instance.turnAllCardsButton.SetActive(true);
                Info.GachaInfo.Instance.closeButton.SetActive(false);
                Info.GachaInfo.Instance.drawOneCardButton.SetActive(false);
                Info.GachaInfo.Instance.drawMoreCardButton.SetActive(false);
                return;
            }
            //进入等待关闭状态
            Info.GachaInfo.Instance.showAllCardsButton.SetActive(false);
            Info.GachaInfo.Instance.turnAllCardsButton.SetActive(false);
            Info.GachaInfo.Instance.closeButton.SetActive(true);
            Info.GachaInfo.Instance.drawOneCardButton.SetActive(true);
            Info.GachaInfo.Instance.drawMoreCardButton.SetActive(true);
        }
        //显露抽的的卡牌卡背
        public static async void ShowCard(Info.GachaCardInfo singleOpenCardInfo)
        {

            //判断是否可以点破信念使卡牌显形
            if (singleOpenCardInfo.state == 1)
            {
                singleOpenCardInfo.state = 2;
                singleOpenCardInfo.faithBrokenParticle.Play();
                await Task.Delay(1500);
                singleOpenCardInfo.cardGenerateParticle.Play();
                await CustomThread.TimerAsync(1, process =>
                {
                    singleOpenCardInfo.cardMaterial.SetFloat("_process", Mathf.Lerp(345, 0, process));
                    singleOpenCardInfo.cardGenerateParticle.transform.localPosition = Vector3.Lerp(new(0, 360, 0), new Vector3(0, -345, 0), process);
                });
                singleOpenCardInfo.cardGenerateParticle.Stop();
            }
            //刷新组件状态
            RefreshOpenCardComponent();
        }
        //翻转生成的卡牌真身
        public static async void TurnCard(Info.GachaCardInfo singleOpenCardInfo)
        {
            Debug.Log("翻开");
            // 判断是否可以翻开
            if (singleOpenCardInfo.state == 2)
            {
                singleOpenCardInfo.state = 3;
                //刷新组件状态
                RefreshOpenCardComponent();
                //反转卡牌
                await CustomThread.TimerAsync(0.4f, process =>
                {
                    singleOpenCardInfo.card.transform.localEulerAngles = new Vector3(0, 90 + 90 * (1 - process), 0);
                    singleOpenCardInfo.card.transform.localPosition = new Vector3(0, 0, process * -256);
                });
                await CustomThread.TimerAsync(0.4f, process =>
                {
                    singleOpenCardInfo.card.transform.localEulerAngles = new Vector3(0, 90 * (1 - process), 0);
                    singleOpenCardInfo.card.transform.localPosition = new Vector3(0, 0, (1 - process) * -256);
                });
                //显示卡牌数量和名字UI
                await CustomThread.TimerAsync(0.6f, process =>
                {
                    singleOpenCardInfo.cardNameUi.GetComponent<Image>().material.SetFloat("_progress", Mathf.Lerp(2, 0, process));
                    singleOpenCardInfo.cardCountUi.GetComponent<Image>().material.SetFloat("_progress", Mathf.Lerp(2, 0, process));
                });
                //显示卡牌数量和名字文字
                singleOpenCardInfo.cardNameUi.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = true;
                singleOpenCardInfo.cardCountUi.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = true;


            }
        }
    }
}
