using Sirenix.OdinInspector;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    public class LoadingManager : MonoBehaviour
    {
        public static LoadingManager manager;
        void Awake()
        {
            manager = this;
        }
        public GameObject ui;

        public GameObject Tex1;
        public GameObject Tex2;
        public GameObject player1;
        public GameObject player2;
        [Button]
        public async Task OpenAsync()
        {
            Tex1.SetActive(true);
            Tex2.SetActive(true);
            player1.SetActive(true);
            player2.SetActive(true);

            player1.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Info.AgainstInfo.CurrentUserInfo.Name;
            player1.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "偶入幻想的异乡人";
            player1.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = Info.AgainstInfo.CurrentUserInfo.UseDeck.DeckName;
            player2.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Info.AgainstInfo.CurrentOpponentInfo.Name;
            player2.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "氪金的土豪";
            player2.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = Info.AgainstInfo.CurrentOpponentInfo.UseDeck.DeckName;
            await CustomThread.TimerAsync(1, process =>
            {
                Tex1.transform.localPosition = Vector3.Lerp(new Vector3(-2300, -265, 0), new Vector3(-1000, -265, 0), process);
                Tex2.transform.localPosition = Vector3.Lerp(new Vector3(2300, -345, 0), new Vector3(1000, 345, 0), process);
            });
            await CustomThread.TimerAsync(1, process =>
            {
                player1.transform.localPosition = Vector3.Lerp(new Vector3(-1200, -372, 0), new Vector3(-500, -372, 0), process);
                player2.transform.localPosition = Vector3.Lerp(new Vector3(1200, 241, 0), new Vector3(500, 241, 0), process);
            });
        }
        [Button]
        public async Task CloseAsync()
        {
            await CustomThread.TimerAsync(1, runAction: process =>
            {
                player1.transform.localPosition = Vector3.Lerp(new Vector3(-1200, -372, 0), new Vector3(-500, -372, 0), 1 - process);
                player2.transform.localPosition = Vector3.Lerp(new Vector3(1200, 241, 0), new Vector3(500, 241, 0), 1 - process);
            });
            await CustomThread.TimerAsync(1, runAction: process =>
            {
                Tex1.transform.localPosition = Vector3.Lerp(new Vector3(-2300, -265, 0), new Vector3(-1000, -265, 0), 1 - process);
                Tex2.transform.localPosition = Vector3.Lerp(new Vector3(2300, 345, 0), new Vector3(1000, 345, 0), 1 - process);
            });
            Tex1.SetActive(false);
            Tex2.SetActive(false);
            player1.SetActive(false);
            player2.SetActive(false);
        }
    }
}

