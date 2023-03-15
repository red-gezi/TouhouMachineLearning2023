using TouhouMachineLearningSummary.Config;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    public class AgainstSummaryItemManager : MonoBehaviour
    {
        public Text playerName;
        public Text time;
        public Text summaryTag;
        public Text result;
        AgainstSummaryManager Summary { get; set; }
        public void ChangeTag()
        {

        }
        public void Favioraty()
        {

        }
        public async void Replay()
        {
            AgainstConfig.Init();
            AgainstConfig.ReplayStart(Summary);
        }
        public void Init(AgainstSummaryManager summary)
        {
            Summary = summary;
            playerName.text = summary.Player1Info.Name + " VS " + summary.Player2Info.Name;
            time.text = summary.UpdateTime.ToString();
            result.text = "Ê¤¸º";
        }
    }
}