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
                if (GUI.Button(new Rect(25, 150, 250, 30), "����Ч��"))
                {
                    var tirggerCard = Info.AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Battle].ContainCardList.FirstOrDefault();
                    var targetCards = Info.AgainstInfo.GameCardsFilter[Orientation.Up][GameRegion.Battle].ContainCardList.Take(3).ToList();
                    _ = GameSystem.PointSystem.Hurt(new Event(tirggerCard, targetCards)
                        .SetPoint(1)
                        .AddDanmuEffect(new Model.DanmuModel((DanmuType)BulletTypeInt, (DanmuColor)BulletColorInt, (DanmuTrack)BulletTrackInt)));
                }
                if (GUI.Button(new Rect(25, 180, 250, 30), "�ٻ���ʱ��Ƭ"))
                {
                    var cards1 = Info.AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Deck].ContainCardList.Take(3);
                    _ = GameSystem.TransferSystem.SummonCard(new Event(null, cards1.ToList()));
                    var cards2 = Info.AgainstInfo.GameCardsFilter[Orientation.Up][GameRegion.Deck].ContainCardList.Take(3);
                    _ = GameSystem.TransferSystem.SummonCard(new Event(null, cards2.ToList()));
                }
                if (GUI.Button(new Rect(25, 180, 250, 30), "���Ź̶����Ч��"))
                {
                    var tirggerCard = Info.AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Battle].ContainCardList.FirstOrDefault();
                    var targetCards = Info.AgainstInfo.GameCardsFilter[Orientation.Up][GameRegion.Battle].ContainCardList.Take(3).ToList();
                    //�����˺��¼�
                    await GameSystem.PointSystem.Hurt(new Event(tirggerCard, targetCards)
                        //�����˺�����
                        .SetPoint(1)
                        //�ӳ�0�룬�ڴ�����������������ɫħ����
                        .AddDanmuEffect(new Model.DanmuModel(DanmuType.MagicCircle, DanmuColor.Blue, DanmuTrack.FixedOnTriggerCard, 0))
                        //�ӳ�1�룬�ڴ�����������������ɫ������Ŀ����
                        .AddDanmuEffect(new Model.DanmuModel(DanmuType.BigBall, DanmuColor.Blue, DanmuTrack.Line, 1))
                        //�ӳ�2�룬��Ŀ�꿨������������ɫ����
                        .AddDanmuEffect(new Model.DanmuModel(DanmuType.Smoke, DanmuColor.Blue, DanmuTrack.FixedOnTargetCard, 2))
                        );
                }
            }
        }
    }
}