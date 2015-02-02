using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportMayhem.Model.GridObjects
{
    class CurvedRail : Rail
    {
        public CurvedRail(int x, int y, Rotation rotation = Rotation.Top) : base(x, y, rotation, TranslateRotationToRailDirection(rotation))
        {

        }

        public static RailDirections TranslateRotationToRailDirection(Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation.Top:
                    goto default;
                case Rotation.Right:
                    return RailDirections.BottomToRight;
                case Rotation.Bottom:
                    return RailDirections.BottomToLeft;
                case Rotation.Left:
                    return RailDirections.TopToLeft;
                default:
                    return RailDirections.TopToRight;
            }
        }
    }
}
