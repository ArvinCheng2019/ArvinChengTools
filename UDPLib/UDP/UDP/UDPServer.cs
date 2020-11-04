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
   internal class UDPServer
    {
        public  MessageReceive onMessageReceiveEvent;


        private Socket mSocket;
        private EndPoint clientEnd;
        private IPEndPoint ipEnd;

        byte[] recvData = new byte[1024];
        byte[] sendData = new byte[1024];
        int recvLen;
        Thread connectThread;
        int sverport;
        int clientPort;

        public UDPServer()
        {

        }




        public int ServerPort
        {
            set { sverport = value; }
        }

        public int ClinetPort
        {
            set { clientPort = value; }
        }

        public void InitSocket()
        {
            ipEnd = new IPEndPoint(IPAddress.Any, sverport);
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mSocket.Bind(ipEnd);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any,clientPort);
            clientEnd = (EndPoint)sender;
            connectThread = new Thread(new ThreadStart(SocketReceive));
            connectThread.Start();
        }

        public void SocketSend(string sendStr, IPEndPoint client)
        {
            try
            {
                sendData = Encoding.UTF8.GetBytes(sendStr);
                mSocket.SendTo(sendData, sendData.Length, SocketFlags.None, client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SocketSend(string sendStr, string clientIP)
        {
            try
            {
                IPEndPoint client = new IPEndPoint(IPAddress.Parse(clientIP), clientPort);
                sendData = Encoding.UTF8.GetBytes(sendStr);
                mSocket.SendTo(sendData, sendData.Length, SocketFlags.None, client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
        //服务器接收
        string recvStr;
        void SocketReceive()
        {
            while (true)
            {
                try
                {
                    recvData = new byte[1024];
                    //获取客户端，获取客户端数据，用引用给客户端赋值
                    recvLen = mSocket.ReceiveFrom(recvData, ref clientEnd);
                    //输出接收到的数据
                    recvStr = Encoding.Default.GetString(recvData, 0, recvLen);
                    Console.WriteLine("server:" + recvStr + "message from: " + clientEnd.ToString());
                    onMessageReceiveEvent(recvStr, GetIP(clientEnd.ToString()));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
        }
        private string GetIP(string ip)
        {
            int index = ip.IndexOf(":");
            string str = ip.Substring(0, index);
            return str;
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
            if (mSocket != null)
            {
                mSocket.Shutdown(SocketShutdown.Both);
                mSocket.Close();
            }
            Process cur = Process.GetCurrentProcess();
            cur.Kill();
        }
    }

    public delegate void MessageReceive(string type, string clientIP);
}
