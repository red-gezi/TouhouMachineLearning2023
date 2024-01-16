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
            _ = Command.SoundEffectCommand.PlayAsync(type);
        }
    }
}