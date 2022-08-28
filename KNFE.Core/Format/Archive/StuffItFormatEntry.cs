using System;
using System.IO;

namespace KNFE.Core.Format.Archive
{
    /// <summary>
    /// Provides storage for a classic (up to v4.5) StuffIt item entry and its information.
    /// </summary>
    public class StuffItFormatEntry : FormatEntry
    {
        // Internal members
        internal byte _rsrcCompMethod;
        internal byte _dataCompMethod;
        internal string _entryName;
        internal ushort _entryNameCrc;
        internal ushort _dirItemCount;
        internal uint _prevEntryOffset;
        internal uint _nextEntryOffset;
        internal uint _parentEntryOffset;
        internal int _firstChildEntryOffset;
        internal string _entryType;
        internal string _entryCreator;
        internal ushort _finderFlags;
        internal DateTime _creation;
        internal DateTime _lastModified;
        internal uint _origRsrcLen;
        internal uint _origDataLen;
        internal uint _compRsrcLen;
        internal uint _compDataLen;
        internal ushort _rsrcCrc;
        internal ushort _dataCrc;
        internal byte _rsrcPaddingLen;
        internal byte _dataPaddingLen;
        internal ushort _entryHeaderCrc;

        // Private constants
        private const int NOT_COMPRESSED = 0;
        private const int RLE90 = 1;
        private const int LZW = 2;
        private const int HUFFMAN = 3;
        /// <summary>
        /// Initializes a new instance of the <see cref="StuffItFormatEntry"/> class as a directory.
        /// </summary>
        /// <param name="path">A relative path to the virtual directory from the root of the <see cref="StuffItFormat"/>.</param>
        public StuffItFormatEntry(string path) : base(path) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StuffItFormatEntry"/> class as a file.
        /// </summary>
        /// <param name="path">A relative path to the virtual file from the root of the <see cref="StuffItFormat"/>.</param>
        public StuffItFormatEntry(string path, Stream source) : base(path, source) { }

        public override EntryProperties Extract(Stream outStream)
        {
            return base.Extract(outStream);
        }
    }
}
