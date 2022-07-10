using System;
using System.IO;
using KNFE.Helper;

namespace KNFE.CLI
{
    /// <summary>
    /// A helper class to handle CLI arguments.
    /// </summary>
    internal static class ArgHandler
    {
        // Internal fields
        internal static string InputFile = "";
        internal static string OutputPath = "";
        internal static FormatDescription Format = null;
        internal static bool VerboseOutput = false;

        /// <summary>
        /// Parses a set of program arguments.
        /// </summary>
        /// <param name="args">A set of arguments passed to the program.</param>
        internal static void ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    #region Display help (-h/--help)
                    case "-h":
                    case "--help":
                        Log.Help();
                        Program.Die();
                        break;
                    #endregion

                    #region Input file (-i/--input)
                    case "-i":
                    case "--input":
                        if (InputFile != "")
                            ArgDie(args[i], "Cannot pass multiple inputs, quitting.");
                        if (i + 1 < args.Length)
                        {
                            if (!File.Exists(args[i + 1]))
                                ArgDie(args[i], $"File '{args[i + 1]}' could not be found, quitting.");
                            InputFile = args[++i];
                        }
                        else
                            ArgDie(args[i]);
                        break;
                    #endregion

                    #region Output path (-o/--output)
                    case "-o":
                    case "--output":
                        if (OutputPath != "")
                            ArgDie(args[i], "Cannot pass multiple inputs, quitting.");
                        if (i + 1 < args.Length)
                        {
                            OutputPath = args[++i];
                        }
                        else
                            ArgDie(args[i]);
                        break;
                    #endregion

                    #region File format (-f/--format)
                    case "-f":
                    case "--format":
                        if (Format != null)
                            ArgDie(args[i], "Cannot pass multiple formats, quitting.");
                        if (i + 1 < args.Length)
                        {
                            FormatDescription temp = FormatHandler.ResolveFromFormatCode(args[++i]);
                            if (temp == null)
                                ArgDie(args[i - 1], $"Format with code '{args[i]}' could not be found, quitting.");
                            Format = temp;
                        }
                        else
                            ArgDie(args[i]);
                        break;
                    #endregion

                    #region Verbose output (-v/--verbose)
                    case "-v":
                    case "--verbose":
                        VerboseOutput = true;
                        break;
                    #endregion

                    default:
                        Program.Die($"Passed invalid argument '{args[i]}', quitting.");
                        break;
                }
            }

            ValidateArgs();
        }

        /// <summary>
        /// Validates passed arguments.
        /// </summary>
        private static void ValidateArgs()
        {
            // Input file
            if (InputFile == "")
                Program.Die("No input file provided, quitting.");
            InputFile = Path.GetFullPath(InputFile);

            // Format
            if (Format == null)
                Program.Die("No format found, quitting.");

            // Output path
            // In case we aren't given an output, try to output to input file's directory
            if (OutputPath == "")
                OutputPath = $"{Path.GetDirectoryName(InputFile)}\\{Path.GetFileName(InputFile)}_out";
            else
                OutputPath = Path.GetFullPath(OutputPath);
            try
            {
                Directory.CreateDirectory(OutputPath);
            }
            catch (Exception e)
            {
                Log.Except(e);
                Program.Die();
            }
        }

        /// <summary>
        /// Exits the program while optionally printing an argument statement.
        /// </summary>
        /// <param name="arg"></param>
        private static void ArgDie(string arg, string reason = null)
        {
            if (reason == null)
                Program.Die($"Passed incomplete argument '{arg}', quitting.");
            else
                Program.Die($"{arg}: {reason}");
        }
    }
}
