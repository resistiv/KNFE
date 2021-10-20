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
        // Start indicator
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
            newLineConfig = Tools.FindNewLine(sr);

            if (newLineConfig.Equals(string.Empty))
            {
                Program.Quit("Could not identify newline configuration, quitting.");
            }

            // Find our header in a text file; validation
            string header = SeekStreamHeader(sr);

            if (header.Equals(string.Empty))
            {
                Program.Quit($"Could not find {base.formatName} header, quitting.");
            }

            // Header is in format "begin <perms> <filename>"
            string[] headerInfo = header.Split(' ');

            // Confirm header style
            if (headerInfo.Length != 3)
            {
                Program.Quit($"Malformed {base.formatName} header, quitting.");
            }

            // Confirm perm range
            int permNum = Convert.ToInt32(headerInfo[1], 8);
            if (permNum < 0 || permNum > 511) // 511 is equivalent to 0777
            {
                Program.Quit($"Malformed {base.formatName} permissions, quitting.");
            }

            permissions = ConvertUnixOctalPerms(permNum);
            outFileName = headerInfo[2];
        }

        public override void ExportData()
        {
            // MemoryStream file = output.Decode();
            string outputPath = CreateOutputPath();
            string dataOutputPath = $"{outputPath}{outFileName}";
            FileStream outFile = null;
            try
            {
                outFile = new FileStream(dataOutputPath, FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                Program.Quit("Quitting.");
            }

            Logger.LogInfo(ToString());
            // WriteFileFromMemory(dataOutputPath, file);
            output.Decode(outFile);
            outFile.Close();
            sr.Close();
        }

        /// <summary>
        /// Converts an octal Unix file permission into a string of ten permission flags.
        /// </summary>
        private string ConvertUnixOctalPerms(int perms)
        {
            string outString = "-";
            // Cycle through the lower 9 bits of our perm string
            for (int i = 8; i >= 0; i--)
            {
                // Test which bit we're on
                switch (i % 3)
                {
                    // 0x4 or 0b100
                    case 2:
                        if ((perms & (int)Math.Pow(2, i)) != 0)
                            outString += "r";
                        else outString += "-";
                        break;
                    // 0x2 or 0b010
                    case 1:
                        if ((perms & (int)Math.Pow(2, i)) != 0)
                            outString += "w";
                        else outString += "-";
                        break;
                    // 0x1 or 0b001
                    case 0:
                        if ((perms & (int)Math.Pow(2, i)) != 0)
                            outString += "x";
                        else outString += "-";
                        break;
                    default:
                        break;
                }
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
            return string.Empty;
        }

        public override string ToString()
        {
            return base.ToString() +
                   $"\tPermissions: {permissions}\n" +
                   $"\tOutput file name: {outFileName}\n";
        }
    }
}
