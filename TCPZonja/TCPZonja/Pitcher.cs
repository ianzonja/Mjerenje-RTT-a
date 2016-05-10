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
    class Pitcher
    {
        public int port = 0;
        public int brzina = 1;
        public int velicina = 300;
        public string ime = "";
        byte[] bafer;
        int porukaPoslano = 0;
        int porukaPrimljenoUProsloj = 0;
        int trenutnaSekunda = 0;
        int i = 0;
        int milisekundaPrijeSlanja = 0;
        int milisekundaPoslijeSlanja = 0;
        int prosjecnoABA;
        int zapamtiRedniBroj;
        int brojacIzgubljenih = 0;
        byte[] msg;
        string poruka;
        int redniBrojPoruke;

        string trenutnoVrijeme;

        public Pitcher(int port, int brzina, int velicina, string ime)
        {
            this.port = port;
            this.brzina = brzina;
            this.velicina = velicina;
            this.ime = ime;
        }

        public void TcpServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            Thread dretvaZaOsluskivanje = new Thread(new ParameterizedThreadStart(Osluskivanje));
            dretvaZaOsluskivanje.Start(listener);
            Thread IspisStatistike = new Thread(new ThreadStart(Statistika));
            IspisStatistike.Start();

            while (true)
            {

                if (DateTime.Now.Millisecond == 0)
                {  //ako je pocetak sekunde moze proci
                    trenutnaSekunda = DateTime.Now.Second;
                    porukaPrimljenoUProsloj = 0;
                    brojacIzgubljenih = 0;
                    for (i = 0; i < brzina; i++)
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        NetworkStream stream = client.GetStream();
                        bafer = new byte[velicina];
                        Generirajporuku();
                        if (stream.CanWrite == true)
                        {
                            milisekundaPrijeSlanja = DateTime.Now.Millisecond;
                            stream.Write(bafer, 0, velicina);
                            porukaPoslano++;
                        }

                        if (stream.CanRead == true)
                        {
                            stream.Read(bafer, 0, bafer.Length);
                            milisekundaPoslijeSlanja = DateTime.Now.Millisecond;
                            porukaPrimljenoUProsloj++;
                        }
                        ProsjecnoVrijemeABA(milisekundaPrijeSlanja, milisekundaPoslijeSlanja);
                        IzgubljenaPoruka(bafer);

                        stream.Close();
                        client.Close();
                    }
                }
            }
        }
        public void Statistika() //ispis
        {
            while (true)
            {
                if (DateTime.Now.Millisecond == 999)
                {
                    Console.WriteLine("Vrijeme: " + DateTime.Now.Second + "," + DateTime.Now.Millisecond + "| Ukupno poslano: " + porukaPoslano + " | Poslano prosle sekunde: " + porukaPrimljenoUProsloj + " | RTT: " + prosjecnoABA + " | Poruka izgubljeno: " + brojacIzgubljenih);
                    System.Threading.Thread.Sleep(1);
                }
            }
        }

        public void Osluskivanje(object a)
        {

            TcpListener listener = a as TcpListener;
            listener.Start();

        }
        public void Generirajporuku()
        {
            msg = new byte[velicina];
            redniBrojPoruke++;

            //ubacivanje rednog broja poruke
            if (redniBrojPoruke >= 1 && redniBrojPoruke <= 9)
            {
                poruka = "000" + redniBrojPoruke;
            }
            else if (redniBrojPoruke >= 10 && redniBrojPoruke <= 99)
            {
                poruka = "00" + redniBrojPoruke;
            }
            else if (redniBrojPoruke >= 100 && redniBrojPoruke <= 999)
            {
                poruka = "0" + redniBrojPoruke;
            }
            else poruka = redniBrojPoruke.ToString();
            zapamtiRedniBroj = redniBrojPoruke;
            //ubacivanje trenutnog vremena
            TrenutnoVrijeme();
            poruka = poruka + trenutnoVrijeme;


            //ubacivanje velicine poruke
            if (velicina >= 50 && velicina <= 99)
            {
                poruka = poruka + "00" + velicina.ToString();
            }
            else if (velicina >= 100 && velicina <= 999)
            {
                poruka = poruka + "0" + velicina.ToString();
            }
            else poruka = poruka + velicina.ToString();

            //ubacivanje trenutno milisekundi
            int trenutnomilisekundi = DateTime.Now.Millisecond;
            if (trenutnomilisekundi >= 0 && trenutnomilisekundi <= 9) poruka = poruka + "00" + trenutnomilisekundi.ToString();
            else if (trenutnomilisekundi >= 10 && trenutnomilisekundi <= 99) poruka = poruka + "0" + trenutnomilisekundi.ToString();
            else poruka = poruka + trenutnomilisekundi.ToString();

            //ubacivanje brzine poruke
            if (brzina >= 1 && brzina <= 9) poruka = poruka + "00" + brzina.ToString();
            else if (brzina >= 10 && brzina <= 99) poruka = poruka + "0" + brzina.ToString();
            else poruka = poruka + brzina.ToString();

            //ubacivanje random znakova za ostatak poruke
            Random rand = new Random();
            for (int i = poruka.Length; i < msg.Length; i++)
            {
                poruka = poruka + "a";
            }

            msg = Encoding.ASCII.GetBytes(poruka);
            bafer = msg;

        }
        public void TrenutnoVrijeme()
        {
            trenutnoVrijeme = DateTime.Now.ToString("hh:mm:ss,ff");
        }

        public int ProsjecnoVrijemeABA(int prije, int poslije)
        {
            prosjecnoABA = poslije - prije;
            return prosjecnoABA;
        }

        public void IzgubljenaPoruka(byte[] pomocni)
        {
            string pomocnaporuka = Encoding.ASCII.GetString(pomocni);
            string zapisiRedniBroj = pomocnaporuka.Substring(0, 4);
            if (zapamtiRedniBroj != int.Parse(zapisiRedniBroj)) brojacIzgubljenih++;
        }
    }
}
