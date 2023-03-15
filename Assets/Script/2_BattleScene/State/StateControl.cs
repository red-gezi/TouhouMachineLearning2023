using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Other;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class StateControl : MonoBehaviour
    {
        async void Start() => await CreatAgainstProcess();
        public static async Task CreatAgainstProcess()
        {
            ImageSummary.Init();
            await SceneCommand.InitAsync(false);
            TaskThrowCommand.Init();
            //如果位于跳转模式则直接跳过对局初始化阶段并在之后从对战记录初始化战场状态
            if (!AgainstInfo.IsJumpMode) { await StateCommand.AgainstStart(); }
            while (true)
            {
                //时序图跳转小局
                ImageSummary.AddRound();
                //再联网对战情况下上传回合初始信息
                Manager.AgainstSummaryManager.UploadRound();
                //根据跳转的回合是否是第0回合（小局前置阶段）判断是否执行小局前抽卡操作
                //在非跳转模式或者跳转目标为第0回合（小局前置阶段）时，会进入小局开始等待换牌阶段，否则直接略过
                if (!AgainstInfo.IsJumpMode || StateCommand.AgainstStateInit())
                {
                    await StateCommand.RoundStart();
                    Debug.LogWarning("开始换牌");
                    await StateCommand.WaitForPlayerExchange();
                    Debug.LogWarning("结束换牌");
                    //await StateCommand.WaitForSelectProperty();
                }
                while (true)
                {
                    //时序图跳转回合
                    ImageSummary.AddTurn();
                    await StateCommand.TurnStart();
                    Manager.AgainstSummaryManager.UploadStartPoint();
                    await StateCommand.WaitForPlayerOperation();
                    Manager.AgainstSummaryManager.UploadEndPoint();
                    if (AgainstInfo.isBoothPass) { break; }
                    await StateCommand.TurnEnd();
                    //时序图结束
                    ImageSummary.EndTurn();
                    //时序图改变操作方
                    ImageSummary.ChangePlayer();
                }
                await StateCommand.RoundEnd();
                if (AgainstInfo.PlayerScore.P1Score == 2 || AgainstInfo.PlayerScore.P2Score == 2) { break; }
                AgainstInfo.roundRank++;
            }
            
            await StateCommand.AgainstEnd();
        }
    }
}