using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace gra
{
    public class Sender
    {
        private Socket sender;
        public Sender(string ipAdress)
        {
            this.sender = new Socket(SocketType.Stream, ProtocolType.Tcp);
            this.sender.Connect(ipAdress, 9998);
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

        public void sendNickName(string name)
        {
            byte[] bytes = new byte[name.Length * sizeof(char)];
            System.Buffer.BlockCopy(name.ToCharArray(), 0, bytes, 0, bytes.Length);

            byte[] len = new byte[1];
            len[0] = (byte)bytes.Length;

            this.sender.Send(len);
            this.sender.Send(bytes);
        }
    }
}
