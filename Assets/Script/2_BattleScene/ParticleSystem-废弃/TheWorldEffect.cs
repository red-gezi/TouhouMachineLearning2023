using UnityEngine;

public class TheWorldEffect : MonoBehaviour
{
    public static int state = 0;
    public static float num;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (state == 0)
        {

        }
        switch (state)
        {
            case (0):
                num = Mathf.Lerp(num, 0, Time.deltaTime * 8);
                break;
            case (1):
                num = Mathf.Lerp(num, 2, Time.deltaTime * 0.8f);
                break;
            default:
                break;
        }
        //transform.GetComponent<Renderer>().material.SetFloat("_theWorld", num);
        transform.localScale = new Vector3(num, 0, num);
    }
    //[Button]
    //public void Play()
    //{
    //    Task.Run(async () =>
    //    {
    //        state = 0;
    //        await Task.Delay(0000);
    //        state = 1;
    //        await Task.Delay(2000);
    //        state = 0;
    //        await Task.Delay(1000);
    //        //state = 2;
    //        //await Task.Delay(3000);
    //        //state = 1;
    //        //await Task.Delay(1000);
    //        //state = 0;
    //        //await Task.Delay(1000);
    //    });
    //}

}
