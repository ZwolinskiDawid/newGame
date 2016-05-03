using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace gra
{
    public class Movable
    {
        public Point Position { get; set; }

        public BitmapImage Appearance { get; set; }

        private Vector direction;
        public Vector Direction
        {
            set
            {
                if((direction.X == 0 && value.X == 0) || (direction.Y == 0 && value.Y == 0))
                {
                    direction = value;
                }
            }
            get
            {
                return direction;
            }
        }

        public void Move()
        {
            Position += Direction;
        }
    }
}
