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
    /// The interface to render a grid object
    /// </summary>
    interface IGridRenderer
    {
        /// <summary>
        /// This method is called from the GraphicsEngine to request the renderer to draw this object
        /// </summary>
        /// <param name="g">The graphics object on which to draw</param>
        /// <param name="go">The object to draw</param>
        /// <param name="p">The point where needs to be drawn</param>
        void RenderGridObject(Graphics g, GridObject go, Point p);
    }
}
