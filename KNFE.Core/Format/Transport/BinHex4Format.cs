using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using KNFE.Core.Checksum;
using KNFE.Core.Encoding;
using KNFE.Core.Encoding.Compression;
using KNFE.Core.Reader;

namespace KNFE.Core.Format.Transport
{
    /// <summary>
    /// Reads and processes the BinHex 4.0 file format.
    /// </summary>
    public class BinHex4Format : Format
    {
        // Private members
        private readonly StreamReader _sr;
        private readonly CrcBinaryReader _cbr;
        private readonly string _origFileName;
        private readonly int _version;
        private readonly string _fileType;
        private readonly string _creator;
        private readonly short _finderFlags;
        private readonly int _dataForkLen;
        private readonly int _rsrcForkLen;
        private readonly ushort _headerCrc;

        // Constants
        private const string MAGIC_ID = "(This file must be converted with BinHex 4.0)";

        /// <summary>
        /// Initializes a new instance of the <see cref="BinHex4Format"/> class from a file name.
        /// </summary>
        /// <param name="fileName">The name of a BinHex 4.0 file to process.</param>
        public BinHex4Format(string fileName)
            : base("BinHex 4.0", fileName)
        {
            _sr = new StreamReader(InFileStream);

            // Read magic
            string inMagic = _sr.ReadLine();
            if (!inMagic.StartsWith(MAGIC_ID))
                throw new InvalidDataException($"Expected header \"{MAGIC_ID}\" within a {FormatName} file.");

            // Fix offset from StreamReader buffering
            _sr.BaseStream.Seek(inMagic.Length, SeekOrigin.Begin);

            // BinHex is somewhat unique in the fact the entire file, header and all, are obscured behind its encoding(s).
            // Normally, we'd decode a certain section of the file when extracting one or more of its FormatEntries,
            // but in order to get any info we have to decode it all here.
            // However, this isn't too big of a problem, as BinHex files (in general) are small enough that memory usage isn't a concern.

            // Decode BinHex 4.0 layer
            MemoryStream bhd = new MemoryStream();
            BinHex4Stream bhs = new BinHex4Stream(InFileStream);
            bhs.Decode(bhd);
            bhd.Position = 0;

            // Decode RLE90 layer
            MemoryStream rd = new MemoryStream();
            Rle90Stream rs = new Rle90Stream(bhd);
            rs.Decode(rd);
            rd.Position = 0;

            // Dispose of BinHex decode stream, no need
            bhd.Close();

            // Read through final decoded data to get attributes
            _cbr = new CrcBinaryReader(rd, new Crc16Ccitt());

            // File name
            int origFileNameLen = _cbr.ReadByte();
            _origFileName = System.Text.Encoding.UTF8.GetString(_cbr.ReadBytes(origFileNameLen));

            // Everything else is self-explanatory
            _version = _cbr.ReadByte();
            _fileType = System.Text.Encoding.UTF8.GetString(_cbr.ReadBytes(4));
            _creator = System.Text.Encoding.UTF8.GetString(_cbr.ReadBytes(4));
            _finderFlags = _cbr.ReadInt16();
            _dataForkLen = BinaryPrimitives.ReverseEndianness(_cbr.ReadInt32());
            _rsrcForkLen = BinaryPrimitives.ReverseEndianness(_cbr.ReadInt32());

            // Header CRC calculation
            _cbr.Crc.UpdateCrc(0);
            _cbr.Crc.UpdateCrc(0);
            ushort calcCrc = (ushort)_cbr.Crc.Crc;
            _headerCrc = BinaryPrimitives.ReverseEndianness(_cbr.ReadUInt16());
            if (_headerCrc != calcCrc)
                throw new InvalidDataException($"Calculated CRC (0x{Convert.ToString(calcCrc, 16).ToUpper()}) did not match the CRC within the header (0x{Convert.ToString(_headerCrc, 16).ToUpper()}) of a {FormatName} file.");

            // Root
            _root = new BinHex4FormatEntry("");

            // Add data fork
            rd.Position = rd.Position + _dataForkLen;
            _root.AddChild(new BinHex4FormatEntry(_origFileName, rd)
            {
                _offset = rd.Position - _dataForkLen,
                _forkLength = _dataForkLen,
                _crc = BinaryPrimitives.ReverseEndianness(_cbr.ReadUInt16()),
                _isDataFork = true
            });

            // Add resource fork
            rd.Position = rd.Position + _rsrcForkLen;
            _root.AddChild(new BinHex4FormatEntry($"._{_origFileName}", rd)
            {
                _offset = rd.Position + - _rsrcForkLen,
                _forkLength = _rsrcForkLen,
                _crc = BinaryPrimitives.ReverseEndianness(_cbr.ReadUInt16()),
                _isDataFork = false
            });
        }

        public override Dictionary<string, string> ToFields()
        {
            if (_fields != null)
                return _fields;

            base.ToFields();

            _fields.Add("Version", _version.ToString());
            _fields.Add("File Type", _fileType);
            _fields.Add("Creator", _creator);
            _fields.Add("Finder Flags", $"0x{Convert.ToString(_finderFlags, 16).ToUpper()}");
            _fields.Add("Header CRC", $"0x{Convert.ToString(_headerCrc, 16).ToUpper()}");

            return _fields;
        }

        public override void Close()
        {
            _sr.Close();
            _cbr.Close();
            base.Close();
        }

        /// <summary>
        /// Returns the file version of this <see cref="BinHex4Format"/>.
        /// </summary>
        public int Version { get { return _version; } }

        /// <summary>
        /// Returns the file type of this <see cref="BinHex4Format"/>.
        /// </summary>
        public string FileType { get { return _fileType; } }

        /// <summary>
        /// Returns the file creator of this <see cref="BinHex4Format"/>.
        /// </summary>
        public string Creator { get { return _creator; } }

        /// <summary>
        /// Returns the Finder flags of this <see cref="BinHex4Format"/>.
        /// </summary>
        public short FinderFlags { get { return _finderFlags; } }

        /// <summary>
        /// Returns the header CRC of this <see cref="BinHex4Format"/>.
        /// </summary>
        public ushort HeaderCrc { get { return _headerCrc; } }
    }
}
