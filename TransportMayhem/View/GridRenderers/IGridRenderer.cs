using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model.GridObjects;

namespace TransportMayhem.View.GridRenderers
{
    /// <summary>
    /// The interface to render a grid object
    /// </summary>
    interface IGridRenderer
    {
        /// <summary>
        /// This method is called from the GraphicsEngine to request the renderer to draw this object. The background is drawn below moving objects
        /// </summary>
        /// <param name="g">The graphics object on which to draw</param>
        /// <param name="go">The object to draw</param>
        /// <param name="p">The point where needs to be drawn</param>
        void RenderGridObjectBackground(Graphics g, GridObject go, Point p);
        /// <summary>
        /// This method is called from the GraphicsEngine to request the renderer to draw this object. The foreground is drawn above moving objects.
        /// </summary>
        /// <param name="g">The graphics object on which to draw</param>
        /// <param name="go">The object to draw</param>
        /// <param name="p">The point where needs to be drawn</param>
        void RenderGridObjectForeground(Graphics g, GridObject go, Point p);
        /// <summary>
        /// This method is called to get a bitmap from the interface that should be drawn by RenderGridObject. Used for post-processing the result of the grid renderer
        /// </summary>
        /// <param name="go">The object that requires drawing</param>
        Texture GetTexture(GridObject go);
    }
}
