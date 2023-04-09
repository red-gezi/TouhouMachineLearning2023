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
        public void InitLandmark(int landmarkType)
        {
            Info.PageComponentInfo.Instance.GachaComponent.SetActive(false);


            switch ((LandmarkType)landmarkType)
            {
                case LandmarkType.HakureiShrine:
                    break;
                case LandmarkType.MyorenTemple:
                    Info.PageComponentInfo.Instance.GachaComponent.SetActive(true);
                    Command.GachaCommand.InitGachaComponent();
                    break;
                default: break;
            }
        }

    }
}
