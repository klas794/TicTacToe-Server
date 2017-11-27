using System.Net.Sockets;

namespace Server1_1
{
    internal class Player
    {
        public Socket Client { get; set; }
        public string Symbol { get; set; }
    }
}