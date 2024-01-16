using System.IO;
using TMPro;
using TouhouMachineLearningSummary.Config;
using TouhouMachineLearningSummary.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    public partial class ConfigManager : MonoBehaviour
    {
        static GameConfig Config => GameConfig.Instance;
        static string ConfigFileSavePath => Application.isMobilePlatform ? Application.persistentDataPath : Directory.GetCurrentDirectory();

        private static void SaveConfig() => File.WriteAllText(ConfigFileSavePath + "/GameConfig.ini", Config.ToJson());
        public static void InitConfig()
        {
            //�ж����ޱ��������ļ�
            //�����򴴽�Ĭ�������ļ�
            //������ؼ�ֵ���ڴ����ļ�ֵ

            if (!File.Exists(ConfigFileSavePath + "/GameConfig.ini"))
            {
                Debug.Log("����Ĭ�������ļ�");
                Config.Width = Screen.currentResolution.width;
                Config.Heigh = Screen.currentResolution.height;
                Config.IsFullScreen = Screen.fullScreen;
                Config.UseLanguage = TranslateManager.currentLanguage;
                Config.ServerMode = "PC_Release";
                Directory.CreateDirectory(ConfigFileSavePath);
                SaveConfig();
            }
            else
            {
                Debug.Log("�������������ļ�");
                GameConfig.Instance = File.ReadAllText(ConfigFileSavePath + "/GameConfig.ini").ToObject<GameConfig>();
                Debug.Log("���÷ֱ���" + Config.Width + " " + Config.Heigh + " " + Config.IsFullScreen);
                Screen.SetResolution(Config.Width, Config.Heigh, Config.IsFullScreen);
                TranslateManager.currentLanguage = Config.UseLanguage;
            }


        }
        /// <summary>
        /// ���ݵ�ǰƽ̨�������ļ������Ӧ�ȸ��汾�ı�ǩ
        /// </summary>
        /// <returns></returns>
        public static string GetServerTag() => Application.isMobilePlatform ? "Android" : Config.ServerMode;

        /////////////////////////////////////////////////////////////////////////////��Ϸ���ý���ָ��////////////////////////////////////////////////////////////////////////////////
        public TextMeshProUGUI ResolutionText;
        public TextMeshProUGUI LanguageText;
        public TextMeshProUGUI CodeText;
        public Slider musicVolume;
        public Slider soundEffectVolume;
        public void SetResolution(int index)
        {
            Config.Width = int.Parse(ResolutionText.text.Split("*")[0]);
            Config.Heigh = int.Parse(ResolutionText.text.Split("*")[1]);
            Debug.Log(Config.Width + "_" + Config.Heigh + "_" + Config.IsFullScreen);
            Screen.SetResolution(Config.Width, Config.Heigh, Config.IsFullScreen);
            SaveConfig();
        }
        public void SetScreenMode(int index)
        {
            Config.IsFullScreen = (index == 0);
            Config.Width = int.Parse(ResolutionText.text.Split("*")[0]);
            Config.Heigh = int.Parse(ResolutionText.text.Split("*")[1]);
            Debug.Log(Config.Width + "_" + Config.Heigh + "_" + Config.IsFullScreen);
            Screen.SetResolution(Config.Width, Config.Heigh, Config.IsFullScreen);
            SaveConfig();
        }
        public void SetLanguage(int index)
        {
            Config.UseLanguage = LanguageText.text;
            TranslateManager.currentLanguage = Config.UseLanguage;
            SaveConfig();
        }
        public void SetMusicVolume(float volume)
        {
            Config.MaxMusicVolume = musicVolume.value;
            SaveConfig();
        }
        public void SetSoundEffectVolume(float volume)
        {
            Config.MaxSoundEffectVolume = soundEffectVolume.value;
            SaveConfig();
        }
        public void SetH_Mode(int index)
        {
            Config.UseLanguage = LanguageText.text;
            TranslateManager.currentLanguage = Config.UseLanguage;
            SaveConfig();
        }
        public void SendCode()
        {

        }
        public void SetServer(int selectIndex)
        {
            Config.ServerMode = selectIndex == 0 ? "PC_Release" : "PC_Test";
            SaveConfig();
        }
    }
}