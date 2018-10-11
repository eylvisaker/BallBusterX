using System;

namespace BallBusterX.Desktop
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new BallBusterDesktopGame())
            {
                game.Config.ParseArgs(args);

                game.Run();
            }
        }
    }
}
