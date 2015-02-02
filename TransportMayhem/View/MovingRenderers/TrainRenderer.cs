using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model;
using TransportMayhem.Model.MovingObjects;

namespace TransportMayhem.View.MovingRenderers
{
    class TrainRenderer : IMovingRenderer
    {
        public static int TrainWidth { get; private set; }
        private Brush tempPen, tempLinePen;
        public TrainRenderer()
        {
            tempPen = new SolidBrush(Color.Pink);
            tempLinePen = new SolidBrush(Color.Lime);
            TrainWidth = (int)(GlobalVars.GRIDSIZE * 0.421875f); //0.421875 is calculated from the Station sprite. The station is 128 pixels wide, and the track is 54 pixels. 54 / 128 =  0.421875
        }
        public void PaintMovingObject(Graphics g, MovingObject mo)
        {
            Train train = mo as Train;
            if(train == null) return;
            Point p = GraphicsEngine.TranslateToView(mo.X, mo.Y);
            Rectangle rect = new Rectangle(p.X - (GlobalVars.GRIDSIZE / 2), p.Y - (TrainWidth / 2),GlobalVars.GRIDSIZE / 2, TrainWidth);

            RotateRectangle(g, rect, train.Rotate, p);
            g.FillRectangle(tempLinePen, p.X - 2, p.Y - 2, 5, 5);
        }

        public void RotateRectangle(Graphics g, Rectangle r, float angle, Point p)
        {
            using (Matrix m = new Matrix())
            {
                m.RotateAt(angle, new PointF(p.X, p.Y));
                g.Transform = m;
                g.FillRectangle(tempPen, r);
                g.ResetTransform();
            }
        }
    }
}
