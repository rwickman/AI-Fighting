using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Newtonsoft.Json;

public class FrameCapture : MonoBehaviour
{
    private AgentManager agentManager;
    private Camera cam;
    private bool shouldSendFrame = false;
    private PolicyConnection policy_con;
    private bool hasStarted = false;
    private const int numColorsInPixel = 3;

    private bool sentEpisodeOverFrame = false;


    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        policy_con = GetComponent<PolicyConnection>();
        agentManager = GameObject.Find("GameManager").GetComponent<AgentManager>();
    }

    public void ResetShouldSendFrame()
    {
        shouldSendFrame = true;
    }

    void OnPostRender()
    {
        Debug.Log("Should SEND FRAME: " + shouldSendFrame);
        if(!hasStarted)
        {
            policy_con.StartConnection(ResetShouldSendFrame);
            Debug.Log("CONNECTED");
            hasStarted = true;
        }
        if (shouldSendFrame && !sentEpisodeOverFrame)
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
        var dict = new Dictionary<string, dynamic>();
        if(!agentManager.isEpisodeOver)
        {
            dict["frame"] = pixels;
        }
        else
        {
            sentEpisodeOverFrame = true;
            policy_con.isEpisodeOver = true;
        }
        dict["reward"] = agentManager.GetReward();
        dict["done"] = agentManager.isEpisodeOver;
        agentManager.ResetReward();
        string jsonStr = JsonConvert.SerializeObject(dict);
        policy_con.SendState(jsonStr);      
    }
}
