namespace Server
{
    public class Log
    {
        public static void Init()
        {
            File.WriteAllText("log.txt", "");
            Summary("日志创建日期"+DateTime.Now);
        }
        public static void Summary(string log)
        {
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                Console.WriteLine(log);
                sw.WriteLine(log);
            }
        }

    }
}
