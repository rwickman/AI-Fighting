using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    private const int ACKLength = 1;

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
            //Debug.Log("Socket connected to " + client.RemoteEndPoint.ToString());
            //SendState();
            client.EndConnect(ar);
            Debug.Log("CONNECT CALLBACK");
            resetSendFrameCallback();            
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public void SendState(string jsonStr)
    {
        //string fakeStateData = "Eventually this will contain state information";
        //Debug.Log(jsonStr);
        //byte[] byteData = Encoding.ASCII.GetBytes(jsonStr);
        byte[] byteData = new byte[headerLength + Encoding.ASCII.GetByteCount(jsonStr)];
        Encoding.ASCII.GetBytes(jsonStr.Length.ToString()).CopyTo(byteData, 0);
        Encoding.ASCII.GetBytes(jsonStr).CopyTo(byteData, headerLength);

        Debug.Log("BYTE LENGTH: " + byteData.Length);
        client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendStateCallback), null);
    }

    private void SendStateCallback(IAsyncResult ar)
    {
        client.EndSend(ar);
        RecieveACK();
    }

    private void RecieveACK()
    {
        // Create the state object.  
        StateObject state = new StateObject();
        state.buffer = new byte[ACKLength];
        client.BeginReceive(state.buffer, 0, ACKLength, 0, new AsyncCallback(ReceiveACKCallback), state);
    }

    private void ReceiveACKCallback(IAsyncResult ar)
    {
        StateObject state = (StateObject)ar.AsyncState;
        
        client.EndReceive(ar);

        int ACKResponse = int.Parse(Encoding.ASCII.GetString(state.buffer));
        if (ACKResponse == 0)
        {
            resetSendFrameCallback();
            Debug.Log("GOT ACK");
        }
        else
        {
            Debug.LogError("ERROR OCCURRED IN RECEIVING ACK!");
        }
    }

}
