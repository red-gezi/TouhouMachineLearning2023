using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    class AssetBundleCommand
    {
        static int totalLoadABCouat = 1;
        static int currentLoadABCouat = 0;
        public static bool AlreadyInit { get; set; } = false;
        static Dictionary<string, List<Object>> assets = new();
        //获取进度
        public static (int, int) GetLoadProcess() => (currentLoadABCouat, totalLoadABCouat);

        /// <summary>
        /// 初始化ab资源包，可选择从热更新拉取或是直接加载本地的
        /// </summary>
        /// <param name="isHotFixedLoad"></param>
        /// <returns></returns>
        public static async Task Init(bool isHotFixedLoad = true)
        {
            if (AlreadyInit) { return; }
            AlreadyInit = true;
            //若直接在编辑器中的后续界面运行时默认加载本地测试版本AB包
            string targetPath = "AB/PC_Test";
            Debug.Log(targetPath);
            //如果当前是从热更界面进入且不是编辑器时从游戏下载的AB路径加载数据包
            if (isHotFixedLoad && !Application.isEditor)
            {
                targetPath = Application.persistentDataPath + $"/AssetBundles/{(Application.isMobilePlatform ? "Android" : Manager.ConfigManager.GetServerTag())}/";
            }
            Directory.CreateDirectory(targetPath);
            List<Task> ABLoadTask = new List<Task>();
            foreach (var file in new DirectoryInfo(targetPath).GetFiles().AsParallel()
            .Where(file => file.Name.Contains("gezi")
            && !file.Name.Contains("meta")
            && !file.Name.Contains("manifest")))
            {
                ABLoadTask.Add(LoadAssetBundle(file.FullName));
            }
            currentLoadABCouat = 0;
            totalLoadABCouat = ABLoadTask.Count;
            await Task.WhenAll(ABLoadTask);
            Debug.LogWarning($"AB包加载完毕");

            foreach (var ab in AssetBundle.GetAllLoadedAssetBundles())
            {
                try
                {
                    //不将场景ab包纳入资源加载
                    if (!ab.name.Contains("scene"))
                    {
                        assets[ab.name] = ab.LoadAllAssets().ToList();
                    }
                }
                catch (System.Exception e)
                {

                    Debug.LogError(ab.name + e.Message);
                }
            }
            Debug.LogWarning("生成AB包字典");
            async Task<AssetBundle> LoadAssetBundle(string path)
            {
                var ABLoadRequir = AssetBundle.LoadFromFileAsync(path);
                while (!ABLoadRequir.isDone) { await Task.Delay(50); }
                Debug.Log(path + "加载完毕");
                currentLoadABCouat++;
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
                    Debug.LogError($"无法从{tag}AB包找到{typeof(T)}类型的资源{fileName}资源,当前已加载{assets.Count}个AB包，请检查资源是否放入ab包或者ab包未载入" );
                }
                return targetAsset as T;
            }
            else
            {
                Debug.LogError("AB包中Tag为"+tag.ToLower()+"的资源无法找到");
            }
            return null;
        }
    }
}
