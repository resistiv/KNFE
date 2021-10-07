using KNFE.Encoding;
using KNFE.Util;
using System;
using System.IO;

namespace KNFE.Format
{
    /// <summary>
    /// Represents a generic file format.
    /// </summary>
    public abstract class FileFormat
    {
        protected readonly FileStream inFileStream;
        protected readonly string fileName;
        protected readonly string formatName;

        public FileFormat(string formatName, string fileName)
        {
            // Check if our file exists
            if (!File.Exists(fileName))
            {
                Program.Quit($"Invalid file '{fileName}', quitting.");
            }

            this.fileName = fileName;
            this.formatName = formatName;

            // Try to open our file, throw exception if anything goes wrong
            try
            {
                inFileStream = File.Open(fileName, FileMode.Open);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                Program.Quit();
            }
        }

        /// <summary>
        /// Decode and save all format-related data to disk.
        /// </summary>
        public abstract void ExportData();

        /// <summary>
        /// Creates an output path based on the input file name.
        /// </summary>
        protected string CreateOutputPath()
        {
            string outputPath = $"{Path.GetDirectoryName(Path.GetFullPath(fileName))}\\{Path.GetFileName(fileName)}_out\\";
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            return outputPath;
        }

        /// <summary>
        /// Writes a file to disk given a path and data in memory.
        /// </summary>
        protected void WriteFileFromStream(string dataOutPath, MemoryStream data)
        {
            try
            {
                using (FileStream outStream = File.Create(dataOutPath))
                {
                    data.Seek(0, SeekOrigin.Begin);
                    data.CopyTo(outStream);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                Program.Quit();
            }
        }

        public override string ToString()
        {
            return $"File name: {fileName}\n" +
                   $"File type: {formatName}\n";
        }
    }
}
