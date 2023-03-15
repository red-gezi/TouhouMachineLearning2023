using System;
using System.Threading.Tasks;
using UnityEngine;

namespace TouhouMachineLearningSummary.Test
{
    public class TaskTest : MonoBehaviour
    {
        // Start is called before the first frame update
        async void Start()
        {
            await Test();
        }
        async Task Test()
        {
            await Test1();
            async Task Test1()
            {
                Debug.Log("Hello, World!");
                throw new Exception();
            }
        }
    }
}