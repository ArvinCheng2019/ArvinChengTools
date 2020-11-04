using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UDP
{

    public delegate void RecivedMessage(string movie);

    internal class UDPClient
    {
        private Socket client;
        EndPoint serverEnd; //服务端  
        IPEndPoint serverIPEnd; //服务端端口  
       public RecivedMessage OnRecivedMessage;
        string recvStr;
        byte[] recvData = new byte[1024];
        byte[] sendData = new byte[1024];
        int recvLen;
        Thread connectThread;
        int sverport;
        int clientPort;
        string serverIP;

        public string ServerIP
        {
            set { serverIP = value; }
        }


        public int ServerPort
        {
            set { sverport = value; }
        }

        public int ClinetPort
        {
            set { clientPort = value; }
        }


        public UDPClient()
        {

        }

        //public static UDPClient GetInstance()
        //{
        //    lock (locker)
        //    {
        //        if (uniqueInstance == null)
        //        {
        //            uniqueInstance = new UDPClient();
        //        }
        //    }

        //    return uniqueInstance;
        //}

        public void SocketSend(string sendStr)
        {
            //清空发送缓存  
            // sendData = new byte[1024];
            //数据类型转换  
            sendData = Encoding.ASCII.GetBytes(sendStr);
            //发送给指定服务端  
            client.SendTo(sendData, sendData.Length, SocketFlags.None, serverIPEnd);
        }

        public void InitSocket()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Bind(new IPEndPoint(IPAddress.Any, clientPort));
            serverIPEnd = new IPEndPoint(IPAddress.Parse(serverIP), sverport);
            // SocketSend("1");

            serverEnd = (IPEndPoint)serverIPEnd;
            //开启一个线程连接，必须的，否则主线程卡死  
            connectThread = new Thread(new ThreadStart(SocketReceive));
            connectThread.IsBackground = true;
            connectThread.Start();
        }

        public void SetReciveFun( Action<string> action)
        {
        }

        void SocketReceive()
        {
            //进入接收循环  
            while (true)
            {
                recvData = new byte[1024];
                recvLen = client.ReceiveFrom(recvData, ref serverEnd);
                //输出接收到的数据  
                recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);
                if (OnRecivedMessage != null)
                    OnRecivedMessage(recvStr);
            }
        }

        public void SocketQuit()
        {
            //关闭线程  
            if (connectThread != null)
            {
                connectThread.Interrupt();
                connectThread.Abort();
            }
            //关闭socket  
            if (client != null)
                client.Close();
        }
    }

}
