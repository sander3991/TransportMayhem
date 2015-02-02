using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransportMayhem.Controller;
using TransportMayhem.Model;
using TransportMayhem.Model.GridObjects;
using TransportMayhem.View;

namespace TransportMayhem
{

    /// <summary>
    /// The Console of the game, only enabled when '-console' is added to the command line parameters
    /// </summary>
    internal static class TMConsole
    {
        internal struct ConsoleCommand
        {
            internal ConsoleDebugDel Method;
            internal string Description;
        }
        /// <summary>
        /// Colors that are used to identify types of messages
        /// </summary>
        private const ConsoleColor COLOR_ERROR = ConsoleColor.Red, COLOR_WARNING = ConsoleColor.Yellow, COLOR_DEFAULT = ConsoleColor.Green;

        /// <summary>
        /// Delegate for a console method
        /// </summary>
        /// <param name="args">the arguments given by the user in the form of an array of strings</param>
        internal delegate void ConsoleDebugDel(string[] args);
        /// <summary>
        /// The dictionary that contains all the commands available to the console
        /// </summary>
        internal static Dictionary<string, ConsoleCommand> commands;
        /// <summary>
        /// Returns true if the console is running, otherwise false.
        /// </summary>
        internal static bool IsStarted { get { return ConsoleThread != null; } }
        /// <summary>
        /// The thread that the console is running on
        /// </summary>
        private static Thread ConsoleThread;
        /// <summary>
        /// The engine that the game is running on
        /// </summary>
        private static GameEngine engine;
        /// <summary>
        /// The title in the form of ASCII art, each array key is a line.
        /// </summary>
        private static string[] title;
        /// <summary>
        /// Keeps track of the FPS counter in the top of the screen
        /// </summary>
        private static int _fps;
        /// <summary>
        /// Sets the FPS counter of the console
        /// </summary>
        internal static int FPS { 
            set
            {
                if (IsStarted)
                {
                    _fps = value;
                    UpdateHeader();
                }
            }
        }

        /// <summary>
        /// Has someone requested to stop the console
        /// </summary>
        private static bool _stopRequested = false;
        /// <summary>
        /// Checks if we are currently printing clicks in the console
        /// </summary>
        private static bool printingClicks = false;

        /// <summary>
        /// Use the static constructor to register internal commands. You can also use the RegisterCommand method from other classes.
        /// </summary>
        static TMConsole()
        {
            commands = new Dictionary<string, ConsoleCommand>();

            RegisterCommand("help", Help);
            RegisterCommand("stopconsole", Stop, "Stops the console from showing. It will not affect anything else.");
            RegisterCommand("showclicks", ShowClicks, "Print out every time a OnClick event is fired");
            RegisterCommand("clear", Clear, "Clears the console from all lines");
            title = new string[]
            {
            " _______                                   _     __  __             _ ",
            "|__   __|                                 | |   |  \\/  |           | |",
            "   | |_ __ __ _ _ __  ___ _ __   ___  _ __| |_  | \\  / | __ _ _   _| |__   ___ _ __ ___  ",
            "   | | '__/ _` | '_ \\/ __| '_ \\ / _ \\| '__| __| | |\\/| |/ _` | | | | '_ \\ / _ \\ '_ ` _ \\ ",
            "   | | | | (_| | | | \\__ \\ |_) | (_) | |  | |_  | |  | | (_| | |_| | | | |  __/ | | | | |",
            "   |_|_|  \\__,_|_| |_|___/ .__/ \\___/|_|   \\__| |_|  |_|\\__,_|\\__, |_| |_|\\___|_| |_| |_|",
            "                         | |                                   __/ |                   ",
            "                         |_|                                  |___/",
            };
        }
        internal static void LogError(string s, object o = null)
        {
            Log(s, o, COLOR_ERROR);
        }
        internal static void LogError(object o)
        {
            LogError(o.ToString());
        }

        internal static void LogWarning(string s, object o = null)
        {
            Log(s, o, COLOR_WARNING);
        }

        internal static void LogWarning(object o)
        {
            LogWarning(o.ToString());
        }

        internal static void Log(string s, object o = null, ConsoleColor color = COLOR_DEFAULT)
        {
            if (!IsStarted) return;
            Console.ForegroundColor = color;
            Console.WriteLine(s);
            if (o != null)
            {
                Console.WriteLine("Associated object: {0}", o);
            }
            Console.ForegroundColor = COLOR_DEFAULT;
        }

        private static void PrintClicks(int x, int y, System.Windows.Forms.MouseButtons button)
        {
            System.Drawing.Point p = GraphicsEngine.TranslateToGrid(x,y);
            Log(String.Format("Click event: X: {0}, Y: {1}, Button: {2}, Grid: {3}/{4}", x, y, button, p.X, p.Y));
            if (engine != null && engine.Grid != null)
            {
                GridObject go = engine.Grid[p];
                if (go != null)
                    Log("Clicked on object:", go);
            }
        }

