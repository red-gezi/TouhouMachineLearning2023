using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
//控制菜单中的摄像机镜头
namespace TouhouMachineLearningSummary.Manager
{
    public class CameraViewManager : MonoBehaviour
    {
        public float frequent;
        public float strength;
        //摄像机是否做一个晃动效果（可被手机陀螺仪影响）
        static bool IsCameraWaggle { get; set; }
        static bool IsOpenWaggle { get; set; }

        static CameraViewManager manager;

        public Transform sceneViewPoint;
        public Transform bookViewPoint;
        public Transform holdViewPoint;
        public Transform pageViewPoint;
        // Start is called before the first frame update
        void Awake() => manager = this;

        private static Vector3 targetPos = Vector3.zero;
        private static Vector3 targetEuler = Vector3.zero;
        /// <summary>
        /// 0 场景视角
        /// 1 书本视角
        /// 2 等待视角
        /// 3 页面视角
        /// </summary>
        /// <param name="isImmediately"></param>
        /// <returns></returns>
        public static async Task MoveToViewAsync(int viewIndex, bool isImmediately = false)
        {
            Debug.Log("切换视角" + viewIndex);

            switch (viewIndex)
            {
                case 0:
                    {
                        targetPos = manager.sceneViewPoint.position;
                        targetEuler = manager.sceneViewPoint.eulerAngles;
                        IsCameraWaggle = true;
                        break;
                    }
                case 1:
                    {
                        targetPos = manager.bookViewPoint.position;
                        targetEuler = manager.bookViewPoint.eulerAngles;
                        IsCameraWaggle = false;

                        break;
                    }
                case 2:
                    {
                        targetPos = manager.holdViewPoint.position;
                        targetEuler = manager.holdViewPoint.eulerAngles;
                        IsCameraWaggle = true;
                        break;
                    }
                case 3:
                    {
                        targetPos = manager.pageViewPoint.position;
                        targetEuler = manager.pageViewPoint.eulerAngles;
                        IsCameraWaggle = false;
                        break;
                    }
            }
            IsOpenWaggle = false;
            if (isImmediately)
            {
                Camera.main.transform.position = targetPos;
                Camera.main.transform.eulerAngles = targetEuler;
            }
            else
            {
                await CustomThread.TimerAsync(1, process =>
                {
                    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPos, process);
                    Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, targetEuler, process);
                });

            }
            IsOpenWaggle = true;
        }
        private void Update()
        {
            if (IsCameraWaggle && IsOpenWaggle)
            {
                Camera.main.transform.position = targetPos + new Vector3(
                    Mathf.PerlinNoise(Time.time * frequent + 0, Time.time * frequent + 0) - 0.5f,
                    Mathf.PerlinNoise(Time.time * frequent + 1, Time.time * frequent + 1) - 0.5f,
                    Mathf.PerlinNoise(Time.time * frequent + 2, Time.time * frequent + 2) - 0.5f
                    ) * strength;
            }
        }
    }
}