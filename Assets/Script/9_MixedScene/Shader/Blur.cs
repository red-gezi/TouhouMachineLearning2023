using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Blur : MonoBehaviour
{
    public Material material;
    public RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, 1);

    // Start is called before the first frame update
    void Start()
    {
        ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);
        GetComponent<Image>().material.SetTexture("_MainTex", renderTexture);
    }
    [Button("НиЭМ")]
    public void Test()
    {
        ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);
        GetComponent<Image>().material.SetTexture("_MainTex", renderTexture);
    }
}
