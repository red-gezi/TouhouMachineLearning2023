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
        //����֪ͨ
        public GameObject RestartNotice;
        //������ѡ�����
        public GameObject ServerSelect;

        string serverTag = "PC";
        //�����Զ����޸ķ�����ip
        //string serverIP = File.ReadAllLines("������Ϣ.txt")[1];

        MD5 md5 = new MD5CryptoServiceProvider();

        static bool isEditor = Application.isEditor;//�Ƿ��Ǳ༭��״̬
        static bool isMobile = Application.isMobilePlatform;//�Ƿ����ƶ�ƽ̨
        static string downLoadPath = isMobile switch
        {
            true => Application.persistentDataPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/",
            false => Application.streamingAssetsPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/"
        };
        //���򼯴洢·��
        static string dllFIleRootPath = isMobile switch
        {
            true => new DirectoryInfo(Application.persistentDataPath).Parent.FullName,
            false => Directory.GetCurrentDirectory()
        };
        async void Start()
        {
            RestartNotice.transform.localScale = new Vector3(1, 0, 1);
            versiousText.text = "��Ҫ�ĳ�v5";
            loadText.text = "��ʼ��������Ϣ";
            ConfigManager.InitConfig();
            loadText.text = "У����Դ��";
        }
        public void ChangeServer(int selectIndex) => serverTag = selectIndex == 0 ? "PC" : "Android";
        public async void StartGame()
        {
            //������ֻ�ģʽǿ��ʹ����ʽ�氲׿��Դ����׿�����в����ˣ�
            if (Application.isMobilePlatform)
            {
                serverTag = "Android";
            }
            //�ص�ѡ�����
            ServerSelect.SetActive(false);
            //����������
            await CheckAssetBundles();
        }
        //У�鱾���ļ�
        private async Task CheckAssetBundles()
        {
            bool isNeedRestartApplication = false;
            //���º�������
            int downloadTaskCount = 0;
            loadText.text = "���AB����";
            //�༭��ģʽ�²���������
            if (!isEditor)
            {
                //AB���洢·��

                loadText.text = "��ʼ�����ļ�";
                Debug.LogWarning("��ʼ�����ļ�" + System.DateTime.Now);
                Directory.CreateDirectory(downLoadPath);
                var httpClient = new HttpClient();
                var responseMessage = await httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/MD5.json");
                if (!responseMessage.IsSuccessStatusCode)
                {
                    loadText.text = "MD5�ļ���ȡ����";
                    return;
                }

                var OnlieMD5FiIeDatas = await responseMessage.Content.ReadAsStringAsync();
                var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
                Debug.Log("MD5�ļ��Ѽ������" + OnlieMD5FiIeDatas);
                loadText.text = "MD5�ļ��Ѽ�����ɣ�";

                //У�鱾���ļ�
                downloadTaskCount = 0;

                foreach (var MD5FiIeData in Md5Dict)
                {
                    //��ǰУ��ı����ļ�
                    FileInfo localFile = null;
                    if (MD5FiIeData.Key.EndsWith(".dll")) //�������Ϸ����dll�ļ�������ݲ�ͬƽ̨�ԱȲ�ͬ·���µ���Ϸ����dll
                    {

                        Debug.LogError("��ǰ����·��Ϊ" + dllFIleRootPath);
                        string currentDllPath = new DirectoryInfo(dllFIleRootPath).GetFiles("TouHouMachineLearning.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                        Debug.LogError("��ǰΪ�ű�·���ڣ�" + currentDllPath);
                        localFile = new FileInfo(currentDllPath);
                        Debug.Log("����dll�ļ�md5ֵΪ��" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                        Debug.Log("����dll�ļ��޸�ʱ��Ϊ��" + localFile.LastWriteTime);
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
                        loadText.text = MD5FiIeData.Key + "У��ɹ�";
                        Debug.LogWarning(MD5FiIeData.Key + "У��ɹ�");
                    }
                    else
                    {
                        loadText.text = MD5FiIeData.Key + "���°汾����ʼ��������";
                        Debug.LogError(MD5FiIeData.Key + "���°汾����ʼ��������");

                        string savePath = localFile.FullName;

                        if (MD5FiIeData.Key.EndsWith(".dll")) //���dll������Ҫ������Ϸ��������
                        {
                            Debug.LogWarning("��Ҫ����");
                            loadText.text = MD5FiIeData.Key + "���´�����Դ";
                            isNeedRestartApplication = true;
                            Debug.LogError("���븲��·��:" + savePath);
                            Debug.LogError("���ش���MD5ֵ" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                            Debug.LogError("�������MD5ֵ" + MD5FiIeData.Value.ToJson());
                        }
                        await DownLoadFile(MD5FiIeData, localFile, savePath);
                        async Task DownLoadFile(KeyValuePair<string, byte[]> MD5FiIeData, FileInfo localFile, string savePath)
                        {
                            loadText.text = $"��������:{MD5FiIeData.Key},���� {downloadTaskCount}/{Md5Dict.Count}";
                            WebClient webClient = new WebClient();
                            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                            await webClient.DownloadFileTaskAsync(new System.Uri($"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/{MD5FiIeData.Key}"), savePath);
                            Debug.LogWarning(MD5FiIeData.Key + "�������");
                            Debug.LogWarning("���������ļ�" + localFile.Name + " " + System.DateTime.Now);
                            void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
                            {
                                processText.text = $"{e.BytesReceived / 1024 / 1024}MB/{e.TotalBytesToReceive / 1024 / 1024}MB";
                                slider.value = e.BytesReceived * 1f / e.TotalBytesToReceive;
                            }
                        }
                    }
                    downloadTaskCount++;
                }
                Debug.LogWarning("ȫ���������");
                loadText.text = "ȫ���������";
                md5.Dispose();
                httpClient.Dispose();
                //����Ķ���dll����Ҫ����
                if (isNeedRestartApplication)
                {
                    Debug.Log("��Ҫ����");
                    //��������ȷ�ϵû�����
                    RestartNotice.GetComponent<AudioSource>().Play();
                    await CustomThread.TimerAsync(0.5f, (process) =>
                    {
                        RestartNotice.transform.localScale = new Vector3(1, process, 1);
                    });
                    //�ȴ��û����������ٽ��м���
                    return;
                }
            }

            //����AB���������м��س���
            Debug.LogWarning("��ʼ��ʼ��AB��");
            AssetBundle.UnloadAllAssetBundles(true);
            loadText.text = "��ʼ������Դ��";
            await SceneCommand.InitAsync(true);
            Debug.LogWarning("��ʼ����ϣ����س���������");
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