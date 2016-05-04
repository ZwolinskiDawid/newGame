using System.Windows;
using System.Windows.Media.Imaging;

namespace gra
{
    public class Movable
    {
        public Container World { get; set; }

        public Point RealPosition { get; set; }
        public Vector RealDirection { set; get; }

        public BitmapImage Appearance { get; set; }

        
        public void moveRealPosition()
        {
            RealPosition += RealDirection;
        }
        /*
        public bool CanMove()
        {
            if (IsOutOfMap(RealPosition+Direction))
            {
                return false;
            }
            return true;
        }

        public bool IsOutOfMap(Point newPosition)
        {
            return newPosition.X < 0 || newPosition.Y < 0 ||
                newPosition.X + World.FieldSize > World.MapSize * World.FieldSize ||
                newPosition.Y + World.FieldSize > World.MapSize * World.FieldSize;
        }
        */
    }
}
