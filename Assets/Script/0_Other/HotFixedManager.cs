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
using TouhouMachineLearningSummary.Thread;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    public class HotFixedManager : MonoBehaviour
    {
        public Text loadText;
        public Text processText;
        public Text versiousText;
        
        public TMP_Dropdown serverSelect;
        public Slider slider;
        //重启通知
        public GameObject RestartNotice;
        //服务器选择界面
        public GameObject ServerSelect;

        string serverTag = "PC";
        //后期自定义修改服务器ip
        //string serverIP = File.ReadAllLines("敏感信息.txt")[1];

        MD5 md5 = new MD5CryptoServiceProvider();

        static bool isEditor = Application.isEditor;//是否是编辑器状态
        static bool isMobile = Application.isMobilePlatform;//是否是移动平台
        static string downLoadPath = isMobile switch
        {
            true => Application.persistentDataPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/",
            false => Application.streamingAssetsPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/"
        };
        //程序集存储路径
        static string dllFIleRootPath = isMobile switch
        {
            true => new DirectoryInfo(Application.persistentDataPath).Parent.FullName,
            false => Directory.GetCurrentDirectory()
        };
        async void Start()
        {
            RestartNotice.transform.localScale = new Vector3(1, 0, 1);
            versiousText.text = "我要改成v5";
            loadText.text = "初始化配置信息";
            ConfigManager.InitConfig();
            loadText.text = "校验资源包";
        }
        public void ChangeServer(int selectIndex) => serverTag = selectIndex == 0 ? "PC" : "Android";
        public async void StartGame()
        {
            //如果是手机模式强制使用正式版安卓资源（安卓不进行测试了）
            if (Application.isMobilePlatform)
            {
                serverTag = "Android";
            }
            //关掉选择界面
            ServerSelect.SetActive(false);
            //开启进度条
            await CheckAssetBundles();
        }
        //校验本地文件
        private async Task CheckAssetBundles()
        {
            bool isNeedRestartApplication = false;
            //已下好任务数
            int downloadTaskCount = 0;
            loadText.text = "检查AB包中";
            //编辑器模式下不进行下载
            if (!isEditor)
            {
                //AB包存储路径

                loadText.text = "开始下载文件";
                Debug.LogWarning("开始下载文件" + System.DateTime.Now);
                Directory.CreateDirectory(downLoadPath);
                var httpClient = new HttpClient();
                var responseMessage = await httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/MD5.json");
                if (!responseMessage.IsSuccessStatusCode)
                {
                    loadText.text = "MD5文件获取出错";
                    return;
                }

                var OnlieMD5FiIeDatas = await responseMessage.Content.ReadAsStringAsync();
                var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
                Debug.Log("MD5文件已加载完成" + OnlieMD5FiIeDatas);
                loadText.text = "MD5文件已加载完成：";

                //校验本地文件
                downloadTaskCount = 0;

                foreach (var MD5FiIeData in Md5Dict)
                {
                    //当前校验的本地文件
                    FileInfo localFile = null;
                    if (MD5FiIeData.Key.EndsWith(".dll")) //如果是游戏程序集dll文件，则根据不同平台对比不同路径下的游戏程序集dll
                    {

                        Debug.LogError("当前程序集路径为" + dllFIleRootPath);
                        string currentDllPath = new DirectoryInfo(dllFIleRootPath).GetFiles("TouHouMachineLearning.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                        Debug.LogError("当前为脚本路径在：" + currentDllPath);
                        localFile = new FileInfo(currentDllPath);
                        Debug.Log("本地dll文件md5值为：" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                        Debug.Log("本地dll文件修改时间为：" + localFile.LastWriteTime);
                        if (isMobile)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        localFile = new FileInfo(downLoadPath + MD5FiIeData.Key);
                    }

                    if (localFile.Exists && MD5FiIeData.Value.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(localFile.FullName))))
                    {
                        loadText.text = MD5FiIeData.Key + "校验成功";
                        Debug.LogWarning(MD5FiIeData.Key + "校验成功");
                    }
                    else
                    {
                        loadText.text = MD5FiIeData.Key + "有新版本，开始重新下载";
                        Debug.LogError(MD5FiIeData.Key + "有新版本，开始重新下载");

                        string savePath = localFile.FullName;

                        if (MD5FiIeData.Key.EndsWith(".dll")) //如果dll，则需要重启游戏进行载入
                        {
                            Debug.LogWarning("需要重启");
                            loadText.text = MD5FiIeData.Key + "更新代码资源";
                            isNeedRestartApplication = true;
                            Debug.LogError("代码覆盖路径:" + savePath);
                            Debug.LogError("本地代码MD5值" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                            Debug.LogError("网络代码MD5值" + MD5FiIeData.Value.ToJson());
                        }
                        await DownLoadFile(MD5FiIeData, localFile, savePath);
                        async Task DownLoadFile(KeyValuePair<string, byte[]> MD5FiIeData, FileInfo localFile, string savePath)
                        {
                            loadText.text = $"正在下载:{MD5FiIeData.Key},进度 {downloadTaskCount}/{Md5Dict.Count}";
                            WebClient webClient = new WebClient();
                            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                            await webClient.DownloadFileTaskAsync(new System.Uri($"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/{MD5FiIeData.Key}"), savePath);
                            Debug.LogWarning(MD5FiIeData.Key + "下载完成");
                            Debug.LogWarning("结束下载文件" + localFile.Name + " " + System.DateTime.Now);
                            void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
                            {
                                processText.text = $"{e.BytesReceived / 1024 / 1024}MB/{e.TotalBytesToReceive / 1024 / 1024}MB";
                                slider.value = e.BytesReceived * 1f / e.TotalBytesToReceive;
                            }
                        }
                    }
                    downloadTaskCount++;
                }
                Debug.LogWarning("全部下载完成");
                loadText.text = "全部下载完成";
                md5.Dispose();
                httpClient.Dispose();
                //如果改动了dll，需要重启
                if (isNeedRestartApplication)
                {
                    Debug.Log("需要重启");
                    //弹个窗，确认得话重启
                    RestartNotice.GetComponent<AudioSource>().Play();
                    await CustomThread.TimerAsync(0.5f, (process) =>
                    {
                        RestartNotice.transform.localScale = new Vector3(1, process, 1);
                    });
                    //等待用户重启，不再进行加载
                    return;
                }
            }

            //加载AB包，并从中加载场景
            Debug.LogWarning("开始初始化AB包");
            AssetBundle.UnloadAllAssetBundles(true);
            loadText.text = "开始加载资源包";
            await SceneCommand.InitAsync(true);
            Debug.LogWarning("初始化完毕，加载场景。。。");
            SceneManager.LoadScene("1_LoginScene");
        }



        public void RestartGame()
        {
            if (isMobile)
            {
                //SceneManager.LoadScene(0);
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
}