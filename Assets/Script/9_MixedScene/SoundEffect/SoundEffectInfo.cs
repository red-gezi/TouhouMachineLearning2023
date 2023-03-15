using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class SoundEffectInfo : MonoBehaviour
    {
        public static GameObject AudioScoure { get; set; } = null;
        public static Dictionary<SoundEffectType, AudioClip> SoundEfects { get; set; } = new Dictionary<SoundEffectType, AudioClip>();
        //void Awake() => Command.SoundEffectCommand.Init();//初始化音效系统
    }
}
