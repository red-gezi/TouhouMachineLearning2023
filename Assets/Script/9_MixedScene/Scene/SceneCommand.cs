using System.Threading.Tasks;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    /// <summary>
    /// 用于不同场景初始化操作
    /// </summary>
    internal class SceneCommand
    {
        static bool IsInit { get; set; }
        public static async Task InitAsync(bool isHotFixedLoad)
        {
            //设置帧数
            //Application.targetFrameRate = 60;
            //加载AB包(仅一次)
            await AssetBundleCommand.Init(isHotFixedLoad);
            //初始化网络系统，用于获取指定版本卡牌信息
            await NetCommand.Init();
            //主界面加载时再加载音效
            SoundEffectCommand.Init();
            TitleCommand.Load();
            //加载本地卡牌数据(仅编辑器模式下需要)
            if (Application.isEditor)
            {
                //Debug.LogError("当前是编辑器模式");
                InspectorCommand.LoadFromJson();
            }
            //根据当前卡牌版本 加载卡牌和卡画数据
            await CardAssemblyManager.SetCurrentAssembly(Info.AgainstInfo.CurrentCardScriptsVersion);
            //加载状态与字段
        }
    }
}
