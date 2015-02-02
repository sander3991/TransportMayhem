using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.View;
using TransportMayhem.Model;
using TransportMayhem.Model.GridObjects;
using TransportMayhem.Model.MovingObjects;

namespace TransportMayhem.Controller
{
    class TrainNetwork
    {
        private int _networkId;
        private List<IRail> _rails;
        public int ID { get { return _networkId; } }
        public TrainNetwork()
        {
            _rails = new List<IRail>();
            _networkId = TrainNetworkRegistry.RegisterToNetwork(this);
        }


        public void Add(IRail rail)
        {
            _rails.Add(rail);
        }

        public bool HasRail(IRail rail)
        {
            return _rails.Contains(rail);
        }
        /// <summary>
        /// Adds the second parameter to the first
        /// </summary>
        /// <param name="keep">The TrainNetwork to keep</param>
        /// <param name="add">The TrainNetwork to add</param>
        /// <returns>The keep object containing the add network</returns>
        public static TrainNetwork operator +(TrainNetwork keep, TrainNetwork add)
        {
            foreach (IRail rail in add._rails)
                if (!keep._rails.Contains(rail))
                    keep._rails.Add(rail);
            add._rails.Clear();
            return keep;
        }
    }
}
