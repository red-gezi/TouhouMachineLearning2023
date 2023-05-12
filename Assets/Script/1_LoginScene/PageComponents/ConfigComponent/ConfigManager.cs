using System.IO;
using TMPro;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public partial class ConfigManager : MonoBehaviour
    {
        static ConfigInfoModel configInfo = new ();
        static string ConfigFileSavePath => Application.isMobilePlatform ? Application.persistentDataPath : Directory.GetCurrentDirectory();

        private static void SaveConfig() => File.WriteAllText(ConfigFileSavePath + "/GameConfig.ini", configInfo.ToJson());
        public static void InitConfig()
        {
            //�ж����ޱ��������ļ�
            //�����򴴽�Ĭ�������ļ�
            //������ؼ�ֵ���ڴ����ļ�ֵ

            if (!File.Exists(ConfigFileSavePath + "/GameConfig.ini"))
            {
                Debug.Log("����Ĭ�������ļ�");
                configInfo.Width = Screen.currentResolution.width;
                configInfo.Heigh = Screen.currentResolution.height;
                configInfo.IsFullScreen = Screen.fullScreen;
                configInfo.UseLanguage = TranslateManager.currentLanguage;
                configInfo.ServerMode = "PC_Release";
                Directory.CreateDirectory(ConfigFileSavePath);
                SaveConfig();
            }
            else
            {
                Debug.Log("�������������ļ�");
                configInfo = File.ReadAllText(ConfigFileSavePath + "/GameConfig.ini").ToObject<ConfigInfoModel>();
                Debug.Log("���÷ֱ���" + configInfo.Width + " " + configInfo.Heigh + " " + configInfo.IsFullScreen);
                Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.IsFullScreen);
                TranslateManager.currentLanguage = configInfo.UseLanguage;
            }


        }
        /// <summary>
        /// ���ݵ�ǰƽ̨�������ļ������Ӧ�ȸ��汾�ı�ǩ
        /// </summary>
        /// <returns></returns>
        public static string GetServerTag()
        {
            if (Application.isMobilePlatform)
            {
                return "Android";
            }
            else
            {
                return configInfo.ServerMode;
            }
        }

        /////////////////////////////////////////////////////////////////////////////��Ϸ���ý���ָ��////////////////////////////////////////////////////////////////////////////////
        public TextMeshProUGUI ResolutionText;
        public TextMeshProUGUI LanguageText;
        public TextMeshProUGUI CodeText;
        public void SetResolution(int index)
        {
            configInfo.Width = int.Parse(ResolutionText.text.Split("*")[0]);
            configInfo.Heigh = int.Parse(ResolutionText.text.Split("*")[1]);
            Debug.Log(configInfo.Width + "_" + configInfo.Heigh + "_" + configInfo.IsFullScreen);
            Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.IsFullScreen);
            SaveConfig();
        }
        public void SetScreenMode(int index)
        {
            configInfo.IsFullScreen = (index == 0);
            configInfo.Width = int.Parse(ResolutionText.text.Split("*")[0]);
            configInfo.Heigh = int.Parse(ResolutionText.text.Split("*")[1]);
            Debug.Log(configInfo.Width + "_" + configInfo.Heigh + "_" + configInfo.IsFullScreen);
            Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.IsFullScreen);
            SaveConfig();
        }
        public void SetLanguage(int index)
        {
            configInfo.UseLanguage = LanguageText.text;
            TranslateManager.currentLanguage = configInfo.UseLanguage;
            SaveConfig();
        }
        public void SetVolume(int index)
        {
            configInfo.UseLanguage = LanguageText.text;
            TranslateManager.currentLanguage = configInfo.UseLanguage;
            SaveConfig();
        }
        public void SetH_Mode(int index)
        {
            configInfo.UseLanguage = LanguageText.text;
            TranslateManager.currentLanguage = configInfo.UseLanguage;
            SaveConfig();
        }
        public void SendCode()
        {

        }
        public void SetServer(int selectIndex)
        {
            configInfo.ServerMode = selectIndex == 0 ? "PC_Release" : "PC_Test";
            SaveConfig();
        }
    }
}