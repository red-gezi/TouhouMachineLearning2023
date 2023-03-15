using System.Linq.Expressions;
namespace Server
{
    class PlanManager
    {
        //设定指定时间间隔触发
        public static void Creat(Action action, int day, int hour, int minutes)
        {
            Timer timer = new Timer(new TimerCallback(o => action()));
            //timer.Change(DateTime.Today.AddDays(0).AddHours(0).AddMinutes(0), new TimeSpan(0, 0, 0, 0))
        }
        //延迟指定时间后执行
        public static void Creat(Action action, int delayMinutes,int spwnSecond=0)
        {
            Timer timer = new Timer(new TimerCallback(o => action()));
            timer.Change(delayMinutes * 1000 * 5, spwnSecond*1000);
        }

    
    }
}