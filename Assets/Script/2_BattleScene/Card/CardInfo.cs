using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class CardInfo : MonoBehaviour
    {
        public static int CreatCardRank;//卡牌创建时的自增命名
        public static GameObject cardModel;
        public GameObject card_Model;
        private void Awake() => cardModel = card_Model;
    }
}

