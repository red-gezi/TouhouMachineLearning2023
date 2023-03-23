using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Config;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Command
{
    class OpenCardCommand
    {
        //初始化抽卡面板状态
        public static void Init()
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
        //抽卡
        public static void DrawCard()
        {
            //向服务器发送请求，等待结果
            //如果没有选择信念，弹窗提示
            if (true)
            {

            }
            //如果服务器扣除失败,弹窗提示失败
            if (true)
            {

            }
            //初始化卡牌展示界面
            else
            {
                int drawCardNum = 5;
                //等待抽卡结果
                InitOpenCardComponent(drawCardNum);
                ShowOpenCardComponent();
            }

        }
        /// ///////////////////////////////////////////////开卡选择相关操作//////////////////////////////////////////

        //初始化开卡组件
        public static void InitOpenCardComponent(int drawCardNum)
        {

        }
        public static async void FaithBroken(Info.SingleOpenCardInfo singleOpenCardInfo)
        {
            //判断是否可以点破信念使卡牌显形
            if (singleOpenCardInfo.state == 1)
            {
                singleOpenCardInfo.faithBrokenParticle.Play();
                await Task.Delay(1500);
                singleOpenCardInfo.cardGenerateParticle.Play();
                await CustomThread.TimerAsync(1, process =>
                {
                    singleOpenCardInfo.cardMaterial.SetFloat("_process", Mathf.Lerp(345, 0, process));
                    singleOpenCardInfo.cardGenerateParticle.transform.localPosition = Vector3.Lerp(new(0, 360, 0), new Vector3(0, -345, 0), process);
                });
                singleOpenCardInfo.cardGenerateParticle.Stop();
                singleOpenCardInfo.state = 2;
            }
            //刷新组件状态
            RefreshOpenCardComponent();
        }
        //刷新组件状态
        public static void RefreshOpenCardComponent()
        {
            if (Manager.OpenCardManager.Instance.singleOpenCardInfos.Any(info=>info.state==1))
            {
                if (Manager.OpenCardManager.Instance.singleOpenCardInfos.Any(info => info.state == 2))
                {
                    //进入等待关闭状态
                }
                else
                {
                    //进入等待翻转状态
                }
            }
            else
            {
                //进入等待开卡状态
            }
        }

        //显示开卡组件
        public static void ShowOpenCardComponent() => Info.PageComponentInfo.Instance.OpenCardComponent.SetActive(true);
        //关闭信念选择组件
        public static void CloseOpenCardComponent() => Info.PageComponentInfo.Instance.OpenCardComponent.SetActive(false);
        //消耗信念创造出卡牌
        public static void ClickFaith()
        {
            //播放动画
            //信念碎裂，粒子上移

            //卡牌
        }
        //展现出生成的卡牌真身
        public static void ShowCard()
        {

        }
    }
}
