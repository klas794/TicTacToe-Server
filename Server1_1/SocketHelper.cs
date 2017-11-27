using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server1_1
{
    public static class SocketHelper
    {
        public static List<Socket> connections = new List<Socket>();

        // Skapa en endpoint
        private static EndPoint MyEndPoint(bool b)
        {
            Tuple<string, int> t;
            int port = 8080;
            string address = "172.20.201.75";

            if (b)
            {
                t = DynamicAddress();
                address = t.Item1;
                port = t.Item2;
            }


            IPAddress adr = IPAddress.Parse(address);
            // jmfr IPAddress adr = IPAddress.Parse("127.0.0.1") // Fungerar då endast lokalt på datorn;
            // jmfr IPAddress adr = IPAddress.Any; // Samtliga ip-adresser på datorn lyssnas på

            IPEndPoint endpoint = new IPEndPoint(adr, port);

            return endpoint;
        }

        public static EndPoint MyEndPoint3()
        {
            IPAddress address = Dns.GetHostAddresses(Dns.GetHostName())
                .First(x => x.AddressFamily == AddressFamily.InterNetwork);

            IPEndPoint endpoint = new IPEndPoint(address, 8080);
            return endpoint;
        }
        
        // Skapa en ip4 tcpsocket
        public static Socket MySocket()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Bind(MyEndPoint3());
                return socket;
            }
            catch(SocketException) {
                Console.WriteLine("Ipadressen matchar inte datorns ipadress");
                return null;
            }
            catch(FormatException)
            {
                Console.WriteLine("Felformaterad ipadress");
                return null;
            }
        }

        public static void BroadCast(Socket socketCurrent, byte[] message)
        {
            foreach (Socket client in connections)
            {
                if(client != socketCurrent)
                {
                    client.Send(message);
                }
            }
        }

        private static Tuple<string,int> DynamicAddress()
        {
            Console.WriteLine("Mata in ipadress: ");
            var address = Console.ReadLine();

            Console.WriteLine("Mata in port: ");
            var port = Console.ReadLine();

            return Tuple.Create(address, int.Parse(port));
        }
    }
}
