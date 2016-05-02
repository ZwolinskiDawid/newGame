﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace gra
{
    class Sender
    {
        private Socket sender;
        public Sender()
        {
            this.sender = new Socket(SocketType.Stream, ProtocolType.Tcp);
            this.sender.Connect("localhost", 9998);
        }

        public void send(int x, int y, int key)
        {
            byte[] buffor = new byte[5];
            buffor[0] = (byte)(x >> 8);
            buffor[1] = (byte)x;
            buffor[2] = (byte)(y >> 8);
            buffor[3] = (byte)y;
            buffor[4] = (byte)key;

            this.sender.Send(buffor);
        }
    }
}