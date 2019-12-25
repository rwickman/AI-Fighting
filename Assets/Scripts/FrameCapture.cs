using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class FrameCapture : MonoBehaviour
{
    private Camera cam;
    private bool shouldSendFrame = false;
    private PolicyConnection policy_con;

    void Awake()
    {  
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        policy_con = GetComponent<PolicyConnection>();
        policy_con.StartConnection(ResetShouldSendFrame);
    }

    public void ResetShouldSendFrame()
    {
        shouldSendFrame = true;
    }

    void OnPostRender()
    {
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
        string jsonStr = JsonSerializer.SerializeFrame(framePixels);
        Debug.Log(jsonStr);
        policy_con.SendState(jsonStr);
    }
}
