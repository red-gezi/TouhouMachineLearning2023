using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    //梦想封印
    partial class DanmuTrackManager : MonoBehaviour
    {
        public float maxDinsance = 2;
        public float speed = 1.5f;
        Vector3 startPosition, endPosition;
        public async Task Execute(Event e, DanmuTrack track)
        {
            this.startPosition = e.TriggerCard.transform.position;
            this.endPosition = e.TargetCard.transform.position;
            Vector3 tempPos = Vector3.zero;
            switch (track)
            {
                case DanmuTrack.Round://环绕并直行
                    {
                        await CustomThread.TimerAsync(Mathf.PI * 6f / 4, (timer) =>
                        {
                            float x = timer * Mathf.PI * speed;
                            transform.position = startPosition + (Mathf.Min(timer * 0.5f, maxDinsance)) * new Vector3(Mathf.Cos(x / 2), 0, Mathf.Sin(x / 2)) + new Vector3(0, 3, 0);
                            transform.forward = Vector3.Cross(Vector3.up, (new Vector3(Mathf.Cos(x / 2 + 0.5f), 0, Mathf.Sin(x / 2 + 0.5f))));
                            transform.localScale = Mathf.Min(timer * 0.2f, 0.5f) * Vector3.one;
                        });
                        tempPos = transform.position;
                        await CustomThread.TimerAsync(1f, (timer) =>
                        {
                            transform.position = Vector3.Lerp(tempPos, endPosition, timer);
                            transform.forward = endPosition - tempPos;
                        });
                        Destroy(gameObject);
                        _ = CameraManager.manager.VibrationCameraAsync();
                        await Task.Delay(11000 * (int)(Mathf.PI * 6f / 4 + 0.5f));
                    }
                    break;
                case DanmuTrack.Line://直射
                    {
                        await CustomThread.TimerAsync(0.5f, (process) =>
                        {
                            transform.position = startPosition + Vector3.up * process;
                            transform.localScale = process * Vector3.one;
                        });
                        tempPos = transform.position;
                        await CustomThread.TimerAsync(0.5f, (process) =>
                        {
                            transform.position = Vector3.Lerp(tempPos, e.TargetCard.transform.position, process);
                        });
                        Destroy(gameObject);
                        _ = CameraManager.manager.VibrationCameraAsync();
                    }
                    break;

                case DanmuTrack.Parabola://抛射
                    await CustomThread.TimerAsync(0.5f, (process) =>
                    {
                        transform.position = startPosition + Vector3.up * process;
                        transform.localScale = process * Vector3.one;
                    });
                    tempPos = transform.position;
                    _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.Laser);
                    Vector3 middlePoint = (tempPos + e.TargetCard.transform.position) / 2 + Vector3.up * 5;

                    await CustomThread.TimerAsync(0.5f, (process) =>
                        {
                            Vector3 tempPonit1= Vector3.Lerp(tempPos, middlePoint, process);
                            Vector3 tempPonit2= Vector3.Lerp(middlePoint, e.TargetCard.transform.position, process);
                            transform.position = Vector3.Lerp(tempPonit1, tempPonit2, process);
                        });
                    Destroy(gameObject);
                    _ = CameraManager.manager.VibrationCameraAsync();
                    break;
                case DanmuTrack.FixedOnTriggerCard://固定位置
                    transform.position = e.TargetCard.transform.position;
                    Destroy(gameObject, 3);
                    break;
                case DanmuTrack.Down://从下而上
                    break;
                case DanmuTrack.Test:
                    await CustomThread.TimerAsync(0.5f, (timer) =>
                    {
                        transform.position = startPosition + Vector3.up * timer;
                        transform.localScale = timer * Vector3.one;
                    });
                    tempPos = transform.position;
                    _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.Laser);
                    await CustomThread.TimerAsync(1f, (timer) =>
                    {
                        transform.position = Vector3.Lerp(tempPos, endPosition, timer * 2);
                    });
                    Destroy(gameObject);
                    _ = CameraManager.manager.VibrationCameraAsync();
                    break;
                default:
                    break;
            }
        }
    }
}