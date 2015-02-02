using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Controller;
using TransportMayhem.Model;
using TransportMayhem.Model.GridObjects;

namespace TransportMayhem.View.GridRenderers
{
    /// <summary>
    /// Renders rails in a grid
    /// </summary>
    class GridRailRenderer : IGridRenderer
    {
        private static Color[] debugColors;
        /// <summary>
        /// Dictionary containg all the bitmaps
        /// </summary>
        public Dictionary<RailDirections, Texture> bitmaps;
        /// <summary>
        /// Creates a new object of a GridRailRenderer
        /// </summary>
        public GridRailRenderer()
        {
            bitmaps = new Dictionary<RailDirections, Texture>();
            using (Bitmap rail = Properties.Resources.Rail_Vert_Rails, track = Properties.Resources.Rail_Vert_Wood)
            {
                bitmaps.Add(RailDirections.Vertical, GenerateTexture(rail, track));
                rail.RotateFlip(RotateFlipType.Rotate90FlipNone);
                track.RotateFlip(RotateFlipType.Rotate90FlipNone);
                bitmaps.Add(RailDirections.Horizontal, GenerateTexture(rail, track));
            }
            using (Bitmap corner = Properties.Resources.Rail_BottomLeft_Rails, track = Properties.Resources.Rail_BottomLeft_Wood)
            {
                bitmaps.Add(RailDirections.BottomToLeft, GenerateTexture(corner, track));
                corner.RotateFlip(RotateFlipType.Rotate90FlipNone);
                track.RotateFlip(RotateFlipType.Rotate90FlipNone);
                bitmaps.Add(RailDirections.TopToLeft, GenerateTexture(corner, track));
                corner.RotateFlip(RotateFlipType.Rotate90FlipNone);
                track.RotateFlip(RotateFlipType.Rotate90FlipNone);
                bitmaps.Add(RailDirections.TopToRight, GenerateTexture(corner, track));
                corner.RotateFlip(RotateFlipType.Rotate90FlipNone);
                track.RotateFlip(RotateFlipType.Rotate90FlipNone);
                bitmaps.Add(RailDirections.BottomToRight, GenerateTexture(corner, track));
            }
            debugColors = new Color[]
            {
                Color.Red,
                Color.Lime,
                Color.Blue,
                Color.Yellow,
                Color.Cyan,
                Color.Purple
            };

        }
        /// <summary>
        /// Generates a bitmap scaled to the gridsize
        /// </summary>
        /// <param name="map">The map to rescale, will be disposed when calling this method.</param>
        /// <returns>The resized Bitmap</returns>
        private Texture GenerateTexture(Bitmap foreGround, Bitmap backGround)
        {
            Size size = new Size(GlobalVars.GRIDSIZE, GlobalVars.GRIDSIZE);
            return new Texture(
                new Bitmap(foreGround, size),
                new Bitmap(backGround, size)
            );
        }

        private void GenerateBitmap(RailDirections rd)
        {
            Bitmap foreGround = new Bitmap(GlobalVars.GRIDSIZE, GlobalVars.GRIDSIZE), backGround = new Bitmap(GlobalVars.GRIDSIZE, GlobalVars.GRIDSIZE);
            using (Graphics fore = Graphics.FromImage(foreGround), back = Graphics.FromImage(backGround))
            {
                foreach(RailDirections r in Enum.GetValues(rd.GetType()))
                {
                    if (r != RailDirections.All && (r & rd) == r)
                    {
                        fore.DrawImage(bitmaps[r].ForeGround, 0, 0);
                        back.DrawImage(bitmaps[r].BackGround, 0, 0);
                    }
                }
            }
            bitmaps.Add(rd, new Texture(foreGround, backGround));
        }
        /// <summary>
        /// Renders a Rail object
        /// </summary>
        /// <param name="g">The Graphics object to draw with</param>
        /// <param name="go">The GridObject to draw. Should be a Rail Object</param>
        /// <param name="p">The point to render the grid at</param>
        public void RenderGridObjectBackground(Graphics g, GridObject go, Point p)
        {
            Rail rail = go as Rail;
            if (rail == null) return; //We have not received a rail object so we can't use this renderer
            Texture text = GetTexture(rail);
            g.DrawImageUnscaled(text.BackGround, p);
            g.DrawImageUnscaled(text.ForeGround, p);
        }

        private Texture GetTexture(Rail rail)
        {
            if (!bitmaps.ContainsKey(rail.RailDirection))
                GenerateBitmap(rail.RailDirection);
            return bitmaps[rail.RailDirection];
        }

        public Texture GetTexture(GridObject go)
        {
            Rail rail = go as Rail;

            return rail == null ? new Texture() : GetTexture(rail);
        }

        public void RenderGridObjectForeground(Graphics g, GridObject go, Point p)
        {
            Rail rail = go as Rail;
            if (rail == null) return;
            TrainNetwork network = TrainNetworkRegistry.GetNetworkForRail(rail);
            using (Pen pen = new Pen(debugColors[network.ID % debugColors.Length]))
            {
                pen.Width = 3;
                g.DrawRectangle(pen, p.X, p.Y, GlobalVars.GRIDSIZE, GlobalVars.GRIDSIZE);
            }
            return; //no foreground rendering needed
        }
    }
}
