using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.View;

namespace TransportMayhem.Model
{
    /// <summary>
    /// The grid in which the GameObjects are assigned a location
    /// </summary>
    class Grid
    {
        /// <summary>
        /// Keeps track of all the gridobjects in the field
        /// </summary>
        private GridObject[,] _gridObjects;
        /// <summary>
        /// The width of the grid in amount of squares
        /// </summary>
        private int _width;
        /// <summary>
        /// The height of the grid in amount of squares
        /// </summary>
        private int _height;
        /// <summary>
        /// The Width of the grid in amount of squares
        /// </summary>
        public int Width
        {
            get { return _width; }
        }
        /// <summary>
        /// The height of the grid in amount of squares
        /// </summary>
        public int Height
        {
            get { return _height; }
        }
        /// <summary>
        /// Create a new grid
        /// </summary>
        /// <param name="width">The width in amount of squares</param>
        /// <param name="height">The height in amount of squares</param>
        public Grid(int width, int height)
        {
            if (width < 1) throw new ArgumentException("Width cannot be lower then 1 wide");
            if (height < 1) throw new ArgumentException("Height cannot be lower then 1 high");
            _gridObjects = new GridObject[width, height];
            _width = width;
            _height = height;
            AddObject(new Rail(1, 1));
            AddObject(new Station(2, 1, 5, 3));
        }

        /// <summary>
        /// Checks if an object is not overlapping another object
        /// </summary>
        /// <param name="location">The location of the object</param>
        /// <param name="width">An optional width of the object. Defaults to 1</param>
        /// <param name="height">Ann optional height of the object. Defaults to 1</param>
        /// <returns>True if an object can be placed given these parameters, otherwise false</returns>
        public bool CanObjectBeAdded(Point location, int width = 1, int height = 1)
        {
            if (width < 1 || height < 1) throw new ArgumentException("Invalid width or height given to CanObjectBeAdded method. Integers must be 1 or higher");
            for(int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (_gridObjects[location.X + x, location.Y + y] != null) return false;
            return true;
        }

        /// <summary>
        /// Attempt to add an object to the grid
        /// </summary>
        /// <param name="go">The object to add to the grid</param>
        /// <returns>True if successfully added, otherwise false</returns>
        public bool AddObject(GridObject go)
        {
            Point p = new Point(go.X, go.Y);
            if (!CanObjectBeAdded(p, go.Width, go.Height)) return false;
            for (int x = 0; x < go.Width; x++)
                for (int y = 0; y < go.Height; y++)
                {
                    int newX = go.X + x, newY = go.Y + y;
                    _gridObjects[newX, newY] = go;
                    GraphicsEngine.UpdateGrid(newX, newY);
                }
            return true;
        }

        /// <summary>
        /// Gets the GridObject at specified point
        /// </summary>
        /// <param name="p">The point where to get the GridObject</param>
        /// <returns>The GridObject at the point, or null when no GridObject exists</returns>
        public GridObject this[System.Drawing.Point p]
        {
            get 
            {
                if (p.X >= _width || p.Y >= _height) return null;
                return _gridObjects[p.X, p.Y];
            }
        }
    }
}
