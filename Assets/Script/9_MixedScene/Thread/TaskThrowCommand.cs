using System.Threading;

namespace TouhouMachineLearningSummary.Command
{
    class TaskThrowCommand
    {
        public static CancellationTokenSource Token { get; set; }
        public static void Init() => Token = new CancellationTokenSource();
        public static void TriggerThrow() => Token.Cancel();
        /// <summary>
        /// 当在播放器模式下停止播放时会在游戏所有循环线程抛出异常防止出现无法中断的子线程
        /// </summary>
        public static void Throw() => Token.Token.ThrowIfCancellationRequested();
    }
}
