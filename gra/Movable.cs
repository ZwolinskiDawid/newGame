﻿using System.Windows;
using System.Windows.Media.Imaging;

namespace gra
{
    public class Movable
    {
        public Container World { get; set; }

        public Point Position { get; set; }

        public BitmapImage Appearance { get; set; }

        private Vector direction;
        public Vector Direction
        {
            set
            {
                if((value.X == 0 || value.Y == 0) && (value.X != direction.X || value.Y != direction.Y))
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
            if (CanMove())
            {
                Position += Direction;
            }
        }

        public bool CanMove()
        {
            if (IsOutOfMap(Position+Direction))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsOutOfMap(Point newPosition)
        {
            return newPosition.X < 0 || newPosition.Y < 0 ||
                newPosition.X > World.MapSize + World.FieldSize ||
                newPosition.Y > World.MapSize + World.FieldSize;
        }
    }
}
