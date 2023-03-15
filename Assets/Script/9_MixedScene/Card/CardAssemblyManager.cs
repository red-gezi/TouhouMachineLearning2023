using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Manager
{

    class CardAssemblyManager : MonoBehaviour
    {
        //是否使用本地卡牌版本，若为是则会卡顿
        static bool isUseLocalAssembly = true;
        /// <summary>
        /// 已下载的历史配置文件
        /// </summary>
        static Dictionary<string, CardConfig> cardConfigs = new Dictionary<string, CardConfig>();
        /// <summary>
        /// 当前使用的配置文件
        /// </summary>
        static CardConfig currentConfig;
        //当前使用的卡牌代码
        static Assembly currentCardScripts;
        //当前使用的卡牌信息
        static List<CardModel> currenttSingleCardInfos;
        static List<CardModel> currentMultiCardInfos;
        //最新版本的卡牌信息
        [ShowInInspector]
        static List<CardModel> lastSingleCardInfos;
        [ShowInInspector]
        static List<CardModel> lastMultiCardInfos;
        //最新版本的卡牌代码
        [ShowInInspector]
        static Assembly lastCardScripts;

        //获取当前引用卡牌数据的日期
        public static string GetCurrentConfigDate => currentConfig.Version;
        [ShowInInspector]
        public static List<CardModel> CurrentSingleCardInfos => currenttSingleCardInfos;
        [ShowInInspector]
        public static List<CardModel> CurrentMultiCardInfos => currentMultiCardInfos;
        [ShowInInspector]
        public static List<CardModel> LastSingleCardInfos => lastSingleCardInfos;
        [ShowInInspector]
        public static List<CardModel> LastMultiCardInfos => lastMultiCardInfos;
        //设置
        public static async Task SetCurrentAssembly(string verison)
        {
            //获取服务器中最新的版本号
            //若未指定特定版本则代表查询最新的版本数据
            //如果不存在该版本数据则联网进行下载“目标版本数据”
            //装载“目标版本数据”到“当前版本数据”中，同时若未指定版本号，则将“当前版本数据”视为“最新版本数据”
            //当普通对战时，卡牌数据从“最新版本数据”中获取，当对战回放时，卡牌数据从“当前版本数据”中获取

            var lastVerison = await Command.NetCommand.GetCardConfigsVersionAsync();
            //判断加载的目标版本
            var targetVerison = (verison == "") ? lastVerison : verison;
            if (cardConfigs.Keys.Contains(targetVerison))
            {
                currentConfig = cardConfigs[targetVerison];
                Debug.Log($"当前已是最新版本{lastVerison}，无需更新");
            }
            else
            {
                Debug.Log($"下载{lastVerison}版本数据");
                currentConfig = await Command.NetCommand.DownloadCardConfigsAsync(targetVerison);
                cardConfigs[currentConfig.Version] = currentConfig;
            }
            //currenttSingleCardInfos = Encoding.UTF8.GetString(currentConfig.SingleCardFileData).ToObject<List<CardModel>>().SelectList(card => card.Init(true, isFromAssetBundle: !Application.isEditor));
            currenttSingleCardInfos = new();
            currentMultiCardInfos = Encoding.UTF8.GetString(currentConfig.MultiCardFileData).ToObject<List<CardModel>>().SelectList(card => card.Init(false));
            currentCardScripts = Assembly.Load(currentConfig.AssemblyFileData);

            if (verison == "")
            {
                lastSingleCardInfos = currenttSingleCardInfos;
                lastMultiCardInfos = currentMultiCardInfos;
            }
            Debug.Log("卡牌版本加载完毕");


            //识别日期编号
            //假如是空的，则先查询是否存在该配置文件
            //假如存在则加载
            //假如不存在下载最后一个配置文件
            //不然下载指定配置文件

            //如果是请求最新的卡牌配置，则直接更新，确保每次对局都是最新版数据
            //步骤
            //查询最新的版本编号
            //检查是否已存在
            //如果已存在直接使用
            //如果不存在则从服务器拉取最新版本
            //为""则获取最新版本的卡牌数据
            //if (targetVerison == "")
            //{
            //    var targetConfig = cardConfigs.Values.ToList().FirstOrDefault(config => config.UpdataTime.ToString() == lastVerison);
            //    if (targetConfig != null)
            //    {
            //        currentConfig = targetConfig;
            //        Debug.Log($"当前已是最新版本{lastVerison}，无需更新");
            //    }
            //    else
            //    {
            //        await DownloadConfig(targetVerison);
            //    }
            //    lastCardScripts = Assembly.Load(currentConfig.AssemblyFileData);
            //    //加载卡牌信息与卡牌图片
            //    currenttSingleCardInfos = Encoding.UTF8.GetString(currentConfig.SingleCardFileData).ToObject<List<CardModel>>().SelectList(card => card.Init(true, isFromAssetBundle: !Application.isEditor));
            //    currentMultiCardInfos = Encoding.UTF8.GetString(currentConfig.MultiCardFileData).ToObject<List<CardModel>>().SelectList(card => card.Init(false, isFromAssetBundle: !Application.isEditor));

            //    lastSingleCardInfos = currenttSingleCardInfos;
            //    lastMultiCardInfos = currentMultiCardInfos;
            //}
            ////否则获取指定日期的卡牌数据
            //else
            //{
            //    if (cardConfigs.Keys.Contains(targetVerison))
            //    {
            //        currentConfig = cardConfigs[targetVerison];
            //    }
            //    else
            //    {
            //        await DownloadConfig(targetVerison);
            //    }
            //    //加载卡牌信息与卡牌图片
            //    currenttSingleCardInfos = Encoding.UTF8.GetString(currentConfig.SingleCardFileData).ToObject<List<CardModel>>().SelectList(card => card.Init(true, isFromAssetBundle: !Application.isEditor));
            //    currentMultiCardInfos = Encoding.UTF8.GetString(currentConfig.MultiCardFileData).ToObject<List<CardModel>>().SelectList(card => card.Init(false, isFromAssetBundle: !Application.isEditor));
            //}
            //currentCardScripts = Assembly.Load(currentConfig.AssemblyFileData);

            //Debug.Log("下载完成");
            ////下载指定版本数据

            //static async Task DownloadConfig(string date)
            //{
            //    currentConfig = await Command.NetCommand.DownloadCardConfigsAsync(date);
            //    cardConfigs[currentConfig.Version] = currentConfig;
            //}
        }
        //public static async Task<string> GetCardConfigsVersionAsync() => await Command.NetCommand.GetCardConfigsVersionAsync();
        /// <summary>
        /// 从加载的dll获得指定id的卡牌脚本
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Type GetCardScript(string id) => currentCardScripts.GetType("TouhouMachineLearningSummary.CardSpace.Card" + id);
        /// <summary>
        /// 获取当前加载版本的卡牌信息，用于在对局内回放指定版本的牌库，卡组信息
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static CardModel GetCurrentCardInfos(string cardID)
        {
            CardModel cardModelInfo = new List<CardModel>()
                .Union(CurrentSingleCardInfos)
                .Union(CurrentMultiCardInfos)
                .FirstOrDefault(info => info.cardID == cardID);
            if (cardModelInfo == null)
            {
                Debug.LogError("卡牌" + cardID + "查找失败");
            }
            return cardModelInfo;
        }
        /// <summary>
        /// 获取最新版本的卡牌信息，用于在对局外获取最新的牌库，卡组信息
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static CardModel GetLastCardInfo(string cardID)
        {
            CardModel cardModelInfo = new List<CardModel>()
                .Union(LastSingleCardInfos)
                .Union(LastMultiCardInfos)
                .FirstOrDefault(info => info.cardID == cardID);
            if (cardModelInfo == null)
            {
                Debug.LogError("卡牌" + cardID + "查找失败");
            }
            return cardModelInfo;
        }
    }

}
