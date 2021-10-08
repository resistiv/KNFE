using System.IO;

namespace KNFE.Encoding
{
    /// <summary>
    /// Represents a uuencoded stream.
    /// </summary>
    public class UuencodeStream : EncodingStream
    {
        // End marker for stream
        private const string END_MARKER = "end";
        // Marker indicating a line of length 0
        private const char ZERO_LENGTH = '`';

        // Text-based encoding, so we utilize a StreamReader over BinaryReader
        private readonly StreamReader sr;

        public UuencodeStream(FileStream stream)
            : base(stream)
        {
            sr = new StreamReader(base.Stream);
        }

        public override MemoryStream Decode()
        {
            // Decoded output
            MemoryStream decoded = new MemoryStream();

            string currentLine = "";
            while (!sr.EndOfStream)
            {
                // Read current line
                currentLine = sr.ReadLine();

                // If the current line is null, we have reached the end of the Stream; break!
                if (currentLine == null)
                    break;
                // EOS if we reach marker
                if (currentLine.StartsWith(END_MARKER))
                    break;
                // Test if we have edge case of empty line, which we assume represents no data rather than an EOS
                if (currentLine.Length == 0)
                    continue;

                // Length indicator
                int lineLength;
                if (currentLine[0] == ZERO_LENGTH || currentLine.Length == 1)
                {
                    // Some encoders also use different zero-length chars, so if the line is only one char long we can safely assume it is meant to represent a zero-length data stream
                    lineLength = 0;
                }
                else
                {
                    // Get length of line in bytes
                    lineLength = GetSixBits(currentLine[0]);
                }

                // Decode line
                for (int i = 1; lineLength > 0; i += 4, lineLength -=3)
                {
                    // Decode by remaining length
                    if (lineLength >= 1)
                    {
                        decoded.WriteByte((byte)(GetSixBits(currentLine[i]) << 2 | GetSixBits(currentLine[i + 1]) >> 4));
                    }
                    if (lineLength >= 2)
                    {
                        decoded.WriteByte((byte)(GetSixBits(currentLine[i + 1]) << 4 | GetSixBits(currentLine[i + 2]) >> 2));
                    }
                    if (lineLength >= 3)
                    {
                        decoded.WriteByte((byte)(GetSixBits(currentLine[i + 2]) << 6 | GetSixBits(currentLine[i + 3])));
                    }
                }
            }

            // RELEASE THE HAAAAAANDLE!!!
            sr.Close();

            decoded.Seek(0, SeekOrigin.Begin);
            return decoded;
        }

        /// <summary>
        /// Decodes a char into a sequence of six bits.
        /// </summary>
        private int GetSixBits(char c)
        {
            return (c - 32) & 0x3F;
        }
    }
}
