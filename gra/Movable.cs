using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace gra
{
    public class Movable
    {
        public double Speed { get; set; }
        public Container World { get; set; }
        public Point RealPosition { get; set; }
        public Vector RealDirection { set; get; }
        public BitmapImage Appearance { get; set; }

        
        public void moveRealPosition()
        {
            if (CanMove())
            {
                RealPosition += RealDirection;
            }
            else
            {
                World.sender.send((int)RealPosition.X, (int)RealPosition.Y, 5);
                RealDirection = new Vector(0, 0);
            }
        }
        
        public bool CanMove()
        {
            if (IsOutOfMap())
            {
                return false;
            }
            else if (this == World.players[World.index] && CollisionWithObstacles())
            {
                return false;
            }
            return true;
        }

        private bool IsOutOfMap()
        {
            Point newPosition = RealPosition + RealDirection;
            return newPosition.X < 0 || newPosition.Y < 0 ||
                newPosition.X + World.FieldSize > World.MapSize * World.FieldSize ||
                newPosition.Y + World.FieldSize > World.MapSize * World.FieldSize;
        }
        
        private bool CollisionWithObstacles()
        {
            Point newPosition = RealPosition + RealDirection;
            Point playerCenter = new Point(newPosition.X + World.FieldSize / 2, newPosition.Y + World.FieldSize / 2);
            Point playerPositionIndex = new Point((int)playerCenter.X / World.FieldSize, (int)playerCenter.Y / World.FieldSize);

            if (RealDirection.X == 0)
            {
                Point obstaclePositionIndex = new Point(playerPositionIndex.X + 1,
                                                    playerPositionIndex.Y + RealDirection.Y / Speed);
                if (CheckObstacle(obstaclePositionIndex, playerCenter)) return true;

                obstaclePositionIndex = new Point(playerPositionIndex.X,
                                                    playerPositionIndex.Y + RealDirection.Y / Speed);
                if (CheckObstacle(obstaclePositionIndex, playerCenter)) return true;
                obstaclePositionIndex = new Point(playerPositionIndex.X - 1,
                                                    playerPositionIndex.Y + RealDirection.Y / Speed);
                if (CheckObstacle(obstaclePositionIndex, playerCenter)) return true;
            }
            else if(RealDirection.Y == 0)
            {
                Point obstaclePositionIndex = new Point(playerPositionIndex.X + RealDirection.X / Speed,
                                                    playerPositionIndex.Y + 1);
                if (CheckObstacle(obstaclePositionIndex, playerCenter)) return true;

                obstaclePositionIndex = new Point(playerPositionIndex.X + RealDirection.X / Speed,
                                                    playerPositionIndex.Y);
                if (CheckObstacle(obstaclePositionIndex, playerCenter)) return true;
                obstaclePositionIndex = new Point(playerPositionIndex.X + RealDirection.X / Speed,
                                                    playerPositionIndex.Y - 1);
                if (CheckObstacle(obstaclePositionIndex, playerCenter)) return true;
            }
            return false;
        }

        private bool CheckObstacle(Point obstaclePositionIndex, Point playerCenter)
        {
            if (obstaclePositionIndex.Y >= 0 && obstaclePositionIndex.Y < World.MapSize && obstaclePositionIndex.X >= 0 &&
                obstaclePositionIndex.X < World.MapSize)
            {
                if (World.MapOfObstacles[(int)obstaclePositionIndex.Y, (int)obstaclePositionIndex.X] != null)
                {
                    Point obstacleCenter = new Point(obstaclePositionIndex.X * World.FieldSize + World.FieldSize / 2,
                                                     obstaclePositionIndex.Y * World.FieldSize + World.FieldSize / 2);
                    double delta = Math.Sqrt(Math.Pow(obstacleCenter.X - playerCenter.X, 2) +
                                             Math.Pow(obstacleCenter.Y - playerCenter.Y, 2));
                    if (delta <= World.FieldSize * 0.8)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
