using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client";
            TcpClient client = new TcpClient("localhost", 20001);

            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            Console.WriteLine("Velkommen til Auktionshuset");
            Console.WriteLine("Tast 1 for at se auktioner");
            Console.WriteLine("Tast 2 for at lukke ned");


            Program p = new Program();

            new Thread(read => p.Read(reader)).Start();
            new Thread(write => p.Write(writer)).Start();
        }

        public void Read(StreamReader reader)
        {
            while (true)
            {
                string read = reader.ReadLine();
                if (read.Trim() == "red")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(reader.ReadLine().PadRight(Console.WindowWidth - 1));
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(read);

                }

            }
        }

        public void Write(StreamWriter writer)
        {
            while (true)
            {
                writer.WriteLine(Console.ReadLine());
            }
        }
    }
}





