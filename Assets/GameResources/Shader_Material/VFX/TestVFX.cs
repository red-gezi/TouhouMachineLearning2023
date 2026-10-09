using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TestVFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<VisualEffect>().SetVector4("Color",(Vector4) Color.red * 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
