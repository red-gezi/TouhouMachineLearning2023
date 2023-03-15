using Sirenix.OdinInspector;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using UnityEngine;

namespace TouhouMachineLearningSummary.Test
{
    public class SoundEffectTest : MonoBehaviour
    {
        [Button]
        public async void TestAudio(SoundEffectType type)
        {
            var audioCLip = SoundEffectInfo.SoundEfects[type];
            AudioSource Source = SoundEffectInfo.AudioScoure.GetComponent<AudioSource>();
            Source.clip = audioCLip;
            Source.Play();
            await Task.Delay((int)(audioCLip.length * 1000));
        }
    }
}