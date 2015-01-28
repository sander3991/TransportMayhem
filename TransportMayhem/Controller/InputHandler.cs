using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransportMayhem.Controller
{
    static class InputHandler
    {
        private static Panel panel;
        public static Action<int, int, MouseButtons> OnClick;
        public static void Start(Panel panel)
        {
            InputHandler.panel = panel;
            SetupListeners();
        }

        static void SetupListeners()
        {
            panel.MouseClick += (object sender, MouseEventArgs e) => { if (OnClick != null) OnClick(e.X, e.Y, e.Button); }; //Send the event
        }
    }
}
