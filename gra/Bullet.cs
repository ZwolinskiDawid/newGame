using System;
using System.Windows;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml;
using System.IO;

namespace gra
{
    public class Bullet : Movable
    {
        public int Owner { get; set; }

        public Bullet(Point position, Vector direction, Container World, int owner)
        {
            RealPosition = position;
            RealDirection = direction;
            Appearance = LoadTexture(@"..\..\Resources\textures.xml");
            this.World = World;

            this.Owner = owner;

            collisions = new Vector[4];
            collisions[0] = new Vector(15, 30);
            collisions[1] = new Vector(30, 30);
            collisions[2] = new Vector(15, 35);
            collisions[3] = new Vector(30, 35);
        }

        public new Vector RealDirection
        {
            get
            {
                return realDirection;
            }
            set
            {
                realDirection = 2 * value;
            }
        }

        private BitmapImage LoadTexture(string texturesXmlDir)
        {
            XmlDocument xml = new XmlDocument();
            string xmlString = (new StreamReader(texturesXmlDir, Encoding.UTF8)).ReadToEnd();
            xml.LoadXml(xmlString);

            XmlNode texture = xml.SelectSingleNode("/textures/bullet");

            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            string exactPath = System.IO.Path.GetFullPath(texture.InnerText);
            var uri = new Uri(exactPath, UriKind.Absolute);
            logo.UriSource = uri;
            logo.EndInit();

            return logo;
        }

        public bool moveAndCheck()
        {
            if (CanMove(RealPosition, RealDirection, 15))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void collisionWithPlayer(Point PositionOfPlayer)
        {
            Point newPosition = RealPosition + RealDirection;

            double x = newPosition.X - PositionOfPlayer.X;
            double y = newPosition.Y - PositionOfPlayer.Y;

            double diff = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            if(diff < 30)
            {
                int corner = new Random().Next(4);

                World.sender.send(corner, Owner, 20);
                World.players[World.index].isDead(Owner, corner);
            }
        }
    }
}
