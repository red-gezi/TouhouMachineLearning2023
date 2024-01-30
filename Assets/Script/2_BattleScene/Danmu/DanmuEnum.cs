
using UnityEngine;
namespace TouhouMachineLearningSummary.GameEnum
{
    [SerializeField]
    public enum DanmuType
    {
        BigBall,
        SmallBall,
        MiBall,
        Butterfly,
        Bomb,//从天而降的轰炸
        Heal,
        TailingBullet,
        Smoke,
        MagicCircle,
    }
    //卡牌轨迹
    public enum DanmuTrack
    {
        Round,//环绕
        Line,//直射
        Parabola,//抛射
        FixedOnTriggerCard,//在触发卡牌上生效
        FixedOnTargetCard,//在目标卡牌上生效
        Down,//从天而降
        RayLine,//在触发卡牌上射向目标卡牌
        Test,
    }
    public enum DanmuColor
    {
        Default,
        Red,
        Blue,
        Green,
        White,
        Black,
    }
}