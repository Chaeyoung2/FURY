using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using System.Net;
using System.Net.Sockets;

// http://unitynetworkwithcsharp.blogspot.com/2017/11/socket.html

public class NetworkService : MonoBehaviour
{
    Socket socket;

    string ipAddress = "192.0.0.1";
    int port = 9000;

    byte[] sendByte;

    // Use This for Initialization
    void Start()
    {
        // Create Socket
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);

        // Connect
        try
        {
            IPAddress ipAddr = IPAddress.Parse(ipAddress);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
            socket.Connect(ipEndPoint);
        }
        catch(SocketException se)
        {
            Debug.Log("Socket Connect Error ! : " + se.ToString());
            return;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Test 1 - send data!");

                int I = Encoding.Default.GetByteCount(sb.ToString());
                byte[] d = Encoding.Default.GetBytes(sb.ToString());
                socket.Send(d, I, 0);

            }
            catch(System.Exception e)
            {
                Debug.Log("Socket send or receive error! : " + e.ToString());
            }
            socket.Disconnect(true);
            socket.Close();
        }
    }
}