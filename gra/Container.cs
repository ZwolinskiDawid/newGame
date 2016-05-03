using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace gra
{
    public class Container
    {
        public MapContainer mapContainer { get; set; }

        public int[,] Map { get; set; }

        public int?[,] MapOfObstacles { get; set; }

        public int MapSize { get; set; }

        public int FieldSize { get; set; }

        public List<Human> players { get; set; }

        public int index { get; set; }

        public int numberOfPlayers { get; set; }

        public Container()
        {
            players = new List<Human>();
            players.Add(new Human(new Point(0, 0), this));
        }

        public void addPlayers()
        {
            for(int i = 1; i < numberOfPlayers; i++)
            {
                if (i == 0) { players.Add(new Human(new Point(0, 0), this)); }
                if (i == 1) { players.Add(new Human(new Point(950, 0), this)); }
                if (i == 2) { players.Add(new Human(new Point(0, 950), this)); }
                if (i == 3) { players.Add(new Human(new Point(950, 950), this)); }
            }
        }
        
        public void CreateMap(Point position)
        {
            LoadMap(@"..\..\Resources\container.xml", @"..\..\Resources\textures.xml");
            this.mapContainer = new MapContainer(this, position);
        }

        private void LoadMap(string xmlDir, string texturesXmlDir)
        {
            XmlDocument xml = new XmlDocument();
            string xmlString = (new StreamReader(xmlDir, Encoding.UTF8)).ReadToEnd();
            xml.LoadXml(xmlString);

            XmlNode containerNode = xml.SelectSingleNode("/container");
            MapSize = Convert.ToInt32(containerNode.Attributes["size"].Value);

            XmlNodeList background = xml.SelectNodes("/container/background/line");
            XmlNodeList foreground = xml.SelectNodes("/container/foreground/line");

            Map = new int[MapSize, MapSize];
            int y = 0;
            foreach (XmlNode line in background)
            {
                int x = 0;
                foreach (var field in line.InnerText)
                {
                    Map[y,x] = Convert.ToInt32(field-'0');
                    x++;
                }
                y++;
            }

            MapOfObstacles = new int?[MapSize, MapSize];
            y = 0;
            foreach (XmlNode line in foreground)
            {
                int x = 0;
                foreach (var field in line.InnerText)
                {
                    if (field != 'n')
                    {
                        MapOfObstacles[y, x] = Convert.ToInt32(field - '0');
                    }
                    else
                    {
                        MapOfObstacles[y, x] = null;
                    }
                    x++;
                }
                y++;
            }

            xmlString = (new StreamReader(texturesXmlDir, Encoding.UTF8)).ReadToEnd();
            xml.LoadXml(xmlString);

            XmlNode textures = xml.SelectSingleNode("/textures");
            FieldSize = Convert.ToInt32(textures.Attributes["textureSize"].Value);
        }

        public void MoveMovables()
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                players[i].Move();
            }
        }
    }
}
