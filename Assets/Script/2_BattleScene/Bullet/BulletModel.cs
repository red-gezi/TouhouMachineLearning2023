using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.Model
{
    public class BulletModel
    {
        //弹幕类型-大球/小球/米弹
        public BulletType bulletType;
        //弹幕颜色
        public BulletColor color;
        public Color bulletColor
        {
            get
            {
                Color bulletColor = Color.white;
                switch (color)
                {
                    case BulletColor.Default:
                        break;
                    case BulletColor.Red:
                        bulletColor = Color.red;
                        break;
                    case BulletColor.Blue:
                        bulletColor = Color.blue;
                        break;
                    case BulletColor.Green:
                        bulletColor = Color.green;
                        break;
                    default:
                        break;
                }
                return bulletColor * 10;
            }
        }
        //弹幕轨迹
        public BulletTrack track;
        //弹幕开始效果
        public BulletStartEffect startEffect;
        //弹幕结束效果
        public BulletEndEffect endEffect;
        public float speed = 1;
        public BulletModel(BulletType bulletType = BulletType.BigBall, BulletColor color = BulletColor.Default, BulletTrack track = BulletTrack.Parabola, BulletStartEffect startEffect = BulletStartEffect.None, BulletEndEffect endEffect = BulletEndEffect.None, float speed = 1)
        {
            this.bulletType = bulletType;
            this.color = color;
            this.track = track;
            this.startEffect = startEffect;
            this.endEffect = endEffect;
            this.speed = speed;
        }
    }
}