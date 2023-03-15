using Sirenix.OdinInspector;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager manager;
        public Vector3 DefaultPos;
        public Vector3 DefaultView;
        float OffsetX = 0;
        float OffsetY = 0;
        void Start()
        {
            manager = this;
            DefaultPos = Camera.main.transform.position;
            DefaultView = Camera.main.transform.eulerAngles;
        }

        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                OffsetX += Input.GetAxis("Mouse X");
                OffsetY -= Input.GetAxis("Mouse Y");
                Vector3 OffsetVector = new Vector3(OffsetY, OffsetX, 0);
                Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, DefaultView + OffsetVector, Time.deltaTime * 2);
            }
            else
            {
                OffsetX = 0;
                OffsetY = 0;
                Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, DefaultView, Time.deltaTime * 2);
            }
        }
        /// <summary>
        /// 震动相机
        /// </summary>
        public float time = 0.1f;
        public float fre = 0.5f;
        public float A = 0.05f;
        [Button]
        public async Task VibrationCameraAsync()
        {
            await CustomThread.TimerAsync(time, process =>
             {
                 Camera.main.transform.position = DefaultPos + A * new Vector3(1, 1, 0) * Mathf.Sin(process * fre) / (process * 0.5f + 0.001f);
             });
            Camera.main.transform.position = DefaultPos;
        }
    }
}