using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.View.GridRenderers;

namespace TransportMayhem.Model
{
    /// <summary>
    /// The blueprint of a GridObject. A GridObject is an object that fits in one or multiple grid squares.
    /// </summary>
    abstract class GridObject
    {
        /// <summary>
        /// Keeps rack of the X location of the GameObject
        /// </summary>
        private int _x;
        /// <summary>
        /// Keeps rack of the Y location of the GameObject
        /// </summary>
        private int _y;
        /// <summary>
        /// Keeps track of the width of the GridObject
        /// </summary>
        private int _width;
        /// <summary>
        /// Keeps track of the height of the GridObject
        /// </summary>
        private int _height;
        /// <summary>
        /// The height of this GridObject
        /// </summary>
        public int Height { get { return _height; } }
        /// <summary>
        /// The width of this GridObject
        /// </summary>
        public int Width { get { return _width; } }
        /// <summary>
        /// The Left-most X Location of this GridObject in the grid
        /// </summary>
        public int X { get { return _x; } }
        /// <summary>
        /// The Top-most Left Y Location of this GridObject in the grid
        /// </summary>
        public int Y { get { return _y; } }

        /// <summary>
        /// The Grid renderer that is used to render this object
        /// </summary>
        public virtual IGridRenderer GridRenderer {
            get { return Renderers.DefaultRenderer; } 
        }
        /// <summary>
        /// Creates the skeleton of a GridObject consisting out of a position and optional width and height which both default to 1.
        /// </summary>
        /// <param name="x">The 0-indexed position in the grids x-axis, may not be negative.</param>
        /// <param name="y">The 0-indexed position in the grids y-axis, may not be negative.</param>
        /// <param name="width">The width of the GridObject in amount of squares, must be higher then 1.</param>
        /// <param name="height">The height of the GridObject in amount of squares, must be higher then 1.</param>
        public GridObject(int x, int y, int width = 1, int height = 1)
        {
            if (width < 1) throw new ArgumentException("A GridObject must be higher or equal to 1 square wide.");
            if (height < 1) throw new ArgumentException("A GridObject must be higher or equal to 1 square high.");
            if (x < 0) throw new ArgumentException("X cannot be negative!");
            if (y < 0) throw new ArgumentException("Y cannot be negative!");
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }
    }
}
