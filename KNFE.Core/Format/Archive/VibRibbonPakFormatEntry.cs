using System;
using System.Collections.Generic;
using System.IO;
using KNFE.Core.Utils;

namespace KNFE.Core.Format.Archive
{
    /// <summary>
    /// Provides storage for a Vib-Ribbon PAK item entry and its information.
    /// </summary>
    public class VibRibbonPakFormatEntry : FormatEntry
    {
        // Internal members
        internal uint _offset = 0;
        internal uint _length = 0;
        internal uint _headerLength = 0;
        /// <summary>
        /// Initializes a new instance of the <see cref="VibRibbonPakFormatEntry"/> class as a directory.
        /// </summary>
        /// <param name="path">A relative path to the virtual directory from the root of the <see cref="VibRibbonPakFormat"/>.</param>
        public VibRibbonPakFormatEntry(string path) : base(path) { }

        /// <summary>
        /// Initializes a new of instance of the <see cref="VibRibbonPakFormatEntry"/> class as a file.
        /// </summary>
        /// <param name="path">A relative path to the virtual file from the root of the <see cref="VibRibbonPakFormat"/>.</param>
        /// <param name="source">The corresponding <see cref="Stream"/> where the virtual file is located.</param>
        public VibRibbonPakFormatEntry(string path, Stream source) : base(path, source) { }

        public override EntryProperties Extract(Stream outStream)
        {
            // Check for invalid call
            if (IsDirectory)
                throw new InvalidOperationException("Attempted to extract data from a directory VibRibbonPakFormatEntry.");

            // Seek to source data offset and extract data
            _source.Seek(_offset + _headerLength, SeekOrigin.Begin);
            Generic.SubStream(_source, outStream, _length);

            // No EntryProperties to write
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
            }
            else
            {
                _fields.Add("File Name", outPath);
                _fields.Add("Offset", $"0x{Convert.ToString(_offset, 16).ToUpper()}");
                _fields.Add("Length", _length.ToString());
            }

            return _fields;
        }

        /// <summary>
        /// Returns the length of the data of this <see cref="VibRibbonPakFormatEntry"/>.
        /// </summary>
        public uint Length { get { return _length; } }
        /// <summary>
        /// Returns the offset of this <see cref="VibRibbonPakFormatEntry"/>'s data within its source Stream.
        /// </summary>
        public uint Offset { get { return _offset; } }
    }
}
