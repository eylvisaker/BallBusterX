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
            var config = new BBXConfig();

            config.ParseArgs(args);

            using (var game = new BallBusterDesktopGame(config))
            {
                game.Run();
            }
        }
    }
}
