using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class client
    {
        static List<client> list = new List<client>();
        NetworkStream _stream;
        StreamReader _reader;
        StreamWriter _writer;
        int _id;


        public client()
        {

        }
        public client(NetworkStream stream, StreamReader reader, StreamWriter writer, int id)
        {
            _stream = stream;
            _reader = reader;
            _writer = writer;
            _id = id;

            list.Add(this);
        }

        public List<client> ReturnClientList()
        {
            return list;
        }

        public NetworkStream ReturnStream()
        {
            return _stream;
        }

        public StreamReader ReturnReader()
        {
            return _reader;
        }

        public StreamWriter ReturnWriter()
        {
            return _writer;
        }

        public int ReturnId()
        {
            return _id;
        }
  



    }
}
