using System;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Test
{
    public class BulletOnGui : MonoBehaviour
    {
        public List<string> toolbarStrings => Init(typeof(BulletType));
        private static List<string> Init(Type type)
        {
            List<string> toolbarStrings = new List<string>();
            var s = Enum.GetValues(type);
            foreach (var item in s)
            {
                toolbarStrings.Add(item.ToString());
            }
            return toolbarStrings;
        }
        int BulletTypeInt;
        int BulletColorInt;
        int BulletTrackInt;
        private void OnGUI()
        {
            if (Application.isEditor)
            {
                BulletTypeInt = GUI.Toolbar(new Rect(25, 25, 1250, 30), BulletTypeInt, Init(typeof(BulletType)).ToArray());
                BulletColorInt = GUI.Toolbar(new Rect(25, 55, 1250, 30), BulletColorInt, Init(typeof(BulletColor)).ToArray());
                BulletTrackInt = GUI.Toolbar(new Rect(25, 85, 1250, 30), BulletTrackInt, Init(typeof(BulletTrack)).ToArray());
                if (GUI.Button(new Rect(25, 150, 250, 30), "´¥·¢Ð§¹û"))
                {
                    var tirggerCard = Info.AgainstInfo.cardSet[Orientation.Down][GameRegion.Battle].CardList.FirstOrDefault();
                    var targetCards = Info.AgainstInfo.cardSet[Orientation.Up][GameRegion.Battle].CardList.Take(3).ToList();
                    _ = GameSystem.PointSystem.Hurt(new Event(tirggerCard, targetCards)
                        .SetPoint(1)
                        .SetBullet(
                           new Model.BulletModel
                            (
                                (BulletType)BulletTypeInt,
                                (BulletColor)BulletColorInt,
                                (BulletTrack)BulletTrackInt
                            )));
                }
                if (GUI.Button(new Rect(25, 180, 250, 30), "ÕÙ»½¿¨Æ¬"))
                {
                    var cards1 = Info.AgainstInfo.cardSet[Orientation.Down][GameRegion.Deck].CardList.Take(3);
                    _ = GameSystem.TransferSystem.SummonCard(new Event(null, cards1.ToList()));
                    var cards2 = Info.AgainstInfo.cardSet[Orientation.Up][GameRegion.Deck].CardList.Take(3);
                    _ = GameSystem.TransferSystem.SummonCard(new Event(null, cards2.ToList()));
                }
            }
        }
    }
}