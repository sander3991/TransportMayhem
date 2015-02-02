using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.View.MovingRenderers;

namespace TransportMayhem.Model.MovingObjects
{
    abstract class MovingObject : GameObject
    {
        /// <summary>
        /// The X and Y location of this moving object.
        /// </summary>
        private float _x, _y;
        /// <summary>
        /// The X location of the moving object.
        /// </summary>
        public float X { get { return _x; } protected set { _x = value; } }
        /// <summary>
        /// The Y location of the moving object
        /// </summary>
        public float Y { get { return _y; } protected set { _y = value; } }
        /// <summary>
        /// The grid location of this object
        /// </summary>
        public int GridX { get { return X >= 0 ? (int)X : (int)X - 1; } protected set { _x = (int)value; } }
        /// <summary>
        /// The grid location of this object
        /// </summary>
        public int GridY { get { return (int)Y >= 0 ? (int)Y : (int)Y - 1; } protected set { _y = (int)value; } }
        /// <summary>
        /// The location of this object in the grid
        /// </summary>
        public override Point Location { get { return new Point(GridX, GridY); } protected set { GridX = value.X; GridY = value.Y; } }
        /// <summary>
        /// The exact location of this object
        /// </summary>
        public PointF LocationF { get { return new PointF(X, Y); } protected set { _x = value.X; _y = value.Y; } }

        /// <summary>
        /// The renderer used by this object
        /// </summary>
        public abstract IMovingRenderer Renderer { get; }
        /// <summary>
        /// Creates a moving object
        /// </summary>
        /// <param name="x">The X location of the object</param>
        /// <param name="y">The Y location of the object</param>
        internal MovingObject(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
