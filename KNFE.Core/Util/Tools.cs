using System;
using System.IO;

namespace KNFE.Core.Util
{
    /// <summary>
    /// Handles various tasks, including miscellaneous conversions and encoding jobs.
    /// </summary>
    public static class Tools
    {
        private static int _bufferSize = 1024 * 16;

        /// <summary>
        /// Finds the newline sequence in a given <see cref="StreamReader"/>.
        /// </summary>
        /// <param name="sr">The <see cref="StreamReader"/> to find a newline sequence within.</param>
        /// <returns>The identified newline sequence.</returns>
        public static string FindNewLine(StreamReader sr)
        {
            // Buffer for reading data
            byte[] data = new byte[sr.BaseStream.Length];

            // Read data into buffer
            sr.BaseStream.Read(data, 0, (int)sr.BaseStream.Length);

            // Convert data to string
            string testData = System.Text.Encoding.ASCII.GetString(data);

            // Seek to beginning of StreamReader to override internal buffering
            sr.BaseStream.Seek(0, SeekOrigin.Begin);

            return FindNewLine(testData);
        }

        /// <summary>
        /// Finds the newline pattern in a given string.
        /// </summary>
        /// <param name="s">The string to search for a newline sequence within.</param>
        /// <returns>The identified newline sequence.</returns>
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
        /// Copies a part of a <see cref="Stream"/> to another <see cref="Stream"/> given a length.
        /// </summary>
        /// <param name="inStream">The input <see cref="Stream"/> to create a sub-<see cref="Stream"/> from.</param>
        /// <param name="outStream">The resultant sub-<see cref="Stream"/>.</param>
        /// <param name="length">The length, in bytes, of the resultant sub-<see cref="Stream"/>.</param>
        public static void SubStream(Stream inStream, Stream outStream, int length)
        {
            // Record start position to seek at end
            long startPos = inStream.Position;
            // Input buffer
            byte[] buffer = new byte[_bufferSize];
            // Count of bytes read in one cycle
            int bytesRead;
            // Read bytes to buffer
            while (length > 0 && (bytesRead = inStream.Read(buffer, 0, length % _bufferSize == 0 ? _bufferSize : length % _bufferSize)) > 0)
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

        /// <summary>
        /// Converts a 32-bit packed MS-DOS time to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dosDate">An unsigned short representing a packed DOS date.</param>
        /// <param name="dosTime">An unsigned short representing a packed DOS time.</param>
        /// <returns>A <see cref="DateTime"/> equivalent to the provided DOS date and time.</returns>
        public static DateTime ConvertDosTime(ushort dosDate, ushort dosTime)
        {
            /* --------------------------------------------------------------------------------
                    Year offset from 1980              Month                  Day
               ┌────────────────────────────┐     ┌─────────────┐     ┌──────────────────┐ 
            ┌────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┐
            │ 15 │ 14 │ 13 │ 12 │ 11 │ 10 │  9 │  8 │  7 │  6 │  5 │  4 │  3 │  2 │  1 │  0 │
            └────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┘
                   Hour (24 HR)                   Minute                  Seconds (/2)
               ┌──────────────────┐     ┌───────────────────────┐     ┌──────────────────┐ 
            ┌────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┐
            │ 15 │ 14 │ 13 │ 12 │ 11 │ 10 │  9 │  8 │  7 │  6 │  5 │  4 │  3 │  2 │  1 │  0 │
            └────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┘
            -------------------------------------------------------------------------------- */

            int year = ((dosDate >> 9) & 0x7F) + 1980;
            int month = (dosDate >> 5) & 0x0F;
            // Error correction, some formats utilize a "blank date"
            if (month == 0)
                month = 1;
            int day = dosDate & 0x1F;
            // See above
            if (day == 0)
                day = 1;
            int hour = (dosTime >> 11) & 0x1F;
            int minute = (dosTime >> 5) & 0x3F;
            int second = (dosTime & 0x1F) * 2;

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
