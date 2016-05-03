using System.Windows;
using System.Windows.Media.Imaging;

namespace gra
{
    public class Movable
    {
        public double speed { get; set; }
        public Point Position { get; set; }

        public BitmapImage Appearance { get; set; }

        private Vector direction;
        public Vector Direction
        {
            set
            {
                if(true)
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
