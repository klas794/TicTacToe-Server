using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server1_1
{
    class MySocketServer
    {
        Socket socket = SocketHelper.MySocket();

        List<Player> _players = new List<Player>();

        public MySocketServer()
        {
            Task task = new Task(() => Listen());
            task.Start();

            ShowServerEndPoint();
            Console.WriteLine("Stäng ner servern med Enter");
            Console.ReadLine();
            socket.Close();
        }

        private void RecieveAndBroadCast(Socket client)
        {
            while (true)
            {
                try
                {
                    byte[] bytes = new Byte[1024];
                    int recBytes = client.Receive(bytes); // Blocking
                    //var player = _players.Single(x => x.Client.RemoteEndPoint == client.RemoteEndPoint);

                    if (recBytes == 0)
                    {
                        //_players.Remove(player);
                        SocketHelper.BroadCast(client, Encoding.UTF8.GetBytes("Opponent disconnected"));
                        break; // klienten stängd => 0
                    }

                    var nettoBytes = (bytes.Take(recBytes)).ToArray<byte>();

                    string response = Encoding.UTF8.GetString(bytes, 0, recBytes);
                    SocketHelper.BroadCast(client, nettoBytes);


                    //Console.WriteLine(player.Symbol + " sending: " + response);
                    
                }
                catch
                {
                    break;
                }
            }
        }

        private void Listen()
        {
            socket.Listen(1); // Lyssnarläge

            while(true) { 
                Socket client = socket.Accept(); // Blocking
                MyClient c = new MyClient(client);

                SocketHelper.connections.Add(client);

                //if(_players.Count == 0)
                //{
                //    _players.Add(new Player { Client= client, Symbol = "x" });
                //}
                //else if (_players.Count == 1)
                //{
                //    _players.Add(new Player { Client = client, Symbol = "o" });
                //}

                Console.WriteLine("Kontakt med: " + client.RemoteEndPoint + " etablerad");

                Console.WriteLine(SocketHelper.connections.Count + " klienter inloggade");

                string welcome = SocketHelper.connections.Count > 2 ?
                    "Game allready full":
                    SocketHelper.connections.Count  == 1 ?
                    "Welcome to Tic Tac Toe Server":
                    "Welcome, start playing...";

                byte[] toBytes = Encoding.ASCII.GetBytes(welcome);
                client.Send(toBytes);

                if(SocketHelper.connections.Count <= 2) { 
                    var task = new Task(() => RecieveAndBroadCast(client));
                    task.Start();
                }

            }
        }

        private void ShowServerEndPoint()
        {
            Console.WriteLine("Tic tac toe");
            Console.WriteLine("-----------");
            Console.WriteLine("Listening to...");
            Console.WriteLine("Address: " + ((IPEndPoint)socket.LocalEndPoint).Address);
            Console.WriteLine("Port: " + ((IPEndPoint)socket.LocalEndPoint).Port);
        }
    }
}
