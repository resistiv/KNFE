using KNFE.Format;
using Pastel;
using System;
using System.Drawing;

namespace KNFE.Util
{
    /// <summary>
    /// Handles various logging duties.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Directly logs a string to stdout.
        /// </summary>
        public static void Log(string s)
        {
            Console.WriteLine(s);
            return;
        }

        /// <summary>
        /// Logs an [INF] prefixed string to stdout.
        /// </summary>
        public static void LogInfo(string s)
        {
            s = s.Replace("\n", $"\n{"[...] ".Pastel(Color.Green)}");
            Console.WriteLine($"{"[INF]".Pastel(Color.Green)} {s}");
            return;
        }

        /// <summary>
        /// Logs a [WRN] prefixed string to stdout.
        /// </summary>
        public static void LogWarn(string s)
        {
            s = s.Replace("\n", $"\n{"[...] ".Pastel(Color.Yellow)}");
            Console.WriteLine($"{"[WRN]".Pastel(Color.Yellow)} {s}");
            return;
        }

        /// <summary>
        /// Logs an [ERR] prefixed string to stdout.
        /// </summary>
        public static void LogError(string s)
        {
            s = s.Replace("\n", $"\n{"[...] ".Pastel(Color.Red)}");
            Console.WriteLine($"{"[ERR]".Pastel(Color.Red)} {s}");
            return;
        }

        /// <summary>
        /// Logs an exception to stdout with LogError().
        /// </summary>
        public static void LogException(Exception e)
        {
            string exceptionString = $"Caught exception {e.GetType()}:\n{e.StackTrace}";
            exceptionString = exceptionString.Replace("\n", $"\n{"[...]".Pastel(Color.Red)}");
            LogError(exceptionString);
            return;
        }

        /// <summary>
        /// Logs a [DEB] prefixed string to stdout.
        /// </summary>
        public static void LogDebug(string s)
        {
            s = s.Replace("\n", $"\n{"[...] ".Pastel(Color.MediumPurple)}");
            Console.WriteLine($"{"[DEB]".Pastel(Color.MediumPurple)} {s}");
            return;
        }

        /// <summary>
        /// Logs the copyright string to stdout.
        /// </summary>
        public static void LogCopyright()
        {
            Console.WriteLine($"\n{Globals.ProgramName.Pastel(Color.MediumPurple)} {Globals.Version()}\n{Globals.Copyright} {Globals.AuthorName.Pastel(Color.LightSalmon)}");
            return;
        }

        /// <summary>
        /// Logs the usage statement to stdout.
        /// </summary>
        public static void LogUsage()
        {
            Console.WriteLine($"Usage: {"knfe".Pastel(Color.MediumPurple)} {"-i".Pastel(Color.LightGreen)} {"filename".Pastel(Color.Orange)} {"-t".Pastel(Color.LightGreen)} {"format".Pastel(Color.Orange)}");
        }

        /// <summary>
        /// Logs a verbose help message to stdout.
        /// </summary>
        public static void LogHelp()
        {
            LogUsage();
            Console.WriteLine($"\n{"General Arguments".Pastel(Color.DeepSkyBlue)}:\n" +
                              $"  {"-h".Pastel(Color.LightGreen)} | {"--help".Pastel(Color.LightGreen)}\t\tDisplay a verbose help message (hey, you're here!)\n" +
                              $"  {"-i".Pastel(Color.LightGreen)} | {"--input".Pastel(Color.LightGreen)}\t\tInput file path\n" +
                              $"  {"-t".Pastel(Color.LightGreen)} | {"--type".Pastel(Color.LightGreen)}\t\tInput file type, indicated by a short or long code\n" +
                              $"\n" +
                              $"{"Supported Formats".Pastel(Color.DeepSkyBlue)}:");
            foreach (FormatDescription fd in FormatHandler.Formats)
            {
                Console.WriteLine($"  {fd.FormatShortCode.Pastel(Color.Orange)} | {fd.FormatLongCode.Pastel(Color.Orange)}\t{fd.FormatName}");
            }
        }
    }
}
