using UnityEngine;
namespace TouhouMachineLearningSummary.Model
{
    class ConfigInfoModel
    {
        public int Width { get; set; }
        public int Heigh { get; set; }
        //1 全屏 2 无边框 3窗口
        public FullScreenMode ScreenMode { get; set; }
        public bool IsFullScreen { get; set; }
        public string UseLanguage { get; set; }
        public float Volume { get; set; }
        public bool H_Mode { get; set; }
        public int IsTestServer { get; set; }
    }
}