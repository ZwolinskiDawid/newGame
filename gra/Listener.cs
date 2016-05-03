using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gra
{
    class Listener
    {
        private Socket listener;
        private Container World;

        public Listener(Container World)
        {
            this.listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            this.listener.Connect("25.88.205.228", 9999);
            this.World = World;
        }

        public void listen()
        {
            byte[] buffor = new byte[6];
            int x, y, key, index;

            while (true)
            {
                this.listener.Receive(buffor);

                x = (int)buffor[1];
                x += (int)buffor[0] << 8;
                y = (int)buffor[3];
                y += (int)buffor[2] << 8;
                key = (int)buffor[4];
                index = (int)buffor[5];

                if(key == 8 && (players[index].Position.X != x || players[index].Position.Y != y))
                {
                    lock (players[index])
                    {
                        players[index].Direction = new Vector(0, 0);
                    }
                }

                players[index].Position = new Point((double)x, (double)y);

                if (key == 0)
                {
                    lock(players[index])
                    {
                        players[index].Direction = new Vector(0, -3);
                    }
                }
                else if (key == 1)
                {
                    lock (players[index])
                    {
                        players[index].Direction = new Vector(0, 3);
                    }
                }
                else if (key == 2)
                {
                    lock (players[index])
                    {
                        players[index].Direction = new Vector(3, 0);
                    }
                }
                else if (key == 3)
                {
                    lock (players[index])
                    {
                        players[index].Direction = new Vector(-3, 0);
                    }
                }
                else if (key == 4 || key == 5 || key == 6 || key == 7)
                {
                    lock (players[index])
                    {
                        players[index].Direction = new Vector(0, 0);
                    }
                }
            }
        }

        public void receive_Map()
        {
            int len = this.receive_Int();

            byte[] buffor = new byte[100];
            int dataSize;
            string map = "";

            while (len > 0)
            {
                if (len < 100)
                {
                    dataSize = this.listener.Receive(buffor, len, SocketFlags.None);
                }
                else
                {
                    dataSize = this.listener.Receive(buffor, 100, SocketFlags.None);
                }

                map += System.Text.Encoding.UTF8.GetString(buffor, 0, dataSize);
                len -= dataSize;
            }

            System.IO.File.WriteAllText(@"..\..\Resources\container.xml", map);
        }

        public int receive_Int()
        {
            byte[] buffor = new byte[4];
            this.listener.Receive(buffor);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffor);
            }

            return BitConverter.ToInt32(buffor, 0);
        }

        private List<Human> players
        {
            get
            {
                lock (World.players)
                {
                    return World.players;
                }
            }
        }
    }
}
