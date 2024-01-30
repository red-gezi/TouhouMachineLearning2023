using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.Model
{
    public class DanmuModel
    {
        //弹幕类型-大球/小球/米弹
        public DanmuType bulletType;
        //弹幕颜色
        public DanmuColor color;
        public Color bulletColor
        {
            get
            {
                Color bulletColor = Color.white;
                switch (color)
                {
                    case DanmuColor.Default:
                        break;
                    case DanmuColor.Red:
                        bulletColor = Color.red;
                        break;
                    case DanmuColor.Blue:
                        bulletColor = Color.blue;
                        break;
                    case DanmuColor.Green:
                        bulletColor = Color.green;
                        break;
                    default:
                        break;
                }
                return bulletColor * 10;
            }
        }
        //弹幕轨迹
        public DanmuTrack track;
        public float delayTime = 1;
        //public BulletModel(
        //    BulletType bulletType = BulletType.BigBall, 
        //    BulletColor color = BulletColor.Default, 
        //    BulletTrack track = BulletTrack.Parabola, 
        //    BulletStartEffect startEffect = BulletStartEffect.None, 
        //    BulletEndEffect endEffect = BulletEndEffect.None,
        //    float speed = 1)
        //{
        //    this.bulletType = bulletType;
        //    this.color = color;
        //    this.track = track;
        //    this.startEffect = startEffect;
        //    this.endEffect = endEffect;
        //    this.speed = speed;
        //}
        public DanmuModel(
            DanmuType bulletType = DanmuType.BigBall,
            DanmuColor color = DanmuColor.Default,
            DanmuTrack track = DanmuTrack.Parabola,
            float delayTime = 1)
        {
            this.bulletType = bulletType;
            this.color = color;
            this.track = track;
            this.delayTime = delayTime;
        }
    }
}