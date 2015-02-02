using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using TransportMayhem.View;
using TransportMayhem.Model;
using TransportMayhem.Model.GridObjects;

namespace TransportMayhem.Controller
{
    /// <summary>
    /// The main engine of the game.
    /// </summary>
    class GameEngine
    {
        public const int UpdateAlways = -1;
        private long _tickCounter = 0;
        public long TickCounter { get { return _tickCounter; } }
        public event Action Tick;
        public const int TICKCOUNT = 60;
        private GraphicsEngine graphics;
        private Grid _grid;
        private GameWindow form;
        private Timer UpdateTimer;
        public Grid Grid
        {
            get { return _grid; }
        }
        public bool IsGraphicsRunning { get { return graphics != null && graphics.IsRunning; } }

        public GameEngine(GameWindow form)
        {
            this.form = form;
            UpdateTimer = new Timer(1000 / TICKCOUNT);
            UpdateTimer.Elapsed += OnUpdate;
        }

        public void Start(Grid grid = null)
        {
            _grid = grid != null ? grid : new Grid(this, 20, 20);
            TrainNetworkRegistry.Initialize(this);
            InputHandler.OnClick += OnClick;
            UpdateTimer.Start();


            _grid.AddObject(new TransportMayhem.Model.MovingObjects.Train(3, 2, Rotation.Left));
            _grid.AddObject(new Station(2, 1, 3, 3));
        }

        private void OnClick(int x, int y, System.Windows.Forms.MouseButtons button)
        {
            if (button == System.Windows.Forms.MouseButtons.Left)
            {
                Point p = GraphicsEngine.TranslateToGrid(x, y);
                GridObject go = _grid[p];
                if (go == null)
                {
                    if (InputHandler.InputType != null)
                    {
                        go = GridObject.InstantiateObject(InputHandler.InputType, p.X, p.Y, 1, 1, InputHandler.SelectedRotation);
                        if(go != null)
                            _grid.AddObject(go);
                    }
                }
                else
                {
                    IClickableObject clicakble = go as IClickableObject;
                    if (clicakble == null) return;
                    clicakble.OnClick(button);
                }
            }
        }

        public void StartGraphics(Graphics g)
        {
            graphics = new GraphicsEngine(g);
            graphics.Start(this);
        }

        private void OnUpdate(object sender, ElapsedEventArgs e)
        {
            _tickCounter++;
            if (Tick != null)
                Tick();
        }

        public void StopGame()
        {
            graphics.Stop();
            TMConsole.Log("Waiting for graphics to stop");
            while (graphics.IsRunning)
                System.Threading.Thread.Sleep(5);
            TMConsole.Log("Waiting for console to stop");
            if (GlobalVars.CONSOLE)
            {
                TMConsole.Stop();
                while (TMConsole.IsStarted)
                    System.Threading.Thread.Sleep(5);
            }
            if (!form.Stopping)
                form.Close();
            InputHandler.OnClick -= OnClick;
        }
    }
}
