using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadHotFixScene : MonoBehaviour
{
    public static Action StartAction = () => Init();
    public static Action EndAction = null;
    enum Platform { Edit, PC, Android }
    static Platform currentPlatform;
    static MD5 md5 = new MD5CryptoServiceProvider();

    static string serverTag = "PC_Release";
    static string localHotFixedSceneBundlePath = "";
    static string localHotFixedAssetBundlePath = "";
    static string localDllOrApkPath = "";


    static string onlineHotFixedSceneBundlePath = "";
    static string onlineHotFixedAssetBundlePath = "";
    static string onlineDllOrApkPath = "";

    static string onlineAB_MD5sFile = "";
    static string onlineDllOrApk_MD5Path = "";
    //配置文件路径
    static string ConfigFileSavePath => (Application.isMobilePlatform ? Application.persistentDataPath : Directory.GetCurrentDirectory()) + "/GameConfig.ini";

    private void Start() => StartAction();
    private static async void Init()
    {
        //判断当前平台
        if (Application.isEditor)
        {
            currentPlatform = Platform.Edit;
        }
        else
        {
            if (Application.isMobilePlatform)
            {
                currentPlatform = Platform.Android;
            }
            else
            {
                currentPlatform = Platform.PC;
            }
        }
        //判断正式版还是测试版，默认为正式版
        if (File.Exists(ConfigFileSavePath) && File.ReadAllText(ConfigFileSavePath).Contains("Test"))
        {
            serverTag = "PC_Test";
            Debug.Log("当前tag为" + serverTag);
        }
        switch (currentPlatform)
        {
            case Platform.Edit:
                //指定热更场景和资源本地路径
                localHotFixedSceneBundlePath = "AB/scene0.gezi";
                localHotFixedAssetBundlePath = "AB/HotFixed.gezi";

                onlineHotFixedSceneBundlePath = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/scene0.gezi";
                onlineHotFixedAssetBundlePath = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/HotFixed.gezi";
                onlineAB_MD5sFile = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/MD5.json";
                onlineDllOrApk_MD5Path = $"http://106.15.38.165:7777/AssetBundles/Dll/{serverTag}/MD5.json";

                break;
            case Platform.PC:
                //指定热更场景和资源本地路径
                localHotFixedSceneBundlePath = "Assetbundles/PC/scene0.gezi";
                localHotFixedAssetBundlePath = "Assetbundles/PC/HotFixed.gezi";
                localDllOrApkPath = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouHouMachineLearning.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;

                onlineHotFixedSceneBundlePath = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/scene0.gezi";
                onlineHotFixedAssetBundlePath = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/HotFixed.gezi";
                onlineAB_MD5sFile = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/MD5.json";
                onlineDllOrApk_MD5Path = $"http://106.15.38.165:7777/AssetBundles/Dll/{serverTag}/MD5.json";
                break;
            case Platform.Android:
                //指定热更场景和资源本地路径
                localHotFixedSceneBundlePath = $"{(Application.persistentDataPath + "Assetbundles")}/scene0.gezi";
                localHotFixedAssetBundlePath = $"{(Application.persistentDataPath + "Assetbundles")}/HotFixed.gezi";
                localDllOrApkPath = Application.persistentDataPath + "/TouhouMachineLearning.apk";

                onlineHotFixedSceneBundlePath = $"http://106.15.38.165:7777/AssetBundles/Android/scene0.gezi";
                onlineHotFixedAssetBundlePath = $"http://106.15.38.165:7777/AssetBundles/Android/HotFixed.gezi";
                localDllOrApkPath = Application.persistentDataPath + "/TouhouMachineLearning.apk";

                onlineAB_MD5sFile = $"http://106.15.38.165:7777/AssetBundles/Android/MD5.json";
                break;
            default:
                break;
        }
        using (var httpClient = new HttpClient())
        {
            //对比热更场景MD5，判断是否下载
            HttpResponseMessage httpResponse;
            byte[] data;
            httpResponse = await httpClient.GetAsync(onlineAB_MD5sFile);
            if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("文件下载失败"); return; }
            var AB_MD5s = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(await responseMessage.Content.ReadAsStringAsync());

            //校验热更场景
            var HotFixScene_Md5 = AB_MD5s["scene0.gezi"];
            if (new FileInfo(localHotFixedSceneBundlePath).Exists && HotFixScene_Md5.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localHotFixedSceneBundlePath).FullName))))
            {
                Debug.Log("热更场景无变动");
            }
            else
            {
                //下载热更新场景
                httpResponse = await httpClient.GetAsync(localHotFixedSceneBundlePath);
                if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("文件下载失败"); return; }
                data = await httpResponse.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(localHotFixedSceneBundlePath, data);
            }

            var HotFixAsset_Md5 = AB_MD5s["HotFixed.gezi"];
            //校验热更场景素材
            if (new FileInfo(localHotFixedAssetBundlePath).Exists && HotFixScene_Md5.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localHotFixedAssetBundlePath).FullName))))
            {
                Debug.Log("热更场景素材无变动");
            }
            else
            {
                //下载热更新场景
                httpResponse = await httpClient.GetAsync(localHotFixedSceneBundlePath);
                if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("文件下载失败"); return; }
                data = await httpResponse.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(localHotFixedAssetBundlePath, data);
            }

            httpResponse = await httpClient.GetAsync(onlineDllOrApk_MD5Path);
            if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("文件下载失败"); return; }
            data = await httpResponse.Content.ReadAsByteArrayAsync();
            //如果是手机端，检查apk变更，否则检查dll变更，若发生变更，则重启
            if (data.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localDllOrApkPath).FullName))))
            {
                Debug.Log("文件无改动");
            }
            else
            {
                if (Application.isMobilePlatform)
                {

                    Application.Quit();
                }
                else
                {
                    httpResponse = await httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/Dll/{serverTag}/MD5.json");
                    if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("文件下载失败"); return; }
                    var dllMD5 = await httpResponse.Content.ReadAsByteArrayAsync();

                    var game = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouhouMachineLearning.exe", SearchOption.AllDirectories).FirstOrDefault();
                    if (game != null)
                    {
                        System.Diagnostics.Process.Start(game.FullName);
                    }
                    Application.Quit();
                }
            }
        }
        md5.ComputeHash();
        //判断文件是否存在或者版本一直，不存在或不匹配直接下载
        //if (!File.Exists(HotFixedSceneBundlePath) || !File.Exists(HotFixedAssetBundlePath))

        if (true)
        {
            Debug.LogError("检测不到热更场景资源包，尝试自动下载");
            new FileInfo(localHotFixedAssetBundlePath).Directory.Create();
            using (var httpClient = new HttpClient())
            {

                //下载热更新场景的资源
                responseMessage = await httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/PC/hotfixscene.gezi");
                if (!responseMessage.IsSuccessStatusCode) { Debug.LogError("文件下载失败"); return; }
                data = await responseMessage.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(localHotFixedAssetBundlePath, data);

            }
        }

        Debug.Log("卸载前场景数量为" + SceneManager.sceneCount);
        AssetBundle.UnloadAllAssetBundles(false);
        //加载热更AB包，切换到热更场景
        Debug.Log("卸载后场景数量为" + SceneManager.sceneCount);
        AssetBundle.LoadFromFile(HotFixedSceneBundlePath);
        AssetBundle.LoadFromFile(localHotFixedAssetBundlePath);
        Debug.LogWarning("重新载入完成");
        SceneManager.LoadScene("0_HotFixScene");
    }
}