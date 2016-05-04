using System;
using System.Windows;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml;
using System.IO;

namespace gra
{
    class Bullet : Movable
    {
        public Bullet(Point p, Vector d, Container c)
        {
            /*
            Position = p;
            Direction = d;
            Appearance = LoadTexture(@"..\..\Resources\textures.xml");
            World = c;
            */
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
    }
}
