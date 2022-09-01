using System;
using System.IO;

namespace KNFE.Core.Utils
{
    /// <summary>
    /// Handles various generic tasks.
    /// </summary>
    public static class Generic
    {
        private static int _bufferSize = 1024 * 16;

        /// <summary>
        /// Copies a part of a <see cref="Stream"/> to another <see cref="Stream"/> given a length.
        /// </summary>
        /// <param name="inStream">The input <see cref="Stream"/> to create a sub-<see cref="Stream"/> from.</param>
        /// <param name="outStream">The resultant sub-<see cref="Stream"/>.</param>
        /// <param name="length">The length, in bytes, of the resultant sub-<see cref="Stream"/>.</param>
        public static void SubStream(Stream inStream, Stream outStream, long length)
        {
            // Record start position to seek at end
            long startPos = inStream.Position;
            // Input buffer
            byte[] buffer = new byte[_bufferSize];
            // Count of bytes read in one cycle
            int bytesRead;
            // Read bytes to buffer
            while (length > 0 && (bytesRead = inStream.Read(buffer, 0, length % _bufferSize == 0 ? _bufferSize : (int)(length % _bufferSize))) > 0)
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
