using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class ClientHandler
    {
        Item items = new Item();

        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();
        int count = 0;

        public void connectClients()
        {
            items.fillList();
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 20001);
            ServerSocket.Start();

            Thread st = new Thread(GravelCall);
            st.Start();

            while (true)
            {
                TcpClient client = ServerSocket.AcceptTcpClient();
                lock (_lock) list_clients.Add(count, client);
                Console.WriteLine("Someone connected!!");


                Thread t = new Thread(handle_clients);
                t.Start(count);
                count++;
            }
        }

        public void GravelCall()
        {
            Gravel gr = Gravel.Instance;
            gr.gravel += new Gravel.gravelEventHandler(BroadCastGravel);
        }

        private void BroadCastGravel(Item item)
        {
            client cl = new client();
            var list = cl.ReturnClientList();
            foreach (client it in list)
            {
                StreamWriter writer = it.ReturnWriter();

                writer.WriteLine("red");
                writer.WriteLine(item.Name + " er nu budt op på: " + item.Price);

            }
            gravelTimer(item);

        }

        private async Task gravelTimer(Item item)
        {
            await Task.Delay(10000);
            int newprice = item.ReturnItem(item.Name).Price;
            if (item.Price == newprice)
            {
                BroadCast("Der er nu 3 sekunder til " + item.Name + " er solgt. Sidste bud er på: " + item.Price);

                await Task.Delay(3000);

                if (item.Price == newprice)
                {
                    BroadCast(item.Name + " er solgt til bruger id:" + item.GetId(item) + " til: " + item.Price);
                    item.DeleteItem(item);
                }
            }

        }

        private void BroadCast(string data)
        {
            client cl = new client();
            var list = cl.ReturnClientList();
            foreach (client it in list)
            {
                StreamWriter writer = it.ReturnWriter();

                writer.WriteLine("red");
                writer.WriteLine(data);
            }
        }
        public void handle_clients(object o)
        {
            int id = (int)o;
            TcpClient client;

            lock (_lock) client = list_clients[id];

            while (true)
            {
                try
                {
                    clientConnection(client);

                }
                catch
                {

                }

            }

            lock (_lock) list_clients.Remove(id);
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private void clientConnection(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                client cl = new client(stream, reader, writer, count);

                writer.AutoFlush = true;

                numberOfCLient(client);
                communication(cl);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void communication(client cl)
        {
            string option;


            while (true)
            {
                StreamReader reader = cl.ReturnReader();
                option = reader.ReadLine();

                if (option != null)
                {
                    switch (option)
                    {
                        case "1":
                            ShowItems(cl);
                            break;
                        case "2":
                            lock (_lock) list_clients.Remove(cl.ReturnId());
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void Menu(client cl)
        {
            StreamWriter writer = cl.ReturnWriter();
            writer.WriteLine("Tast 1 for at se auktioner");
            writer.WriteLine("Tast 2 for at lukke ned");
        }

        private void ShowItems(client cl)
        {
            StreamReader reader = cl.ReturnReader();
            StreamWriter writer = cl.ReturnWriter();
            items.showItemList(writer, reader);
            writer.WriteLine("Tast 1 for at byde på et item");
            writer.WriteLine("Tast 2 for start menu");
            string choice = reader.ReadLine();
            switch (choice)
            {
                case "1":
                    BidOnItem(cl);
                    break;
                case "2":
                    Menu(cl);
                    break;
            }
        }

        private void BidOnItem(client cl)
        {
            StreamReader reader = cl.ReturnReader();
            StreamWriter writer = cl.ReturnWriter();
            Item item = null;
            bool exist = false;
            while (!exist)
            {
                writer.WriteLine("Skriv navnet på det item du vil byde på");
                string name = reader.ReadLine();
                item = items.ReturnItem(name);
                if (item != null)
                {
                    exist = true;
                }
                else
                {
                    writer.WriteLine("Dette item eksistere ikke. Skriv et gyldigt item!");
                }
            }
            writer.WriteLine("Genstanden: {0} har prisen: {1}", item.Name, item.Price);

            bool parsed = false;
            while (!parsed)
            {
                writer.WriteLine("Hvor meget vil du byde?");
                string bid = reader.ReadLine();
                int iBid;
                if (Int32.TryParse(bid, out iBid))
                {
                    if (iBid > item.Price)
                    {
                        item.Price = iBid;
                        items.ChangePrice(item, cl.ReturnId());
                        parsed = true;
                    }
                    else
                    {
                        writer.WriteLine("Budet skal være højere end det nuværende");
                    }
                }
                else
                {
                    writer.WriteLine("Skriv et tal");
                }

            }

            ShowItems(cl);
        }

        private void numberOfCLient(TcpClient client)
        {
            var clientVal = list_clients.Single(x => x.Value == client);
            int value = clientVal.Key;
            Console.WriteLine("Client {0} {1} joined the Auctionhouse", ++value, list_clients.Count);
        }
    }
}

