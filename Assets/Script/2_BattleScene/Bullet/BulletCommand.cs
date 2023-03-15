using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    class BulletCommand
    {
        //配置类型
        //配置颜色
        //附加控制器
        //生成空物体
        //配置danmu
        public static async Task InitBulletAsync(Event e)
        {
            //bool isEnd = false;
            Model.BulletModel danmuInfo = e.BulletModel;
            if (danmuInfo != null)
            {
                BulletTrackManager trackManager = null;
                //子弹，场景2
                GameObject newBullet = GameObject.Instantiate(AssetBundleCommand.Load<GameObject>("Bullet", danmuInfo.bulletType.ToString()));
                if (danmuInfo.color != BulletColor.Default)
                {
                    newBullet.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", danmuInfo.bulletColor);
                }
                trackManager = newBullet.AddComponent<BulletTrackManager>();
                await trackManager.Play(e, danmuInfo.track);
            }
        }
    }
}