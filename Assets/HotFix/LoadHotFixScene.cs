using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
//该脚本无法被热更，修改需要重新打包，因只复制修改
public class LoadHotFixScene : MonoBehaviour
{
    public static Action EndAction = null;
    static MD5 md5 = new MD5CryptoServiceProvider();
    public static Action StartAction = () => Init();


    static string serverTag = "PC_Release";
    static string hotFixSceneFileName = "scene0.gezi";
    static string hotFixAssetFileName = "hotfixscene.gezi";
    static string serverAssetUrl = $"http://106.15.38.165:7777/AssetBundles";

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
        //判断正式版还是测试版，默认为正式版
        if (File.Exists(ConfigFileSavePath) && File.ReadAllText(ConfigFileSavePath).Contains("Test"))
        {
            serverTag = "PC_Test";
            Debug.Log("当前tag为" + serverTag);
        }
        //在不同平台指定不同的路径
        if (Application.isMobilePlatform)
        {
            //指定热更场景和资源本地路径
            localHotFixSceneBundlePath = $"{Application.persistentDataPath}/Assetbundles/{hotFixSceneFileName}";
            localHotFixAssetBundlePath = $"{Application.persistentDataPath}/Assetbundles/{hotFixAssetFileName}";
            localDllOrApkPath = $"{Application.persistentDataPath}/APK/TouHouMachineLearningSummary.apk";
            //指定热更场景和资源网络路径
            onlineHotFixSceneBundlePath = $"{serverAssetUrl}/Android/{hotFixSceneFileName}";
            onlineHotFixAssetBundlePath = $"{serverAssetUrl}/Android/{hotFixAssetFileName}";
            onlineAB_MD5sFile = $"{serverAssetUrl}/Android/MD5.json";
            onlineDllOrApk_MD5Path = $"{serverAssetUrl}/APK/MD5.json";
            onlineDllOrApkPath = $"{serverAssetUrl}/APK/TouHouMachineLearningSummary.apk";
        }
        else
        {
            //指定热更场景和资源本地路径
            localHotFixSceneBundlePath = $"Assetbundles/{serverTag}/{hotFixSceneFileName}";
            localHotFixAssetBundlePath = $"Assetbundles/{serverTag}/{hotFixAssetFileName}";
            localDllOrApkPath = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouHouMachineLearningSummary.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
            //指定热更场景和资源网络路径
            onlineHotFixSceneBundlePath = $"{serverAssetUrl}/{serverTag}/{hotFixSceneFileName}";
            onlineHotFixAssetBundlePath = $"{serverAssetUrl}/{serverTag}/{hotFixAssetFileName}";
            onlineAB_MD5sFile = $"{serverAssetUrl}/{serverTag}/MD5.json";
            onlineDllOrApk_MD5Path = $"{serverAssetUrl}/Dll/{serverTag}/MD5.json";
            onlineDllOrApkPath = $"{serverAssetUrl}/Dll/{serverTag}/TouHouMachineLearningSummary.dll";
        }
        using (var httpClient = new HttpClient())
        {
            //对比热更场景MD5，判断是否下载
            byte[] data;
            HttpResponseMessage httpResponse;
            httpResponse = await httpClient.GetAsync(onlineAB_MD5sFile);
            if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("onlineAB_MD5sFile文件下载失败"); return; }
            var AB_MD5s = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(await httpResponse.Content.ReadAsStringAsync());

            //校验热更场景
            if (new FileInfo(localHotFixSceneBundlePath).Exists && AB_MD5s[hotFixSceneFileName].SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localHotFixSceneBundlePath).FullName))))
            {
                Debug.Log("热更场景无变动");
            }
            else
            {
                //下载热更新场景
                httpResponse = await httpClient.GetAsync(onlineHotFixSceneBundlePath);
                if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("热更场景文件下载失败"); return; }
                data = await httpResponse.Content.ReadAsByteArrayAsync();
                Directory.CreateDirectory(new FileInfo(localHotFixSceneBundlePath).DirectoryName);
                File.WriteAllBytes(localHotFixSceneBundlePath, data);
            }

            //校验热更场景素材
            if (new FileInfo(localHotFixAssetBundlePath).Exists && AB_MD5s[hotFixAssetFileName].SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localHotFixAssetBundlePath).FullName))))
            {
                Debug.Log("热更场景素材无变动");
            }
            else
            {
                //下载热更新场景
                httpResponse = await httpClient.GetAsync(onlineHotFixAssetBundlePath);
                if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("热更场景素材文件下载失败"); return; }
                data = await httpResponse.Content.ReadAsByteArrayAsync();
                Directory.CreateDirectory(new FileInfo(localHotFixAssetBundlePath).DirectoryName);
                File.WriteAllBytes(localHotFixAssetBundlePath, data);
            }

            httpResponse = await httpClient.GetAsync(onlineDllOrApk_MD5Path);
            if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("dll或者apk的md5文件下载失败");  }
            data = await httpResponse.Content.ReadAsByteArrayAsync();
            //如果是手机端，检查apk变更，否则检查dll变更，若发生变更，则重启
            if (data.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localDllOrApkPath).FullName))))
            {
                Debug.Log("文件无改动");
            }
            else
            {
                httpResponse = await httpClient.GetAsync(onlineDllOrApkPath);
                if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("DllOrApk文件下载失败");  }
                //报存相关的dll或者apk文件
                if (!Application.isEditor)
                {
                    Directory.CreateDirectory(new FileInfo(localDllOrApkPath).DirectoryName);
                    File.WriteAllBytes(localDllOrApkPath, await httpResponse.Content.ReadAsByteArrayAsync());
                    if (Application.isMobilePlatform)
                    {
                        //安卓端重启
                        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                        string packageName = activity.Call<string>("getPackageName");

                        AndroidJavaObject intent = activity.Call<AndroidJavaObject>("getBaseContext").Call<AndroidJavaObject>("getPackageManager").Call<AndroidJavaObject>("getLaunchIntentForPackage", packageName);

                        intent.Call<AndroidJavaObject>("addFlags", 0x00200000); //FLAG_ACTIVITY_CLEAR_TOP
                        intent.Call<AndroidJavaObject>("addFlags", 0x10000000); //FLAG_ACTIVITY_NEW_TASK

                        activity.Call("startActivity", intent);

                        AndroidJavaObject unityActivity = activity.Call<AndroidJavaObject>("getApplicationContext");
                        unityActivity.Call<AndroidJavaObject>("getPackageManager").Call<AndroidJavaObject>("restartPackage", packageName);
                        Application.Quit();

                        //安卓端重启重新安装
                        //AndroidJavaClass intentObj = new AndroidJavaClass("android.content.Intent");
                        //AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", intentObj.GetStatic<string>("ACTION_INSTALL_PACKAGE"));
                        //Application.Quit();

                        //// 设置 APK 文件的 Uri
                        //AndroidJavaClass uriObj = new AndroidJavaClass("android.net.Uri");
                        //AndroidJavaObject uri = uriObj.CallStatic<AndroidJavaObject>("parse", "file://" + "待修改");
                        //intent.Call<AndroidJavaObject>("setData", uri);

                        //// 调用安装程序
                        //AndroidJavaClass unityObj = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                        //AndroidJavaObject context = unityObj.GetStatic<AndroidJavaObject>("currentActivity");
                        //context.Call("startActivity", intent);
                    }
                    else
                    {
                        var game = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouHouMachineLearningSummary.exe", SearchOption.AllDirectories).FirstOrDefault();
                        if (game != null)
                        {
                            System.Diagnostics.Process.Start(game.FullName);
                        }
                        Application.Quit();
                    }
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