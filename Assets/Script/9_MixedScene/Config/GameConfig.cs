using UnityEngine;
namespace TouhouMachineLearningSummary.Config
{
    class GameConfig
    {
        public static GameConfig Instance { get; set; } = new();

        public int Width { get; set; }
        public int Heigh { get; set; }
        //1 全屏 2 无边框 3窗口
        public FullScreenMode ScreenMode { get; set; }
        public bool IsFullScreen { get; set; }
        public string UseLanguage { get; set; }
        //用户设置的音乐音量倍数
        public float MaxMusicVolume { get; set; } = 1;
        //用户设置的音乐音量倍数
        public float MaxSoundEffectVolume { get; set; } = 1;
        public bool H_Mode { get; set; }
        public string ServerMode { get; set; }
    }
}