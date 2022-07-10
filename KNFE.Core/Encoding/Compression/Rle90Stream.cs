using System.IO;

namespace KNFE.Core.Encoding.Compression
{
    public class Rle90Stream : EncodingStream
    {
        // Private members
        private readonly BinaryReader _br;
        private readonly long _streamEnd;

        // Constant members
        private const byte RLE_MARKER = 0x90;

        public Rle90Stream(Stream stream, int streamLength = -1)
            : base(stream)
        {
            _br = new BinaryReader(Stream);
            _streamEnd = streamLength == -1 ? Stream.Length : Stream.Position + streamLength;
        }

        public override void Decode(Stream outStream)
        {
            byte currentByte = 0;
            byte lastByte = 0;
            byte runLength = 0;

            // Read until specified EOS
            while (_br.BaseStream.Position != _streamEnd)
            {
                // Read in working byte
                currentByte = _br.ReadByte();

                // RLE
                if (currentByte == RLE_MARKER)
                {
                    // Identify run length
                    runLength = _br.ReadByte();

                    // Literal 0x90 if length is 0
                    if (runLength == 0)
                    {
                        outStream.WriteByte(RLE_MARKER);
                        lastByte = currentByte;
                    }
                    else
                    {
                        // We decrement from the runLength first because we've already written one of the bytes from the run as a literal;
                        // That's why we have a lastByte value in the first place!!
                        while (--runLength > 0)
                            outStream.WriteByte(lastByte);
                    }
                }
                // Literal
                else
                {
                    outStream.WriteByte(currentByte);
                    lastByte = currentByte;
                }
            }
        }
    }
}
