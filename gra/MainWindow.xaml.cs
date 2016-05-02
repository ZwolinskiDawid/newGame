﻿using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace gra
{
    public partial class MainWindow : Window
    {
        private Container World;
        private delegate void handler(); //allow to change ui
        private Sender sender;
        private Listener listener;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DoWork()
        {
            while (true)
            {
                this.Dispatcher.Invoke(new handler(MoveMap), new object[] { });
                Thread.Sleep(10);
            }
        }

        private void MoveMap()
        {
            Point position = CenterOfGameScreen;
            position.X -= playerPosition.X;
            position.Y -= playerPosition.Y;

            Canvas.SetLeft(mapImage, position.X);
            Canvas.SetTop(mapImage, position.Y);

            for (int i = 0; i < World.numberOfPlayers; i++)
            {
                if(i != World.index)
                {
                    position.X += World.players[i].Position.X;
                    position.Y += World.players[i].Position.Y;

                    Canvas.SetLeft(World.mapContainer.players[i], position.X);
                    Canvas.SetTop(World.mapContainer.players[i], position.Y);
                }
            }
        }        

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(!e.IsRepeat)
            {
                if (e.Key == Key.Up)
                {
                    this.sender.send((int)playerPosition.X, (int)playerPosition.Y - 50, (int)e.Key);
                }
                else if (e.Key == Key.Down)
                {
                    this.sender.send((int)playerPosition.X, (int)playerPosition.Y + 50, (int)e.Key);
                }
                else if (e.Key == Key.Right)
                {
                    this.sender.send((int)playerPosition.X + 50, (int)playerPosition.Y, (int)e.Key);
                }
                else if (e.Key == Key.Left)
                {
                    this.sender.send((int)playerPosition.X - 50, (int)playerPosition.Y, (int)e.Key);
                }
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            World = new Container();
            this.sender = new Sender();
            this.listener = new Listener(World);

            start.IsEnabled = true;
            status.Content = "Status: Connected";
            connect.IsEnabled = false;
        }

        private void Start_Game_Click(object sender, RoutedEventArgs e)
        {
            listener.receive_Map();

            World.CreateMap(CenterOfGameScreen);

            World.index = listener.receive_Int();
            World.numberOfPlayers = listener.receive_Int();
            World.addPlayers();
            World.mapContainer.CreatePlayer(World, CenterOfGameScreen);

            start.IsEnabled = false;

            //============SWITCH ON MAP===============

            MoveMap();

            gameBorder.Children.Add(mapImage);
            for(int i = 0; i < World.numberOfPlayers; i++)
            {
                gameBorder.Children.Add(World.mapContainer.players[i]);
            }

            //========NEW THREAD HANDLING MAP==========

            Thread worker = new Thread(new ThreadStart(DoWork));
            worker.IsBackground = true;
            worker.Start();

            //========NEW THREAD = LISTENER===============

            Thread worker2 = new Thread(new ThreadStart(this.listener.listen));
            worker2.Start();
        }

        private Point playerPosition
        {
            get
            {
                lock(World.players)
                {
                    return World.players[World.index].Position;
                }
            }
            set
            {
                lock (World.players)
                {
                    World.players[World.index].Position = value;
                }
            }
        }

        private Image mapImage
        {
            get
            {
                lock(World.mapContainer.image)
                {
                    return World.mapContainer.image;
                }
            }
        }

        private Point CenterOfGameScreen
        {
            get
            {
                double size = gameBorder.Width / 2 - World.FieldSize / 2;
                return new Point(size, size);
            }
        }
    }
}