        private static void ShowClicks(string[] args)
        {
            bool value;
            if (args.Length == 0 || (args[0] != "true" && args[0] != "false")) value = !printingClicks;
            else value = Convert.ToBoolean(args[0]);
            if (value == printingClicks) return;
            if (value)
            {
                InputHandler.OnClick += PrintClicks;
                Log("Listening for mouse clicks...");
            }
            else
            {
                InputHandler.OnClick -= PrintClicks;
                Log("No longer listening for mouse clicks.");
            }
            printingClicks = value;
        }
        /// <summary>
        /// Start the console if it hasn't already started
        /// </summary>
        /// <param name="engine">The GameEngine that the game is running on</param>
        internal static void Start(GameEngine engine)
        {
            if (IsStarted) return; //We don't want to make another thread, nor are we able to Allocate another Console to this process.
            TMConsole.engine = engine;
            AllocConsole();
            if (IsStarted) return;
            _stopRequested = false;
            ConsoleThread = new Thread(new ThreadStart(Loop));
            ConsoleThread.Name = "Console Listener Thread";
            int largestString = title[0].Length;
            for (int i = 1; i < title.Length; i++)
                largestString = Math.Max(largestString, title[i].Length);
            largestString++;
            Console.Title = String.Format("{0} Console", Properties.Resources.TITLE);
            Console.WindowWidth = Math.Min(largestString, Console.LargestWindowWidth);
            Console.ForegroundColor = COLOR_DEFAULT;
            ConsoleThread.Start();
        }

        /// <summary>
        /// Stop the console if it is running
        /// </summary>
        /// <param name="args">The arguments given by the console when called from the console. Is never used</param>
        internal static void Stop(string[] args = null)
        {
            if (!IsStarted) return;
            _stopRequested = true;
            FreeConsole();
        }

        /// <summary>
        /// The main loop that listens to input on the console
        /// </summary>
        private static void Loop()
        {
            PrintTitle();
            UpdateHeader(); //set the header before we start looping
            while (!_stopRequested) //start the loop
            {
                try
                {
                    var input = Console.ReadLine(); //We wait for input
                    string[] inputStrings = input.Split(' '); //We split the input in an array seperated by spaces
                    if (inputStrings.Length == 0) return; //We check if the array has atleast 1 string in it (Otherwise we don't have input and can't do anything!)
                    if (commands.ContainsKey(inputStrings[0])) //Check if the first string, which should be the command, is known in our Dictionary.
                    {
                        string[] commandParams = new string[inputStrings.Length - 1];
                        for (int i = 0; i < commandParams.Length; i++)
                            commandParams[i] = inputStrings[i + 1];//we copy the remaining values to the array that the command will receive
                        try //Try to prevent the console from crashing due to bugs in the code.
                        {
                            commands[inputStrings[0]].Method(commandParams); //Run the commands Delegate method
                        }
                        catch (Exception e)
                        {
                            LogError("An unhandled exception occured!");
                            LogError(e.Source);
                            LogError(e.StackTrace);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = COLOR_WARNING;
                        LogWarning("Invalid command, use 'help' for functions");
                    }
                    Console.ForegroundColor = COLOR_DEFAULT; //set it back to default for the next loop and user input
                }
                catch (Exception)
                {

                }
            }
            FreeConsole();
            TMConsole.engine = null;
            ConsoleThread = null;
        }

        /// <summary>
        /// Registers a command to the console
        /// </summary>
        /// <param name="command">The command that people need to use in the console</param>
        /// <param name="method">The delegate method that is called when users enter the command. A string[] will be sent as argument, it will contain every word after the command.</param>
        /// <returns>True if successfully registered the command, otherwise fasle</returns>
        public static bool RegisterCommand(string command, ConsoleDebugDel method, string description = null)
        {
            if (commands.ContainsKey(command)) return false;
            commands.Add(command, new ConsoleCommand {Method = method, Description = description });
            return true;
        }

        private static void Clear(string[] args = null)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            PrintTitle();
            UpdateHeader();
        }

        /// <summary>
        /// Help-command for the console, prints out all commands but itself.
        /// </summary>
        /// <param name="args">Unused</param>
        private static void Help(string[] args = null)
        {
            Log("Known commands:");
            foreach (var key in commands.Keys)
            {
                if (key != "help")
                {
                    Console.Write(" - {0}", key);
                    var command = commands[key];
                    if (command.Description != null)
                        Console.Write(": {0}", command.Description);
                    Console.WriteLine();
                }
            }
        }
        /// <summary>
        /// Updates the header with new information.
        /// </summary>
        private static void UpdateHeader()
        {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.SetCursorPosition(0, title.Length);
            Console.WriteLine("FPS: {0,3}", _fps);
            Console.WriteLine(new string('-', Console.WindowWidth - 1));
            Console.SetCursorPosition(left, Math.Max(top, Console.CursorTop));
        }

        private static void PrintTitle()
        {
            foreach (string s in title) //set the title before we start looping
                Console.WriteLine(s);
        }

        /// <summary>
        /// Allocates a new Console, may only be used once per process, unless FreeConsole is called
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        /// <summary>
        /// Free the Console of our grip. Suffering shall await this fool!
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();
    }
}
