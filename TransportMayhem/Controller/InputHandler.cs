using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TransportMayhem.Model;
using TransportMayhem.Model.GridObjects;
using TransportMayhem.View;

namespace TransportMayhem.Controller
{
    /// <summary>
    /// Handles all the input in the game
    /// </summary>
    static class InputHandler
    {
        /// <summary>
        /// The selected rotation
        /// </summary>
        private static Rotation _rotation = Rotation.Top;
        /// <summary>
        /// Keeps track of the currently selected rotation for objects
        /// </summary>
        public static Rotation SelectedRotation { get { return _rotation; } }
        /// <summary>
        /// Saves the current type
        /// </summary>
        private static Type _inputType;
        /// <summary>
        /// What type is selected
        /// </summary>
        public static Type InputType {
            get { return _inputType; }
            set { 
                _inputType = value; 
                _ghostObject = value == null ? null : GridObject.InstantiateObject(value, 0, 0, 1, 1, _rotation);
            }
        }
        /// <summary>
        /// The panel which we're listening to for input
        /// </summary>
        private static Panel panel;
        /// <summary>
        /// The private field of the ghostobject to notify the user where (s)he's placing an item and where
        /// </summary>
        private static GridObject _ghostObject;
        /// <summary>
        /// Bool to define wether the ghostobject should be shown
        /// </summary>
        private static bool showGhostObject = true;
        /// <summary>
        /// The ghostobject to notify the user where (s)he's placing an item and where
        /// </summary>
        public static GridObject GhostObject { get { return showGhostObject ? _ghostObject : null; } }
        /// <summary>
        /// When a mouse click is registered on the panel, this event is fired
        /// </summary>
        public static Action<int, int, MouseButtons> OnClick;
        /// <summary>
        /// Start the inputhandler on given panel
        /// </summary>
        /// <param name="panel"></param>
        public static void Start(Panel panel)
        {
            InputHandler.panel = panel;
            SetupListeners();
        }
        /// <summary>
        /// Setup default listeners
        /// </summary>
        static void SetupListeners()
        {
            panel.MouseClick += panel_MouseClick;
            panel.MouseMove += panel_MouseMove;
            panel.MouseLeave += panel_MouseLeave;
            panel.MouseEnter += panel_MouseEnter;
        }


        static void panel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _ghostObject != null)
            {
                _rotation = RailUtils.Rotate(_rotation);
                _ghostObject = _inputType == null ? null : GridObject.InstantiateObject(_inputType, 0, 0, 1, 1, _rotation);
            }
            if (OnClick != null) OnClick(e.X, e.Y, e.Button);
        }

        static void panel_MouseEnter(object sender, EventArgs e)
        {
            showGhostObject = true;
        }

        static void panel_MouseLeave(object sender, EventArgs e)
        {
            showGhostObject = false;
        }

        static void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_ghostObject == null) return;
            lock (_ghostObject)
            {
                _ghostObject.X = GraphicsEngine.TranslateToGrid(e.X);
                _ghostObject.Y = GraphicsEngine.TranslateToGrid(e.Y);
            }
        }
    }
}
