using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model;

namespace TransportMayhem.View
{
    static class Renderers
    {
        public static readonly GridDefaultRenderer DefaultRenderer = new GridDefaultRenderer();
        public static readonly GridRailRenderer RailRenderer = new GridRailRenderer();
    }
}
