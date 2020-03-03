using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;

// Socket 클라이언트
// http://unitynetworkwithcsharp.blogspot.com/2017/11/socket.html
// 가장 Low 레벨의 클래스

public class Networking : MonoBehaviour
{
    Socket socket;
    string ipAddress = "127.0.0.1";
    int port = 9000;
    byte[] sendByte;
    private Thread thread;
    byte[] receiverBuff = new byte[128];

    public Text LoginText;
    public GameObject player;

    // Use This for Initialization
    void Start()
    {
        InitializeNetwork();
        CreateThread();
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
            catch (System.Exception e)
            {
                Debug.Log("Socket send or receive error! : " + e.ToString());
            }
            socket.Disconnect(true);
            socket.Close();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {

            SendPacket_PlayerMove();
        }

        // socket 클라이언트도 receive 무한정 대기를 하는군,, 이러면 안대
        //byte[] receiveBuff = new byte[4000];
        //int n = socket.Receive(receiveBuff);
    }

    //////////////////////////////


    void InitializeNetwork()
    {
        // Create Socket
        /// IP 사용, 스트림 소켓 사용, TCP 프로토콜 사용
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Connect
        try
        {
            // IPAddress 클래스
            /// IP 주소
            /// IP 주소 문자열로부터 파싱하여 IPAddress 객체를 만든다.
            IPAddress ipAddr = IPAddress.Parse(ipAddress);

            // IPEndPoint 클래스
            /// EndPoint
            /// TCP나 UDP는 IP 주소와 함께 포트번호를 사용함
            // 서버에 연결
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
            // 소켓 객체의 Connect() 메서드를 호출하여 서버 종단점에 연결
            socket.Connect(ipEndPoint);
        }
        catch (SocketException se)
        {
            Debug.Log("Socket Connect Error ! : " + se.ToString());
            return;
        }
    }
    void CreateThread()
    {
        thread = new Thread(ReceivePacket);
        thread.Start();
    }

    void SendPacket_PlayerMove()
    {
        /////////////////////////// 1차 시도
        //Vector3 playerPos = player.transform.position;

        ////  byte[] pos_array = InfoSender.SerializeObject(playerPos);


        //byte[] buff = new byte[12];

        //Buffer.BlockCopy(BitConverter.GetBytes(playerPos.x), 0, buff, 0, 4);
        //Buffer.BlockCopy(BitConverter.GetBytes(playerPos.y), 0, buff, 4, 4);
        //Buffer.BlockCopy(BitConverter.GetBytes(playerPos.z), 0, buff, 8, 4);

        //// MemoryCopy(buff, &playerPos, 128, sizeof(playerPos));
        ////Buffer.BlockCopy(buff, 128, &playerPos, sizeof(playerPos), sizeof(playerPos));
        //// System.arraycopy(buff, 128, &playerPos, 0, 7);
        //// System.Array.Copy(buff, pos_array, 3);

        //socket.Send(buff, SocketFlags.None);

        //// byte to string
        //string str = Encoding.Default.GetString(buff);
        //Debug.Log(str);


        //////////////////////////////////////////////////////// 2차 시도
        //StringBuilder sb = new StringBuilder();

        //sb.Append("Test 1 - send data!");

        //int I = Encoding.Default.GetByteCount(sb.ToString());
        //byte[] d = Encoding.Default.GetBytes(sb.ToString());
        //socket.Send(d, I, 0);


        ///////////////////////////////////////////////////////// 3차 시도
        //Vector3 playerPos = player.transform.position;
        //byte[] buffer = new byte[12];
        //Buffer.BlockCopy(BitConverter.GetBytes(playerPos.x), 0, buffer, 0, 4);
        //Buffer.BlockCopy(BitConverter.GetBytes(playerPos.y), 0, buffer, 4, 4);
        //Buffer.BlockCopy(BitConverter.GetBytes(playerPos.z), 0, buffer, 8, 4);
        //socket.Send(buffer, 12, 0);

        ////////////////////////////////////////////////////// 4차 시도
        //Vector3 playerPos = player.transform.position;
        //byte[] x = System.BitConverter.GetBytes(playerPos.x);
        //socket.Send(x, 4, 0);

        ///////////////////////////////////////////////////// 5차 시도 : 0만 받음 --> 됨!! 근데 4바이트만 받는듯하다
        Vector3 playerPos = player.transform.position;
        StringBuilder sb = new StringBuilder();
        sb.Append("0" + " " + playerPos.x.ToString() + " " + playerPos.y.ToString() + " " + playerPos.z.ToString());
        int I = Encoding.Default.GetByteCount(sb.ToString());
        byte[] d = Encoding.Default.GetBytes(sb.ToString());
        socket.Send(d, I, 0);

        ///////////////////////////////////////////////////// 6차 시도 : 아예 잘못된 문법
        //Vector3 playerPos = player.transform.position;
        //byte[] buffer = new byte[12];
        //socket.Send((byte[])&playerPos.x, 12, 0);

        //////////////////////////////////////////////////// 7차 시도 !!!!!!!!!!!!!!
        //Vector3 playerPos = player.transform.position;
        //byte[] x = new byte[4];
        //Buffer.BlockCopy(BitConverter.GetBytes(playerPos.x), 0, x, 0, 4);
        //socket.Send(x, 4, 0);

        //////////////////////////////////////////////////// 8차 시도
        //Vector3 playerPos = player.transform.position;
        //byte[] buff = new byte[sizeof(float)*3];
        //Buffer.BlockCopy(BitConverter.GetBytes(playerPos.x), 0, buff, 0 * sizeof(float), sizeof(float));
        //Buffer.BlockCopy(BitConverter.GetBytes(playerPos.y), 0, buff, 0 * sizeof(float), sizeof(float));
        //Buffer.BlockCopy(BitConverter.GetBytes(playerPos.z), 0, buff, 0 * sizeof(float), sizeof(float));
        //socket.Send(buff, 12, 0);

        //Debug.Log("PlayerPos.x : " + playerPos.x + ",  PlayerPos.y : " + playerPos.y + ", PlayerPos.z : " + playerPos.z);
        //Debug.Log("Buff : " + BitConverter.ToString(buff));
    }
    void ReceivePacket()
    {
        while (true)
        {
            int n = socket.Receive(receiverBuff);
            string data = Encoding.UTF8.GetString(receiverBuff, 0, n);
            Debug.Log(data);

            if (receiverBuff[0] == '0') // 0번째 패킷을 받았다 = 서버로부터 login_ok 패킷을 받았다
            {
                int id = receiverBuff[2] - '0';
                Debug.Log(id);
                LoginText.text = "서버에 [" + id.ToString() + "]번째로 접속 성공!";
            }
        }
    }

}
