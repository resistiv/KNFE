using System.IO;

namespace KNFE.Core.Encoding
{
    /// <summary>
    /// Handles uuencoding.
    /// </summary>
    public class UuStream : EncodingStream
    {
        // Private members
        private StreamReader _sr;

        // Constant members
        private const byte DECODE_SHIFT = 0x20;
        private const byte DECODE_MASK = 0x3F;
        private const string EOS_MARKER = "end";

        /// <summary>
        /// Initializes a new instance of a <see cref="UuStream"/> class with a specified source <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The source <see cref="Stream"/> to read from.</param>
        public UuStream(Stream stream)
            : base(stream)
        {
            _sr = new StreamReader(stream);
        }

        public override void Decode(Stream outStream)
        {
            string currentLine = "";
            while (!_sr.EndOfStream)
            {
                // Read current line
                currentLine = _sr.ReadLine();

                // EOS
                if (currentLine == null)
                    break;
                if (currentLine.StartsWith(EOS_MARKER))
                    break;

                // Edge case
                if (currentLine.Length == 0)
                    continue;

                // Length indicator
                int lineLength;

                // Some encoders encode blank lines with a single, differing character
                // So, if we only have one char, assume a zero-length data stream
                if (currentLine.Length == 1)
                    lineLength = 0;
                else
                    lineLength = GetSixBits(currentLine[0]);

                // Decode line
                for (int i = 1; lineLength > 0; i += 4, lineLength -= 3)
                {
                    if (lineLength >= 1)
                        outStream.WriteByte((byte)(GetSixBits(currentLine[i]) << 2 | GetSixBits(currentLine[i + 1]) >> 4));
                    if (lineLength >= 2)
                        outStream.WriteByte((byte)(GetSixBits(currentLine[i + 1]) << 4 | GetSixBits(currentLine[i + 2]) >> 2));
                    if (lineLength >= 3)
                        outStream.WriteByte((byte)(GetSixBits(currentLine[i + 2]) << 6 | GetSixBits(currentLine[i + 3])));
                }
            }

            return;
        }

        /// <summary>
        /// Decodes a uuencoded character into a sequence of six bits.
        /// </summary>
        /// <param name="c">The character to decode.</param>
        /// <returns>The decoded six bits.</returns>
        private int GetSixBits(char c)
        {
            return (c - DECODE_SHIFT) & DECODE_MASK;
        }
    }
}
