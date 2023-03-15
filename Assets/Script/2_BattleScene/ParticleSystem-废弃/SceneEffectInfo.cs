using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class SceneEffectInfo : MonoBehaviour
    {
        public TheWorldEffect _theWorldEffect;
        public static TheWorldEffect theWorldEffect;
        private void Awake() => theWorldEffect = _theWorldEffect;
    }
}
