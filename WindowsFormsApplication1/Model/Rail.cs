using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.View;

namespace TransportMayhem.Model
{
    class Rail : GridObject
    {
        protected override IGridRenderer _defaultGridRenderer { get { return Renderers.RailRenderer; } }
        public Rail(int x, int y) : base(x, y, 1, 1)
        {

        }
    }
}
