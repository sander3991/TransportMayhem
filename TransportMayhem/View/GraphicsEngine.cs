using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransportMayhem.Controller;
using TransportMayhem.Model;

namespace TransportMayhem.View
{
    /// <summary>
    /// Handles all the rendering of the game
    /// </summary>
    class GraphicsEngine
    {
        /// <summary>
        /// The graphics object associated with the panel we're drawing in.
        /// </summary>
        private Graphics drawHandle;
        /// <summary>
        /// The thread that we're running the engine in. Is null if it is not running.
        /// </summary>
        private Thread renderThread;
        /// <summary>
        /// The GameEngine the game is running on.
        /// </summary>
        private GameEngine engine;
        /// <summary>
        /// Bitmap Assets that are loaded
        /// </summary>
        private Bitmap defaultTexture;
        /// <summary>
        /// The pen used to draw the borders of the grid
        /// </summary>
        private Pen borderPen;
        /// <summary>
        /// Bool to check if we are currently rendering
        /// </summary>
        private bool render;
        /// <summary>
        /// The size of a square in pixels
        /// </summary>
        private static int GRIDSIZE 
        {
            get { return GlobalVars.GRIDSIZE; }
            set { GlobalVars.GRIDSIZE = value; }
        }
        /// <summary>
        /// What points need to be updated
        /// </summary>
        private static List<Point> _updateGrids = new List<Point>();
        /// <summary>
        /// Is the GraphicsEngine running?
        /// </summary>
        public bool IsRunning { get { return renderThread != null; } }
        /// <summary>
        /// The height and width of the canvas.
        /// </summary>
        public const int CANVAS_HEIGHT = 900, CANVAS_WIDTH = 1200;
        /// <summary>
        /// Creates a new GraphicsEngine object using the specified Graphics object.
        /// </summary>
        /// <param name="g">Graphics object to draw with</param>
        public GraphicsEngine(Graphics g)
        {
            drawHandle = g;
            GRIDSIZE = 100;
        }

        /// <summary>
        /// Starts the GraphicsEngine using the GameEngine
        /// </summary>
        /// <param name="engine">The engine that the game runs on</param>
        public void Start(GameEngine engine)
        {
            if (IsRunning) return;
            this.engine = engine; //assign engine field
            LoadAssets(); //Load all the supplied assets
            render = true; //Make sure that our loop-breaking boolean is set to true so it starts looping
            renderThread = new Thread(new ThreadStart(Render)); //Create the thread
            renderThread.Name = "Rendering Thread"; //Name the thread
            renderThread.Start(); //Start the thread
        }

        /// <summary>
        /// Ask the Rendering Thread to stop its executon.
        /// </summary>
        public void Stop()
        {
            render = false;
        }

        /// <summary>
        /// Load all the assets in memory
        /// </summary>
        private void LoadAssets()
        {
            defaultTexture = Properties.Resources.Missing_Texture;
            borderPen = new Pen(Color.Lime);
        }

        /// <summary>
        /// Unloads all the assets in memory
        /// </summary>
        private void UnloadAssets()
        {
            defaultTexture.Dispose();
            borderPen.Dispose();
        }

        /// <summary>
        /// The main loop to render
        /// </summary>
        private void Render()
        {
            int framesRendered = 0;
            long startTime = Environment.TickCount;

            Bitmap frame = new Bitmap(CANVAS_WIDTH, CANVAS_HEIGHT); //This bitmap is used to draw upon on the loop, doing this allows us to only draw 1 image on the panel
            Graphics frameGraphics = Graphics.FromImage(frame); //Extract the graphics object from the Bitmap
            frameGraphics.Clear(Color.Red);
            IterateGridObjects(frameGraphics);
            while (render)
            {
                UpdateGrids(frameGraphics);
                drawHandle.DrawImageUnscaled(frame, 0, 0);
                framesRendered++;
                if (Environment.TickCount >= startTime + 1000) //Each second we update our frames
                {
                    TMConsole.FPS = framesRendered;
                    startTime = Environment.TickCount;
                    framesRendered = 0; //reset to 0 for the next second
                }
            }
            //Dispose used frames and bitmaps and assets
            frame.Dispose();
            frameGraphics.Dispose();
            UnloadAssets();
            //Sets the renderthread to null, we're done with it
            renderThread = null;
        }



