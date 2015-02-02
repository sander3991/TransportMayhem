using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model.GridObjects;

namespace TransportMayhem.Model
{
    /* LEFT BOTT RIGH TOP
     * X000 0X00 00X0 000X
     * Each 4bits represent a 'FROM' direction, the order is defined as in Rotation. Rotation.Top = 0, so it's the first set of 4 bits.
     * The X marks a dead spot, a direction can't go to itself.
     * 0010 0001 1000 0100 > Each 4 bits is one direction.
     * ^    ^    ^    ^< The 'top' bit array shows that we're able to move to the second bit, which is bottom
     * ^    ^    ^< The 'right' bit array show that we're able to move to the fourth bit, which is left.
     * ^    ^< The 'bottom' bit array shows that we're able to move to the first bit, which is top.
     * ^< The 'left' bit array shows that we're able to move to the second bit, which is right.
     */
    [Flags]
    enum RailDirections : int
    {
        Vertical = 260,         // 0000 0001 0000 0100
        Horizontal = 8320,      // 0010 0000 1000 0000
        TopToRight = 18,        // 0000 0000 0001 0010
        TopToLeft = 4104,       // 0001 0000 0000 1000
        BottomToRight = 576,    // 0000 0010 0100 0000
        BottomToLeft = 18432,   // 0100 1000 0000 0000
        All = 31710,            // 0111 1011 1101 1110
        //--------------------- if we use an or statement on all of these the result would be
        // 0111 1011 1101 1110  Note the deadspots as described above.
    }

    enum Rotation : byte
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3,
    }
    static class RailUtils
    {
        private static Array RotationValues = Enum.GetValues(typeof(Rotation));
        public static RailDirections RotationToRailDirection(Rotation rotation)
        {
            return rotation == Rotation.Top || rotation == Rotation.Bottom ? RailDirections.Vertical : RailDirections.Horizontal;
        }
        private static List<Rotation> RailDirectionToRotationRaw(RailDirections rd, Rotation from, List<Rotation> list = null)
        {
            if(list == null)
                list = new List<Rotation>();
            int railBits = (int)rd;
            byte value = (byte)(((int)(rd)) >> ((int)from * 4));
            foreach (Rotation rotation in RotationValues)
            {
                byte rotationValue = (byte)(1 << (byte)rotation);
                if ((rotationValue & value) == rotationValue && !list.Contains(rotation))
                    list.Add(rotation);
            }
            return list;
        }
        public static Rotation[] RailDirectionToRotation(RailDirections rd)
        {
            List<Rotation> result = new List<Rotation>();
            foreach (Rotation rot in RotationValues)
                if(result.Count < RotationValues.Length) //we still have more possible rotations to add. If we're on the same length we have all rotations available to us
                    result = RailDirectionToRotationRaw(rd, rot, result);
            return result.ToArray();
        }

        public static Rotation[] RailDirectionToRotation(RailDirections rd, Rotation from)
        {

            return RailDirectionToRotationRaw(rd,from).ToArray();
        }

        public static Rotation Rotate(Rotation rotate, bool clockwise = true)
        {
            switch(rotate)
            {
                case Rotation.Top:
                    return clockwise ? Rotation.Right : Rotation.Left;
                case Rotation.Bottom:
                    return clockwise ? Rotation.Left : Rotation.Right;
                case Rotation.Right:
                    return clockwise ? Rotation.Bottom : Rotation.Top;
                case Rotation.Left:
                    return clockwise ? Rotation.Top : Rotation.Bottom;
                default:
                    TMConsole.LogError("Invalid value received in RailUtils.Rotate", rotate);
                    return Rotation.Top;
            }
        }

        public static bool IsRailDirectionValid(RailDirections rd)
        {
            return (rd & ~RailDirections.All) == 0;
        }

        public static Rotation OppositeRotation(Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation.Top:
                    return Rotation.Bottom;
                case Rotation.Left:
                    return Rotation.Right;
                case Rotation.Right:
                    return Rotation.Left;
                case Rotation.Bottom:
                    return Rotation.Top;
                default:
                    TMConsole.LogError("Invalid value received in RailUtils.OppositeRotation", rotation);
                    return Rotation.Top;
            }
        }

        public static int RotationToAngle(Rotation r)
        {
            switch (r)
            {
                case Rotation.Top:
                    return 270;
                case Rotation.Right:
                    return 0;
                case Rotation.Bottom:
                    return 90;
                case Rotation.Left:
                    return 180;
                default:
                    return 0;
            }
        }

        public static bool IsDirectionCurved(Rotation from, Rotation to)
        {
            return from != OppositeRotation(to);
        }
        /// <summary>
        /// Gets the offset for the rotation. If you want the offset of Rotation.Left with the default amount of 1, the result would be X: -1, Y: 0.
        /// </summary>
        /// <param name="rot">The rotation you would like the access off</param>
        /// <param name="amount">The amount of places the offset must go to</param>
        /// <returns>The X and Y values of how much the offset must be</returns>
        public static Point GetOffsetForRotation(Rotation rot, int amount = 1)
        {
            switch (rot)
            {
                case Rotation.Left:
                    return new Point(-amount, 0);
                case Rotation.Right:
                    return new Point(amount, 0);
                case Rotation.Top:
                    return new Point(0, -amount);
                case Rotation.Bottom:
                    return new Point(0, amount);
                default:
                    return new Point(0, 0);
            }
        }
        /// <summary>
        /// Gets the offset for the rotation. If object A is in X:2 Y:2, and you want the offset of Rotation.Left, the result would be X: 1, Y: 2.
        /// </summary>
        /// <param name="rot">The rotation you would like the access off</param>
        /// <param name="p">The point to apply the offset to</param>
        /// <param name="amount">The amount of places the offset must go to</param>
        /// <returns>The X and Y values of how much the offset is from the given point</returns>
        public static Point GetOffsetForRotation(Rotation rot, Point p, int amount = 1)
        {
            Point offset = GetOffsetForRotation(rot, amount);
            p.X += offset.X;
            p.Y += offset.Y;
            return p;
        }

        public static bool DoRailsAttach(GameObject go1, GameObject go2)
        {
            IRail rail1 = go1 as IRail, rail2 = go2 as IRail;
            if (rail1 == null || rail2 == null) throw new ArgumentException("Gameobjects must implement IRail interface to be compared");
            foreach (Rotation rail1Rot in RailDirectionToRotation(rail1.RailDirection))
                if (new Rectangle(GetOffsetForRotation(rail1Rot, go1.Location), new Size(1,1)).IntersectsWith(go2.Rectangle))
                {
                    Rotation opposite = OppositeRotation(rail1Rot);
                    foreach (Rotation rail2Rot in RailDirectionToRotation(rail2.RailDirection))
                        if (rail2Rot == opposite) return true;
                    break;
                }
            return false;
        }
    }
}
