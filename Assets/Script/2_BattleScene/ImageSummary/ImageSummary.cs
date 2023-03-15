using System.Collections.Generic;
using System.IO;


namespace TouhouMachineLearningSummary.Other
{
    internal class ImageSummary
    {
        static Stack<string> nodes = new Stack<string>();
        static string filePath = "ImageSummary.txt";
        static int player = 0;
        static int round = 1;
        static int turn = 1;
        static string MainNode => $"第{round}局_第{turn}回合_玩家{player}";
        public static void Init()
        {
            File.WriteAllText(filePath, "");
            nodes = new();
            nodes.Push("对局开始");
            player = 0;
            round = 0;
            turn = 0;
        }
        public static void ChangePlayer() => player = 1 - player;
        public static void AddRound() => round++;
        public static void AddTurn()
        {
            turn++;
            nodes.Push(MainNode);
        }
        public static void AddNode(Event e)
        {
            string trigger = e.TriggerCard == null ? "系统" : e.TriggerCard.TranslateName;
            string targetCard = e.TargetCard == null ? "无" : e.TargetCard.TranslateName;
            string newNode = $"{e.TriggerTime}_{trigger}_{e.TriggerType}";
            string info = $"{nodes.Peek()}->{newNode}:触发{targetCard}_{e.TriggerType}效果";
            if (e.TargetState != GameEnum.CardState.None) info += $"_目标状态{e.TargetState}";
            if (e.TargetFiled != GameEnum.CardField.None) info += $"_目标字段{e.TargetFiled}值{e.point}";
            File.AppendAllLines(filePath, new List<string>() { info });
            nodes.Push(newNode);
        }
        public static void RebackNode()
        {
            string newNode = nodes.Pop();
            string LastNode = nodes.Peek();
            File.AppendAllLines(filePath, new List<string>() { $"{newNode}->{LastNode}:效果执行完毕" });
        }
        public static void EndTurn()
        {
            string newNode = nodes.Pop();
            File.AppendAllLines(filePath, new List<string>() { $"{newNode}->{newNode}:结束回合", "######" });
        }
    }
}
