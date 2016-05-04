using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;

namespace gra
{
    public class MapContainer
    {
        public Image image { get; set; }
        public List<Image> players { get; set; }
        public List<System.Drawing.Image> Textures { get; set; }

        public MapContainer(Container World, Point position)
        {
            LoadTextures(@"..\..\Resources\textures.xml");
            CreateMap(World);
            players = new List<Image>();
        }

        public void CreateMap(Container World)
        {
            int size = World.MapSize * World.FieldSize;

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(size, size);
            System.Drawing.Graphics draw = System.Drawing.Graphics.FromImage(bmp);

            for (int y = 0; y < World.MapSize; y++)
            {
                for (int x = 0; x < World.MapSize; x++)
                {
                    Point fieldPosition = new Point(x * World.FieldSize, y * World.FieldSize);
                    draw.DrawImage(Textures[World.Map[y, x]], (int)fieldPosition.X, (int)fieldPosition.Y);

                    if (World.MapOfObstacles[y, x] != null)
                    {
                        draw.DrawImage(Textures[Convert.ToInt32(World.MapOfObstacles[y, x])], (int)fieldPosition.X, (int)fieldPosition.Y);
                    }
                }
            }

            MemoryStream memory = new MemoryStream();
            bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            image = new Image();
            image.Width = size;
            image.Height = size;
            image.Source = bitmapImage;
        }

        public void CreatePlayer(Container World, Point position)
        {
            for(int i = 0; i < World.numberOfPlayers; i++)
            {
                players.Add(new Image());
                players[i].Width = World.FieldSize;
                players[i].Height = World.FieldSize;
                players[i].Source = World.players[i].Appearance;

                if(i != World.index)
                {
                    Canvas.SetLeft(players[i], position.X + World.players[i].RealPosition.X);
                    Canvas.SetTop(players[i], position.Y + World.players[i].RealPosition.Y);
                }
                else
                {
                    Canvas.SetLeft(players[i], position.X);
                    Canvas.SetTop(players[i], position.Y);
                }
            }            
        }

        public void LoadTextures(string texturesXmlDir)
        {
            Textures = new List<System.Drawing.Image>();

            XmlDocument xml = new XmlDocument();
            string xmlString = (new StreamReader(texturesXmlDir, Encoding.UTF8)).ReadToEnd();
            xml.LoadXml(xmlString);

            XmlNodeList textureList = xml.SelectNodes("/textures/texture");

            foreach (XmlNode texture in textureList)
            {
                Textures.Add(System.Drawing.Image.FromFile(texture.InnerText));
            }
        }
    }
}
