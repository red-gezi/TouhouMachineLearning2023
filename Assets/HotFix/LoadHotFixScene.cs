using System;
using System.IO;
using System.Net.Http;
using UnityEngine;//
using UnityEngine.SceneManagement;

public class LoadHotFixScene : MonoBehaviour
{
    public static Action StartAction = () => Init();
    public static Action EndAction = null;
    private void Start() => StartAction();
    private static async void Init()
    {
        //指定热更场景和资源所在路径
        string HotFixedSceneBundlePath = $"{(Application.isEditor ? "AB" : "Assetbundles/PC")}/scene0.gezi";
        string HotFixedAssetBundlePath = $"{(Application.isEditor ? "AB" : "Assetbundles/PC")}/HotFixed.gezi";

        //判断文件是否存在或者版本一直，不存在或不匹配直接下载
        //if (!File.Exists(HotFixedSceneBundlePath) || !File.Exists(HotFixedAssetBundlePath))
        if (true)
        {
            Debug.LogError("检测不到热更场景资源包，尝试自动下载");
            new FileInfo(HotFixedAssetBundlePath).Directory.Create();
            using (var httpClient = new HttpClient())
            {
                //下载热更新场景
                var responseMessage = await httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/PC/scene0.gezi");
                if (!responseMessage.IsSuccessStatusCode)
                {
                    Debug.LogError("文件下载失败");
                    return;
                }
                var data = await responseMessage.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(HotFixedSceneBundlePath, data);
                //下载热更新场景的资源
                responseMessage = await httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/PC/hotfixscene.gezi");
                if (!responseMessage.IsSuccessStatusCode)
                {
                    Debug.LogError("文件下载失败");
                    return;
                }
                data = await responseMessage.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(HotFixedAssetBundlePath, data);

            }
        }

        Debug.Log("卸载前场景数量为" + SceneManager.sceneCount);
        AssetBundle.UnloadAllAssetBundles(false);
        //加载热更AB包，切换到热更场景
        Debug.Log("卸载后场景数量为" + SceneManager.sceneCount);
        AssetBundle.LoadFromFile(HotFixedSceneBundlePath);
        AssetBundle.LoadFromFile(HotFixedAssetBundlePath);
        //var s = AssetBundle.GetAllLoadedAssetBundles();
        Debug.LogWarning("重新载入完成");
        SceneManager.LoadScene("0_HotFixScene");
    }
}