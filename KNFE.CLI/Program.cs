using System;
using System.IO;
using KNFE.Core.Format;
using KNFE.Helper;

namespace KNFE.CLI
{
    public class Program
    {
        /// <summary>
        /// Main program entry point.
        /// </summary>
        public static void Main(string[] args)
        {
            Log.Intro();
            ArgHandler.ParseArgs(args);

            // Try to instantiate our FileFormat
            Format format = null;
            try
            {
                format = FormatHandler.InstantiateFormat(ArgHandler.Format, ArgHandler.InputFile);
            }
            catch (Exception e)
            {
                Log.Except(e);
                Die();
            }

            Log.Info($"Input file: {Path.GetFileName(ArgHandler.InputFile)}");
            Log.Info($"File format: {ArgHandler.Format.Name}");

            // Start extracting from root dir
            ExtractHandler.ExtractDirectory(ArgHandler.OutputPath, format.Root, true, ArgHandler.VerboseOutput);

            Console.WriteLine();
            Log.Info("Done.");
        }

        /// <summary>
        /// Exits the program while optionally printing an error.
        /// </summary>
        /// <param name="s">The error to print to stdout.</param>
        public static void Die(string s = null)
        {
            if (s != null)
                Log.Error(s);
            Environment.Exit(-1);
        }
    }
}
