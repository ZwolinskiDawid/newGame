using System.Threading;
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
            World.MoveMovables();

            sender.send((int)getPlayerPosition(World.index).X,
                        (int)getPlayerPosition(World.index).Y, 0);

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

            /*
            //=============MOVE BULLETS===============
            for (int i = 0; i < World.numberOfPlayers; i++)
            {
                if(i != World.index)
                {
                    Canvas.SetLeft(getPlayerImage(i), position.X + getPlayerPosition(i).X);
                    Canvas.SetTop(getPlayerImage(i), position.Y + getPlayerPosition(i).Y);
                }
            }
            */
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                if (e.Key == Key.Up)
                {
                    World.players[World.index].Direction = new Vector(0, -3);
                }
                else if (e.Key == Key.Down)
                {
                    World.players[World.index].Direction = new Vector(0, 3);
                }
                else if (e.Key == Key.Right)
                {
                    World.players[World.index].Direction = new Vector(3, 0);
                }
                else if (e.Key == Key.Left)
                {
                    World.players[World.index].Direction = new Vector(-3, 0);
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                if (e.Key == Key.Up && World.players[World.index].Direction.Y == -3)
                {
                    World.players[World.index].Direction = new Vector(0, 0);
                }
                else if (e.Key == Key.Down && World.players[World.index].Direction.Y == 3)
                {
                    World.players[World.index].Direction = new Vector(0, 0);
                }
                else if (e.Key == Key.Right && World.players[World.index].Direction.X == 3)
                {
                    World.players[World.index].Direction = new Vector(0, 0);
                }
                else if (e.Key == Key.Left && World.players[World.index].Direction.X == -3)
                {
                    World.players[World.index].Direction = new Vector(0, 0);
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

        private Point getPlayerPosition(int index)
        {
            lock(World.players[index])
            {
                return World.players[index].Position;
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
