using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TouhouMachineLearningSummary.Manager
{
    public enum LandmarkType
    {
        HakureiShrine,
        MyorenTemple
    }
    public class LandmarkManager : MonoBehaviour
    {
        LandmarkManager Instance { get; set; }
        public void Awake() => Instance = this;
        public void InitHakureiShrine()
        {
            Info.PageComponentInfo.Instance.GachaComponent.SetActive(false);
        }
        public void InitMyorenTemple()
        {
            Info.PageComponentInfo.Instance.GachaComponent.SetActive(true);
            Command.GachaCommand.InitGachaComponent();
        }
    }
}
