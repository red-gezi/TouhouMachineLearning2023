using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public class CoinManager : MonoBehaviour
    {
        ////////////////////////////硬币相关动画曲线/////////////////////
        public AnimationCurve regionCcurve;
        public AnimationCurve campCcurve;
        public AnimationCurve centerCcurve;
        public AnimationCurve scaleCcurve;
        ////////////////////////////硬币相关组件/////////////////////
        public GameObject coin;
        public GameObject camp;
        public GameObject center;
        public List<GameObject> Regions;

        static float regionTime;
        static Vector3 regions_start;
        static Vector3 regions_end;

        static float campTime;
        static Vector3 camp_start;
        static Vector3 camp_end;

        static float centerTime;
        static Vector3 center_start;
        static Vector3 center_end;

        static float scaleTime;
        static Vector3 scalePos_start;
        static Vector3 scalePos_end;
        static Vector3 targetCoinScale = Vector3.one;

        [ShowInInspector]
        public Vector3 startpos => transform.position;
        [ShowInInspector]
        public Vector3 endpos => transform.localPosition;
        // Start is called before the first frame update
        ////////////////////////////硬币计时器//////////////////////////
        static bool isTimerStart;
        public static int limitTime { get; set; }
        public static float time { get; set; }
        public static bool isTimeout => time > limitTime;
        [ShowInInspector]
        public static int Process => Mathf.Clamp((int)(time / limitTime * 360), 0, 360);
        public static void SetCoinTimerStart(int limit_time)
        {
            time = 0;
            isTimerStart = true;
            limitTime = limit_time;
        }
        public static void SetCoinTimerClose()
        {
            time = 0;
            isTimerStart = false;
        }

        void Start()
        {
            scalePos_start = transform.position;
            scalePos_end = transform.position;
            _ = TimerInit();
        }

        private async Task TimerInit()
        {
            await Task.Delay(1000);
            Fold();
            ShowCurrentPlayerCoin(true);
            while (true)
            {
                TaskThrowCommand.Throw();
                coin.transform.GetChild(1).transform.eulerAngles = new Vector3(0, 0, -Process);
                await Task.Delay(1000);
            }
        }

        private void Update()
        {
            Regions.ForEach(region => region.transform.localPosition = Vector3.Lerp(regions_start, regions_end, regionCcurve.Evaluate(Time.time - regionTime)));
            camp.transform.eulerAngles = Vector3.Lerp(camp_start, camp_end, campCcurve.Evaluate(Time.time - campTime));
            center.transform.eulerAngles = Vector3.Lerp(center_start, center_end, centerCcurve.Evaluate((Time.time - centerTime)));
            coin.transform.position = Vector3.Lerp(transform.position, scalePos_end, Time.deltaTime * 5);
            coin.transform.localScale = Vector3.Lerp(transform.localScale, targetCoinScale, Time.deltaTime * 5);

            //硬币计时
            if (isTimerStart)
            {
                time += Time.deltaTime;
            }
        }
        //绑定于ui
        public void ChangeProperty(int i)
        {
            if (Info.AgainstInfo.IsWaitForSelectProperty)
            {
                ChangePropertyAsync((BattleRegion)i);
            }
        }
        [Button("切换属性")]
        public static async Task ChangePropertyAsync(BattleRegion region)
        {
            Info.AgainstInfo.SelectProperty = region;
            Debug.Log("设置属性为" + Info.AgainstInfo.SelectProperty);
            center_end = new Vector3(0, 0, 360 + (int)region * 90);
            centerTime = Time.time;
            await Task.Delay(1000);
            Fold();
        }
        [Button("展开")]
        public static void Unfold()
        {
            regions_start = new Vector3(0, 0, 0);
            regions_end = new Vector3(0, 25, 0);
            regionTime = Time.time;
        }
        [Button("关闭")]
        public static void Fold()
        {
            regions_start = new Vector3(0, 25, 0);
            regions_end = new Vector3(0, 0, 0);
            regionTime = Time.time;
        }
        [Button("旋转")]
        public void ShowCurrentPlayerCoin(bool isMyturn)
        {
            camp_start = new Vector3(0, isMyturn ? 180 : 0, 0);
            camp_end = new Vector3(0, isMyturn ? 0 : 180, 0);
            campTime = Time.time;
        }
        [Button("放大")]
        public static void ScaleUp()
        {
            targetCoinScale = Vector3.one * 3;
            scalePos_end = new Vector3(1920 / 2, 1080 / 2, 0);
        }
        [Button("缩小")]
        public static void ScaleDown()
        {
            targetCoinScale = Vector3.one;
            scalePos_end = scalePos_start;
        }
    }
}