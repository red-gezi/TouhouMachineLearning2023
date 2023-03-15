using TouhouMachineLearningSummary.Command;
using UnityEngine;
namespace TouhouMachineLearningSummary.Other
{
    public class DontDestory : MonoBehaviour
    {
        static bool IsInit { get; set; } = false;
        void Awake()
        {
            if (IsInit)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this);
                IsInit = true;
            }
        }

        private void OnApplicationQuit() => TaskThrowCommand.Token.Cancel();
    }
}