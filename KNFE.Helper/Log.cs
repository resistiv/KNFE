using System;
using System.Collections.Generic;

namespace KNFE.Helper
{
    /// <summary>
    /// Handles logging and console output. Used exclusively by KNFE.CLI.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Logs the introductory information of KNFE to stdout.
        /// </summary>
        public static void Intro()
        {
            Console.WriteLine($"\n{Globals.ProgramName} v{Globals.Version}\n{Globals.ProgramCopyright} {Globals.ProgramAuthor}");
        }

        /// <summary>
        /// Logs the help information of KNFE to stdout.
        /// </summary>
        public static void Help()
        {
            Console.WriteLine($"Usage: {Globals.ProgramName}.CLI -i <infile> -f <formatcode> [-o <outdir>] [-v]");
            Console.WriteLine($"\nGeneral Arguments:\n" +
                              $"  -h | --help\t\tDisplay a help message (hey, you're here!)\n" +
                              $"  -i | --input\t\tInput file path\n" +
                              $"  -f | --format\t\tInput file format\n\n" +
                              $"  -o | --output\t\tOutput file path\n" +
                              $"  -v | --verbose\t\tEnable verbose output\n" +
                              $"Supported Formats:");
            foreach (FormatDescription fd in FormatHandler.Formats)
            {
                Console.WriteLine($"  {fd.Identifier}\t\t{fd.Name}");
            }
        }

        /// <summary>
        /// Logs a specified string to stdout.
        /// </summary>
        /// <param name="s">The string to be written to stdout.</param>
        public static void Out(string s)
        {
            Console.WriteLine(s);
        }

        /// <summary>
        /// Logs an [INFO]-prefixed string to stdout.
        /// </summary>
        /// <param name="s">The string to be written to stdout.</param>
        public static void Info(string s)
        {
            s = s.Replace("\n", $"\n[....] ");
            Console.WriteLine($"[INFO] {s}");
        }

        /// <summary>
        /// Logs a set of [INFO]-prefixed strings to stdout.
        /// </summary>
        /// <param name="dict">The set of strings to be written to stdout.</param>
        public static void Info(Dictionary<string, string> dict)
        {
            string s = "";
            bool first = true;
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                if (first)
                    first = false;
                else
                    s += "\t";

                s += $"{kvp.Key}: {kvp.Value}\n";
            }
            Info(s);
        }

        /// <summary>
        /// Logs a [WARN]-prefixed string to stdout.
        /// </summary>
        /// <param name="s">The string to be written to stdout.</param>
        public static void Warn(string s)
        {
            s = s.Replace("\n", $"\n[....] ");
            Console.WriteLine($"[WARN] {s}");
        }

        /// <summary>
        /// Logs an [ERROR]-prefixed string to stdout
        /// </summary>
        /// <param name="s">The string to be written to stdout.</param>
        public static void Error(string s)
        {
            s = s.Replace("\n", $"\n[.....] ");
            Console.WriteLine($"[ERROR] {s}");
        }

        /// <summary>
        /// Logs an exception to stdout.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> to be written to stdout.</param>
        public static void Except(Exception e)
        {
            string exceptionString = $"Caught exception {e.GetType()}:\n{e.Message}\n\nStack:\n{e.StackTrace}";
            Error(exceptionString);
        }

        /// <summary>
        /// Logs the current file being extracted to stdout.
        /// </summary>
        /// <param name="s">The name of the file currently being extracted to be written to stdout.</param>
        public static void Extract(string s)
        {
            Console.Write("\r" + "\r".PadLeft(_extractLength + 1));
            string outStr = $"[INFO] Extracting {s}...";
            _extractLength = outStr.Length;
            Console.Write(outStr);
        }

        private static int _extractLength = 0;
    }
}
