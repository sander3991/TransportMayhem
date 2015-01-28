using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model;

namespace TransportMayhem.View
{
    /// <summary>
    /// A list with available renderers
    /// </summary>
    static class Renderers
    {
        /// <summary>
        /// The default renderer
        /// </summary>
        public static readonly GridDefaultRenderer DefaultRenderer = new GridDefaultRenderer();
        /// <summary>
        /// The track renderer
        /// </summary>
        public static readonly GridRailRenderer RailRenderer = new GridRailRenderer();
    }
}
