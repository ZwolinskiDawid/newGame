using System.Windows;
using System.Windows.Media.Imaging;

namespace gra
{
    public class Movable
    {
        public Container World { get; set; }

        public Point RealPosition { get; set; }
        public Vector realDirection;

        public BitmapImage Appearance { get; set; }
        public Vector[] collisions { get; set; }

        public Vector RealDirection
        {
            get
            {
                return realDirection;
            }

            set
            {
                realDirection = value;
            }
        }

        public bool CanMove(Point RealPosition, Vector RealDirection, int margin)
        {
            if (IsOutOfMap(RealPosition, RealDirection, margin) || CollisionWithObstacles(RealPosition, RealDirection))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected bool IsOutOfMap(Point RealPosition, Vector RealDirection, int margin)
        {
            Point newPosition = RealPosition + RealDirection;
            return newPosition.X < -margin || newPosition.Y < -margin || 
                newPosition.X + World.FieldSize > World.MapSize * World.FieldSize + margin || 
                newPosition.Y + World.FieldSize > World.MapSize * World.FieldSize + margin;
        }

        protected bool CollisionWithObstacles(Point RealPosition, Vector RealDirection)
        {
            Point newPosition = RealPosition + RealDirection, tmp;
            int x, y;

            for(int i = 0; i < 4; i++)
            {
                tmp = newPosition + collisions[i];

                x = (int)(tmp.X / World.FieldSize);
                y = (int)(tmp.Y / World.FieldSize);

                if (World.MapOfObstacles[y, x] != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
