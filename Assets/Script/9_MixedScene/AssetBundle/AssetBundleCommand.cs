using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    class AssetBundleCommand
    {
        public static bool AlreadyInit { get; set; } = false;
        static Dictionary<string, List<UnityEngine.Object>> assets = new();
        /// <summary>
        /// 初始化ab资源包，可选择从热更新拉取或是直接加载本地的
        /// </summary>
        /// <param name="isHotFixedLoad"></param>
        /// <returns></returns>
        public static async Task Init(bool isHotFixedLoad = true)
        {
            if (AlreadyInit) { return; }
            AlreadyInit = true;

            string targetPath = "";
            //选择从下载下来的热更新目录拉去还是本地获取
            //当为热更模式且不是编辑器时从游戏数据更新路径加载，否在直接加载本地的
            if (isHotFixedLoad)
            {
                if (Application.isEditor)
                {
                    targetPath = "AssetBundles/PC";
                }
                else
                {
                    if (Application.isMobilePlatform)
                    {
                        targetPath = Application.persistentDataPath + $"/AssetBundles/Android/";
                    }
                    else
                    {
                        targetPath = Application.streamingAssetsPath + $"/AssetBundles/PC/";
                    }
                }
            }
            else
            {
                targetPath = "AssetBundles/PC";
            }

            Directory.CreateDirectory(targetPath);

            List<Task> ABLoadTask = new List<Task>();
            foreach (var file in new DirectoryInfo(targetPath)
                .GetFiles()
                .AsParallel()
               .Where(file => file.Name.Contains("gezi") && !file.Name.Contains("meta") && !file.Name.Contains("manifest")))
            {

                ABLoadTask.Add(LoadAssetBundle(file.FullName));
            }
            await Task.WhenAll(ABLoadTask);
            Debug.LogWarning($"AB包加载完毕");

            foreach (var ab in AssetBundle.GetAllLoadedAssetBundles())
            {
                assets[ab.name] = ab.LoadAllAssets().ToList();
            }
            Debug.LogWarning("生成AB包字典");

            async Task<AssetBundle> LoadAssetBundle(string path)
            {
                var ABLoadRequir = AssetBundle.LoadFromFileAsync(path);
                while (!ABLoadRequir.isDone) { await Task.Delay(50); }
                Debug.Log(path + "加载完毕");
                return ABLoadRequir.assetBundle;
            }
        }

        /// <summary>
        /// 从带有tag名的AB包中加载素材
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T Load<T>(string tag, string fileName) where T : UnityEngine.Object
        {
            var targetAssets = assets.FirstOrDefault(asset => asset.Key.Contains(tag.ToLower())).Value;
            if (targetAssets != null)
            {
                var targetAsset = targetAssets.FirstOrDefault(asset => asset.name == fileName && asset.GetType() == typeof(T));
                if (targetAsset == null)
                {
                    Debug.LogError($"无法找到资源,已加载{assets.Count}个资源" + typeof(T) + "-" + tag + "-" + fileName);
                }
                //Debug.LogError($"已完成资源集加载");
                //targetAssets.ForEach(x => Debug.Log(x.name));
                return targetAsset as T;
            }
            return null;
        }
    }
}
