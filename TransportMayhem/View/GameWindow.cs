using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TransportMayhem.Controller;

namespace TransportMayhem
{
    public partial class GameWindow : Form
    {
        bool _stopping = false;
        public bool Stopping { get { return _stopping; } }
        /// <summary>
        /// The GameEngine that runs the entire game
        /// </summary>
        private GameEngine game;
        /// <summary>
        /// Initialize the components and the GameEngine
        /// </summary>
        public GameWindow()
        {
            InitializeComponent();
            game = new GameEngine(this);
        }
        /// <summary>
        /// When the paint method is called, we use the graphics of our canvas object to do the rendering in a seperate thread.
        /// </summary>
        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            game.StartGraphics(canvas.CreateGraphics());
        }
        /// <summary>
        /// If the console is enabled, we load that aswell when the form is loaded
        /// </summary>
        private void TransportMayhemForm_Load(object sender, EventArgs e)
        {
            game.Start();
            InputHandler.Start(canvas);
            if (GlobalVars.CONSOLE)
                TMConsole.Start(game);
        }
        /// <summary>
        /// If the game is closed we make sure that the engine also stops doing what its doing.
        /// </summary>
        private void TransportMayhemForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _stopping = true;
            game.StopGame();
        }
    }
}
