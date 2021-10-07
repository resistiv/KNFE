using KNFE.Format;

namespace KNFE.Util
{
    /// <summary>
    /// Provides utilities for resolving program arguments for processing.
    /// </summary>
    public static class ArgumentHandler
    {
        /// <summary>
        /// Compiles the program RunProperties from an array of arguments.
        /// </summary>
        public static RunProperties ResolveArguments(string[] args)
        {
            RunProperties p = new RunProperties();

            if (args.Length == 0)
            {
                Logger.LogUsage();
                Program.Quit();
            }

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-h":
                    case "--help":
                        Logger.LogHelp();
                        Program.Quit();
                        break;
                    case "-i":
                    case "--input":
                        if (i + 1 < args.Length)
                            p.FileName = args[++i];
                        else
                            Program.Quit($"Passed invalid number of arguments for '{args[i]}', quitting.");
                        break;
                    case "-t":
                    case "--type":
                        if (i + 1 < args.Length)
                            p.Format = FormatHandler.ResolveFormat(args[++i]);
                        else
                            Program.Quit($"Passed invalid number of arguments for '{args[i]}', quitting.");
                        break;
                    default:
                        Program.Quit($"Passed invalid argument '{args[i]}', quitting.");
                        break;
                }
            }

            ValidateProperties(p);

            return p;
        }

        /// <summary>
        /// Provides basic validation for RunProperties parameters.
        /// </summary>
        private static void ValidateProperties(RunProperties p)
        {
            if (p.FileName.Equals(""))
            {
                Program.Quit($"No input file provided, quitting.");
            }
            if (p.Format == FormatType.Invalid)
            {
                Program.Quit($"Invalid format provided, quitting.");
            }
        }
    }

    /// <summary>
    /// Represents a set of resolved parameters for which to run the program on.
    /// </summary>
    public class RunProperties
    {
        public string FileName = "";
        // Dummy format Type to differentiate between invalid and unknown types
        public FormatType Format = FormatType.None;
    }
}
