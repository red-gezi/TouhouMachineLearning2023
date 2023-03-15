using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics;

public class VersionsHub : Hub
{
    static Process progress = null;
    public static void Init()
    {
        Console.WriteLine("启动版本控制器");
        progress = Process.Start("dotnet", "Server/Server.dll");
    }
    public void StartServer() => progress = Process.Start("dotnet", "Server/Server.dll");
    public void CloseServer() => progress.Kill();

    public bool UpdateServer(byte[] datas)
    {
        try
        {
            Console.WriteLine("收到更新信息");
            CloseServer();
            File.WriteAllBytes("Server/Server.dll", datas);
            Console.WriteLine("进行版本更新,当前服务器更新时间为" + new FileInfo("Server/Server.dll").LastWriteTime);
            StartServer();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}