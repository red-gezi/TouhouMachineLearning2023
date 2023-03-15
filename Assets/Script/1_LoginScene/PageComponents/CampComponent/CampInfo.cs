using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Info
{
    public class CampInfo
    {
        //public static List<SingleCampInfo> campInfos = new List<SingleCampInfo>();
        public static List<(Camp camp, string campName, string campIntroduction, Sprite campTex)> campInfos = new();
        public static List<CardModel> leaderInfos = new List<CardModel>();
        public static (Camp camp, string campName, string campIntroduction, Sprite campTex) GetCampInfo(Camp camp) => campInfos.FirstOrDefault(info => info.camp == camp);
        public static CardModel GetLeaderInfo(string leaderId) => leaderInfos.FirstOrDefault(leader => leader.cardID == leaderId);
        public class SingleCampInfo
        {
            public Camp camp;
            public string campName;
            public string campIntroduction;
            public Sprite campTex;
            public SingleCampInfo(Camp camp, string campName, string campIntroduction, Sprite campTex)
            {
                this.camp = camp;
                this.campName = campName;
                this.campIntroduction = campIntroduction;
                this.campTex = campTex;
            }
        }
    }
}