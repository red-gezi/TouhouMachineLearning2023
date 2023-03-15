using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    //废弃
    public class ParticleInfo : MonoBehaviour
    {
        public static ParticleInfo Instance;
        public ParticleSystem[] ParticleEffect;
        private void Awake() => Instance = this;
        public GameObject GainBullet;
        public GameObject HurtBullet;
    }
}

