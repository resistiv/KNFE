using System;
using System.Collections.Generic;
using System.IO;

namespace KNFE.Helper
{
    /// <summary>
    /// Provides utilities for resolving file formats.
    /// </summary>
    public static class FormatHandler
    {
        /// <summary>
        /// A list of all available, resolvable file formats.
        /// </summary>
        public static FormatDescription[] Formats =
        {
            new FormatDescription()
            {
                Name = "Fallout 1 DAT",
                Extensions = new string[] {"dat"},
                AssemblyType = typeof(KNFE.Core.Format.Archive.Fallout1DatFormat)
            }
        };

        /// <summary>
        /// Resolves an array of possible matching FormatDescriptions from a file name's extension.
        /// </summary>
        /// <param name="fileName">The file to resolve a FormatDescription from.</param>
        /// <returns>An array of possible FormatDescriptions that match the extension pattern of the given file name.</returns>
        public static FormatDescription[] ResolveFromFileName(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException();

            // Gets the file extension
            string ext = Path.GetExtension(fileName).ToLower().Substring(1);
            List<FormatDescription> fds = new List<FormatDescription>();

            // Iterate through format list and find matching extensions
            foreach (FormatDescription fd in Formats)
            {
                foreach (string s in fd.Extensions)
                {
                    // Matching extension? Add current FormatDescription and break to next
                    if (s == ext)
                    {
                        fds.Add(fd);
                        break;
                    }
                }
            }

            return fds.ToArray();
        }
    }
}