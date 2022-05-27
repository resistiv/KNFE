using KNFE.Core.Format;
using System;
using System.IO;

namespace KNFE.Helper
{
    /// <summary>
    /// Provides utilities for extracting and writing data to storage.
    /// </summary>
    public static class ExtractHandler
    {
        /// <summary>
        /// Initializes the extraction of a particular file FormatEntry.
        /// </summary>
        /// <param name="path">The directory to write the file to.</param>
        /// <param name="entry">The file FormatEntry to extract.</param>
        public static void ExtractFile(string path, FormatEntry entry)
        {
            if (entry.IsDirectory)
                throw new InvalidOperationException("Attempted to extract a file from a non-file FormatEntry.");
            
            // Attempt to create our output FileStream
            string outPath = $"{path}\\{entry.ItemPath}";
            FileStream fs = new FileStream(outPath, FileMode.OpenOrCreate, FileAccess.Write);

            // Extract to FileStream
            EntryProperties prop = entry.Extract(fs);
            fs.Close();

            // Set properties to file
            if (prop != null)
                prop.SetAttributes(outPath);
        }

        /// <summary>
        /// Initializes the extraction of a particular directory FormatEntry.
        /// </summary>
        /// <param name="path">The directory to write the directory's children to.</param>
        /// <param name="entry">The directory FormatEntry to extract children from.</param>
        public static void ExtractDirectory(string path, FormatEntry entry)
        {
            if (!entry.IsDirectory)
                throw new InvalidOperationException("Attempted to extract a directory from a non-directory FormatEntry.");

            string dirPath = $"{path}\\{entry.ItemPath}";

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            // Iterate through children
            foreach (FormatEntry fe in entry.Children)
            {
                if (fe.IsDirectory)
                    ExtractDirectory(dirPath, fe);
                else
                    ExtractFile(dirPath, fe);
            }
        }
    }
}
