using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using TouhouMachineLearningSummary.Control;
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
        public static void InitDrawCardPage()
        {
            Info.PageComponentInfo.Instance.FaithComponent.SetActive(false);
            Info.PageComponentInfo.Instance.OpenCardComponent.SetActive(false);
        }
        /// ///////////////////////////////////////////////信念选择相关操作//////////////////////////////////////////
        //打开信念选择组件
        public static void ShowFaithComponent() => Info.PageComponentInfo.Instance.FaithComponent.SetActive(true);
        //关闭信念选择组件
        public static void CloseFaithComponent() => Info.PageComponentInfo.Instance.FaithComponent.SetActive(false);
        //添加信念
        public static void AddFaith(int index)
        {
            if (Info.PageComponentInfo.SelectFaiths.Count < 5)
            {
                var targetFaiths = Info.AgainstInfo.OnlineUserInfo.Faiths[index];
                //当点击的信念剩余数量小于同类型已选的信念数量才会被添加
                if (Info.PageComponentInfo.SelectFaiths.Count(faith => faith.BelongUser == targetFaiths.BelongUser) < targetFaiths.Count)
                {
                    Info.PageComponentInfo.SelectFaiths.Add(targetFaiths);
                }
                else
                {
                    //播放无效音效
                }
            }

        }
        //移除信念
        public static void RemoveFaith(int index)
        {
            Info.PageComponentInfo.SelectFaiths.RemoveAt(index);
        }
        //锁定信念
        public static void ChangeLockFaith(int index)
        {
            Info.AgainstInfo.OnlineUserInfo.Faiths[index].IsLock = !Info.AgainstInfo.OnlineUserInfo.Faiths[index].IsLock;
            //更新面板的锁
        }
        //快速选择信念
        public static void QuickSelectFaith()
        {
            if (Info.PageComponentInfo.SelectFaiths.Count == 5) return;
            for (int i = 0; i < Info.AgainstInfo.OnlineUserInfo.Faiths.Count; i++)
            {
                var targetFaiths = Info.AgainstInfo.OnlineUserInfo.Faiths[i];

                //计算该信仰未被使用的数量
                var unusedNum = targetFaiths.Count - Info.PageComponentInfo.SelectFaiths.Count(faith => faith.BelongUser == targetFaiths.BelongUser);
                Debug.Log("第" + i + "项未被使用的数量为" + unusedNum);
                for (int j = 0; j < unusedNum; i++)
                {
                    Info.PageComponentInfo.SelectFaiths.Add(targetFaiths);
                    if (Info.PageComponentInfo.SelectFaiths.Count == 5) return;
                }
            }
        }
        public static void QuickDrawCard(int drawCardCount)
        {
            List<Faith> drawCards = new();
            for (int i = 0; i < drawCardCount; i++)
            {
                drawCards.Add(new Faith());
            }
            DrawCard(drawCards);
        }
        //抽卡
        public static async void DrawCard(List<Faith> selectFaiths)
        {

            //如果没有选择信念，弹窗提示
            if (!selectFaiths.Any())
            {

                return;
            }
            //向服务器发送请求，等待结果
            List<string> drawCardId = await Command.NetCommand.DrawCardAsync(UserInfoManager.UID, UserInfoManager.Password, selectFaiths);
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
        /// <summary>
        /// 获得开卡界面中的所有卡牌UI组件
        /// </summary>
        /// <returns></returns>
        static List<Info.GachaBoardInfo> GetOpenCardInfos() => Manager.GachaManager.Instance.singleOpenCardInfos;
        //初始化配置每个开卡组件
        public static void InitOpenCardComponent(List<string> drawCardId)
        {
            for (int i = 0; i < 5; i++)
            {
                var singleOpenCardInfo = GetOpenCardInfos()[i];
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
            RefreshOpenCardComponent();
        }
        //显示开卡组件
        public static void ShowOpenCardComponent() => Info.PageComponentInfo.Instance.OpenCardComponent.SetActive(true);
        //关闭开卡组件
        public static void CloseOpenCardComponent() => Info.PageComponentInfo.Instance.OpenCardComponent.SetActive(false);
        //刷新组件状态
        public static void RefreshOpenCardComponent()
        {
            if (GetOpenCardInfos().Any(info => info.state == 1))
            {
                //进入等待开卡状态
                Manager.GachaManager.Instance.showAllCardsButton.SetActive(true);
                Manager.GachaManager.Instance.turnAllCardsButton.SetActive(false);
                Manager.GachaManager.Instance.closeButton.SetActive(false);
                Manager.GachaManager.Instance.drawOneCardButton.SetActive(false);
                Manager.GachaManager.Instance.drawMoreCardButton.SetActive(false);
                return;
            }
            if (GetOpenCardInfos().Any(info => info.state == 2))
            {
                //进入等待翻转状态
                Manager.GachaManager.Instance.showAllCardsButton.SetActive(false);
                Manager.GachaManager.Instance.turnAllCardsButton.SetActive(true);
                Manager.GachaManager.Instance.closeButton.SetActive(false);
                Manager.GachaManager.Instance.drawOneCardButton.SetActive(false);
                Manager.GachaManager.Instance.drawMoreCardButton.SetActive(false);
                return;
            }
            //进入等待关闭状态
            Manager.GachaManager.Instance.showAllCardsButton.SetActive(false);
            Manager.GachaManager.Instance.turnAllCardsButton.SetActive(false);
            Manager.GachaManager.Instance.closeButton.SetActive(true);
            Manager.GachaManager.Instance.drawOneCardButton.SetActive(true);
            Manager.GachaManager.Instance.drawMoreCardButton.SetActive(true);
        }
        //显露抽的的卡牌卡背
        public static async void ShowCard(Info.GachaBoardInfo singleOpenCardInfo)
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
        public static async void TurnCard(Info.GachaBoardInfo singleOpenCardInfo)
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
