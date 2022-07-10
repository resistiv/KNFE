using System.IO;

namespace KNFE.Core.Encoding
{
    /// <summary>
    /// Represents a generic stream utilizing some sort of encoding. This is an abstract class.
    /// </summary>
    public abstract class EncodingStream
    {
        // Protected members
        protected readonly Stream Stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingStream"/> class with a specified source <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">The source <see cref="System.IO.Stream"/> to read from.</param>
        public EncodingStream(Stream stream)
        {
            Stream = stream;
        }

        /// <summary>
        /// Decodes an encoded stream and writes it to an output <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="outStream">The <see cref="System.IO.Stream"/> to write all output to.</param>
        public abstract void Decode(Stream outStream);
    }
}
