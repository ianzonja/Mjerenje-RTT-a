using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TCPZonja
{
    class Program
    {
        static void Main(string[] args)
        {
            Catcher catcher;
            Pitcher pitcher;

            if (args[0] == "-c" && args[1] == "-bind" && args[3] == "-port")
            {
                Console.WriteLine("Uso u catchera");
                catcher = new Catcher(args[2], int.Parse(args[4]));
                catcher.clientConnect();
            }

            else if (args[0] == "-p" && args[1] == "-port" && args[3] == "-mps" && args[5] == "-size")
            {
                Console.WriteLine("Uso u pitchera");
                pitcher = new Pitcher(int.Parse(args[2]), int.Parse(args[4]), int.Parse(args[6]), args[7]);
                pitcher.TcpServer();

            }

        }
    }
}
