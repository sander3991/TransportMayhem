using System;
using System.Collections.Generic;
using System.Drawing;
using TransportMayhem.Model;
using TransportMayhem.Model.GridObjects;
using TransportMayhem.Model.MovingObjects;

namespace TransportMayhem.Controller
{
    static class TrainNetworkRegistry
    {
        private static List<TrainNetwork> _networks;
        private static Grid _grid;

        public static int RegisterToNetwork(TrainNetwork network)
        {
            if (!_networks.Contains(network))
                _networks.Add(network);
            return _networks.IndexOf(network);
        }

        private static void UnregisterNetwork(TrainNetwork network)
        {
            if (_networks.Contains(network))
                _networks.Remove(network);
        }
        public static void Initialize(GameEngine engine)
        {
            TrainNetworkRegistry._grid = engine.Grid;
            _networks = new List<TrainNetwork>();
            _grid.RailAdded += Grid_RailAdded;
            _grid.RailRemoved += Grid_RailRemoved;
        }

        public static TrainNetwork GetNetworkForRail(IRail rail)
        {
            foreach (TrainNetwork network in _networks)
                if (network.HasRail(rail))
                    return network;
            return null;
        }

        private static void Grid_RailRemoved(RailArgs railArgs)
        {
            Rail rail = railArgs.Rail as Rail;
            if (rail != null) rail.RailUpdated -= rail_RailUpdated;
        }

        private static TrainNetwork CombineNetwork(TrainNetwork a, TrainNetwork b)
        {
            if (a == null || b == null) throw new ArgumentException("Can't combine a TrainNetwork that's null!");
            TrainNetwork newNetwork = a.ID > b.ID ? b + a : a + b;
            if (newNetwork == a)
                UnregisterNetwork(b);
            else
                UnregisterNetwork(a);
            return newNetwork;
        }



        private static void Grid_RailAdded(RailArgs railArgs)
        {
            IRail iRail = railArgs.Rail;
            if (iRail == null) return;
            Rotation[] sides = railArgs.Sides;
            TrainNetwork network = null;
            foreach (Rotation rot in sides)
            {
                GridObject go = _grid[RailUtils.GetOffsetForRotation(rot, railArgs.RawObject.Location)];
                if (go == null || go == iRail) continue;
                IRail rail2 = go as IRail;
                if (rail2 == null) continue;
                TrainNetwork newNetwork = GetNetworkForRail(rail2);
                if (network == null)
                {
                    if (RailUtils.DoRailsAttach(railArgs.RawObject, go))
                    {
                        network = newNetwork;
                        network.Add(iRail);
                    }
                }
                else if(newNetwork != network)
                    network = CombineNetwork(network, newNetwork);
            }
            if (network == null)
            {
                TrainNetwork newNetwork = new TrainNetwork(); //No need to register the object, it does so himself in its constructor
                newNetwork.Add(iRail);
            }
            Rail rail = iRail as Rail;
            if (rail != null) rail.RailUpdated += rail_RailUpdated;
        }

        static void rail_RailUpdated(RailArgs args)
        {
            Rotation[] sides = args.Sides;
            TrainNetwork network = GetNetworkForRail(args.Rail);
            foreach (Rotation side in sides)
            {
                GameObject go = _grid[RailUtils.GetOffsetForRotation(side, args.RawObject.Location)];
                if (go == null) continue;
                IRail rail = go as IRail;
                if (rail == null) continue;
                TrainNetwork compareNetwork = GetNetworkForRail(rail);
                if (network != compareNetwork)
                    CombineNetwork(network, compareNetwork);
            }
        }
        /// <summary>
        /// Gets the directions for the trains current location in the grid
        /// </summary>
        /// <param name="train">The train that you would like to check its directions for</param>
        /// <returns>An array with Rotation values that indicate where the train can go. Empty array if not successfull</returns>
        public static Rotation[] GetDirections(Train train)
        {
            Point location = train.Location;
            GridObject go = _grid[location];
            if (go == null) return new Rotation[0];
            IRail rail = go as IRail;
            if (rail == null) return new Rotation[0];
            return RailUtils.RailDirectionToRotation(rail.RailDirection, RailUtils.OppositeRotation(train.Direction));
        }
    }
}
