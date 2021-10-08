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
            Logger.LogCopyright();
            DebugTests();
            RunProperties properties = ArgumentHandler.ResolveArguments(args);
            FileFormat fmt = FormatHandler.ResolveFormat(properties.Format, properties.FileName);
            fmt.ExportData();
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

        /// <summary>
        /// Runs diagnostic debugging tests.
        /// </summary>
        private static void DebugTests()
        {
            // Test BigEndian Converters
            Logger.LogDebug("BE: 0x1122");
            Logger.LogDebug($"LE: 0x{Convert.ToString(BigEndian.ToLeShort(0x1122), 16).ToUpper()}");
            Logger.LogDebug("BE: 0x11223344");
            Logger.LogDebug($"LE: 0x{Convert.ToString(BigEndian.ToLeInt(0x11223344), 16).ToUpper()}");
            Logger.LogDebug("BE: 0x1122334455667788");
            Logger.LogDebug($"LE: 0x{Convert.ToString(BigEndian.ToLeLong(0x1122334455667788), 16).ToUpper()}");
        }
    }
}
