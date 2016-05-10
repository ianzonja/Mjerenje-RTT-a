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
    class Catcher
    {
        public string ip;
        public int port;
        public string poruka = "";
        private int velicinaPoruke = 3000;
        public int prosjecnoAB = 0;
        string trenutnoVrijemeUMS;
        byte[] bafer;
        int brzina;
        int i = 0;



        public Catcher(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        public void clientConnect()
        {
            while (true)
            {
                if (i == brzina - 1) i = 0;
                TcpClient client = new TcpClient();
                client.Connect(ip, port);
                NetworkStream stream = client.GetStream();
                Console.WriteLine("Uso u beskonacnu");
                if (velicinaPoruke == 0)
                {
                    bafer = new byte[3000];
                }
                else bafer = new byte[velicinaPoruke];
                if (stream.CanRead == true)
                {
                    velicinaPoruke = stream.Read(bafer, 0, velicinaPoruke);
                    trenutnoVrijemeUMS = DateTime.Now.Millisecond.ToString();
                    poruka = Encoding.ASCII.GetString(bafer);
                    Console.WriteLine("Primio: " + poruka);
                    Console.WriteLine("Velicina poruke: " + velicinaPoruke);
                    string pom = poruka.Substring(22, 3); //izvuci brzinu
                    brzina = int.Parse(pom);
                }
                if (stream.CanWrite == true)
                {
                    stream.Write(bafer, 0, bafer.Length);
                }
                stream.Close();
                client.Close();
            }
        }       
    }
}
