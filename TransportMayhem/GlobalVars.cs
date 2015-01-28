using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportMayhem
{
    /// <summary>
    /// Stores global variables that are used throughout the game.
    /// </summary>
    static class GlobalVars
    {
        /// <summary>
        /// Is the Console registerd as enabled
        /// </summary>
        public static bool CONSOLE { get; internal set; }
        public static int GRIDSIZE { get; internal set; }
    }
}
