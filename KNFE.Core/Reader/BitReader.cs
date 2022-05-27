using System.IO;

namespace KNFE.Core.Reader
{
    /// <summary>
    /// Reads primitive data types as binary values, alongside reading raw binary bits.
    /// </summary>
    public class BitReader : BinaryReader
    {
        // Bit storage
        private int _bitsLeft;
        private byte _bitBuffer;

        /// <summary>
        /// Initializes a new instance of <see cref="BitReader"/> class from a specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read from.</param>
        public BitReader(Stream stream)
            : base(stream)
        {
            _bitsLeft = 0;
        }

        /// <summary>
        /// Reads a specified number of bits, up to 32, from the current <see cref="Stream"/>.
        /// </summary>
        /// <param name="count">The number of bits to be read from the <see cref="Stream"/>.</param>
        /// <returns>The bits read from the <see cref="Stream"/>, or <c>-1</c> if the end of the <see cref="Stream"/> is reached.</returns>
        public int ReadBits(int count)
        {
            int buffer = 0;
            int temp;

            // Traverse bits
            while (count > 0)
            {
                // Shift the buffer 
                buffer <<= 1;

                // Check for read error
                temp = ReadBit();
                if (temp == -1)
                    return -1;
                // Else, buffer that bit
                else
                    buffer |= (temp & 0x01);
                count--;
            }

            return buffer;
        }

        /// <summary>
        /// Returns a bit from the current <see cref="Stream"/> and advances the current position of the <see cref="Stream"/> by one byte upon reaching a byte boundary.
        /// </summary>
        /// <returns>The next bit read from the <see cref="Stream"/>, or <c>-1</c> if the end of the <see cref="Stream"/> is reached.</returns>
        public int ReadBit()
        {
            // Out of bits? Try to read another byte
            if (_bitsLeft == 0)
            {
                // Try to read a byte, catch EOS
                int temp = ReadByte();
                if (temp == -1)
                    return -1;
                // Buffer that byte
                else
                    _bitBuffer = (byte)(temp & 0xFF);
                _bitsLeft = 8;
            }

            // Get our LSB for current bit
            int bit = _bitBuffer & 0x1;

            // Adjust our running buffer and counter
            _bitBuffer >>= 1;
            _bitsLeft--;

            return bit;
        }
    }
}
