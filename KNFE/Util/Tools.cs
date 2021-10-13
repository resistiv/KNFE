using System.IO;

namespace KNFE.Util
{
    /// <summary>
    /// Represents a general set of tools used across various formats.
    /// </summary>
    public static class Tools
    {
        private static int BUFFER_SIZE = 1024 * 16;

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

        /// <summary>
        /// Copies a part of a Stream to another Stream given a length.
        /// </summary>
        public static void SubStream(Stream inStream, Stream outStream, int length)
        {
            // Record start position to seek at end
            long startPos = inStream.Position;
            // Input buffer
            byte[] buffer = new byte[BUFFER_SIZE];
            // Count of bytes read in one cycle
            int bytesRead;
            // Read bytes to buffer
            while (length > 0 && (bytesRead = inStream.Read(buffer, 0, length % BUFFER_SIZE == 0 ? BUFFER_SIZE : length % BUFFER_SIZE)) > 0)
            {
                // Write to output
                outStream.Write(buffer, 0, bytesRead);
                // We read some bytes, so now we have less to read total; decrement length
                length -= bytesRead;
            }

            // Since we read with a buffer, we can sometimes go over the desired position in the inStream, so we seek back
            inStream.Seek(startPos + length, SeekOrigin.Begin);

            return;
        }
    }
}
