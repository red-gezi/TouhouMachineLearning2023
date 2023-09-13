using Microsoft.AspNetCore.SignalR;
using TouhouMachineLearningSummary.GameEnum;

namespace Server
{
    class HoldListManager
    {
        static Dictionary<AgainstModeType, List<HoldInfo>> HoldLists { get; set; } = new Dictionary<AgainstModeType, List<HoldInfo>>();

        static List<PlayerInfo> playerInfos = new List<PlayerInfo>();
        public static void Init()
        {

            foreach (AgainstModeType againstMode in Enum.GetValues(typeof(AgainstModeType)))
            {
                HoldLists[againstMode] = new List<HoldInfo>();
            }
        }
        public static void Add(AgainstModeType againstMode,int FirstMode, PlayerInfo playerInfo, PlayerInfo virtualOpponentInfo,IClientProxy caller)
        {
            Console.WriteLine(playerInfo.UID + "加入" + againstMode.ToString());
            HoldLists[againstMode].Add(new HoldInfo(FirstMode,playerInfo, virtualOpponentInfo,caller));
        }
        public static bool Remove(AgainstModeType againstMode, string uid)
        {
            HoldInfo? target = HoldLists[againstMode].FirstOrDefault(x => x.UserInfo.UID == uid);
            if (target == null)
            {
                return false;
            }
            else
            {
                HoldLists[againstMode].Remove(target);
                return true;
            }
        }
        
        public static void Show()
        {
            Console.WriteLine("############################");
            Console.WriteLine("当前用户列表");
            foreach (var holdList in HoldLists)
            {
                holdList.Value.ForEach(info => Console.WriteLine(info.UserInfo.Account + "-等级" + info.Rank + "-胜率" + info.WinRate));
            }

            Console.WriteLine("############################");
        }
        static void AddUser()
        {
            Random random = new Random();
            Console.WriteLine("新增20名用户");
            for (int i = 0; i < 20; i++)
            {
                var insertUser = new PlayerInfo().Creat($"用户{i}", "", "", new List<Deck>() { new Deck() }, null);
                insertUser.Rank = random.Next(0, 22);
                insertUser.WinRate = random.NextSingle() * 50 + 25;
                //HoldListManager.Add(AgainstModeType.Casual, insertUser);
            }
        }
        public static int Match()
        {
            int count = 0;
            for (int i = 0; i < HoldLists.Count; i++)
            {
                var mode = HoldLists.Keys.ToList()[i];
                var targetHoldList = HoldLists.Values.ToList()[i];
                if (mode == AgainstModeType.Story || mode == AgainstModeType.Practice)
                {
                    //单人类型，构造一个虚拟对手，并一起加入房间，随后从等待列表移除
                    targetHoldList.ForEach(info => RoomManager.CreatRoom(mode,info, new HoldInfo(0,info.VirtualOpponentInfo)));
                    count += targetHoldList.Count();
                    targetHoldList.Clear();
                }
                else if (mode == AgainstModeType.Casual || mode == AgainstModeType.Rank || mode == AgainstModeType.Arena)
                {
                    //多人类型，从排队最久的用户开始检索，寻找一定范围内等级+胜率差最低的单位 成对创建房间并移除
                    List<HoldInfo> successMatchList = new List<HoldInfo>();
                    //循环匹配列表
                    for (int j = 0; j < targetHoldList.Count; j++)
                    {
                        var currentHoldInfo = targetHoldList[j];
                        //如果当前目标还没有成功匹配
                        if (!successMatchList.Contains(currentHoldInfo))
                        {
                            var tempMathchList = targetHoldList
                                 .Skip(j + 1)//直接跳到当前匹配目标之后开始
                                 .Except(successMatchList)//排除成功匹配到的目标
                                 .Take(50);//先暂定十个，会根据匹配时长等级上升
                            var result = tempMathchList
                                .Select(tempHoldInfo => new { score = MatchRule(tempHoldInfo, currentHoldInfo), holdInfo = tempHoldInfo })
                                .OrderBy(x => x.score)
                                .FirstOrDefault();
                            //先暂定为5，会根据匹配时长等级上升
                            if (result != null && result.score < 1)
                            {
                                successMatchList.Add(currentHoldInfo);
                                successMatchList.Add(result.holdInfo);
                                RoomManager.CreatRoom(mode,currentHoldInfo, result.holdInfo);
                                Console.WriteLine($"成功配对目标{ currentHoldInfo.UserInfo.Account}-等级:{currentHoldInfo.Rank}-胜率：{(int)currentHoldInfo.WinRate}%" +
                                    $" ---- { result.holdInfo.UserInfo.Account}-等级:{result.holdInfo.Rank}-胜率：{(int)result.holdInfo.WinRate}%");
                            }
                        }
                    }
                    //移除已成功匹配的目标
                    successMatchList.ForEach(targetHoldInfo => targetHoldList.Remove(targetHoldInfo));
                    count += successMatchList.Count;
                }
                //Console.Write("-");
                //Console.WriteLine("当前等待列表" + targetHoldList.Count);
                //Console.WriteLine("等待最久的时间为" + (targetHoldList.FirstOrDefault()?.JoinTime - DateTime.Now));
            }
            return count;
            //匹配规则
            static float MatchRule(HoldInfo target, HoldInfo other)
            {
                var value1 = target.Rank - other.Rank;
                var value2 = target.WinRate - other.WinRate;
                return Math.Abs(value1 * 2 + value2 / 10);
            }
        }

    }
}