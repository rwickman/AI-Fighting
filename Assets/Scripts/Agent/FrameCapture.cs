using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Newtonsoft.Json;

public class FrameCapture : MonoBehaviour
{
    private Camera cam;
    private bool shouldSendFrame = false;
    private PolicyConnection policy_con;
    private bool hasStarted = false;
    private const int numColorsInPixel = 3;

    void Awake()
    {  
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        policy_con = GetComponent<PolicyConnection>();
       
    }

    public void ResetShouldSendFrame()
    {
        shouldSendFrame = true;
    }

    void OnPostRender()
    {
        if(!hasStarted)
        {
            policy_con.StartConnection(ResetShouldSendFrame);
            hasStarted = true;
        }
        if (shouldSendFrame)
        {
            shouldSendFrame = false;
            RenderTexture renderTexture = cam.activeTexture;
            Texture2D tex2d = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

            RenderTexture.active = renderTexture;
            tex2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex2d.Apply();
            Color32[] framePixels = tex2d.GetPixels32();
            Thread createFrameThread = new Thread(() => CreateFrame(framePixels) );
            createFrameThread.Start();
        }
    }

    void CreateFrame(Color32[] framePixels)
    {
        int[,] pixels = new int[framePixels.Length, numColorsInPixel];
        for (int i = 0; i < framePixels.Length; i++)
        {
            Color32 pixelColor = framePixels[i];
            pixels[i, 0] = pixelColor.r;
            pixels[i, 1] = pixelColor.g;
            pixels[i, 2] = pixelColor.b;

        }

        string jsonStr = JsonConvert.SerializeObject(pixels);
        policy_con.SendState(jsonStr);
    }
}
