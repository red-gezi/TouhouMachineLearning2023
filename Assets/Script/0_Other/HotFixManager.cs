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
        //����֪ͨ
        public GameObject RestartNotice;
        //������ѡ�����
        public GameObject ServerSelect;

        static string serverTag = "PC";
        //�����Զ����޸ķ�����ip
        //string serverIP = File.ReadAllLines("������Ϣ.txt")[1];
        static string serverAssetUrl = $"http://106.15.38.165:7777/AssetBundles";
        MD5 md5 = new MD5CryptoServiceProvider();

        void Start()
        {
            RestartNotice.transform.localScale = new Vector3(1, 0, 1);
            versiousText.text = verious;
            loadText.text = "��ʼ��������Ϣ";
            ConfigManager.InitConfig();
            loadText.text = "У����Դ��";
        }
        public void ChangeServer(int selectIndex) => serverTag = selectIndex == 0 ? "PC" : "Test";
        public async void StartGame()
        {
            //�ص�ѡ�����
            ServerSelect.SetActive(false);
            //���ºͼ���AB��
            await CheckAssetBundles();
        }
        //У�鱾���ļ�
        private async Task CheckAssetBundles()
        {
            loadText.text = "��ʼ����AB����Դ���";
            //���AB����Դ��ǩ
            var serverTag = ConfigManager.GetServerTag();
            loadText.text = "��ʼ�����ļ�";
            Debug.LogWarning("��ʼ�����ļ�" + System.DateTime.Now);

            string downLoadPath = Application.isMobilePlatform ? Application.persistentDataPath + $"/Assetbundles/Android/" : $"{Directory.GetCurrentDirectory()}/Assetbundles/{serverTag}/";
            Directory.CreateDirectory(downLoadPath);
            using (var httpClient = new HttpClient())
            {
                var responseMessage = await httpClient.GetAsync($"{serverAssetUrl}/{serverTag}/MD5.json");
                if (!responseMessage.IsSuccessStatusCode) { loadText.text = "MD5�ļ���ȡ����"; return; }
                var OnlieMD5FiIeDatas = await responseMessage.Content.ReadAsStringAsync();
                var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
                Debug.Log("MD5�ļ��Ѽ������" + OnlieMD5FiIeDatas);
                loadText.text = "MD5�ļ��Ѽ�����ɣ�";

                //���º�������
                int downloadTaskCount = 0;
                //��ʼ����У�鲢���±���AB���ļ�
                foreach (var MD5FiIeData in Md5Dict)
                {
                    //��ǰУ��ı����ļ�
                    FileInfo localFile = new FileInfo(downLoadPath + MD5FiIeData.Key);
                    if (localFile.Exists && MD5FiIeData.Value.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(localFile.FullName))))
                    {
                        loadText.text = MD5FiIeData.Key + "У��ɹ�����������";
                        Debug.LogWarning(MD5FiIeData.Key + "У��ɹ�����������");
                    }
                    else
                    {
                        loadText.text = MD5FiIeData.Key + "���°汾����ʼ��������";
                        Debug.LogError(MD5FiIeData.Key + "���°汾����ʼ��������");
                        await DownLoadFile(MD5FiIeData, localFile);
                        async Task DownLoadFile(KeyValuePair<string, byte[]> MD5FiIeData, FileInfo localFile)
                        {
                            loadText.text = $"��������:{MD5FiIeData.Key},���� {downloadTaskCount}/{Md5Dict.Count}";
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                                Debug.LogWarning("�����ļ�" + $"{serverAssetUrl}/{serverTag}/{MD5FiIeData.Key}");
                                await webClient.DownloadFileTaskAsync(new System.Uri($"{serverAssetUrl}/{serverTag}/{MD5FiIeData.Key}"), localFile.FullName);
                                Debug.LogWarning(MD5FiIeData.Key + "�������");
                                Debug.LogWarning("���������ļ�" + localFile.Name + " " + System.DateTime.Now);
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
                Debug.LogWarning("ȫ��AB���������");
                loadText.text = "ȫ��AB���������";


                string localDllOrApkPath = "";
                string onlineDllOrApkPath = "";
                string onlineDllOrApk_MD5Path = "";


                //�ڲ�ͬƽָ̨����ͬ��·��
                if (Application.isMobilePlatform)
                {
                    //ָ���ȸ���������Դ����·��
                    localDllOrApkPath = $"{Application.persistentDataPath}/APK/TouHouMachineLearningSummary.apk";
                    onlineDllOrApkPath = $"{serverAssetUrl}/APK/TouHouMachineLearningSummary.apk";
                    onlineDllOrApk_MD5Path = $"{serverAssetUrl}/APK/MD5.json";
                }
                else
                {
                    //ָ���ȸ���������Դ����·��
                    localDllOrApkPath = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouHouMachineLearningSummary.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                    //ָ���ȸ���������Դ����·��
                    onlineDllOrApkPath = $"{serverAssetUrl}/Dll/{serverTag}/TouHouMachineLearningSummary.dll";
                    onlineDllOrApk_MD5Path = $"{serverAssetUrl}/Dll/{serverTag}/MD5.json";
                }

                responseMessage = await httpClient.GetAsync(onlineDllOrApk_MD5Path);
                if (!responseMessage.IsSuccessStatusCode) { Debug.LogError("dll����apk��md5�ļ�����ʧ��"); return; }
                var data = await responseMessage.Content.ReadAsByteArrayAsync();
                //������ֻ��ˣ����apk�����������dll����������������������
                if (data.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localDllOrApkPath).FullName))))
                {
                    Debug.Log("�ļ��޸Ķ�");
                }
                else
                {
                    responseMessage = await httpClient.GetAsync(onlineDllOrApkPath);
                    if (!responseMessage.IsSuccessStatusCode) { Debug.LogError("�ļ�����ʧ��"); return; }
                    //������ص�dll����apk�ļ�
                    if (!Application.isEditor)
                    {
                        File.WriteAllBytes(localDllOrApkPath, await responseMessage.Content.ReadAsByteArrayAsync());
                        RestartGame();
                    }
                }
            }
            md5.Dispose();
           
            //����AB���������м��س���
            Debug.LogWarning("��ʼ��ʼ��AB��");
            AssetBundle.UnloadAllAssetBundles(true);
            loadText.text = "��Դ��У����ϣ���Ů������~~~~~";
            await SceneCommand.InitAsync(true);
            //��ʾAB�����ؽ���
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
            Debug.LogWarning("AB��������ϣ��л�����������");
            SceneManager.LoadScene("1_LoginScene", LoadSceneMode.Single);
        }
        public void RestartGame()
        {
            if (Application.isMobilePlatform)
            {
                //�������ϰ�׿����
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
            if (GUI.Button(new Rect(0, 150, 100, 50), "���¼���"))
            {
                RestartGame();
            }
            if (GUI.Button(new Rect(0, 200, 100, 50), "�˳�"))
            {
                Application.Quit();

            }
        }
    }
}