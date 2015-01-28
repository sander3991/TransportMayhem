using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model;

namespace TransportMayhem.View.GridRenderers
{
    class GridStationRenderer : IGridRenderer
    {
        private Bitmap _stationHorizontal, _stationVertical;
        public GridStationRenderer()
        {
            Bitmap horizontal = Properties.Resources.Station_Hori;
            _stationHorizontal = new Bitmap(horizontal, new Size(GlobalVars.GRIDSIZE, GlobalVars.GRIDSIZE));
            horizontal.Dispose();
            Bitmap vertical = Properties.Resources.Station_Vert;
            _stationVertical = new Bitmap(vertical, new Size(GlobalVars.GRIDSIZE, GlobalVars.GRIDSIZE));
            vertical.Dispose();
        }
        public void RenderGridObject(Graphics g, GridObject go, Point p)
        {
            Station station = go as Station;
            if (station == null) return;
            //TODO: Add logic to define vertical/horizontal
            g.DrawImageUnscaled(_stationHorizontal, GraphicsEngine.TranslateToView(p));
        }
    }
}
