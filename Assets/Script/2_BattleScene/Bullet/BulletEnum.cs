
using UnityEngine;
namespace TouhouMachineLearningSummary.GameEnum
{
    [SerializeField]
    public enum BulletType
    {
        //DreamOfSeal,
        //Hurt,
        BigBall,
        SmallBall,
        MiBall,
        Butterfly,
        Bomb,//从天而降的轰炸
        Heal,
        TailingBullet
    }
    //卡牌轨迹
    public enum BulletTrack
    {
        Round,//环绕
        Line,//直射
        Parabola,//抛射
        Fixed,//直接在卡牌上生效
        Down,//从天而降
        Test,
    }
    public enum BulletColor
    {
        Default,
        Red,
        Blue,
        Green,
        White,
        Black,
    }
    //击中对方后的演出特效
    public enum BulletStartEffect
    {
        None,
        Smoke
    }
    //击中对方后的演出特效
    public enum BulletEndEffect
    {
        None,
        Smoke
    }
}