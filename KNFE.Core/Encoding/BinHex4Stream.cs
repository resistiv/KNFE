using System;
using System.IO;

namespace KNFE.Core.Encoding
{
    public class BinHex4Stream : EncodingStream
    {
        // Private members
        private readonly BinaryReader _br;
        private readonly byte[] sixBitTable = new byte[256];

        // Constant members
        private const char STREAM_MARKER = ':';
        private const byte INVALID_ENTRY = 0x40; // '@'
        private const string CHARBANK = "!\"#$%&'()*+,-012345689@ABCDEFGHIJKLMNPQRSTUVXYZ[`abcdefhijklmpqr";

        public BinHex4Stream(Stream stream)
            : base(stream)
        {
            _br = new BinaryReader(Stream);
        }

        public override void Decode(Stream outStream)
        {
            // Build table
            BuildSixBitTable();

            // Seek to stream beginning
            while (_br.ReadChar() != STREAM_MARKER)
            {
                if (_br.BaseStream.Position == _br.BaseStream.Length)
                    throw new InvalidDataException($"Could not find stream start marker \":\" in BinHex4Stream.");
            }

            int bitBuffer = 0;
            int bitsIn = 0;

            // Traverse stream
            char currentChar;
            while ((currentChar = _br.ReadChar()) != STREAM_MARKER)
            {
                // Ignore newlines
                if (currentChar == '\n' || currentChar == '\r')
                    continue;

                // Get bits and shift them into the buffer
                int bits = GetSixBits(currentChar);
                bitBuffer = (bitBuffer << 6) | bits;
                bitsIn += 6;

                // If we have enough bits, output them
                if (bitsIn >= 8)
                {
                    bitsIn -= 8;
                    outStream.WriteByte((byte)(bitBuffer >> bitsIn));
                }
            }
        }

        /// <summary>
        /// Decodes a BinHex'd character into a sequence of six bits.
        /// </summary>
        /// <param name="c">The character to decode.</param>
        /// <returns>The decoded six bits.</returns>
        private int GetSixBits(char c)
        {
            c &= (char)0x7F;

            int d = sixBitTable[c];

            if (d == INVALID_ENTRY)
            {
                throw new InvalidDataException($"Received invalid character '{c}' in BinHex4Stream.");
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
    }
}
