using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransportMayhem.Controller;
using TransportMayhem.Model;
using TransportMayhem.Model.GridObjects;
using TransportMayhem.Model.MovingObjects;
using TransportMayhem.View.GridRenderers;

namespace TransportMayhem.View
{
    /// <summary>
    /// Handles all the rendering of the game
    /// </summary>
    class GraphicsEngine
    {
        private GridObject prevGhostObject = null;
        /// <summary>
        /// The location of the ghost object
        /// </summary>
        private Point prevGhostLocation = new Point(-1, -1);
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
        /// The Background of the game
        /// </summary>
        private Bitmap BackgroundGrass;
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
            drawHandle.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            GRIDSIZE = 50;
            Renderers.Initialize();
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
            borderPen = new Pen(Color.Lime);
            using (Bitmap grass = Properties.Resources.Grass)
            {
                BackgroundGrass = new Bitmap(grass, GRIDSIZE, GRIDSIZE);
            }
        }

        /// <summary>
        /// Unloads all the assets in memory
        /// </summary>
        private void UnloadAssets()
        {
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
            Bitmap movingFrame = new Bitmap(CANVAS_WIDTH, CANVAS_HEIGHT);
            Graphics frameGraphics = Graphics.FromImage(frame); //Extract the graphics object from the Bitmap
            Graphics movingGraphics = Graphics.FromImage(movingFrame);
            frameGraphics.Clear(Color.Red);
            IterateGridObjects(frameGraphics);
            BufferedGraphicsContext buffer = new BufferedGraphicsContext();
            BufferedGraphics bufferedGraphics = buffer.Allocate(drawHandle, new Rectangle(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT));
            while (render)
            {
                UpdateGrids(frameGraphics);
                DrawGhostObject(frameGraphics);
                movingGraphics.Clear(Color.Transparent);
                DrawMovingObjects(movingGraphics);
                bufferedGraphics.Graphics.DrawImageUnscaled(frame, 0, 0);
                bufferedGraphics.Graphics.DrawImageUnscaled(movingFrame, 0, 0);
                bufferedGraphics.Render();
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
            movingFrame.Dispose();
            movingGraphics.Dispose();
            buffer.Dispose();
            UnloadAssets();
            //Sets the renderthread to null, we're done with it
            renderThread = null;
        }

        private void DrawMovingObjects(Graphics g)
        {
            foreach (MovingObject mo in engine.Grid.MovingObjects)
            {
                mo.Renderer.PaintMovingObject(g, mo);
            }
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
            lock (GraphicsEngine._updateGrids)
            {
                
                foreach (Point p in GraphicsEngine._updateGrids)
                    UpdateGrid(g, p);
                GraphicsEngine._updateGrids.Clear();
            }
        }
        /// <summary>
        /// Update a single grid
        /// </summary>
        /// <param name="g">The graphics object ot paint with</param>
        /// <param name="p">The point in the grid to paint</param>
        private void UpdateGrid(Graphics g, Point p)
        {
            if (!engine.Grid.InBounds(p)) return;
            Point drawPoint = TranslateToView(p);
            DrawBackground(g, drawPoint);
            DrawGridObject(g, drawPoint, engine.Grid[p]);
            DrawGridOutline(g, drawPoint);
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
                    UpdateGrid(g, p);
                }
            }
        }

        /// <summary>
        /// Calculates the offset of a float number
        /// </summary>
        /// <param name="f">The float to calculate the offset off. Must be lower then 1</param>
        /// <returns>The amount of pixels the offset is</returns>
        private static int CalculateFloatOffset(float f)
        {
            return (int) (GRIDSIZE * f);
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
        /// Translate a floating number to their pixel location
        /// </summary>
        /// <param name="x">The x location in the grid to translate</param>
        /// <param name="y">The y location in the grid to translate</param>
        /// <returns>A Point object containing the location in the view</returns>
        public static Point TranslateToView(float x, float y)
        {
            int gridX = (int)x, gridY = (int)y;
            x -= gridX; //x and y now only hold the decimal value
            y -= gridY;
            Point p = TranslateToView(new Point(gridX, gridY));
            p.X += CalculateFloatOffset(x);
            p.Y += CalculateFloatOffset(y);
            return p;
        }

        /// <summary>
        /// Draws the background of the grid object
        /// </summary>
        /// <param name="g">The graphics object to use</param>
        /// <param name="p">The point where to add the background</param>
        private void DrawBackground(Graphics g, Point p)
        {
            g.DrawImageUnscaled(BackgroundGrass, p);
        }

        /// <summary>
        /// Draws the outline of the grid
        /// </summary>
        /// <param name="g">The graphics object to paint with</param>
        /// <param name="p">The point to check for a gridobject</param>
        private void DrawGridOutline(Graphics g, Point p)
        {
            g.DrawRectangle(borderPen, p.X, p.Y, GRIDSIZE, GRIDSIZE);
        }
        /// <summary>
        /// Checks if the given point has a GridObject and draws it if present
        /// </summary>
        /// <param name="g">The graphics object to paint with</param>
        /// <param name="p">The object to paint, can be null</param>
        private void DrawGridObject(Graphics g, Point p, GridObject go)
        {
            if (go == null) return;
            go.GridRenderer.RenderGridObjectBackground(g, go, p);
            go.GridRenderer.RenderGridObjectForeground(g, go, p);
        }
        /// <summary>
        /// Resets the ghost object
        /// </summary>
        /// <param name="g">The graphics object to draw with, if available.</param>
        private void ResetGhostObject(Graphics g)
        {
            if (g == null)
                UpdateGrid(prevGhostLocation);
            else
                UpdateGrid(g, prevGhostLocation);
            prevGhostLocation.X = -1;
            prevGhostObject = null;
        }
        /// <summary>
        /// Draws the ghost object on the screen
        /// </summary>
        /// <param name="g">The graphics object to draw with</param>
        private void DrawGhostObject(Graphics g)
        {
            GridObject go = InputHandler.GhostObject;
            if (go == null)
            {
                if (prevGhostObject != null)
                    ResetGhostObject(g);
                return;
            }
            lock (go) //Lock the object so external sources can't update its location. This is necesary to keep other threads from updating it while we register its location and draw
            {
                if (prevGhostLocation != go.Location || prevGhostObject != go)
                {
                    if (prevGhostLocation.X >= 0) //if -1 we don't have to update a ghost object cause it wasn't drawn
                        ResetGhostObject(g);
                    if (engine.Grid.InBounds(go.Location))
                    {
                        Texture map = go.GridRenderer.GetTexture(go);
                        if (map.BackGround == null && map.ForeGround == null) return;
                        ColorMatrix colorMatrix = new ColorMatrix( //Greyscale 50% seethrough
                            new float[][] 
                            {
                                new float[] {.3f, .3f, .3f, 0, 0},
                                new float[] {.59f, .59f, .59f, 0, 0},
                                new float[] {.11f, .11f, .11f, 0, 0},
                                new float[] {0, 0, 0, 0.5f, 0},
                                new float[] {0, 0, 0, 0, 1}
                            });
                        ImageAttributes attributes = new ImageAttributes();
                        attributes.SetColorMatrix(colorMatrix);
                        Rectangle Rect = new Rectangle(TranslateToView(go.Location), new Size(GRIDSIZE, GRIDSIZE));
                        if(map.BackGround != null)
                            g.DrawImage(map.BackGround, Rect, 0, 0, map.BackGround.Width, map.BackGround.Height, GraphicsUnit.Pixel, attributes); //Draw the map ourselves with the attributes defined above
                        if (map.ForeGround != null)
                            g.DrawImage(map.ForeGround, Rect, 0, 0, map.ForeGround.Width, map.ForeGround.Height, GraphicsUnit.Pixel, attributes);
                        prevGhostLocation = go.Location; //SAve the location so we can update it when we're done
                        prevGhostObject = go;
                    }
                } 
            }
        }
    }

    
}
