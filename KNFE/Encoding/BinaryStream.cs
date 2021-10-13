using KNFE.Util;
using System.IO;

namespace KNFE.Encoding
{
    /// <summary>
    /// Represents a non-encoded binary data stream.
    /// </summary>
    public class BinaryStream : EncodingStream
    {
        private readonly int? streamLength;
        
        public BinaryStream(Stream stream)
            : base(stream)
        {
            streamLength = null;
        }

        public BinaryStream(Stream stream, int streamLength)
            : base(stream)
        {
            this.streamLength = streamLength;
        }

        public override void Decode(Stream outStream)
        {
            // Since this stream doesn't need to be decoded, we copy it into the output stream
            if (streamLength != null)
            {
                Tools.SubStream(base.Stream, outStream, (int)streamLength);
            }
            else
            {
                base.Stream.CopyTo(outStream);
            }
            return;
        }
    }
}
