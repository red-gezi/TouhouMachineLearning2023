
using Microsoft.AspNetCore.SignalR;
using System;

namespace Server
{
    class Room
    {

        public string RoomId { get; set; }
        public AgainstModeType Mode { get; set; }
        public bool IsCanEnter => P2 == null;
        public bool IsEmpty => P1 == null && P2 == null;
        public bool IsContain(string Account) => Account == Player1Info.Account || Account == Player2Info.Account;
        public IClientProxy P1 { get; set; }
        public IClientProxy P2 { get; set; }
        public List<IClientProxy> clientProxies { get; set; }
        public PlayerInfo Player1Info { get; set; }
        public PlayerInfo Player2Info { get; set; }
        public AgainstSummary Summary { get; set; } = new AgainstSummary();

        public Room(string roomId) => RoomId = roomId;
        internal void Creat(HoldInfo player1, HoldInfo player2)
        {
            //此处打乱顺序
            P1 = player1.Client;
            P2 = player2.Client;
            Player1Info = player1.UserInfo;
            Player2Info = player2.UserInfo;
            Console.WriteLine("我开房啦！！！！！！！！！！！！！！///////");

            Player1Info = Player1Info.ShufflePlayerDeck();
            Player2Info = Player2Info.ShufflePlayerDeck();
            //发送房间号，决定先后手，将玩家牌组信息打乱并发送给对方
            Summary._id = Guid.NewGuid().ToString("N");
            Summary.Player1Info = Player1Info;
            Summary.Player2Info = Player2Info;

            bool isP1FirstTurn = false;
            bool isP2FirstTurn = false;

            //当在故事模式或练习模式时，可以自定义先后手，否则随机先后手
            if ((Mode== AgainstModeType.Story|| Mode== AgainstModeType.Practice)&& player1.FirstMode!=0)
            {
                (player1.FirstMode == 1 ? ref isP1FirstTurn : ref isP2FirstTurn) = true;
            }
            else
            {
                (new Random(DateTime.Now.Millisecond).NextDouble() > 0.5f ? ref isP1FirstTurn : ref isP2FirstTurn) = true;
            }
            //如果是多人模式，，或者时先后手为0，随机赋予先后手
            //如果是单人模式，可以操作先后手

            Summary.AssemblyVerision = MongoDbCommand.GetLastCardUpdateVersion();

            P1?.SendAsync("StartAgainst", new object[] { RoomId, true, isP1FirstTurn, Player1Info, Player2Info });
            P2?.SendAsync("StartAgainst", new object[] { RoomId, false, isP2FirstTurn, Player2Info, Player1Info });


        }
        public AgainstSummary ReConnect(IClientProxy player, bool isPlayer1)
        {
            Console.WriteLine($"重新连接房间房间：房客信息{(isPlayer1 ? Player1Info.Name : Player2Info.Name)}\n\n");
            if (isPlayer1)
            {
                P1 = player;
            }
            else
            {
                P2 = player;
            }
            return Summary;
        }
        public void AsyncInfo(NetAcyncType netAcyncType, bool isPlayer1, object[] command)
        {
            Console.WriteLine("同步消息" + netAcyncType.ToString());
            (isPlayer1 ? P2 : P1).SendAsync("Async", netAcyncType, command);
        }
    }
}
