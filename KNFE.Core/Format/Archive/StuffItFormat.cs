using KNFE.Core.Checksum;
using KNFE.Core.Reader;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;

namespace KNFE.Core.Format.Archive
{
    /// <summary>
    /// Reads and processes the classic (up to v4.5) StuffIt file format.
    /// </summary>
    public class StuffItFormat : Format
    {
        // Private members
        private readonly CrcBinaryReader _cbr;
        private readonly ushort _entryCount;
        private readonly uint _archiveLen;
        private readonly byte _version;
        private readonly uint _headerLen;
        private readonly ushort _headerCrc;

        // Constants
        private const string MAGIC_ID = "SIT!";
        private const string SIGNATURE = "rLau";

        public StuffItFormat(string fileName)
            : base("StuffIt", fileName)
        {
            _cbr = new CrcBinaryReader(InFileStream, new Crc16Arc());

            // Read magic
            string inMagic = new string(_cbr.ReadChars(4));
            if (!inMagic.Equals(MAGIC_ID))
                throw new InvalidDataException($"Expected magic ID \"{MAGIC_ID}\" within a {FormatName} file.");

            _entryCount = BinaryPrimitives.ReverseEndianness(_cbr.ReadUInt16());
            _archiveLen = BinaryPrimitives.ReverseEndianness(_cbr.ReadUInt16());

            // Read signature
            string inSig = new string(_cbr.ReadChars(4));
            if (!inSig.Equals(SIGNATURE))
                throw new InvalidDataException($"Expected signature \"{SIGNATURE}\" within a {FormatName} file.");

            _version = _cbr.ReadByte();
            // Consume reserved byte for CRC
            _cbr.ReadByte();
            _headerLen = BinaryPrimitives.ReverseEndianness(_cbr.ReadUInt32());
            _headerCrc = BinaryPrimitives.ReverseEndianness(_cbr.ReadUInt16());

        }

        public override Dictionary<string, string> ToFields()
        {
            if (_fields != null)
                return _fields;

            base.ToFields();

            _fields.Add("Entry Count", _entryCount.ToString());
            _fields.Add("Archive Length", $"0x{Convert.ToString(_archiveLen, 16).ToUpper()}");
            _fields.Add("StuffIt Version", _version == 0x01 ? "v1.5.x and below" : "v1.6 to v4.5");
            _fields.Add("Header Length", $"0x{Convert.ToString(_headerLen, 16).ToUpper()}");
            _fields.Add("Header CRC", $"0x{Convert.ToString(_headerCrc, 16).ToUpper()}");

            return _fields;
        }

        public override void Close()
        {
            _cbr.Close();
            base.Close();
        }

        /// <summary>
        /// Returns the number of entries in this <see cref="StuffItFormat"/>.
        /// </summary>
        public ushort EntryCount { get { return _entryCount; } }

        /// <summary>
        /// Returns the length of this <see cref="StuffItFormat"/> archive.
        /// </summary>
        public uint ArchiveLength { get { return _archiveLen; } }

        /// <summary>
        /// Returns the version of this <see cref="StuffItFormat"/>.
        /// </summary>
        public int Version { get { return _version; } }

        /// <summary>
        /// Returns the header length of this <see cref="StuffItFormat"/>.
        /// </summary>
        public uint HeaderLength { get { return _headerLen; } }

        /// <summary>
        /// Returns the CRC of the header of this <see cref="StuffItFormat"/>.
        /// </summary>
        public ushort HeaderCrc { get { return _headerCrc; } }
    }
}
