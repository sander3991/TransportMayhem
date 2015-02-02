using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportMayhem.Model
{
    /// <summary>
    /// A game object is the blueprint for any object in the game
    /// </summary>
    abstract class GameObject
    {
        /// <summary>
        /// The location of the GameObject in the grid
        /// </summary>
        public abstract Point Location { get; protected set; }
    }
}
