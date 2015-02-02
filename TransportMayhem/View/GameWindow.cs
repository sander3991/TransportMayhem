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
            game = new GameEngine(this);
            InitializeComponent();
            Tuple<Type, Bitmap>[] createButtons = new Tuple<Type, Bitmap>[]
            {
                new Tuple<Type, Bitmap>(typeof(TransportMayhem.Model.GridObjects.Rail), Properties.Resources.Straight),
                new Tuple<Type, Bitmap>(typeof(TransportMayhem.Model.GridObjects.CurvedRail), Properties.Resources.Curved),
                new Tuple<Type, Bitmap>(typeof(TransportMayhem.Model.GridObjects.Station), Properties.Resources.Station_Hori),
            };
            for (int i = 0; i < createButtons.Length; i++)
            {
                Button button = CreateSelectButton(createButtons[i], i);
                this.Controls.Add(button);
                button.BringToFront();
            }

        }

        private Button CreateSelectButton(Tuple<Type, Bitmap> data, int id)
        {
            Button button = new Button();

            button.Location = new Point(id * 40, 0);
            button.Name = "button" + id;
            button.Size = new Size(40, 40);
            button.TabIndex = 0;
            button.Image = new Bitmap(data.Item2, new Size(40, 40));
            button.UseVisualStyleBackColor = true;
            button.Tag = data.Item1;
            button.Click += OnClick_Item;
            return button;
        }
        /// <summary>
        /// When the paint method is called, we use the graphics of our canvas object to do the rendering in a seperate thread.
        /// </summary>
        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            if(!game.IsGraphicsRunning)
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

        private void OnClick_Item(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            Type type = button.Tag as Type;
            InputHandler.InputType = type;
        }
    }
}
