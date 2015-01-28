using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportMayhem.Model
{
    /// <summary>
    /// A station object where trains load and unload their passengers/cargo
    /// </summary>
    class Station : GridObject
    {
        /// <summary>
        /// Creates a station object
        /// </summary>
        /// <param name="x">The x position of this object</param>
        /// <param name="y">The y position of this object</param>
        /// <param name="width">The width of the object</param>
        /// <param name="height">The height of the object</param>
        public Station(int x, int y, int width, int height)
            : base(x, y, width, height)
        {

        }
    }
}
