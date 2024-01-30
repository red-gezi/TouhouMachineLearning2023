using System;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Test
{
    public class DanmuOnGui : MonoBehaviour
    {
        public bool ShowDanmuTestUI;
        public List<string> toolbarStrings => Init(typeof(DanmuType));
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
        private async void OnGUI()
        {
            if (Application.isEditor&&ShowDanmuTestUI)
            {
                BulletTypeInt = GUI.Toolbar(new Rect(25, 25, 1250, 30), BulletTypeInt, Init(typeof(DanmuType)).ToArray());
                BulletColorInt = GUI.Toolbar(new Rect(25, 55, 1250, 30), BulletColorInt, Init(typeof(DanmuColor)).ToArray());
                BulletTrackInt = GUI.Toolbar(new Rect(25, 85, 1250, 30), BulletTrackInt, Init(typeof(DanmuTrack)).ToArray());
                if (GUI.Button(new Rect(25, 150, 250, 30), "触发效果"))
                {
                    var tirggerCard = Info.AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Battle].ContainCardList.FirstOrDefault();
                    var targetCards = Info.AgainstInfo.GameCardsFilter[Orientation.Up][GameRegion.Battle].ContainCardList.Take(3).ToList();
                    _ = GameSystem.PointSystem.Hurt(new Event(tirggerCard, targetCards)
                        .SetPoint(1)
                        .AddDanmuEffect(new Model.DanmuModel((DanmuType)BulletTypeInt, (DanmuColor)BulletColorInt, (DanmuTrack)BulletTrackInt)));
                }
                if (GUI.Button(new Rect(25, 180, 250, 30), "召唤临时卡片"))
                {
                    var cards1 = Info.AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Deck].ContainCardList.Take(3);
                    _ = GameSystem.TransferSystem.SummonCard(new Event(null, cards1.ToList()));
                    var cards2 = Info.AgainstInfo.GameCardsFilter[Orientation.Up][GameRegion.Deck].ContainCardList.Take(3);
                    _ = GameSystem.TransferSystem.SummonCard(new Event(null, cards2.ToList()));
                }
                if (GUI.Button(new Rect(25, 180, 250, 30), "播放固定组合效果"))
                {
                    var tirggerCard = Info.AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Battle].ContainCardList.FirstOrDefault();
                    var targetCards = Info.AgainstInfo.GameCardsFilter[Orientation.Up][GameRegion.Battle].ContainCardList.Take(3).ToList();
                    //触发伤害事件
                    await GameSystem.PointSystem.Hurt(new Event(tirggerCard, targetCards)
                        //设置伤害点数
                        .SetPoint(1)
                        //延迟0秒，在触发卡牌身上生成蓝色魔法阵
                        .AddDanmuEffect(new Model.DanmuModel(DanmuType.MagicCircle, DanmuColor.Blue, DanmuTrack.FixedOnTriggerCard, 0))
                        //延迟1秒，在触发卡牌身上生成蓝色球射向目标牌
                        .AddDanmuEffect(new Model.DanmuModel(DanmuType.BigBall, DanmuColor.Blue, DanmuTrack.Line, 1))
                        //延迟2秒，在目标卡牌身上生成蓝色烟雾
                        .AddDanmuEffect(new Model.DanmuModel(DanmuType.Smoke, DanmuColor.Blue, DanmuTrack.FixedOnTargetCard, 2))
                        );
                }
            }
        }
    }
}