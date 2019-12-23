using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCapture : MonoBehaviour
{

    // Only get a frame every 4 frames
    public int skipCaptureRate = 4;
    private Camera cam;
    private int lastCapture = 0;
    
    
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void OnPostRender()
    {
        if (lastCapture == skipCaptureRate)
        {
            lastCapture = 1;
            RenderTexture renderTexture = cam.activeTexture;

            // assumes you have your RenderTexture renderTexture
            Texture2D tex2d = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

            RenderTexture.active = renderTexture;
            tex2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex2d.Apply();
            Color32[] framePixels = tex2d.GetPixels32();
            Frame curFrame = new Frame();
            Pixel[] pixels = new Pixel[framePixels.Length];
            for (int i = 0; i < framePixels.Length; i++)
            {
                Color32 pixelColor = framePixels[i];
                Pixel pixel = new Pixel();
                pixel.rgb = new int[] { pixelColor.r, pixelColor.g, pixelColor.b };
                pixels[i] = pixel;

            }
            Frame frame = new Frame();
            frame.pixels = pixels;
            //string jsonStr = JsonUtility.ToJson(frame);
            //Debug.Log(jsonStr);
            //
        }
        else
        {
            lastCapture++;
        }
    }
}
