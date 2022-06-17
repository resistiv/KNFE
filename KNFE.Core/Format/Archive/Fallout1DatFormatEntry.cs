using System;
using System.Collections.Generic;
using System.IO;
using KNFE.Core.Encoding.Compression;
using KNFE.Core.Util;

namespace KNFE.Core.Format.Archive
{
    /// <summary>
    /// Provides storage for a Fallout 1 *.DAT item entry and its information.
    /// </summary>
    public class Fallout1DatFormatEntry : FormatEntry
    {
        // Internal members
        internal bool _isCompressed = false;
        internal int _offset = -1;
        internal int _originalLength = -1;
        internal int _compressedLength = -1;
        internal int _fileCount = -1;

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
                throw new InvalidOperationException("Attempted to extract data from a directory Fallout1DatFormatEntry.");

            // Seek to source data offset and extract based on compression method
            _source.Seek(_offset, SeekOrigin.Begin);
            if (!IsCompressed)
            {
                Tools.SubStream(_source, outStream, _originalLength);
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
            Dictionary<string, string> dict = new Dictionary<string, string>();

            // Construct path with parental pathing
            string outPath = GetFullPath();

            if (IsDirectory)
            {
                dict.Add("Directory Path", outPath);
                if (_fileCount > -1)
                    dict.Add("File Count", _fileCount.ToString());
            }
            else
            {
                dict.Add("File Path", outPath);
                dict.Add("Compression", _isCompressed ? "LZSS" : "None");
                dict.Add("Offset", $"0x{Convert.ToString(_offset, 16).ToUpper()}");
                dict.Add("Original Size", _originalLength.ToString());
                if (_isCompressed)
                    dict.Add("Compressed Size", _compressedLength.ToString());
            }

            return dict;
        }

        /// <summary>
        /// Returns <c>true</c> when this <see cref="Fallout1DatFormatEntry"/> is LZSS-compressed and <c>false</c> when it is non-compressed.
        /// </summary>
        public bool IsCompressed { get { return _isCompressed; } }
        /// <summary>
        /// Returns the offset of this <see cref="Fallout1DatFormatEntry"/>'s data within its source Stream.
        /// </summary>
        public int Offset { get { return _offset; } }
        /// <summary>
        /// Returns the non-compressed length of the data of this <see cref="Fallout1DatFormatEntry"/>.
        /// </summary>
        public int OriginalLength { get { return _offset; } }
        /// <summary>
        /// Returns the compressed length of the data of this <see cref="Fallout1DatFormatEntry"/>.
        /// </summary>
        public int CompressedLength { get { return _compressedLength; } }
        /// <summary>
        /// Returns the number of child <see cref="Fallout1DatFormatEntry"/>s within this <see cref="Fallout1DatFormatEntry"/>.
        /// </summary>
        public int FileCount { get { return _fileCount; } }
    }
}
