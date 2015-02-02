using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportMayhem.Model
{
    interface IUpdateable
    {
        /// <summary>
        /// Called each tick in the game. A tick is 1/60th of a second
        /// </summary>
        void QuickUpdate();
        /// <summary>
        /// Called 10 times per second. For each 6th tick the SlowUpdate is called after QuickUpdate.
        /// </summary>
        void SlowUpdate();
    }
}
