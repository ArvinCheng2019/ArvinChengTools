using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDP;

namespace udp_client
{
    class Program
    {
        static int clinetport = 57200, serverport = 57201;
        static void Main(string[] args)
        {
            UDPStartUp.GetInstance().RunUDPClient("127.0.0.1", serverport, clinetport);
            UDPStartUp.GetInstance().SendMessage("5", null);
            UDPStartUp.GetInstance().SetClientRevicedMsg(RevicedMsg);

            Console.ReadLine();
        }

        static void RevicedMsg(string msg)
        {
            Console.WriteLine("Client Msg:" + msg);
        }

    }
}
