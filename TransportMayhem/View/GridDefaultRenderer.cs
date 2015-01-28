using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Model;

namespace TransportMayhem.View
{
    class GridDefaultRenderer : IGridRenderer
    {
        /// <summary>
        /// A Dictionary containing Bitmaps for types of objects
        /// </summary>
        private Dictionary<Type, Bitmap> renderMaps;
        private Bitmap _defaultTexture = Properties.Resources.Missing_Texture;

        public GridDefaultRenderer()
        {
            renderMaps = new Dictionary<Type, Bitmap>();
        }


        public void RegisterTypeRenderer(Type type, Bitmap map, int width = 1, int height = 1)
        {
            if (map.Width * height != map.Height * width) throw new ArgumentException("The supplied bitmap is not a square, or not in the given dimennsions");
            if (renderMaps.ContainsKey(type))
            {
                Bitmap oldMap = renderMaps[type];
                oldMap.Dispose();
                renderMaps.Remove(type);
            }
            if (map.Width != GraphicsEngine.TranslateToView(width))
            {
                Bitmap newBitmap = new Bitmap(map, new Size(GraphicsEngine.TranslateToView(width), GraphicsEngine.TranslateToView(height)));
                map.Dispose(); //Dispose the original, we no longer need it
                map = newBitmap;
            }
            renderMaps.Add(type, map);
        }

        public void RenderGridObject(Graphics g, GridObject go, Point p)
        {
            Type type = go.GetType();
            if (renderMaps.ContainsKey(type))
                g.DrawImageUnscaled(renderMaps[type], GraphicsEngine.TranslateToView(p));
            else
            {
                Point point = new Point();
                for(int x = 0; x < go.Width; x++)
                {
                    point.X = GraphicsEngine.TranslateToView(go.X + x);
                    for(int y = 0; y < go.Height; y++)
                    {
                        point.Y = GraphicsEngine.TranslateToView(go.Y + y);
                        g.DrawImageUnscaled(_defaultTexture, point);

                    }
                }
            }
        }
    }
}
