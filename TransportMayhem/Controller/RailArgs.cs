using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model;
using TransportMayhem.Model.GridObjects;
using TransportMayhem.Model.MovingObjects;
namespace TransportMayhem.Controller
{
    struct RailArgs
    {

        private Rotation[] _sides;
        public Rotation[] Sides
        {
            get
            {
                if (_sides == null)
                    _sides = RailUtils.RailDirectionToRotation(RailDirections);
                return _sides;
            }
        }
        public RailDirections RailDirections { get { return _rail.RailDirection; } }
        public IRail Rail { get { return _rail; } }
        public GameObject RawObject { get { return _raw; } }
        private IRail _rail;
        private GameObject _raw;
        public RailArgs(GameObject raw)
        {
            if (raw == null) throw new ArgumentException("Arguments 'rail' and 'raw' may not be null!");
            IRail rail = raw as IRail;
            if (rail == null) throw new ArgumentException("Arguments 'rail' and 'raw' may not be null!");
            this._rail = rail;
            this._raw = raw;

            //generate on request
            _sides = null;
        }
    }
}
