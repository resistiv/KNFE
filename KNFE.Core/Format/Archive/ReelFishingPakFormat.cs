using System.Collections.Generic;
using System.IO;

namespace KNFE.Core.Format.Archive
{
    /// <summary>
    /// Reads and processes Reel Fishing's PAK/MB file format.
    /// </summary>
    public class ReelFishingPakFormat : Format
    {
        // Private members
        private readonly BinaryReader _br;
        private readonly ushort _version;
        private readonly ushort _fileCount;
        private readonly uint[] _fileOffsets;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReelFishingPakFormat"/> class from a file name.
        /// </summary>
        /// <param name="fileName">The name of a Reel Fishing PAK/MB file to process.</param>
        public ReelFishingPakFormat(string fileName)
            : base("Reel Fishing PAK", fileName)
        {
            _br = new BinaryReader(InFileStream);

            // Make root
            _root = new ReelFishingPakFormatEntry("");

            // Read number of files
            _version = _br.ReadUInt16();
            _fileCount = _br.ReadUInt16();
            _fileOffsets = new uint[_fileCount];

            // Read TOC
            for (int i = 0; i < _fileCount; i++)
            {
                _fileOffsets[i] = _br.ReadUInt32();
            }

            // Read file info
            string baseFileName = Path.GetFileName(FileName);
            for (int i = 0; i < _fileCount; i++)
            {
                // Create entry
                ReelFishingPakFormatEntry entry = new ReelFishingPakFormatEntry($"{baseFileName}[{i}]", _br.BaseStream);
                _root.AddChild(entry);

                // Add attributes
                entry._offset = _fileOffsets[i];
                if (i + 1 == _fileCount)
                {
                    entry._length = (uint)(_br.BaseStream.Length - _fileOffsets[i]);
                }
                else
                {
                    entry._length = _fileOffsets[i + 1] - _fileOffsets[i];
                }
            }
        }

        public override Dictionary<string, string> ToFields()
        {
            if (_fields != null)
            {
                return _fields;
            }

            base.ToFields();

            _fields.Add("Version", _version.ToString());
            _fields.Add("File Count", _fileCount.ToString());

            return _fields;
        }

        public override void Close()
        {
            _br.Close();
            base.Close();
        }

        /// <summary>
        /// Returns the version number of this <see cref="ReelFishingPakFormat"/>.
        /// </summary>
        public ushort Version { get { return _version; } }
        /// <summary>
        /// Returns the number of files contained within this <see cref="ReelFishingPakFormat"/>.
        /// </summary>
        public ushort FileCount { get { return _fileCount; } }
    }
}
