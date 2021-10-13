using KNFE.Format;
using KNFE.Util;
using System;

namespace KNFE
{
    /// <summary>
    /// Main program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main program entry point.
        /// </summary>
        public static void Main(string[] args)
        {
            // Output copyright information
            Logger.LogCopyright();

            // Parses all our arguments and creates a structure for it
            RunProperties properties = ArgumentHandler.ResolveArguments(args);

            // Creates an instance from the properties we got a second ago, sets up preliminary format-specific information
            FileFormat fmt = FormatHandler.ResolveFormat(properties);

            // Export all data from the above instance; decodes all data from any possible streams and writes it to disk
            fmt.ExportData();

            // Done!
            Logger.LogInfo("Done.");
        }

        /// <summary>
        /// Prints an error and exits the program.
        /// </summary>
        public static void Quit(string s)
        {
            Logger.LogError(s);
            Quit();
        }

        /// <summary>
        /// Exits the program.
        /// </summary>
        public static void Quit()
        {
            Environment.Exit(1);
        }
    }
}
