using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    public class LoadingBarManager : MonoBehaviour
    {
        public AnimationCurve curve;
        void Update() => Enumerable.Range(0, 8).ToList().ForEach(i => GetComponent<Image>().material.SetFloat("_heigh_" + i, curve.Evaluate((i / 7f + Time.time) % 1)));
    }
}