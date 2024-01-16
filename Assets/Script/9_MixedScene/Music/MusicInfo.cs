using Sirenix.OdinInspector;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class MusicInfo : MonoBehaviour
    {
        public static MusicInfo Instance { get; set; }
        private void Awake() => Instance = this;
        public AudioSource audioScoure;
        [ShowInInspector]
        public static Dictionary<MusicType, AudioClip> Musics { get; set; } = new();
    }
}
