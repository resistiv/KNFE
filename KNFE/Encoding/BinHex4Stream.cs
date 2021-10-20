using System.IO;

namespace KNFE.Encoding
{
    /// <summary>
    /// Represents a BinHex 4.0 encoded stream.
    /// </summary>
    public class BinHex4Stream : EncodingStream
    {
        // Start and end indicator
        private const char MARKER = ':';
        // Invalid character marker
        private const byte INVALID_ENTRY = 0x40; // '@'

        // Decoding table
        private readonly byte[] sixBitTable = new byte[256];

        // Character bank of all valid encoded chars
        private const string CHARBANK = "!\"#$%&'()*+,-012345689@ABCDEFGHIJKLMNPQRSTUVXYZ[`abcdefhijklmpqr";

        // Reader
        private readonly BinaryReader br;

        // Bit processing
        private int bitBuffer, bitsLeft;

        public BinHex4Stream(Stream stream)
            : base(stream)
        {
            br = new BinaryReader(base.Stream);
        }

        public override void Decode(Stream outStream)
        {
            // Build table
            BuildSixBitTable();

            // Check for proper start of stream
            if (br.ReadChar() != MARKER)
            {
                Program.Quit($"Invalid start of BinHex 4.0 stream, quitting.");
            }

            char currentChar;
            // Iterate through stream
            while (!IsEndOfStream())
            {
                currentChar = br.ReadChar();

                // End of encoded stream
                if (currentChar == MARKER)
                    break;

                // Some encoders add an extra '!' character directly before the ending marker, so we check if
                // a.) we are the second to last character,
                // b.) the character is, indeed, an '!', and
                // c.) we have no bits left to process, so this character is, indeed, extraneous
                // If all this is true, we advance forth
                if (IsSecondToLastByte() && currentChar == '!' && bitsLeft == 0)
                {
                    continue;
                }
                else
                {
                    // Decode current char
                    int bits = GetSixBits(currentChar);
                    // Buffer decoded bits
                    bitBuffer = (bitBuffer << 6) | bits;
                    // Increment how many bits in buffer
                    bitsLeft += 6;

                    // If we have a full byte
                    if (bitsLeft >= 8)
                    {
                        // Decrement bits out of buffer
                        bitsLeft -= 8;
                        // Write decoded byte to output
                        outStream.WriteByte((byte)(bitBuffer >> bitsLeft));
                    }
                }
            }
            // Release file handle
            br.Close();

            outStream.Seek(0, SeekOrigin.Begin);
            return;
        }

        /// <summary>
        /// Decodes a char into a sequence of six bits.
        /// </summary>
        private int GetSixBits(char c)
        {
            c &= (char)0x7F;

            int d = sixBitTable[c];

            if (d == INVALID_ENTRY)
            {
                Program.Quit($"Invalid character in BinHex 4.0 stream, quitting.");
            }

            return d;
        }

        /// <summary>
        /// Populates the table of values used for decoding BinHex-encoded characters.
        /// </summary>
        private void BuildSixBitTable()
        {
            // Flushes table
            for (int i = 0; i < sixBitTable.Length; i++)
            {
                sixBitTable[i] = INVALID_ENTRY;
            }
            // Populates valid fields with their decoded equivalent
            for (int i = 0; i < CHARBANK.Length; i++)
            {
                sixBitTable[CHARBANK[i]] = (byte)i;
            }
        }

        /// <summary>
        /// Determines if the BinaryReader is at the end of its stream.
        /// </summary>
        private bool IsEndOfStream()
        {
            return br.BaseStream.Position == br.BaseStream.Length;
        }

        /// <summary>
        /// Determines if the BinaryReader is at the second to last byte in the stream.
        /// </summary>
        private bool IsSecondToLastByte()
        {
            return br.BaseStream.Position - 2 == br.BaseStream.Length;
        }
    }
}
