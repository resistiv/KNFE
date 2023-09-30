using System;
using System.Collections.Generic;
using System.IO;
using KNFE.Core.Utils;

namespace KNFE.Core.Format.Archive
{
    /// <summary>
    /// Provides storage for a Reel Fishing PAK/MB item entry and its information.
    /// </summary>
    public class ReelFishingPakFormatEntry : FormatEntry
    {
        // Internal members
        internal uint _offset = 0;
        internal uint _length = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReelFishingPakFormatEntry"/> class as a directory.
        /// </summary>
        /// <param name="path">A relative path to the virtual directory from the root of the <see cref="ReelFishingPakFormat"/>.</param>
        public ReelFishingPakFormatEntry(string path) : base(path) { }

        /// <summary>
        /// Initializes a new of instance of the <see cref="ReelFishingPakFormatEntry"/> class as a file.
        /// </summary>
        /// <param name="path">A relative path to the virtual file from the root of the <see cref="ReelFishingPakFormat"/>.</param>
        /// <param name="source">The corresponding <see cref="Stream"/> where the virtual file is located.</param>
        public ReelFishingPakFormatEntry(string path, Stream source) : base(path, source) { }

        public override EntryProperties Extract(Stream outStream)
        {
            // Check for invalid call
            if (IsDirectory)
            {
                throw new InvalidOperationException("Attempted to extract data from a directory ReelFishingPakFormatEntry.");
            }

            // Seek to source data offset and extract data
            _source.Seek(_offset, SeekOrigin.Begin);
            Generic.SubStream(_source, outStream, _length);

            // No EntryProperties to write
            return null;
        }

        public override Dictionary<string, string> ToFields()
        {
            if (_fields != null)
                return _fields;

            _fields = new Dictionary<string, string>
            {
                { "Offset", $"0x{Convert.ToString(_offset, 16).ToUpper()}" }
            };

            return _fields;
        }

        /// <summary>
        /// Returns the offset of this <see cref="ReelFishingPakFormatEntry"/>'s data within its source Stream.
        /// </summary>
        public uint Offset { get { return _offset; } }
    }
}
