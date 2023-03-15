using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace TouhouMachineLearningSummary.Manager
{
    public class MapLineManager : MonoBehaviour
    {
        public GameObject startPoint;
        public GameObject controlPoint1;
        public GameObject controlPoint2;
        public GameObject controlPoint3;
        public GameObject endPoint;
        LineRenderer lineRenderer => GetComponent<LineRenderer>();

        [Button("更新线条")]
        // Start is called before the first frame update
        async void SetPoint()
        {
            while (!Application.isPlaying)
            {
                List<Vector3> tempPointss = new List<Vector3>()
            {
              Vector3.up*0.01f+ startPoint.transform.position,
              Vector3.up*0.01f+ controlPoint1.transform.position,
              Vector3.up*0.01f+ controlPoint2.transform.position,
              Vector3.up*0.01f+ controlPoint3.transform.position,
              Vector3.up*0.01f+ endPoint.transform.position,
            };

                List<Vector3> targetPoint = Enumerable.Range(0, 20).Select(i => GetCurvePoint(tempPointss, i * 1.0f / 20)).ToList();

                lineRenderer.positionCount = targetPoint.Count;
                lineRenderer.SetPositions(targetPoint.ToArray());

                Vector3 GetCurvePoint(List<Vector3> inputPoints, float process)
                {
                    if (inputPoints.Count > 1)
                    {
                        var inputTempPoints = Enumerable.Range(0, inputPoints.Count - 1)
                              .Select(i => Vector3.Lerp(inputPoints[i], inputPoints[i + 1], process))
                              .ToList();
                        return GetCurvePoint(inputTempPoints, process);
                    }
                    else
                    {
                        return inputPoints.First();
                    }
                }
                await Task.Delay(100);
            }

        }
        [Button("重置线条")]
        void ResetPoint()
        {
            controlPoint1.transform.position = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, 0.25f);
            controlPoint2.transform.position = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, 0.5f);
            controlPoint3.transform.position = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, 0.75f);
        }
    }
}