using System;
using System.Collections.Generic;
using System.IO;
using KNFE.Core.Checksum;
using KNFE.Core.Reader;

namespace KNFE.Core.Format
{
    /// <summary>
    /// Provides storage for a BinHex 4.0 item entry and its information.
    /// </summary>
    public class BinHex4FormatEntry : FormatEntry
    {
        // Internal members
        internal long _startOffset;
        internal int _forkLength;
        internal ushort _crc;
        internal bool _isDataFork;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinHex4FormatEntry"/> class as a directory.
        /// </summary>
        /// <param name="path">A relative path to the virtual directory from the root of the <see cref="BinHex4Format"/>.</param>
        public BinHex4FormatEntry(string path) : base(path) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinHex4FormatEntry"/> class as a file.
        /// </summary>
        /// <param name="path">A relative path to the virtual file from the root of the <see cref="BinHex4Format"/>.</param>
        public BinHex4FormatEntry(string path, Stream source) : base(path, source) { }

        public override EntryProperties Extract(Stream outStream)
        {
            // Check for invalid call
            if (IsDirectory)
                throw new InvalidOperationException("Attempted to extract data from a directory BinHex4FormatEntry.");

            // Read out data
            _source.Seek(_startOffset, SeekOrigin.Begin);
            CrcBinaryReader _cbr = new CrcBinaryReader(_source, new Crc16Ccitt());
            outStream.Write(_cbr.ReadBytes(_forkLength), 0, _forkLength);

            // Check CRC
            _cbr.Crc.UpdateCrc(0);
            _cbr.Crc.UpdateCrc(0);
            ushort calcCrc = (ushort)_cbr.Crc.Crc;
            if (calcCrc != _crc)
                throw new InvalidDataException($"Calculated CRC (0x{Convert.ToString(calcCrc, 16).ToUpper()}) did not match the data CRC (0x{Convert.ToString(_crc, 16).ToUpper()}) of a BinHex4FormatEntry.");

            // No info for EntryProperties, so null
            return null;
        }

        public override Dictionary<string, string> ToFields()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            
            if (_isDataFork)
            {
                dict.Add("Data Fork Name", ItemPath);
                dict.Add("Data Fork Length", _forkLength.ToString());
            }
            else
            {
                dict.Add("Resource Fork Name", ItemPath);
                dict.Add("Resource Fork Length", _forkLength.ToString());
            }
            dict.Add("CRC", $"0x{Convert.ToString(_crc, 16).ToUpper()}");

            return dict;
        }

        /// <summary>
        /// Returns the length of this <see cref="BinHex4FormatEntry"/>.
        /// </summary>
        public int ForkLength { get { return _forkLength; } }

        /// <summary>
        /// Returns the stored CRC of this <see cref="BinHex4FormatEntry"/>.
        /// </summary>
        public ushort Crc { get { return _crc; } }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="BinHex4FormatEntry"/> is a data fork, and <c>false</c> if it is a resource fork.
        /// </summary>
        public bool IsDataFork { get { return _isDataFork; } }
    }
}
