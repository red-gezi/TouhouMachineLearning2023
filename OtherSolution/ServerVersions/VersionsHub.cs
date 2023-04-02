using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics;
using Server;
public class VersionsHub : Hub
{
    static Process progress = null;
    public static void Init()
    {
        Console.WriteLine("启动版本控制器");
        Console.WriteLine("更新日期23.4.2_0");
        progress = Process.Start("dotnet", "Server/Server.dll");
    }
    public void StartServer() => progress = Process.Start("dotnet", "Server/Server.dll");
    public void CloseServer() => progress.Kill();

    public string UpdateServer(byte[] datas,string commandPassword)
    {
         //对比服务器的更新密码
        if (commandPassword.GetSaltHash("514") == "DiEv6gZWigvcJ64kqymfGyrCHFhdSso8RPuUIsEzJZI=")
        {
            try
            {
                Console.WriteLine("收到更新信息");
                CloseServer();
                File.WriteAllBytes("Server/Server.dll", datas);
                Console.WriteLine("进行版本更新,当前服务器更新时间为" + new FileInfo("Server/Server.dll").LastWriteTime);
                StartServer();
                return "更新成功";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }
        else
        {
            return "指令密码输入错误，服务器拒绝修改";
        }
    }
}