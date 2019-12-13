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
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int MaxBufferSize = 512;
        public int gameType;
        // Receive buffer.  
        public byte[] buffer;
    }

    public string unixSocket = "/home/ryan/code/Unity/ai_controller";

    private Socket client;

    // Start is called before the first frame update
    void Start()
    {
        StartConnection();
    }

    private void StartConnection()
    {
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
            SendState();
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void SendState()
    {
        string fakeStateData = "Eventually this will contain state information";
        byte[] byteData = Encoding.ASCII.GetBytes(fakeStateData);
        client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendStateCallback), null);
    }

    void SendStateCallback(System.IAsyncResult ar)
    {
    }

}
