using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model;
using TransportMayhem.Model.GridObjects;

namespace TransportMayhem.View.GridRenderers
{
    class GridStationRenderer : IGridRenderer
    {
        private Bitmap _stationHorizontal, _stationVertical;
        public GridStationRenderer()
        {
            using (Bitmap horizontal = Properties.Resources.Station_Hori)
            {
                _stationHorizontal = new Bitmap(horizontal, new Size(GlobalVars.GRIDSIZE, GlobalVars.GRIDSIZE));
                horizontal.RotateFlip(RotateFlipType.Rotate90FlipNone);
                _stationVertical = new Bitmap(horizontal, new Size(GlobalVars.GRIDSIZE, GlobalVars.GRIDSIZE));
            }
        }
        public void RenderGridObjectBackground(Graphics g, GridObject go, Point p)
        {
            Station station = go as Station;
            if (station == null) return;
            g.DrawImageUnscaled(GetBitmap(station), p);
        }

        private Bitmap GetBitmap(Station station)
        {
            return station.Rotation == Rotation.Top || station.Rotation == Rotation.Bottom ? _stationVertical : _stationHorizontal;
        }

        public Texture GetTexture(GridObject go)
        {
            Station station = go as Station;
            return station == null ? new Texture() : new Texture(GetBitmap(station), null);
        }


        public void RenderGridObjectForeground(Graphics g, GridObject go, Point p)
        {
            return; //No foreground rendering needed yet
        }
    }
}
