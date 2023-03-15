using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    class TitleCommand
    {
        /// <summary>
        /// 加载称号数据文件
        /// </summary>
        public static void Load()
        {
            string titleData = Application.isEditor ? File.ReadAllText(@"Assets\GameResources\GameData\Title.json") : AssetBundleCommand.Load<TextAsset>("GameData", "Title").text;
            Info.TitleInfo.Titles = titleData.ToObject<List<TitleModel>>();
        }
        /// <summary>
        /// 玩家解锁指定称号
        /// </summary>
        /// <param name="tag"></param>
        public static async void Unlock(string tag)
        {
            if (!Info.AgainstInfo.OnlineUserInfo.UnlockTitleTags.Contains(tag))
            {
                var tempUnlockTitleTags = Info.AgainstInfo.OnlineUserInfo.UnlockTitleTags.Clone();
                tempUnlockTitleTags.Add(tag);
                bool result = await Command.NetCommand.UpdateInfoAsync(GameEnum.UpdateType.UnlockTitles, tempUnlockTitleTags);
                if (result)
                {
                    Info.AgainstInfo.OnlineUserInfo.UnlockTitleTags = tempUnlockTitleTags;
                }

            }
        }
        /// <summary>
        /// 获得所有已解锁的前缀/后缀称号，1开头为前缀，2开头为后缀
        /// </summary>
        public static List<TitleModel> GetUnlockTitles(bool isPrefix) => Info.TitleInfo.Titles
            .Where(title => Info.AgainstInfo.OnlineUserInfo.UnlockTitleTags.Contains(title.tag))
            .Where(title => title.tag.StartsWith(isPrefix ? "1" : "2"))
            .ToList();
        /// <summary>
        /// 获得玩家当前称号
        /// </summary>
        public static string GetCurrentTitle()
        {
            //临时更新下
            Load();
            string prefix = Info.TitleInfo.Titles.FirstOrDefault(title => title.tag == Info.AgainstInfo.OnlineUserInfo.UsePrefixTitleTag).titles[Manager.TranslateManager.currentLanguage];
            string suffix = Info.TitleInfo.Titles.FirstOrDefault(title => title.tag == Info.AgainstInfo.OnlineUserInfo.UseSuffixTitleTag).titles[Manager.TranslateManager.currentLanguage];
            return prefix + "的" + suffix;
        }
        /// <summary>
        /// 保存玩家当前使用的称号
        /// </summary>
        /// <param name="tag"></param>
        public static async void SaveTitle(string tag)
        {
            UpdateType updateType = tag.StartsWith("1") ? UpdateType.PrefixTitle : UpdateType.SuffixTitle;
            bool result = await Command.NetCommand.UpdateInfoAsync(updateType, tag);
            if (result)
            {
                Debug.Log("称号更新" + result);
                if (updateType == UpdateType.PrefixTitle)
                {
                    Info.AgainstInfo.OnlineUserInfo.UsePrefixTitleTag = tag;
                }
                if (updateType == UpdateType.SuffixTitle)
                {
                    Info.AgainstInfo.OnlineUserInfo.UseSuffixTitleTag = tag;
                }
            }
        }
    }
}
