using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;

namespace KNFE.Core.Format.Archive
{
    /// <summary>
    /// Reads and processes Fallout 1's DAT file format.
    /// </summary>
    public class Fallout1DatFormat : Format
    {
        // Private members
        private readonly BinaryReader _br;
        private readonly int _dirCount;

        // Private constants
        private const int FLAG_TEXT = 0x20;
        private const int FLAG_LZSS = 0x40;

        /// <summary>
        /// Initializes a new instance of the <see cref="Fallout1DatFormat"/> class from a file name.
        /// </summary>
        /// <param name="fileName">The name of a Fallout 1 DAT file to process.</param>
        public Fallout1DatFormat(string fileName)
            : base("Fallout 1 DAT", fileName)
        {
            // Init reader
            _br = new BinaryReader(InFileStream);

            // Read expected number of directories and init root dir
            _dirCount = BinaryPrimitives.ReverseEndianness(_br.ReadInt32());
            if (_dirCount < 1)
                throw new InvalidDataException($"Received invalid directory count ({_dirCount}) within a {FormatName} file.");

            _root = new Fallout1DatFormatEntry("");

            // Skip unknown bytes; doesn't matter to processing
            _br.BaseStream.Seek(12, SeekOrigin.Current);

            // Read directory names block
            for (int i = 0; i < _dirCount; i++)
            {
                // string tempName = _br.ReadString();
                string tempName = "";
                int tempNameLen = _br.ReadByte();
                while (tempNameLen-- > 0)
                    tempName += _br.ReadChar();

                _root.AddChild(new Fallout1DatFormatEntry(tempName));
            }

            // Read directory contents block
            foreach (Fallout1DatFormatEntry dir in _root.Children)
            {
                // Get file count of the current directory
                int dirFileCount = BinaryPrimitives.ReverseEndianness(_br.ReadInt32());
                if (dirFileCount < 1)
                    throw new InvalidDataException($"Received invalid directory file count ({dirFileCount}) within a {FormatName} file entry.");

                // Record item count
                dir._fileCount = dirFileCount;

                // Skip unknown bytes; doesn't matter to processing
                _br.BaseStream.Seek(12, SeekOrigin.Current);

                // Read file information block
                for (int i = 0; i < dirFileCount; i++)
                {
                    // string tempFileName = _br.ReadString();
                    string tempFileName = "";
                    int tempNameLen = _br.ReadByte();
                    while (tempNameLen-- > 0)
                        tempFileName += _br.ReadChar();

                    Fallout1DatFormatEntry tempEntry = new Fallout1DatFormatEntry(tempFileName, InFileStream);

                    // Read compression attribute
                    int compFlags = BinaryPrimitives.ReverseEndianness(_br.ReadInt32());
                    switch (compFlags)
                    {
                        case FLAG_LZSS:
                            tempEntry._isCompressed = true;
                            break;
                        case FLAG_TEXT:
                            tempEntry._isCompressed = false;
                            break;
                        default:
                            throw new InvalidDataException($"Received invalid compression flag (0x{Convert.ToString(compFlags, 16).ToUpper()}) within a {FormatName} file entry.");
                    }

                    // Read remaining file attributes
                    tempEntry._offset = BinaryPrimitives.ReverseEndianness(_br.ReadInt32());
                    if (tempEntry._offset > _br.BaseStream.Length)
                        throw new InvalidDataException($"Received invalid data offset (0x{Convert.ToString(tempEntry._offset, 16).ToUpper()}) within a {FormatName} file entry.");

                    tempEntry._originalLength = BinaryPrimitives.ReverseEndianness(_br.ReadInt32());

                    tempEntry._compressedLength = BinaryPrimitives.ReverseEndianness(_br.ReadInt32());
                    if (tempEntry._compressedLength + tempEntry._offset > _br.BaseStream.Length)
                        throw new InvalidDataException($"Received invalid data length (0x{Convert.ToString(tempEntry._compressedLength, 16).ToUpper()}) within a {FormatName} file entry.");

                    // Add to our working directory
                    dir.AddChild(tempEntry);
                }
            }

            NormalizeStructure();
        }

        public override Dictionary<string, string> ToFields()
        {
            if (_fields != null)
                return _fields;

            base.ToFields();

            _fields.Add("Directory Count", _dirCount.ToString());

            return _fields;
        }

        public override void Close()
        {
            _br.Close();
            base.Close();
        }

        /// <summary>
        /// Normalizes the virtual file tree to allow for proper processing through KNFE.UI.
        /// </summary>
        private void NormalizeStructure()
        {
            // New root
            Fallout1DatFormatEntry tempRoot = new Fallout1DatFormatEntry("");
            // For stylistic ordering, we note the pseudo-root (directory ".") to add files to bottom of tree
            Fallout1DatFormatEntry pseudoRoot = null;

            foreach (Fallout1DatFormatEntry fe in _root.Children)
            {
                // Start with full path (coming from a DAT file, directories are fully-qualified)
                string dirBuffer = fe.ItemPath;
                // Start with our new root, work down
                Fallout1DatFormatEntry curEntry = tempRoot;
                while (dirBuffer.Length != 0)
                {
                    // Pseudo-root, set and break out!!
                    if (dirBuffer == ".")
                    {
                        pseudoRoot = fe;
                        break;
                    }

                    // Trim directory name
                    string curDir;
                    int slashIndex = dirBuffer.IndexOf('\\');
                    if (slashIndex == -1)
                    {
                        curDir = dirBuffer;
                        dirBuffer = string.Empty;
                    }
                    else
                    {
                        curDir = dirBuffer.Substring(0, slashIndex);
                        dirBuffer = dirBuffer.Substring(slashIndex + 1);
                    }

                    // If our directory already exists, get it
                    Fallout1DatFormatEntry tempEntry = (Fallout1DatFormatEntry)curEntry.GetChild(curDir);
                    // If not, make it and add to tree
                    if (tempEntry == null)
                    {
                        tempEntry = new Fallout1DatFormatEntry(curDir);
                        curEntry.AddChild(tempEntry);
                    }
                    curEntry = tempEntry;
                }

                // Pseudo-root, we don't want to add the files yet so we continue
                if (fe == pseudoRoot)
                    continue;

                // Set only attribute
                curEntry._fileCount = fe.FileCount;

                // Add files
                foreach (Fallout1DatFormatEntry fde in fe.Children)
                {
                    curEntry.AddChild(fde);
                }
            }

            // Add pseudo-root files if a pseudo-root exists
            if (pseudoRoot != null)
            {
                foreach (Fallout1DatFormatEntry fe in pseudoRoot.Children)
                {
                    tempRoot.AddChild(fe);
                }
            }

            _root = tempRoot;
            GC.Collect();
        }

        /// <summary>
        /// Returns the number of directories contained within this <see cref="Fallout1DatFormat"/>.
        /// </summary>
        public int DirectoryCount { get { return _dirCount; } }
    }
}