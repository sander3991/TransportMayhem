using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model.MovingObjects;

namespace TransportMayhem.View.MovingRenderers
{
    interface IMovingRenderer
    {
        void PaintMovingObject(Graphics g, MovingObject mo);
    }
}
