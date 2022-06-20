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

                // Blank line, no data
                if (currentLine.Length == 0)
                    continue;

                // Some encoders encode blank lines with a single, varying character
                // So, if we only have one char on this line, assume a zero-length data stream
                int bytesOut;
                if (currentLine.Length == 1)
                    bytesOut = 0;
                else
                    bytesOut = GetSixBits(currentLine[0]);

                int bitBuffer = 0;
                byte bitsIn = 0;

                // Loop through line data
                // i keeps track of the bytes output
                // j keeps track of the current line position
                for (int i = 0, j = 1; i < bytesOut; j++)
                {
                    // Get bits and shift them into the buffer
                    int bits = GetSixBits(currentLine[j]);
                    bitBuffer = (bitBuffer << 6) | bits;
                    bitsIn += 6;

                    // If we have enough bits, output them
                    if (bitsIn >= 8)
                    {
                        bitsIn -= 8;
                        outStream.WriteByte((byte)(bitBuffer >> bitsIn));
                        i++;
                    }
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
