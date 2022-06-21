using System;
using System.IO;

namespace KNFE.Core.Format
{
    /// <summary>
    /// Reads and processes the uuencode file format.
    /// </summary>
    public class UuFormat : Format
    {
        // Private members
        private readonly StreamReader _sr;

        // Private constants
        private const string HEADER = "begin";

        public UuFormat(string fileName)
            : base("uuencode", fileName)
        {
            // Init reader
            _sr = new StreamReader(InFileStream);

            // Read header
            // begin <unix perms> <filename>
            string fullHead = _sr.ReadLine();
            string[] header = fullHead.Split(' ');
            if (header.Length != 3)
                throw new InvalidDataException($"Received malformed header within a {FormatName} file.");

            // Compare magic "begin"
            if (header[0] != HEADER)
                throw new InvalidDataException($"Received invalid header ({header[0]}) within a {FormatName} file.");

            // Convert octal perms to int
            int permNum = Convert.ToInt32(header[1], 8);
            if (permNum < 0 || permNum > 511) // 511 is equal to 0777
                throw new InvalidDataException($"Received invalid file permissions ({header[1]}) within a {FormatName} file.");

            // Save to perms
            string perms = ConvertUnixOctalPermissions(permNum);

            // Fix offset from StreamReader buffering
            _sr.BaseStream.Seek(fullHead.Length, SeekOrigin.Begin);
            if (_sr.BaseStream.ReadByte() == '\r')
                _sr.BaseStream.Seek(1, SeekOrigin.Current);

            _root = new UuFormatEntry("");
            _root.AddChild(new UuFormatEntry(header[2], InFileStream) { _perms = perms, _startOffset = _sr.BaseStream.Position});
        }

        public override void Close()
        {
            _sr.Close();
            base.Close();
        }

        /// <summary>
        /// Converts an octal Unix file permissions representation into a string of ten permission flags.
        /// </summary>
        /// <param name="perms">The octal Unix file permissions to convert.</param>
        /// <returns>The resulting permission flags.</returns>
        private string ConvertUnixOctalPermissions(int perms)
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
                        outString += (perms & (int)Math.Pow(2, i)) != 0 ? "r" : "-";
                        break;
                    // 0x2 or 0b010
                    case 1:
                        outString += (perms & (int)Math.Pow(2, i)) != 0 ? "w" : "-";
                        break;
                    // 0x1 or 0b001
                    case 0:
                        outString += (perms & (int)Math.Pow(2, i)) != 0 ? "x" : "-";
                        break;
                    default:
                        break;
                }
            }

            return outString;
        }
    }
}
