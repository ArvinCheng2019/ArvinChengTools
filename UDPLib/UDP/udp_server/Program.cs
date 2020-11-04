using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using UDP;

namespace udp_server
{
    class Program
    {
        static int clinetport = 57200, serverport = 57201;
        static void Main(string[] args)
        {
            UDPStartUp.GetInstance().RunUDPServer(serverport, clinetport);


            
            UDPStartUp.GetInstance().SetServerRecivedMsg(RevicedMsg);

            Console.ReadLine();
        }

        static void RevicedMsg( string type, string msg)
        {
            Console.WriteLine("Server Msg:" + msg + "  type: " + type);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), clinetport);
            UDPStartUp.GetInstance().SendMessage("57", ip);
        }
    }
}
