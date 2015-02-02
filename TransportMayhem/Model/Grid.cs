using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Controller;
using TransportMayhem.Model.GridObjects;
using TransportMayhem.Model.MovingObjects;
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
        /// Private field containing the moving objects
        /// </summary>
        private List<MovingObject> _movingObjects;
        /// <summary>
        /// A list containing all the moving objects in the field
        /// </summary>
        public List<MovingObject> MovingObjects { get { return _movingObjects; } }
        /// <summary>
        /// A list containg updateable objects
        /// </summary>
        private List<IUpdateable> _updateableObjects;
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

        public event Action<GameObject> ObjectAdded;
        public event Action<GameObject> ObjectRemoved;
        public event Action<RailArgs> RailAdded;
        public event Action<RailArgs> RailRemoved;

        /// <summary>
        /// The GameEngine that's running this grid
        /// </summary>
        private GameEngine engine;
        /// <summary>
        /// Create a new grid
        /// </summary>
        /// <param name="width">The width in amount of squares</param>
        /// <param name="height">The height in amount of squares</param>
        public Grid(GameEngine engine, int width, int height)
        {
            if (width < 1) throw new ArgumentException("Width cannot be lower then 1 wide");
            if (height < 1) throw new ArgumentException("Height cannot be lower then 1 high");
            _gridObjects = new GridObject[width, height];
            _movingObjects = new List<MovingObject>();
            _updateableObjects = new List<IUpdateable>();
            _width = width;
            _height = height;
            engine.Tick += OnUpdate;
            this.engine = engine;
        }

        private void OnUpdate()
        {
            bool doSlowUpdate = (engine.TickCounter % 6) == 0;
            foreach (IUpdateable updateable in _updateableObjects)
            {
                updateable.QuickUpdate();
                if (doSlowUpdate)
                    updateable.SlowUpdate();

            }
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
            if (width < 1 || height < 1) return false;
            for(int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (_gridObjects[location.X + x, location.Y + y] != null) return false;
            return true;
        }

        /// <summary>
        /// Do some general registering for all gameobjects
        /// </summary>
        /// <param name="go">The game object that is registered</param>
        private void RegisterObject(GameObject go)
        {
            IUpdateable updateable = go as IUpdateable;
            if (updateable != null)
                if (!_updateableObjects.Contains(updateable))
                    _updateableObjects.Add(updateable);
            IRail rail = go as IRail;
            if (rail != null && RailAdded != null)
                RailAdded(new RailArgs(rail, go));
            if(ObjectAdded != null)
                ObjectAdded(go);
        }

        /// <summary>
        /// Add a moving object to the grid
        /// </summary>
        /// <param name="mo">The moving object to add</param>
        /// <returns>True if an object can be placed, otherwise false</returns>
        public bool AddObject(MovingObject mo)
        {
            if (_movingObjects.Contains(mo)) return false;
            _movingObjects.Add(mo);
            RegisterObject(mo);
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
            RegisterObject(go);
            return true;
        }

        private void UnregisterObject(GameObject go)
        {
            IUpdateable updateable = go as IUpdateable;
            if (updateable != null && _updateableObjects.Contains(updateable))
                _updateableObjects.Remove(updateable);
            IRail rail = go as IRail;
            if (rail != null && RailRemoved != null)
                RailRemoved(new RailArgs(rail, go));
            if (ObjectRemoved != null) ObjectRemoved(go);
        }

        public bool RemoveObject(GridObject go)
        {
            if (go == _gridObjects[go.X, go.Y])
            {
                _gridObjects[go.X, go.Y] = null;
                UnregisterObject(go);
                return true;
            }
            return false;
        }

        public bool RemoveObject(MovingObject mo)
        {
            if (_movingObjects.Contains(mo))
            {
                _movingObjects.Remove(mo);
                UnregisterObject(mo);
            }
            return false;
        }

        public bool InBounds(Point p)
        {
            return p.X >= 0 && p.X < _width && p.Y >= 0 && p.Y < _height;
        }

        /// <summary>
        /// Gets the GridObject at specified point
        /// </summary>
        /// <param name="p">The point where to get the GridObject</param>
        /// <returns>The GridObject at the point, or null when no GridObject exists</returns>
        public GridObject this[Point p]
        {
            get 
            {
                if (!InBounds(p)) return null;
                return _gridObjects[p.X, p.Y];
            }
        }
    }
}
