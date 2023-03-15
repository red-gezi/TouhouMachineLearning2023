using System.Net;
using System.Net.Sockets;
using System.Text;
/// <summary>
/// 一个http服务器，用于客户端向服务端请求下载游戏资源ab包
/// </summary>
public class HttpServer
{
    public static void Init()
    {
        Task.Run(() =>
        {
            TcpListener server = new TcpListener(IPAddress.Parse("0.0.0.0"), 7777);
            server.Start();
            Console.WriteLine("启动Http服务器");
            while (true)
            {
                //接受新连接
                Socket socket = server.AcceptSocket();
                Task.Run(() =>
                {
                    Socket currentClient = socket;
                    if (currentClient.Connected)
                    {
                        Console.WriteLine($"{currentClient.RemoteEndPoint}已连接");
                        Byte[] receive = new Byte[1024];
                        currentClient.Receive(receive, receive.Length, 0);
                        //转换成字符串类型
                        string buffer = Encoding.ASCII.GetString(receive);
                        //只处理"get"请求类型
                        if (buffer.Substring(0, 3) != "GET")
                        {
                            Console.WriteLine("只处理get请求类型..");
                            currentClient.Close();
                            return;
                        }
                        // 查找 "HTTP" 的位置
                        int startPoint = buffer.IndexOf("HTTP", 1);
                        string httpVersion = buffer.Substring(startPoint, 8);
                        // 得到请求类型和文件目录文件名
                        string request = buffer.Substring(0, startPoint - 1);
                        request.Replace("\\", "/");

                        //得带请求文件名
                        startPoint = request.IndexOf("/") + 1;
                        string filePath = request.Substring(startPoint);
                        //获取虚拟目录物理路径
                        var targetFile = new FileInfo(Directory.GetCurrentDirectory() + "/" + (filePath == "" ? "index.html" : filePath));
                        Console.WriteLine("请求文件: " + targetFile.FullName);
                        if (targetFile.Exists)
                        {
                            int dataPoint = 0;
                            string sResponse = "";
                            FileStream fs = new FileStream(targetFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                            BinaryReader reader = new BinaryReader(fs);
                            byte[] bytes = new byte[fs.Length];
                            int read;
                            while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                sResponse = sResponse + Encoding.ASCII.GetString(bytes, 0, read);
                                dataPoint = dataPoint + read;
                            }
                            reader.Close();
                            fs.Close();

                            string contentType = targetFile.Extension == ".html" ? "text/HTML" : "application/octet-stream";
                            SendHeader(httpVersion, contentType, dataPoint, " 200 OK", currentClient);
                            SendToBrowser(bytes, currentClient);
                        }
                        else
                        {
                            string message = "404";
                            SendHeader(httpVersion, "text/HTML", message.Length, " 404 Not Found", currentClient);
                            SendToBrowser(Encoding.ASCII.GetBytes(message), currentClient);
                        }
                        currentClient.Close();
                    }
                });
            }
        });
        static void SendToBrowser(Byte[] bSendData, Socket mySocket) => mySocket.Send(bSendData, bSendData.Length, 0);
        static void SendHeader(string sHttpVersion, string contentType, int iTotBytes, string sStatusCode, Socket mySocket)
        {
            string sBuffer = "";
            sBuffer = sBuffer + sHttpVersion + sStatusCode + "\r\n";
            sBuffer = sBuffer + "Server: cx1193719-b\r\n";
            sBuffer = sBuffer + $"Content-Type: {contentType}\r\n";
            sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
            sBuffer = sBuffer + "Content-Length: " + iTotBytes + "\r\n\r\n";
            Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);
            SendToBrowser(bSendData, mySocket);
            Console.WriteLine("Total Bytes : " + iTotBytes.ToString());
        }
    }
}

