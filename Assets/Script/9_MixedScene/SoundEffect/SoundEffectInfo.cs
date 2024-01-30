using Sirenix.OdinInspector;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class SoundEffectInfo : MonoBehaviour
    {
        public static SoundEffectInfo Instance { get; set; }
        private void Awake() => Instance = this;
        public GameObject audioScoure  = null;
        [ShowInInspector]
        public static Dictionary<SoundEffectType, AudioClip> SoundEfects { get; set; } = new Dictionary<SoundEffectType, AudioClip>();
    }
}
