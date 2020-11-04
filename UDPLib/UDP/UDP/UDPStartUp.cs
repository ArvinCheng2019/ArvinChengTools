using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace UDP
{
    public enum UDPState
    {
        NONE,
        UDP_SERVER,
        UDP_CLIENT
    }

    public class UDPStartUp
    {
        private UDPState mState = UDPState.NONE;

        private UDPServer mServer;
        private UDPClient mClient;


        private static UDPStartUp uniqueInstance;
        private static readonly object locker = new object();

        public static UDPStartUp GetInstance()
        {
            lock (locker)
            {
                if (uniqueInstance == null)
                {
                    uniqueInstance = new UDPStartUp();
                }
            }
            return uniqueInstance;
        }


        public void RunUDPServer(int serverPort, int clientPort)
        {
            mState = UDPState.UDP_SERVER;
            RunServer(serverPort, clientPort);
        }

        public void RunUDPClient( string serverIP, int serverPort,int clientPort )
        {
            mState = UDPState.UDP_CLIENT;
            RunClinet(serverIP,  serverPort,  clientPort);
        }


        public void SocketQuit()
        {
            switch (mState)
            {
                case UDPState.UDP_CLIENT:
                    {
                        ClientQuite();
                    }
                    break;
                case UDPState.UDP_SERVER:
                    {
                        ServerQuite();
                    }
                    break;
            }
        }


        private void RunServer(int serverPort, int clientPort)
        {
            mServer = new UDPServer();
            mServer.ClinetPort = clientPort;
            mServer.ServerPort = serverPort;
            mServer.InitSocket();
        }

        private void RunClinet( string ip, int serverPort, int clientPort)
        {
            mClient = new UDPClient();
            mClient.ClinetPort = clientPort;
            mClient.ServerPort = serverPort;
            mClient.ServerIP = ip;

            mClient.InitSocket();
        }

        private void ServerQuite()
        {
            if(mServer !=null)
                mServer.SocketQuit();
        }

        private void ClientQuite()
        {
            if (mClient != null)
                mClient.SocketQuit();
            mClient = null;
        }


        public void SendMessage(string msg, IPEndPoint ip)
        {
            if (mState == UDPState.UDP_SERVER)
            {
                mServer.SocketSend(msg, ip);
            }
            else if (mState == UDPState.UDP_CLIENT)
            {
                mClient.SocketSend(msg);
            }
        }


        public void SetClientRevicedMsg(UDP.RecivedMessage onRecivedMessage)
        {
            if (mClient.OnRecivedMessage == null)
                mClient.OnRecivedMessage += onRecivedMessage;
        }

        public void SetServerRecivedMsg(UDP.MessageReceive onRecivedMessage  )
        {
            if (mServer.onMessageReceiveEvent == null)
                mServer.onMessageReceiveEvent += onRecivedMessage;
        }

    }
}
