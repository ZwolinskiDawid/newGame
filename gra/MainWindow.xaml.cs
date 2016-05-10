using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System;

namespace gra
{
    public partial class MainWindow : Window
    {
        private Container World;
        private delegate void handler(); //allow to change ui

        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 4; i++)
            {
                getLabelOfPlayers(i).Visibility = Visibility.Hidden;
                getLabelOfResults(i).Visibility = Visibility.Hidden;
            }
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
            World.movePLayers();
            World.addBullets(CenterOfGameScreen);
            World.moveBullets();

            Point position = CenterOfGameScreen;
            position.X -= getPlayerPosition(World.index).X;
            position.Y -= getPlayerPosition(World.index).Y;

            Canvas.SetLeft(mapImage, position.X);
            Canvas.SetTop(mapImage, position.Y);

            //=============MOVE PLAYERS===============
            for (int i = 0; i < World.numberOfPlayers; i++)
            {
                if(i != World.index)
                {
                    Canvas.SetLeft(getPlayerImage(i), position.X + getPlayerPosition(i).X);
                    Canvas.SetTop(getPlayerImage(i), position.Y + getPlayerPosition(i).Y);
                }
            }

            //=============MOVE BULLETS===============
            lock(World.mapContainer.bullets)
            {
                for (int i = 0; i < World.bullets.Count; i++)
                {
                    Canvas.SetLeft(World.mapContainer.bullets[i], position.X + World.bullets[i].RealPosition.X);
                    Canvas.SetTop(World.mapContainer.bullets[i], position.Y + World.bullets[i].RealPosition.Y);
                }
            }

            for (int i = 0; i < World.numberOfPlayers; i++)
            {
                lock(World.Result)
                {
                    getLabelOfResults(i).Content = World.Result.Result[i].ToString();
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                if (e.Key == Key.Up)
                {
                    World.players[World.index].RealDirection = new Vector(0, -3);
                    World.sender.send((int)getPlayerPosition(World.index).X,
                        (int)getPlayerPosition(World.index).Y, 0);
                }
                else if (e.Key == Key.Down)
                {
                    World.players[World.index].RealDirection = new Vector(0, 3);
                    World.sender.send((int)getPlayerPosition(World.index).X,
                        (int)getPlayerPosition(World.index).Y, 1);
                }
                else if (e.Key == Key.Right)
                {
                    World.players[World.index].RealDirection = new Vector(3, 0);
                    World.sender.send((int)getPlayerPosition(World.index).X,
                        (int)getPlayerPosition(World.index).Y, 2);
                }
                else if (e.Key == Key.Left)
                {
                    World.players[World.index].RealDirection = new Vector(-3, 0);
                    World.sender.send((int)getPlayerPosition(World.index).X,
                        (int)getPlayerPosition(World.index).Y, 3);
                }
                else if(e.Key == Key.Space)
                {
                    World.addBullet(CenterOfGameScreen, getPlayerPosition(World.index), World.players[World.index].PreviousDirection);

                    World.sender.send((int)getPlayerPosition(World.index).X,
                                      (int)getPlayerPosition(World.index).Y,
                                       World.Directions.IndexOf(World.players[World.index].PreviousDirection) + 10);
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                if (e.Key == Key.Up && World.players[World.index].RealDirection.Y == -3)
                {
                    World.players[World.index].RealDirection = new Vector(0, 0);
                    World.sender.send((int)getPlayerPosition(World.index).X,
                        (int)getPlayerPosition(World.index).Y, 4);
                }
                else if (e.Key == Key.Down && World.players[World.index].RealDirection.Y == 3)
                {
                    World.players[World.index].RealDirection = new Vector(0, 0);
                    World.sender.send((int)getPlayerPosition(World.index).X,
                        (int)getPlayerPosition(World.index).Y, 4);
                }
                else if (e.Key == Key.Right && World.players[World.index].RealDirection.X == 3)
                {
                    World.players[World.index].RealDirection = new Vector(0, 0);
                    World.sender.send((int)getPlayerPosition(World.index).X,
                        (int)getPlayerPosition(World.index).Y, 4);
                }
                else if (e.Key == Key.Left && World.players[World.index].RealDirection.X == -3)
                {
                    World.players[World.index].RealDirection = new Vector(0, 0);
                    World.sender.send((int)getPlayerPosition(World.index).X,
                        (int)getPlayerPosition(World.index).Y, 4);
                }
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            World = new Container(gameBorder, this.ipAdress.Text);

            start.IsEnabled = true;
            status.Content = "Status:   Connected";
            connect.IsEnabled = false;
        }

        private void Start_Game_Click(object sender, RoutedEventArgs e)
        {
            start.IsEnabled = false;

            World.listener.receive_Map();

            World.CreateMap(CenterOfGameScreen);

            World.index = World.listener.receive_Int();
            World.numberOfPlayers = World.listener.receive_Int();
            World.addPlayers();
            World.mapContainer.CreatePlayer(World, CenterOfGameScreen);

            for (int i = 0; i < 4; i++)
            {
                if (i < World.numberOfPlayers)
                {
                    getLabelOfPlayers(i).Visibility = Visibility.Visible;
                    getLabelOfResults(i).Visibility = Visibility.Visible;
                    getLabelOfResults(i).Content = World.Result.Result[i].ToString();
                }
                else
                {
                    getLabelOfPlayers(i).Visibility = Visibility.Hidden;
                    getLabelOfResults(i).Visibility = Visibility.Hidden;
                }
            }

            //============SWITCH ON MAP===============

            MoveMap();

            gameBorder.Children.Add(mapImage);
            for (int i = 0; i < World.numberOfPlayers; i++)
            {
                gameBorder.Children.Add(World.mapContainer.players[i]);
            }

            //========NEW THREAD HANDLING MAP==========

            Thread worker = new Thread(new ThreadStart(DoWork));
            worker.IsBackground = true;
            worker.Start();

            //========NEW THREAD = LISTENER===============

            Thread worker2 = new Thread(new ThreadStart(World.listener.listen));
            worker2.IsBackground = true;
            worker2.Start();
        }

        private Label getLabelOfPlayers(int index)
        {
            if (index == 0) { return this.player0; }
            else if (index == 1) { return this.player1; }
            else if (index == 2) { return this.player2; }
            else { return this.player3; }
        }

        private Label getLabelOfResults(int index)
        {
            if (index == 0) { return this.result0; }
            else if (index == 1) { return this.result1; }
            else if (index == 2) { return this.result2; }
            else { return this.result3; }
        }

        private Point getPlayerPosition(int index) // Real Position
        {
            lock(World.players[index])
            {
                return World.players[index].RealPosition;
            }
        }

        private Image getPlayerImage(int index)
        {
            lock(World.mapContainer.players[index])
            {
                return World.mapContainer.players[index];
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
