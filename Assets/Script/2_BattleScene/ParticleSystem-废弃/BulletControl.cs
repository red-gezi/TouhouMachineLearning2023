using Sirenix.OdinInspector;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{

    class BulletControl : MonoBehaviour
    {
        public Vector3 startPos;
        public Vector3 middlePos;
        public Vector3 endPos;
        public float speed = 0.8f;
        [Range(0, 1)]
        public float progress;
        private void Update()
        {
            Vector3 middle1 = Vector3.Lerp(startPos, middlePos, progress);
            Vector3 middle2 = Vector3.Lerp(middlePos, endPos, progress);
            transform.position = Vector3.Lerp(middle1, middle2, progress);
            progress += Time.deltaTime * speed;
            progress = Mathf.Clamp(progress, 0, 1);
            if (progress == 1)
            {
                Destroy(gameObject, 2);
            }
        }
        [Button("初始化")]
        public void Init(Card a, Card b)
        {
            startPos = a.transform.position - Vector3.up;
            endPos = b.transform.position - Vector3.up;
            middlePos = (endPos + startPos) / 2 + Vector3.up * 5;
        }
        [Button("播放")]
        public void Play() => progress = 0;
    }

}
