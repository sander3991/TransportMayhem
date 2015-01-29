using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TransportMayhem.Model;

namespace TransportMayhem.Controller
{
    /// <summary>
    /// Handles all the input in the game
    /// </summary>
    static class InputHandler
    {
        /// <summary>
        /// Saves the current type
        /// </summary>
        private static Type _inputType;
        /// <summary>
        /// What type is selected
        /// </summary>
        public static Type InputType {
            get { return _inputType; }
            set { _inputType = value; TMConsole.Log("Input Type changed", value); }
        }
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

        public static GridObject GetNewObject(int x, int y, int width = -1, int height = -1)
        {
            if (InputType == null) return null;
            int constructorCount = width == -1 ? 2 : 4;
            Object[] constructorParams = null;
            Type[] types = null;
            if(constructorCount == 2)
            {
                types = new Type[] {typeof(int), typeof(int)};
                constructorParams = new Object[] {x, y};
            }
            else if(constructorCount == 4)
            {
                types = new Type[] {typeof(int), typeof(int), typeof(int), typeof(int)};
                constructorParams = new Object[] {x,y,width,height};
            }
            System.Reflection.ConstructorInfo ci = InputType.GetConstructor(types);
            if (ci == null)
            {
                TMConsole.LogError("Failed to receive constructorinformation for type.", InputType);
                return null;
            } 
            return ci.Invoke(constructorParams) as GridObject;
        }
    }
}
