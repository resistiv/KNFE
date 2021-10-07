using KNFE.Encoding;
using KNFE.Util;
using System;
using System.IO;

namespace KNFE.Format
{
    /// <summary>
    /// Represents a uuencoded file.
    /// </summary>
    public class UuencodeFormat : FileFormat
    {
        // Start and end indicators
        private const string HEADER = "begin";

        private readonly string newLineConfig;
        private readonly string permissions;
        private readonly string outFileName;

        private readonly StreamReader sr;
        private UuencodeStream output;

        public UuencodeFormat(string formatName, string fileName)
            : base(formatName, fileName)
        {
            sr = new StreamReader(inFileStream);
            output = new UuencodeStream(inFileStream);

            // Identify newline pattern in order to properly calculate an offset
            newLineConfig = FindNewLine(sr);
            if (newLineConfig.Equals(string.Empty))
                Program.Quit("Could not identify newline configuration, quitting.");

            // Find our header in a text file; validation
            string header = SeekStreamHeader(sr);

            if (header == null)
            {
                Program.Quit($"Could not find uuencode header, quitting.");
            }

            // Header is in format "begin <perms> <filename>"
            string[] headerInfo = header.Split(' ');

            // Confirm header style
            if (headerInfo.Length != 3)
            {
                Program.Quit("Malformed uuencode header, quitting.");
            }

            // Confirm perm range
            if (Convert.ToInt32(headerInfo[1]) < 0 || Convert.ToInt32(headerInfo[1]) > 777)
            {
                Program.Quit("Malformed uuencode permissions, quitting.");
            }

            permissions = ConvertUnixOctalPerms(headerInfo[1]);
            outFileName = headerInfo[2];
        }

        public override void ExportData()
        {
            MemoryStream file = output.Decode();
            string outputPath = CreateOutputPath();
            string dataOutputPath = $"{outputPath}{outFileName}";

            Logger.LogInfo(ToString());
            WriteFileFromStream(dataOutputPath, file);
            sr.Close();
        }

        /// <summary>
        /// Converts an octal Unix file permission into a string of ten permission flags.
        /// </summary>
        private string ConvertUnixOctalPerms(string octal)
        {
            string outString = "-";
            foreach (char c in octal)
            {
                // Read permission is third bit set; e.g. 0b100
                if ((Convert.ToInt32($"{c}", 8) & 0x4) >> 2 != 0)
                {
                    outString += "r";
                }
                else
                    outString += "-";

                // Write permission is second bit set; e.g. 0b010
                if ((Convert.ToInt32($"{c}", 8) & 0x2) >> 1 != 0)
                {
                    outString += "w";
                }
                else
                    outString += "-";

                // Execute permission is first bit set; e.g. 0b001
                if ((Convert.ToInt32($"{c}", 8) & 0x1) != 0)
                {
                    outString += "x";
                }
                else
                    outString += "-";
            }

            return outString;
        }

        /// <summary>
        /// Finds a uuencode header within a Stream.
        /// </summary>
        /// <returns>Uuencode header line</returns>
        private string SeekStreamHeader(StreamReader sr)
        {
            // Need an offset since StreamReader buffers data rather than seeking through a stream on a read-by-read basis
            int offset = 0;
            while (!sr.EndOfStream)
            {
                // Read a line
                string temp = sr.ReadLine();
                offset += temp.Length + newLineConfig.Length;
                // EOS
                if (temp == null)
                    break;
                // Found our header! Return
                else if (temp.Contains(HEADER))
                {
                    sr.BaseStream.Seek(offset, SeekOrigin.Begin);
                    return temp;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the newline pattern for the current stream.
        /// </summary>
        private string FindNewLine(StreamReader sr)
        {
            // Buffer for reading data
            byte[] data = new byte[sr.BaseStream.Length];
            // Read data into buffer
            sr.BaseStream.Read(data, 0, (int)sr.BaseStream.Length);

            // Convert data to string
            string testData = System.Text.Encoding.ASCII.GetString(data);

            // Check cases
            string[] cases = { "\r\n", "\r", "\n" };
            for (int i = 0; i < cases.Length; i++)
            {
                if (testData.Contains($"{cases[i]}"))
                {
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    return cases[i];
                }
            }
            return string.Empty;
        }

        public override string ToString()
        {
            return base.ToString() +
                   $"\tPermissions: {permissions}\n" +
                   $"\tOutput file name: {outFileName}";
        }
    }
}
