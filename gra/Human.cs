using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;

namespace gra
{
    public class Human : Movable
    {
        public Point TargetPosition { get; set; }
        public Vector TargetDirection { set; get; }

        public Human(Point Position, Container World)
        {
            RealPosition = Position;
            TargetPosition = Position;

            this.World = World;

            RealDirection = new Vector(0, 0);
            TargetDirection = new Vector(0, 0);

            Appearance = LoadTexture(@"..\..\Resources\textures.xml");

            collisions = new Vector[4];
            collisions[0] = new Vector(12, 30);
            collisions[1] = new Vector(45, 30);
            collisions[2] = new Vector(12, 45);
            collisions[3] = new Vector(45, 45);
        }

        private BitmapImage LoadTexture(string texturesXmlDir)
        {
            XmlDocument xml = new XmlDocument();
            string xmlString = (new StreamReader(texturesXmlDir, Encoding.UTF8)).ReadToEnd();
            xml.LoadXml(xmlString);

            XmlNode texture = xml.SelectSingleNode("/textures/human");

            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            string exactPath = System.IO.Path.GetFullPath(texture.InnerText);
            var uri = new Uri(exactPath, UriKind.Absolute);
            logo.UriSource = uri;
            logo.EndInit();

            return logo;
        }

        public void setRealDirection()
        {
            TargetPosition += TargetDirection;

            if (RealPosition != TargetPosition)
            {
                double a = TargetPosition.X - RealPosition.X;
                double b = TargetPosition.Y - RealPosition.Y;

                if (a != 0 && b != 0)
                {
                    double c = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
                    double x = 3 * a / c;
                    double y = x * b / a;

                    RealDirection = new Vector(x, y);
                }
                else if (a == 0 && b != 0)
                {
                    RealDirection = new Vector(0, (b > 0) ? 3 : -3);
                }
                else if (b == 0 && a != 0)
                {
                    RealDirection = new Vector((a > 0) ? 3 : -3, 0);
                }
            }
        }

        public void moveRealPosition()
        {
            if(this != World.players[World.index])
            {
                if (RealDirection != new Vector(0,0))
                {
                    Vector diff = TargetPosition - RealPosition;

                    if (RealDirection.Length > diff.Length)
                    {
                        RealPosition = TargetPosition;
                        RealDirection = new Vector(0, 0);
                    }
                    else
                    {
                        RealPosition += RealDirection;
                    }
                }
                else
                {
                    RealPosition += RealDirection;
                }
            }
            else if (CanMove(RealPosition, RealDirection, 0))
            {
                RealPosition += RealDirection;
            }
            else
            {
                World.sender.send((int)RealPosition.X, (int)RealPosition.Y, 4);
                RealDirection = new Vector(0, 0);
            }
        }

        public void isDead(int whoKill, int corner)
        {
            this.RealPosition = World.getCorner(corner);
            this.TargetPosition = World.getCorner(corner);

            this.RealDirection = new Vector(0, 0);
            this.TargetDirection = new Vector(0, 0);

            lock(World.Result)
            {
                World.Result.addPoint(whoKill);
            }
        }
    }
}
