using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransportMayhem.Controller
{
    /// <summary>
    /// Handles all the input in the game
    /// </summary>
    static class InputHandler
    {
        /// <summary>
        /// The panel which we're listening to for input
        /// </summary>
        private static Panel panel;
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
            panel.MouseClick += (object sender, MouseEventArgs e) => { if (OnClick != null) OnClick(e.X, e.Y, e.Button); }; //Send the event
        }
    }
}
