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
        public Bullet(Point position, Vector direction, Container World)
        {
            RealPosition = position;
            RealDirection = direction;
            Appearance = LoadTexture(@"..\..\Resources\textures.xml");
            this.World = World;

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
    }
}
