using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.View;
using TransportMayhem.View.GridRenderers;

namespace TransportMayhem.Model.GridObjects
{
    /// <summary>
    /// The blueprint of a GridObject. A GridObject is an object that fits in one or multiple grid squares.
    /// </summary>
    abstract class GridObject : GameObject
    {
        /// <summary>
        /// Keeps rack of the location of the GameObject
        /// </summary>
        private Point _location;
        /// <summary>
        /// Keeps track of the size of the GridObject
        /// </summary>
        private Size _size;
        /// <summary>
        /// The private field to save the rotation
        /// </summary>
        private Rotation _rotation;
        /// <summary>
        /// The height of this GridObject
        /// </summary>
        public int Height { get { return _size.Height; } }
        /// <summary>
        /// The width of this GridObject
        /// </summary>
        public int Width { get { return _size.Width; } }
        /// <summary>
        /// The Left-most X Location of this GridObject in the grid
        /// </summary>
        public int X { get { return _location.X; } internal set { _location.X = value; } }
        /// <summary>
        /// The Top-most Left Y Location of this GridObject in the grid
        /// </summary>
        public int Y { get { return _location.Y; } internal set { _location.Y = value; } }
        /// <summary>
        /// The location of this Grid Object
        /// </summary>
        public override Point Location { get { return _location; } protected set { _location = value; } }
        /// <summary>
        /// The Grid renderer that is used to render this object
        /// </summary>
        public virtual IGridRenderer GridRenderer {
            get { return Renderers.DefaultRenderer; } 
        }
        /// <summary>
        /// The Rotation of the current object
        /// </summary>
        public Rotation Rotation { private set { _rotation = value; } get { return _rotation; } }
        /// <summary>
        /// Creates the skeleton of a GridObject consisting out of a position and optional width and height which both default to 1.
        /// </summary>
        /// <param name="x">The 0-indexed position in the grids x-axis, may not be negative.</param>
        /// <param name="y">The 0-indexed position in the grids y-axis, may not be negative.</param>
        /// <param name="width">The width of the GridObject in amount of squares, must be higher then 1.</param>
        /// <param name="height">The height of the GridObject in amount of squares, must be higher then 1.</param>
        public GridObject(int x, int y, int width = 1, int height = 1, Rotation rotation = Rotation.Top)
        {
            if (width < 1) throw new ArgumentException("A GridObject must be higher or equal to 1 square wide.");
            if (height < 1) throw new ArgumentException("A GridObject must be higher or equal to 1 square high.");
            if (x < 0) throw new ArgumentException("X cannot be negative!");
            if (y < 0) throw new ArgumentException("Y cannot be negative!");
            _location = new Point(x, y);
            _size = new Size(width, height);
            _rotation = rotation;
        }
        /// <summary>
        /// Instantiate an object of the given type
        /// </summary>
        /// <param name="type">The type to instantiate</param>
        /// <param name="x">The x location where this new item needs to be</param>
        /// <param name="y">The y location where this new item needs to be</param>
        /// <param name="width">The width of the item, can be omitted if not needed</param>
        /// <param name="height">The height of the item, can be omitted if not needed</param>
        /// <returns>A GridObject if successfull, otherwise null</returns>
        public static GridObject InstantiateObject(Type type, int x, int y, int width = 1, int height = 1, Rotation rotation = Rotation.Top)
        {
            switch (type.Name)
            {
                case "Rail": return new Rail(x, y, rotation);
                case "Station": return new Station(x,y,width, height, rotation);
                case "CurvedRail": return new CurvedRail(x, y, rotation);
                default:
                    TMConsole.LogError("Failed to instantiate type " + type.FullName + ". It is not know in the method GridObject.InstantiateObject");
                    return null;
            }
        }
    }
}
