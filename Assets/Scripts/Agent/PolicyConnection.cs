using System;
using System.Net;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;

public class PolicyConnection : MonoBehaviour
{

    // State object for receiving data from remote device.  
    public class StateObject
    {
        // Size of receive buffer.  
        public const int MaxBufferSize = 512;
        // Receive buffer.  
        public byte[] buffer;
    }

    public string unixSocket = "/home/ryan/code/Unity/AI Fighting/Assets/Scripts/python/ai_controller";
    [HideInInspector]
    public delegate void ResetShouldSendFrame();

    private ResetShouldSendFrame resetSendFrameCallback;

    private Socket client;
    private const int headerLength = 8;
    //private const int ACKLength = 1;
    private Agent agent;
    private AgentManager agentManager;
    public bool isEpisodeOver = false;

    void Awake()
    {
        agent = GetComponentInParent<Agent>();
        agentManager = GameObject.Find("GameManager").GetComponent<AgentManager>();
    }

    public void StartConnection(ResetShouldSendFrame resetSendFrameCallback)
    {
        this.resetSendFrameCallback = resetSendFrameCallback;
        client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
        var unixEP = new UnixEndPoint(unixSocket);
        // client.Connect(unixEp);
        client.BeginConnect(unixEP,
            new AsyncCallback(ConnectCallback), null);
    }


    private void ConnectCallback(System.IAsyncResult ar)
    {
        try
        {
            client.EndConnect(ar);
            resetSendFrameCallback();            
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public void SendState(string jsonStr)
    {
        byte[] byteData = new byte[headerLength + Encoding.ASCII.GetByteCount(jsonStr)];
        Encoding.ASCII.GetBytes(jsonStr.Length.ToString()).CopyTo(byteData, 0);
        Encoding.ASCII.GetBytes(jsonStr).CopyTo(byteData, headerLength);

        client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendStateCallback), null);
    }

    private void SendStateCallback(IAsyncResult ar)
    {
        client.EndSend(ar);
        if(isEpisodeOver)
        {
            ReceiveEnd();
        }
        else
        {
            ReceiveAction();
        }
        
    }

    private void ReceiveAction()
    {
        // Create the state object.  
        StateObject state = new StateObject();
        state.buffer = new byte[headerLength];
        client.BeginReceive(state.buffer, 0, headerLength, 0,
                            new AsyncCallback(ReceiveActionBody), state);
    }

    private void ReceiveActionBody(IAsyncResult ar)
    {
        StateObject state = (StateObject)ar.AsyncState;
        client.EndReceive(ar);

        int packetLength = int.Parse(Encoding.ASCII.GetString(state.buffer));
        state.buffer = new byte[packetLength];
        client.BeginReceive(state.buffer, 0, packetLength, 0, 
                            new AsyncCallback(ReceiveActionBodyCallback), state);
    }
    
    private void ReceiveActionBodyCallback(IAsyncResult ar)
    {
        client.EndReceive(ar);
        
        StateObject state = (StateObject)ar.AsyncState;
        string actionJson = Encoding.ASCII.GetString(state.buffer);
        agent.SetAction(JsonConvert.DeserializeObject<Dictionary<string, float>>(actionJson));
        resetSendFrameCallback();
    }
    
    private void ReceiveEnd()
    {
        Debug.Log("RECEIVE END");
        // Create the state object.  
        StateObject state = new StateObject();
        state.buffer = new byte[1];
        client.BeginReceive(state.buffer, 0, 1, 0,
                            new AsyncCallback(ReceiveEndCallback), state);
    }

    private void ReceiveEndCallback(IAsyncResult ar)
    {
        Debug.Log("ReceiveEndCallback");
        client.EndReceive(ar);
        agentManager.shouldRestart = true;
    }
}
