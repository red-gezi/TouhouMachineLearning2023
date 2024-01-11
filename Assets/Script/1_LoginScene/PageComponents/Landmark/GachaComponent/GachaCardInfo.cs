using UnityEngine;

namespace TouhouMachineLearningSummary.Info
{
    //单个开卡组件相关信息
    public class GachaCardInfo : MonoBehaviour
    {
        public ParticleSystem faithBrokenParticle;
        public ParticleSystem cardGenerateParticle;
        public GameObject card;
        public Material cardMaterial;

        public GameObject faithUserUi;
        public GameObject faithUserIcon;
        public GameObject cardNameUi;
        
        public GameObject cardCountUi;
        /// <summary>
        /// 卡牌状态，0为未使用，1为未显形，2为显形后背面朝上，3为翻转过来正面显示
        /// </summary>
        public int state = 1;

    }
}