using System.IO;

namespace KNFE.Encoding
{
    /// <summary>
    /// Represents a non-encoded binary data stream.
    /// </summary>
    public class BinaryStream : EncodingStream
    {
        public BinaryStream(Stream stream)
            : base(stream)
        { }

        public override MemoryStream Decode()
        {
            // Since this is stream doesn't need to be decoded, we copy it into memory and return
            MemoryStream outStream = new MemoryStream();
            Stream.CopyTo(outStream);
            return outStream;
        }
    }
}
