using System.IO;

namespace KNFE.Encoding
{
    /// <summary>
    /// Represents a generic stream utilizing some sort of encoding.
    /// </summary>
    public abstract class EncodingStream
    {
        public Stream Stream;

        public EncodingStream(Stream stream)
        {
            Stream = stream;
        }

        /// <summary>
        /// Decodes an encoded stream and saves it to memory.
        /// </summary>
        public abstract MemoryStream Decode();
    }
}
