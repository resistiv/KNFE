using System;
using System.Collections.Generic;
using System.IO;
using KNFE.Core.Encoding.Compression;
using KNFE.Core.Utils;

namespace KNFE.Core.Format.Archive
{
    /// <summary>
    /// Provides storage for a Fallout 1 DAT item entry and its information.
    /// </summary>
    public class Fallout1DatFormatEntry : FormatEntry
    {
        // Internal members
        internal bool _isCompressed = false;
        internal uint _offset = 0;
        internal uint _originalLength = 0;
        internal uint _compressedLength = 0;
        internal uint _fileCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Fallout1DatFormatEntry"/> class as a directory.
        /// </summary>
        /// <param name="path">A relative path to the virtual directory from the root of the <see cref="Fallout1DatFormat"/>.</param>
        public Fallout1DatFormatEntry(string path) : base(path) { }

        /// <summary>
        /// Initializes a new of instance of the <see cref="Fallout1DatFormatEntry"/> class as a file.
        /// </summary>
        /// <param name="path">A relative path to the virtual file from the root of the <see cref="Fallout1DatFormat"/>.</param>
        /// <param name="source">The corresponding <see cref="Stream"/> where the virtual file is located.</param>
        public Fallout1DatFormatEntry(string path, Stream source) : base(path, source) { }

        public override EntryProperties Extract(Stream outStream)
        {
            // Check for invalid call
            if (IsDirectory)
                throw new InvalidOperationException($"{GetType().Name}: Attempted to extract data from a directory.");

            // Seek to source data offset and extract based on compression method
            _source.Seek(_offset, SeekOrigin.Begin);
            if (!IsCompressed)
            {
                Generic.SubStream(_source, outStream, _originalLength);
            }
            else
            {
                Fallout1LzssStream stream = new Fallout1LzssStream(_source, _compressedLength);
                stream.Decode(outStream);
            }
               
            // Fallout doesn't retain file info pertinent to EntryProperties, so return null
            return null;
        }

        public override Dictionary<string, string> ToFields()
        {
            if (_fields != null)
                return _fields;

            _fields = new Dictionary<string, string>();

            // Construct path with parental pathing
            string outPath = GetFullPath();

            if (IsDirectory)
            {
                _fields.Add("Directory Name", outPath);
                if (_fileCount > -1)
                    _fields.Add("File Count", _fileCount.ToString());
            }
            else
            {
                _fields.Add("File Name", outPath);
                _fields.Add("Compression", _isCompressed ? "LZSS" : "None");
                _fields.Add("Offset", $"0x{Convert.ToString(_offset, 16).ToUpper()}");
                _fields.Add("Original Length", _originalLength.ToString());
                if (_isCompressed)
                    _fields.Add("Compressed Length", _compressedLength.ToString());
            }

            return _fields;
        }

        /// <summary>
        /// Returns <c>true</c> when this <see cref="Fallout1DatFormatEntry"/> is LZSS-compressed and <c>false</c> when it is non-compressed.
        /// </summary>
        public bool IsCompressed { get { return _isCompressed; } }
        /// <summary>
        /// Returns the offset of this <see cref="Fallout1DatFormatEntry"/>'s data within its source Stream.
        /// </summary>
        public uint Offset { get { return _offset; } }
        /// <summary>
        /// Returns the non-compressed length of the data of this <see cref="Fallout1DatFormatEntry"/>.
        /// </summary>
        public uint OriginalLength { get { return _originalLength; } }
        /// <summary>
        /// Returns the compressed length of the data of this <see cref="Fallout1DatFormatEntry"/>.
        /// </summary>
        public uint CompressedLength { get { return _compressedLength; } }
        /// <summary>
        /// Returns the number of child <see cref="Fallout1DatFormatEntry"/>s within this <see cref="Fallout1DatFormatEntry"/>.
        /// </summary>
        public uint FileCount { get { return _fileCount; } }
    }
}
