using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    class DanmuCommand
    {
        //配置类型
        //配置颜色
        //附加控制器
        //生成空物体
        //配置danmu
        public static async Task CreatDanmuAsync(Event e)
        {
            var tasks = e.BulletModels.Select(async danmuInfo =>
            {
                await Task.Delay((int)(danmuInfo.delayTime * 1000));
                //子弹，场景2
                GameObject newBullet = GameObject.Instantiate(AssetBundleCommand.Load<GameObject>("Bullet", danmuInfo.bulletType.ToString()));
                DanmuTrackManager trackManager = newBullet.AddComponent<DanmuTrackManager>();

                //指定为默认颜色时不对弹幕颜色进行修改
                if (danmuInfo.color != DanmuColor.Default)
                {
                    newBullet.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", danmuInfo.bulletColor);
                }
                //根据子弹类型添加音效
                switch (danmuInfo.bulletType)
                {
                    case DanmuType.BigBall:
                        _ = SoundEffectCommand.PlayAsync(SoundEffectType.Laser);
                        break;
                    case DanmuType.SmallBall:
                        break;
                    case DanmuType.MiBall:
                        break;
                    case DanmuType.Butterfly:
                        break;
                    case DanmuType.Bomb:
                        break;
                    case DanmuType.Heal:
                        break;
                    case DanmuType.TailingBullet:
                        break;
                    case DanmuType.Smoke:
                        break;
                    default:
                        break;
                }
                await trackManager.Execute(e, danmuInfo.track);
            });
            await Task.WhenAll(tasks);
        }
    }
}