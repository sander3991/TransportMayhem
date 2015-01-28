using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model;

namespace TransportMayhem.View
{
    class GridRailRenderer : IGridRenderer
    {
        private Bitmap rail = Properties.Resources.Rail;

        public GridRailRenderer()
        {
            Bitmap map = Properties.Resources.Rail;
            rail = new Bitmap(map, GlobalVars.GRIDSIZE, GlobalVars.GRIDSIZE);
            map.Dispose();
        }
        public void RenderGridObject(System.Drawing.Graphics g, Model.GridObject go, System.Drawing.Point p)
        {
            Rail rail = go as Rail;
            if (rail == null) return; //We have not received a rail object so we can't use this renderer
            p = GraphicsEngine.TranslateToView(p);
            g.DrawImageUnscaled(this.rail, p);
        }
    }
}
