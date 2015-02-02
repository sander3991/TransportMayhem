using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.View;
using TransportMayhem.View.GridRenderers;

namespace TransportMayhem.Model.GridObjects
{
    /// <summary>
    /// A station object where trains load and unload their passengers/cargo
    /// </summary>
    class Station : GridObject, IRail
    {
        /// <summary>
        /// The default renderer for a station object
        /// </summary>
        public override IGridRenderer GridRenderer { get { return Renderers.StationRenderer; } }
        /// <summary>
        /// The direction this track is facing
        /// </summary>
        public RailDirections RailDirection { get { return Rotation == Rotation.Top || Rotation == Rotation.Bottom ? RailDirections.Vertical : RailDirections.Horizontal; } }
        /// <summary>
        /// Creates a station object
        /// </summary>
        /// <param name="x">The x position of this object</param>
        /// <param name="y">The y position of this object</param>
        /// <param name="width">The width of the object</param>
        /// <param name="height">The height of the object</param>
        public Station(int x, int y, int width, int height, Rotation rotation = Rotation.Left)
            : base(x, y, width, height, rotation)
        {

        }
    }
}
