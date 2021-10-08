using System.IO;

namespace KNFE.Encoding.Compression
{
    /// <summary>
    /// Represents an RLE90 (run-length encoded 0x90) stream.
    /// </summary>
    public class Rle90Stream : EncodingStream
    {
        private readonly BinaryReader br;

        // Run indicator
        private const byte MARKER_BYTE = 0x90;
        // Literal 0x90 indicator
        private const byte LITERAL_BYTE = 0x00;

        // Current working byte
        private byte currentByte;
        // Buffer for the previously read byte to keep track of runs
        private byte lastByte;
        // Byte indicating the length of a run of bytes
        private byte runLength;

        public Rle90Stream(Stream stream)
            : base(stream)
        {
            br = new BinaryReader(base.stream);
        }

        public override MemoryStream Decode()
        {
            MemoryStream decoded = new MemoryStream();

            // Iterate through byte stream
            while (!IsLastByte())
            {
                // Read in our current working byte 
                currentByte = br.ReadByte();

                // Found a run marker!!
                if (currentByte.Equals(MARKER_BYTE))
                {
                    // Identify how long the current run is
                    runLength = br.ReadByte();

                    // If the run length is zero, the run is a literal 0x90
                    if (runLength == LITERAL_BYTE)
                    {
                        decoded.WriteByte(MARKER_BYTE);
                        lastByte = MARKER_BYTE;
                    }
                    else
                    {
                        // Write bytes to output from run
                        // We decrement from the runLength first because we've already written one of the bytes from the run as a literal; that's why we have a lastByte value in the first place!
                        while (--runLength > 0)
                        {
                            decoded.WriteByte(lastByte);
                        }
                    }
                }
                // Literal byte
                else
                {
                    decoded.WriteByte(currentByte);
                    lastByte = currentByte;
                }
            }

            // Release that file handle!!!
            br.Close();

            decoded.Seek(0, SeekOrigin.Begin);
            return decoded;
        }

        /// <summary>
        /// Checks if the end of the base stream has been reached.
        /// </summary>
        private bool IsLastByte()
        {
            return br.BaseStream.Position == br.BaseStream.Length;
        }
    }
}
