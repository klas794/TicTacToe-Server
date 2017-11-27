using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server1_1
{
    class MyClient
    {
        public Socket client { get; set; }

        public MyClient(Socket client)
        {
            this.client = client;

            Task task = new Task(() => Listen());
            task.Start();
        }

        public void Listen()
        {
            while(true)
            {
                try { 
                    byte[] bytes = new Byte[1024];
                    int recBytes = client.Receive(bytes); // Blocking
                    if (recBytes == 0) break; // klienten stängd => 0

                    var nettoBytes = (bytes.Take(recBytes)).ToArray<byte>();

                    string response = Encoding.UTF8.GetString(bytes, 0, recBytes);
                    if (response == "x") break;
                    SocketHelper.BroadCast(client, nettoBytes);
                    // Console.Write(response);

                    //Console.WriteLine("Skickar tid...");
                    //var time = DateTime.Now.ToLongTimeString();
                    //client.Send(Encoding.UTF8.GetBytes(time));
                }
                catch
                {
                    break;
                }
            }
            Console.WriteLine("Klienten stänger");
            SocketHelper.connections.Remove(client);
            client.Close();
            Console.WriteLine(SocketHelper.connections.Count + " klienter inloggade");
        }
    }
}
