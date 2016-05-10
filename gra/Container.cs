using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace gra
{
    public class Container
    {
        public Results Result { get; private set; }

        public List<dynamic> BulletsToAdd;

        public List<Vector> Directions;

        Canvas gameBorder;
        public Sender sender { get; private set; }
        public Listener listener { get; private set; }
        public MapContainer mapContainer { get; set; }

        public int[,] Map { get; set; }

        public int?[,] MapOfObstacles { get; set; }

        public int MapSize { get; set; }

        public int FieldSize { get; set; }

        public List<Human> players { get; set; }
        public List<Bullet> bullets { get; set; }

        public int index { get; set; }

        public int numberOfPlayers { get; set; }

        public Container(Canvas gameBorder, string ipAdress)
        {
            this.gameBorder = gameBorder;

            players = new List<Human>();
            players.Add(new Human(getCorner(0), this));

            this.sender = new Sender(ipAdress);
            this.listener = new Listener(ipAdress, this);

            bullets = new List<Bullet>();

            Directions = new List<Vector>();
            Directions.Add(new Vector(0, -3));
            Directions.Add(new Vector(0, 3));
            Directions.Add(new Vector(3, 0));
            Directions.Add(new Vector(-3, 0));
            Directions.Add(new Vector(0, 0));

            BulletsToAdd = new List<dynamic>();

            Result = new Results();
        }

        public void addPlayers()
        {
            for(int i = 1; i < numberOfPlayers; i++)
            {
                if (i == 1) { players.Add(new Human(getCorner(1), this)); }
                if (i == 2) { players.Add(new Human(getCorner(2), this)); }
                if (i == 3) { players.Add(new Human(getCorner(3), this)); }
            }
        }

        public Point getCorner(int index)
        {
            if (index == 0) { return new Point(0, 0); }
            else if (index == 1) { return new Point(950, 0); }
            else if (index == 2) { return new Point(0, 950); }
            else { return new Point(950, 950); }
        }

        public void addBullet(Point CenterOfGameScreen, Point Position, Vector Direction)
        {
            bullets.Add(new Bullet(Position, Direction, this, index));

            Point position = CenterOfGameScreen;
            position.X -= players[index].RealPosition.X;
            position.Y -= players[index].RealPosition.Y;

            mapContainer.addBullet(this, position);
            gameBorder.Children.Add(mapContainer.bullets[mapContainer.bullets.Count - 1]);
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

        public void movePLayers()
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if(i != index)
                {
                    lock(players[i])
                    {
                        players[i].setRealDirection();
                    }
                }
            }

            for (int i = 0; i < numberOfPlayers; i++)
            {
                lock(players[i])
                {
                    players[i].moveRealPosition();
                }
            }
        }

        public void moveBullets()
        {
            List<Bullet> toRemove = new List<Bullet>();

            foreach (Bullet bullet in bullets)
            {
                if(bullet.moveAndCheck())
                {
                    if(bullet.Owner != index)
                    {
                        lock (players[index])
                        {
                            bullet.collisionWithPlayer(players[index].RealPosition);
                        }
                    }

                    bullet.RealPosition += bullet.RealDirection;
                }
                else
                {
                    toRemove.Add(bullet);
                }
            }

            foreach(Bullet bullet in toRemove)
            {
                int index = bullets.IndexOf(bullet);

                Image image = mapContainer.bullets[index];
                gameBorder.Children.Remove(image);

                mapContainer.bullets.Remove(image);
                bullets.Remove(bullet);
            }
        }

        public void addBullets(Point CenterOfGameScreen)
        {
            lock(BulletsToAdd)
            {
                foreach(dynamic bullet in BulletsToAdd)
                {
                    bullets.Add(new Bullet(bullet.Position, bullet.Direction, this, bullet.Owner));

                    Point position = CenterOfGameScreen;
                    position.X -= players[index].RealPosition.X;
                    position.Y -= players[index].RealPosition.Y;

                    mapContainer.addBullet(this, position);
                    gameBorder.Children.Add(mapContainer.bullets[mapContainer.bullets.Count - 1]);
                }

                BulletsToAdd.Clear();
            }
        }
    }
}
