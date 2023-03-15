using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
public class UIScaler : MonoBehaviour
{
    [ShowInInspector]
    public Texture2D texture;
    float lastK;
    public float width_k;
    public float height_k;
    public float side;
    [Button]
    private void CreatTexture()
    {
        //ºÜ¿¨
        texture = new Texture2D(256, 256);
        int size = 256;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //rÊÇ0~1
                float r = i * 1f / size;
                if (r < side)
                {
                    r = r * width_k;
                }
                else if (r > 1 - side)
                {
                    r = r * width_k + (1 - width_k);
                }
                float g = j * 1f / size;
                if (g < side)
                {
                    g = g * height_k;
                }
                else if (g > 1 - side)
                {
                    g = g * height_k + (1 - height_k);
                }
                texture.SetPixel(i, j, new Color(r, g, 0));

            }
        }
        texture.Apply();
        if (GetComponent<Renderer>() == null)
        {
            GetComponent<Image>().material.SetTexture("_UV", texture);

        }
        else
        {
            GetComponent<Renderer>().sharedMaterial.SetTexture("_UV", texture);

        }
    }
    void Update() => CreatTexture();
}
