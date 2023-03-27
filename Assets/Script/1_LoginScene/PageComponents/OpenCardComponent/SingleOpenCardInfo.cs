using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Info
{
    //单个开卡组件信心
    public class SingleOpenCardInfo : MonoBehaviour
    {
        public ParticleSystem faithBrokenParticle;
        public ParticleSystem cardGenerateParticle;
        public GameObject card;
        public Material cardMaterial;

        public GameObject faithUserUi;
        public GameObject cardNameUi;
        public GameObject cardCountUi;
        //卡牌状态，0为未使用，1为未显形，2为显形后背面朝上，3为翻转过来正面显示
        public int state = 1;

    }
}