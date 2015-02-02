using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model;
using TransportMayhem.View.GridRenderers;
using TransportMayhem.View.MovingRenderers;

namespace TransportMayhem.View
{
    struct Texture
    {
        public Bitmap ForeGround;
        public Bitmap BackGround;
        public Texture(Bitmap foreground, Bitmap background)
        {
            ForeGround = foreground;
            BackGround = background;
        }
    }
    /// <summary>
    /// A list with available renderers
    /// </summary>
    static class Renderers
    {
        public static bool IsInitialized { get { return DefaultRenderer != null; } }
        /// <summary>
        /// The default renderer
        /// </summary>
        public static GridDefaultRenderer DefaultRenderer {get; private set;}
        /// <summary>
        /// The track renderer
        /// </summary>
        public static GridRailRenderer RailRenderer {get; private set;}
        /// <summary>
        /// The station renderer
        /// </summary>
        public static GridStationRenderer StationRenderer {get; private set;}
        /// <summary>
        /// The train renderer
        /// </summary>
        public static TrainRenderer TrainRenderer = new TrainRenderer();
        public static void Initialize()
        {
            if (IsInitialized) return;
            DefaultRenderer = new GridDefaultRenderer();
            RailRenderer = new GridRailRenderer();
            StationRenderer = new GridStationRenderer();
            TrainRenderer = new TrainRenderer();
        }
    }

}