        /// <summary>
        /// Requests for the GraphicsEngine to update the grid
        /// </summary>
        /// <param name="x">The x location of the update</param>
        /// <param name="y">The y location of the update</param>
        public static void UpdateGrid(int x, int y)
        {
            UpdateGrid(new Point(x, y));
        }
        /// <summary>
        /// Requests for the GraphicsEngine to update the grid
        /// </summary>
        /// <param name="p">The location of the update</param>
        public static void UpdateGrid(Point p)
        {
            lock (_updateGrids)
            {
                if (!_updateGrids.Contains(p)) _updateGrids.Add(p);
            }
        }

        /// <summary>
        /// Updates the grids that are marked for an update
        /// </summary>
        /// <param name="g">The graphics object</param>
        private void UpdateGrids(Graphics g)
        {
            Grid grid = engine.Grid;
            lock (GraphicsEngine._updateGrids)
            {
                foreach (Point p in GraphicsEngine._updateGrids)
                {
                    DrawGridOutline(g, p);
                    DrawGridObject(g, p, grid[p]);
                }
                GraphicsEngine._updateGrids.Clear();
            }
        }

        /// <summary>
        /// Used to iterate over all gridobjects of a grid and call the required paint methods of those grids.
        /// </summary>
        /// <param name="g">The graphics to draw on</param>
        private void IterateGridObjects(Graphics g)
        {
            _updateGrids.Clear();
            Grid grid = engine.Grid;
            Point p = new Point();
            for (int x = 0; x < grid.Width; x++)
            {
                p.X = x;
                for (int y = 0; y < grid.Height; y++)
                {
                    p.Y = y;
                    DrawGridOutline(g, p);
                    DrawGridObject(g, p, grid[p]);
                }
            }
        }

        /// <summary>
        /// Translates a grid position towards a view position in pixels.
        /// </summary>
        /// <param name="i">The value to convert</param>
        /// <returns>The value in pixels</returns>
        public static int TranslateToView(int i)
        {
            return i * GRIDSIZE;
        }
        /// <summary>
        /// Translates a grid position towards a view position in pixels.
        /// </summary>
        /// <param name="p">The point to convert</param>
        /// <returns>A new Point struct in pixels</returns>
        public static Point TranslateToView(Point p)
        {
            return new Point(TranslateToView(p.X), TranslateToView(p.Y));
        }
        /// <summary>
        /// Translates a pixel position to its position in the grid
        /// </summary>
        /// <param name="i">The value to convert</param>
        /// <returns>The value in the grid</returns>
        public static int TranslateToGrid(int i)
        {
            return i / GRIDSIZE;
        }
        /// <summary>
        /// Translates a x and y Point to its position in the grid
        /// </summary>
        /// <param name="i">The value to convert</param>
        /// <returns>The point in the grid</returns>
        public static Point TranslateToGrid(int x, int y)
        {
            return new Point(TranslateToGrid(x), TranslateToGrid(y));
        }

        /// <summary>
        /// Translates a pixel point to its position in the grid
        /// </summary>
        /// <param name="i">The point to convert</param>
        /// <returns>The point in the grid</returns>
        public static Point TranslateToGrid(Point p)
        {
            return new Point(TranslateToGrid(p.X), TranslateToGrid(p.Y));
        }

        /// <summary>
        /// Draws the outline of the grid
        /// </summary>
        /// <param name="g">The graphics object to paint with</param>
        /// <param name="p">The point to check for a gridobject</param>
        private void DrawGridOutline(Graphics g, Point p)
        {
            g.DrawRectangle(borderPen, TranslateToView(p.X), TranslateToView(p.Y), GRIDSIZE, GRIDSIZE);
        }
        /// <summary>
        /// Checks if the given point has a GridObject and draws it if present
        /// </summary>
        /// <param name="g">The graphics object to paint with</param>
        /// <param name="p">The object to paint, can be null</param>
        private void DrawGridObject(Graphics g, Point p, GridObject go)
        {
            if (go == null) return;
            go.GridRenderer.RenderGridObject(g, go, p);
        }
    }

    
}
