using System.Collections.Generic;
using System.IO;

namespace KNFE.Core.Format.Archive
{
    /// <summary>
    /// Reads and processes Vib-Ribbon's PAK file format.
    /// </summary>
    public class VibRibbonPakFormat : Format
    {
        // Private members
        private readonly BinaryReader _br;
        private readonly int _fileCount;
        private readonly int[] _fileOffsets;

        /// <summary>
        /// Initializes a new instance of the <see cref="VibRibbonPakFormat"/> class from a file name.
        /// </summary>
        /// <param name="fileName">The name of a Vib-Ribbon PAK file to process.</param>
        public VibRibbonPakFormat(string fileName)
            : base("Vib-Ribbon PAK", fileName)
        {
            _br = new BinaryReader(InFileStream);

            // Make root
            _root = new VibRibbonPakFormatEntry("");

            // Read number of files
            _fileCount = _br.ReadInt32();
            _fileOffsets = new int[_fileCount];

            // Read TOC
            for (int i = 0; i < _fileCount; i++)
                _fileOffsets[i] = _br.ReadInt32();

            // Read file info
            for (int i = 0; i < _fileCount; i++)
            {
                // Seek to file
                _br.BaseStream.Seek(_fileOffsets[i], SeekOrigin.Begin);

                // Read file name
                char currentChar;
                string subFileName = "";
                while ((currentChar = _br.ReadChar()) != '\0')
                    subFileName += currentChar;
                // The file name is null-padded to the nearest 4-byte border, so read to that border
                _br.BaseStream.Seek(3 - (subFileName.Length % 4), SeekOrigin.Current);

                // Construct directory
                string dirBuf = subFileName;
                VibRibbonPakFormatEntry curEntry = (VibRibbonPakFormatEntry)_root;
                while (dirBuf.Length != 0)
                {
                    int slashIndex = dirBuf.IndexOf('/');
                    // File
                    if (slashIndex == -1)
                    {
                        // Create file and add to final dir
                        VibRibbonPakFormatEntry temp = new VibRibbonPakFormatEntry(dirBuf, InFileStream);
                        curEntry.AddChild(temp);

                        // Set to file and empty buffer
                        curEntry = temp;
                        dirBuf = string.Empty;
                    }
                    // Directory
                    else
                    {
                        // Get topmost directory name
                        string curDir = dirBuf.Substring(0, slashIndex);

                        // Does it already exist?
                        VibRibbonPakFormatEntry tempEntry = (VibRibbonPakFormatEntry)curEntry.GetChild(curDir);
                        if (tempEntry == null)
                        {
                            // If not, create and add
                            tempEntry = new VibRibbonPakFormatEntry(curDir);
                            curEntry.AddChild(tempEntry);
                        }
                        curEntry = tempEntry;

                        // Move directory buffer
                        dirBuf = dirBuf.Substring(slashIndex + 1);
                    }
                }

                // Add attributes
                curEntry._length = _br.ReadInt32();
                curEntry._offset = _fileOffsets[i];
                curEntry._headerLength = (int)(_br.BaseStream.Position - _fileOffsets[i]);
            }
        }

        public override Dictionary<string, string> ToFields()
        {
            if (_fields != null)
                return _fields;

            base.ToFields();

            _fields.Add("File Count", _fileCount.ToString());

            return _fields;
        }

        public override void Close()
        {
            _br.Close();
            base.Close();
        }

        /// <summary>
        /// Returns the number of files contained within this <see cref="VibRibbonPakFormat"/>.
        /// </summary>
        public int FileCount { get { return _fileCount; } }
    }
}
