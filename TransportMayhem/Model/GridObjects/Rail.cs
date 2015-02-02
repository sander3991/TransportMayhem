using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Controller;
using TransportMayhem.View;
using TransportMayhem.View.GridRenderers;

namespace TransportMayhem.Model.GridObjects
{

    /// <summary>
    /// A Rail object that trains drive on
    /// </summary>
    class Rail : GridObject, IClickableObject, IRail
    {

        private RailDirections _railDirection;
        public event Action<Rail> RailUpdated;
        /// <summary>
        /// The rail object defaults to the RailRenderer
        /// </summary>
        public override IGridRenderer GridRenderer { get { return Renderers.RailRenderer; } }

        public RailDirections RailDirection { 
            get { return _railDirection; } 
            protected set { 
                _railDirection = value;
                if(RailUpdated != null)
                    RailUpdated(this);
            } 
        }
        /// <summary>
        /// Creates a rail object at the specified location
        /// </summary>
        /// <param name="x">The x location of the rail</param>
        /// <param name="y">The y location of the rail</param>
        public Rail(int x, int y, Rotation rotation = Rotation.Top) : this(x, y, rotation, RailUtils.RotationToRailDirection(rotation)) { }
        /// <summary>
        /// A constructor that can be used by subclasses if they define their RailDirections
        /// </summary>
        /// <param name="x">The x location of the rail</param>
        /// <param name="y">The y location of the rail</param>
        /// <param name="rotation">The rotation of the object</param>
        /// <param name="railDirections">The Raildirections the rail is set to</param>
        protected Rail(int x, int y, Rotation rotation, RailDirections railDirections): base(x, y, 1, 1)
        {
            RailDirection = railDirections;
        }

        public void OnClick(System.Windows.Forms.MouseButtons button)
        {
            Type railType = typeof(Rail);
            Type inputType = InputHandler.InputType;
            if (inputType == null) return;
            Rotation rotation = InputHandler.SelectedRotation;
            if (inputType == railType || inputType.IsSubclassOf(railType))
            {
                RailDirections prevDirect = _railDirection;
                Rail rail = GridObject.InstantiateObject(inputType, 0, 0, 1, 1, rotation) as Rail;
                if (rail == null) return;
                RailDirection |= rail.RailDirection;
                if(prevDirect != _railDirection)
                    GraphicsEngine.UpdateGrid(Location);
            }
        }

        public override string ToString()
        {
            return String.Format("Rail object. Consisting out of the RailDirections: {0}", RailDirection);
        }
    }
}
