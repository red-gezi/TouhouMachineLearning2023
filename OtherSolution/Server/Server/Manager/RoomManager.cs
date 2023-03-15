using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    partial class RoomManager
    {
        public static List<Room> Rooms { get; set; } = new List<Room>();
        public static Room? GetRoom(string RoomId) => Rooms.FirstOrDefault(room => room.RoomId == RoomId);
        public static Room? ContainPlayerRoom(string account) => Rooms.FirstOrDefault(room => room.Player1Info.Account == account || room.Player2Info.Account == account);
        public static void CreatRoom(AgainstModeType mode, HoldInfo player1, HoldInfo player2)
        {
            string roomId = Guid.NewGuid().ToString("N");
            Console.WriteLine($"创建房间{roomId}");
            Room TargetRoom = new Room(roomId);
            TargetRoom.Mode = mode; ;
            Rooms.Add(TargetRoom);
            TargetRoom.Creat(player1, player2);

        }
        public static bool DisponseRoom(string roomID, string account, int p1Score, int p2Score)
        {
            Room? TargetRoom = GetRoom(roomID);
            //验证是否合法
            if (TargetRoom != null && (TargetRoom.Player1Info.Account == account || TargetRoom.Player2Info.Account == account))
            {
                Console.WriteLine("销毁房间"+ roomID);
                //房间上传数据
                TargetRoom.Summary.UploadAgentSummary(p1Score, p2Score);
                Rooms.Remove(TargetRoom);
                return true;
            }
            return false;
        }
    }
}
