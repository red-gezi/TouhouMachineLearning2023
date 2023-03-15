using System;


internal class Log
{
    static bool isLog = false;
    public static void Show(string tag)
    {
        if (isLog)
        {
            UnityEngine.Debug.Log(tag + ":" + DateTime.Now);
        }
    }
}

