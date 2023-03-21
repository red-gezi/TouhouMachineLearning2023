using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    //用于展示单人多人页面中的卡组面板
    public class OpenCardManager : MonoBehaviour
    {
        public ParticleSystem faithBrokenParticle;
        public ParticleSystem cardGenerateParticle;
        public Material cardMaterial;
        private void Start()
        {
            cardMaterial = new Material(GetComponent<Image>().material);
            GetComponent<Image>().material = cardMaterial;  // 这里重新设置下材质参数
        }
        public async void FaithClick()
        {
            faithBrokenParticle.Play();
            await Task.Delay(1500);
            cardGenerateParticle.Play();
            await CustomThread.TimerAsync(1, process =>
            {
                cardMaterial.SetFloat("_process", Mathf.Lerp(345, 0, process));
                cardGenerateParticle.transform.localPosition = Vector3.Lerp(new(0, 360, 0), new Vector3(0, -345, 0), process);
            });
            cardGenerateParticle.Stop();
        }
    }
}