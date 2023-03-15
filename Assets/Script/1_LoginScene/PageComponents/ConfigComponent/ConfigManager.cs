using System.IO;
using TMPro;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public partial class ConfigManager : MonoBehaviour
    {
        static ConfigInfoModel configInfo = new ConfigInfoModel();
        static string ConfigFileSavePath => Application.isMobilePlatform ? Application.persistentDataPath : Directory.GetCurrentDirectory();

        private static void SaveConfig() => File.WriteAllText(ConfigFileSavePath + "/GameConfig.ini", configInfo.ToJson());
        public static void InitConfig()
        {
            //判断有无本地配置文件
            //若无则创建默认配置文件
            //若有则控件值等于储存文件值

            if (!File.Exists(ConfigFileSavePath + "/GameConfig.ini"))
            {
                Debug.Log("生成默认配置文件");
                configInfo.Width = Screen.currentResolution.width;
                configInfo.Heigh = Screen.currentResolution.height;
                configInfo.IsFullScreen = Screen.fullScreen;
                configInfo.UseLanguage = TranslateManager.currentLanguage;
                Directory.CreateDirectory(ConfigFileSavePath);
                SaveConfig();
            }
            else
            {
                Debug.Log("加载已有配置文件");
                configInfo = File.ReadAllText(ConfigFileSavePath + "/GameConfig.ini").ToObject<ConfigInfoModel>();
                Debug.LogError("设置分辨率" + configInfo.Width + " " + configInfo.Heigh + " " + configInfo.IsFullScreen);
                Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.IsFullScreen);
                TranslateManager.currentLanguage = configInfo.UseLanguage;
            }


        }
        /// <summary>
        /// 根据当前平台和配置文件获得相应热更版本的标签
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
                return (configInfo.IsTestServer == 1) ? "Test" : "PC";
            }
        }
        /////////////////////////////////////////////////////////////////////////////游戏设置界面指令////////////////////////////////////////////////////////////////////////////////
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
    }
}