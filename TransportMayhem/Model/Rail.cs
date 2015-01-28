using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.View;

namespace TransportMayhem.Model
{
    /// <summary>
    /// A Rail object that trains drive on
    /// </summary>
    class Rail : GridObject
    {
        /// <summary>
        /// The rail object defaults to the RailRenderer
        /// </summary>
        protected override IGridRenderer _defaultGridRenderer { get { return Renderers.RailRenderer; } }
        /// <summary>
        /// Creates a rail object at the specified location
        /// </summary>
        /// <param name="x">The x location of the rail</param>
        /// <param name="y">The y location of the rail</param>
        public Rail(int x, int y) : base(x, y, 1, 1)
        {

        }
    }
}
