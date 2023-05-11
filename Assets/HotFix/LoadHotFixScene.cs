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
    static string localHotFixSceneBundlePath = "";
    static string localHotFixAssetBundlePath = "";
    static string localDllOrApkPath = "";


    static string onlineHotFixSceneBundlePath = "";
    static string onlineHotFixAssetBundlePath = "";
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
                localHotFixSceneBundlePath = "AB/scene0.gezi";
                localHotFixAssetBundlePath = "AB/HotFixed.gezi";
                //指定热更场景和资源网络路径
                onlineHotFixSceneBundlePath = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/scene0.gezi";
                onlineHotFixAssetBundlePath = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/HotFixed.gezi";
                onlineAB_MD5sFile = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/MD5.json";
                onlineDllOrApkPath= $"http://106.15.38.165:7777/AssetBundles/Dll/{serverTag}/TouhouMachineLearning.dll";
                onlineDllOrApk_MD5Path = $"http://106.15.38.165:7777/AssetBundles/Dll/{serverTag}/MD5.json";

                break;
            case Platform.PC:
                //指定热更场景和资源本地路径
                localHotFixSceneBundlePath = "Assetbundles/PC/scene0.gezi";
                localHotFixAssetBundlePath = "Assetbundles/PC/HotFixed.gezi";
                localDllOrApkPath = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouHouMachineLearning.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                //指定热更场景和资源网络路径
                onlineHotFixSceneBundlePath = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/scene0.gezi";
                onlineHotFixAssetBundlePath = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/HotFixed.gezi";
                onlineAB_MD5sFile = $"http://106.15.38.165:7777/AssetBundles/{serverTag}/MD5.json";
                onlineDllOrApkPath = $"http://106.15.38.165:7777/AssetBundles/Dll/{serverTag}/TouhouMachineLearning.dll";
                onlineDllOrApk_MD5Path = $"http://106.15.38.165:7777/AssetBundles/Dll/{serverTag}/MD5.json";
                break;
            case Platform.Android:
                //指定热更场景和资源本地路径
                localHotFixSceneBundlePath = $"{(Application.persistentDataPath + "Assetbundles")}/scene0.gezi";
                localHotFixAssetBundlePath = $"{(Application.persistentDataPath + "Assetbundles")}/HotFixed.gezi";
                localDllOrApkPath = Application.persistentDataPath + "/TouhouMachineLearning.apk";
                //指定热更场景和资源网络路径
                onlineHotFixSceneBundlePath = $"http://106.15.38.165:7777/AssetBundles/Android/scene0.gezi";
                onlineHotFixAssetBundlePath = $"http://106.15.38.165:7777/AssetBundles/Android/HotFixed.gezi";
                onlineDllOrApkPath = $"http://106.15.38.165:7777/AssetBundles/APK/TouhouMachineLearning.apk";
                onlineAB_MD5sFile = $"http://106.15.38.165:7777/AssetBundles/Android/MD5.json";
                break;
            default:
                break;
        }
        using (var httpClient = new HttpClient())
        {
            //对比热更场景MD5，判断是否下载
            byte[] data;
            HttpResponseMessage httpResponse;
            httpResponse = await httpClient.GetAsync(onlineAB_MD5sFile);
            if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("文件下载失败"); return; }
            var AB_MD5s = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(await httpResponse.Content.ReadAsStringAsync());

            //校验热更场景
            if (new FileInfo(localHotFixSceneBundlePath).Exists && AB_MD5s["scene0.gezi"].SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localHotFixSceneBundlePath).FullName))))
            {
                Debug.Log("热更场景无变动");
            }
            else
            {
                //下载热更新场景
                httpResponse = await httpClient.GetAsync(onlineHotFixSceneBundlePath);
                if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("文件下载失败"); return; }
                data = await httpResponse.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(localHotFixSceneBundlePath, data);
            }

            //校验热更场景素材
            if (new FileInfo(localHotFixAssetBundlePath).Exists && AB_MD5s["hotfixscene.gezi"].SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localHotFixAssetBundlePath).FullName))))
            {
                Debug.Log("热更场景素材无变动");
            }
            else
            {
                //下载热更新场景
                httpResponse = await httpClient.GetAsync(onlineHotFixAssetBundlePath);
                if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("热更场景素材文件下载失败"); return; }
                data = await httpResponse.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(localHotFixAssetBundlePath, data);
            }

            httpResponse = await httpClient.GetAsync(onlineDllOrApk_MD5Path);
            if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("dll或者apk的md5文件下载失败"); return; }
            data = await httpResponse.Content.ReadAsByteArrayAsync();
            //如果是手机端，检查apk变更，否则检查dll变更，若发生变更，则重启
            if (data.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localDllOrApkPath).FullName))))
            {
                Debug.Log("文件无改动");
            }
            else
            {
                httpResponse = await httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/Dll/{serverTag}/TouhouMachineLearning.dll");
                if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("文件下载失败"); return; }
                //报存相关的dll或者apk文件
                File.WriteAllBytes(localDllOrApkPath, await httpResponse.Content.ReadAsByteArrayAsync());

                if (Application.isMobilePlatform)
                {
                    //后续加上安卓重启
                    Application.Quit();
                }
                else
                {
                    var game = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouhouMachineLearning.exe", SearchOption.AllDirectories).FirstOrDefault();
                    if (game != null)
                    {
                        System.Diagnostics.Process.Start(game.FullName);
                    }
                    Application.Quit();
                }
            }
        }
        AssetBundle.UnloadAllAssetBundles(true);
        //加载热更AB包，切换到热更场景
        AssetBundle.LoadFromFile(localHotFixSceneBundlePath);
        AssetBundle.LoadFromFile(localHotFixAssetBundlePath);
        Debug.LogWarning("重新载入完成");
        SceneManager.LoadScene("0_HotFixScene");
    }
}