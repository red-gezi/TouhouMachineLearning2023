using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Info
{
    //用于展示单人多人页面中的卡组面板
    public class SingleOpenCardInfo : MonoBehaviour
    {
        public ParticleSystem faithBrokenParticle;
        public ParticleSystem cardGenerateParticle;
        public Material cardMaterial;
        //卡牌状态，0为未使用，1为未显形，2为显形后背面朝上，3为翻转过来正面显示
        public int state = 1;
        private void Awake()
        {
            cardMaterial = new Material(GetComponent<Image>().material);
            GetComponent<Image>().material = cardMaterial;  // 这里重新设置下材质参数
        }

    }
}