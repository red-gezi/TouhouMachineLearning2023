using Sirenix.OdinInspector;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.Test
{
    public class MusicTest : MonoBehaviour
    {
        [Button]
        public void TestAudio(int type)
        {
            MusicCommand.Play((MusicType)type);
        }
    }
}