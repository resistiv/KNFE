using System.IO;

namespace KNFE.Util
{
    /// <summary>
    /// Represents a general set of tools used across various formats.
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Reads and returns a length-prefixed string, length being one byte, from a BinaryReader.
        /// </summary>
        public static string ReadByteString(BinaryReader br)
        {
            int stringLength = br.ReadByte();
            string outStr = System.Text.Encoding.UTF8.GetString(br.ReadBytes(stringLength));
            return outStr;
        }

        /// <summary>
        /// Finds the newline pattern in a given StreamReader.
        /// </summary>
        public static string FindNewLine(StreamReader sr)
        {
            // Buffer for reading data
            byte[] data = new byte[sr.BaseStream.Length];
            // Read data into buffer
            sr.BaseStream.Read(data, 0, (int)sr.BaseStream.Length);

            // Convert data to string
            string testData = System.Text.Encoding.ASCII.GetString(data);

            sr.BaseStream.Seek(0, SeekOrigin.Begin);

            return FindNewLine(testData);
        }

        /// <summary>
        /// Finds the newline pattern in a given string.
        /// </summary>
        public static string FindNewLine(string s)
        {
            // Check cases
            string[] cases = { "\r\n", "\r", "\n" };
            for (int i = 0; i < cases.Length; i++)
            {
                if (s.Contains($"{cases[i]}"))
                {
                    return cases[i];
                }
            }
            return string.Empty;
        }
    }
}
