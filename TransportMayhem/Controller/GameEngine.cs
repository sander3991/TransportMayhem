using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.View;
using TransportMayhem.Model;

namespace TransportMayhem.Controller
{
    /// <summary>
    /// The main engine of the game.
    /// </summary>
    class GameEngine
    {
        /// <summary>
        /// 
        /// </summary>
        private GraphicsEngine graphics;
        private Grid _grid;
        private GameWindow form; 
        public Grid Grid
        {
            get { return _grid; }
        }

        public GameEngine(GameWindow form)
        {
            this.form = form;
        }

        public void Start(Grid grid = null)
        {
            _grid = grid != null ? grid : new Grid(20, 20);
            InputHandler.OnClick += OnClick;
        }

        private void OnClick(int x, int y, System.Windows.Forms.MouseButtons button)
        {
            if (button == System.Windows.Forms.MouseButtons.Left)
            {
                Point p = GraphicsEngine.TranslateToGrid(x, y);
                _grid.AddObject(new Rail(p.X, p.Y));
            }
        }

        public void StartGraphics(Graphics g)
        {
            graphics = new GraphicsEngine(g);
            graphics.Start(this);
        }

        public void StopGame()
        {
            graphics.Stop();
            if (GlobalVars.CONSOLE)
                TMConsole.Stop();
            if (!form.Stopping)
                form.Close();
            InputHandler.OnClick -= OnClick;
        }
    }
}
