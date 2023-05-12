using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TMPro;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Extension;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    public class HotFixManager : MonoBehaviour
    {
        public string verious;
        public Text loadText;
        public Text processText;
        public Text versiousText;

        public TMP_Dropdown serverSelect;
        public Slider slider;
        //重启通知
        public GameObject RestartNotice;
        //服务器选择界面
        public GameObject ServerSelect;

        static string serverTag = "PC";
        //后期自定义修改服务器ip
        //string serverIP = File.ReadAllLines("敏感信息.txt")[1];
        static string serverAssetUrl = $"http://106.15.38.165:7777/AssetBundles";
        MD5 md5 = new MD5CryptoServiceProvider();

        void Start()
        {
            RestartNotice.transform.localScale = new Vector3(1, 0, 1);
            versiousText.text = verious;
            loadText.text = "初始化配置信息";
            ConfigManager.InitConfig();
            loadText.text = "校验资源包";
        }
        public void ChangeServer(int selectIndex) => serverTag = selectIndex == 0 ? "PC" : "Test";
        public async void StartGame()
        {
            //关掉选择界面
            ServerSelect.SetActive(false);
            //更新和加载AB包
            await CheckAssetBundles();
        }
        //校验本地文件
        private async Task CheckAssetBundles()
        {
            loadText.text = "开始本地AB包资源检测";
            //获得AB包来源标签
            var serverTag = ConfigManager.GetServerTag();
            loadText.text = "开始下载文件";
            Debug.LogWarning("开始下载文件" + System.DateTime.Now);

            string downLoadPath = Application.isMobilePlatform ? Application.persistentDataPath + $"/Assetbundles/Android/" : $"{Directory.GetCurrentDirectory()}/Assetbundles/{serverTag}/";
            Directory.CreateDirectory(downLoadPath);
            using (var httpClient = new HttpClient())
            {
                var responseMessage = await httpClient.GetAsync($"{serverAssetUrl}/{serverTag}/MD5.json");
                if (!responseMessage.IsSuccessStatusCode) { loadText.text = "MD5文件获取出错"; return; }
                var OnlieMD5FiIeDatas = await responseMessage.Content.ReadAsStringAsync();
                var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
                Debug.Log("MD5文件已加载完成" + OnlieMD5FiIeDatas);
                loadText.text = "MD5文件已加载完成：";

                //已下好任务数
                int downloadTaskCount = 0;
                //开始遍历校验并更新本地AB包文件
                foreach (var MD5FiIeData in Md5Dict)
                {
                    //当前校验的本地文件
                    FileInfo localFile = new FileInfo(downLoadPath + MD5FiIeData.Key);
                    if (localFile.Exists && MD5FiIeData.Value.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(localFile.FullName))))
                    {
                        loadText.text = MD5FiIeData.Key + "校验成功，无需下载";
                        Debug.LogWarning(MD5FiIeData.Key + "校验成功，无需下载");
                    }
                    else
                    {
                        loadText.text = MD5FiIeData.Key + "有新版本，开始重新下载";
                        Debug.LogError(MD5FiIeData.Key + "有新版本，开始重新下载");
                        await DownLoadFile(MD5FiIeData, localFile);
                        async Task DownLoadFile(KeyValuePair<string, byte[]> MD5FiIeData, FileInfo localFile)
                        {
                            loadText.text = $"正在下载:{MD5FiIeData.Key},进度 {downloadTaskCount}/{Md5Dict.Count}";
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                                Debug.LogWarning("下载文件" + $"{serverAssetUrl}/{serverTag}/{MD5FiIeData.Key}");
                                await webClient.DownloadFileTaskAsync(new System.Uri($"{serverAssetUrl}/{serverTag}/{MD5FiIeData.Key}"), localFile.FullName);
                                Debug.LogWarning(MD5FiIeData.Key + "下载完成");
                                Debug.LogWarning("结束下载文件" + localFile.Name + " " + System.DateTime.Now);
                                void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
                                {
                                    processText.text = $"{e.BytesReceived / 1024 / 1024}MB/{e.TotalBytesToReceive / 1024 / 1024}MB";
                                    slider.value = e.BytesReceived * 1f / e.TotalBytesToReceive;
                                }
                            }
                        }
                    }
                    downloadTaskCount++;
                }
                Debug.LogWarning("全部AB包下载完成");
                loadText.text = "全部AB包下载完成";


                string localDllOrApkPath = "";
                string onlineDllOrApkPath = "";
                string onlineDllOrApk_MD5Path = "";


                //在不同平台指定不同的路径
                if (Application.isMobilePlatform)
                {
                    //指定热更场景和资源本地路径
                    localDllOrApkPath = $"{Application.persistentDataPath}/APK/TouHouMachineLearningSummary.apk";
                    onlineDllOrApkPath = $"{serverAssetUrl}/APK/TouHouMachineLearningSummary.apk";
                    onlineDllOrApk_MD5Path = $"{serverAssetUrl}/APK/MD5.json";
                }
                else
                {
                    //指定热更场景和资源本地路径
                    localDllOrApkPath = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouHouMachineLearningSummary.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                    //指定热更场景和资源网络路径
                    onlineDllOrApkPath = $"{serverAssetUrl}/Dll/{serverTag}/TouHouMachineLearningSummary.dll";
                    onlineDllOrApk_MD5Path = $"{serverAssetUrl}/Dll/{serverTag}/MD5.json";
                }

                responseMessage = await httpClient.GetAsync(onlineDllOrApk_MD5Path);
                if (!responseMessage.IsSuccessStatusCode) { Debug.LogError("dll或者apk的md5文件下载失败"); return; }
                var data = await responseMessage.Content.ReadAsByteArrayAsync();
                //如果是手机端，检查apk变更，否则检查dll变更，若发生变更，则重启
                if (data.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localDllOrApkPath).FullName))))
                {
                    Debug.Log("文件无改动");
                }
                else
                {
                    responseMessage = await httpClient.GetAsync(onlineDllOrApkPath);
                    if (!responseMessage.IsSuccessStatusCode) { Debug.LogError("文件下载失败"); return; }
                    //保存相关的dll或者apk文件
                    if (!Application.isEditor)
                    {
                        File.WriteAllBytes(localDllOrApkPath, await responseMessage.Content.ReadAsByteArrayAsync());
                        RestartGame();
                    }
                }
            }
            md5.Dispose();
           
            //加载AB包，并从中加载场景
            Debug.LogWarning("开始初始化AB包");
            AssetBundle.UnloadAllAssetBundles(true);
            loadText.text = "资源包校验完毕，少女加载中~~~~~";
            await SceneCommand.InitAsync(true);
            //显示AB包加载进度
            while (true)
            {
                (int currentLoadABCouat, int totalLoadABCouat) process = AssetBundleCommand.GetLoadProcess();
                slider.value = process.currentLoadABCouat * 1.0f / process.totalLoadABCouat;
                processText.text = $"{process.currentLoadABCouat}/{process.totalLoadABCouat}";
                if (process.currentLoadABCouat == process.totalLoadABCouat)
                {
                    break;
                }
                await Task.Delay(50);
            }
            Debug.LogWarning("AB包加载完毕，切换场景。。。");
            SceneManager.LoadScene("1_LoginScene", LoadSceneMode.Single);
        }
        public void RestartGame()
        {
            if (Application.isMobilePlatform)
            {
                //后续加上安卓重启
                Application.Quit();
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
        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 150, 100, 50), "重新加载"))
            {
                RestartGame();
            }
            if (GUI.Button(new Rect(0, 200, 100, 50), "退出"))
            {
                Application.Quit();

            }
        }
    }
}