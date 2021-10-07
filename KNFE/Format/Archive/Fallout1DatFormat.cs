using KNFE.Encoding;
using KNFE.Encoding.Compression;
using KNFE.Util;
using System;
using System.IO;

namespace KNFE.Format.Archive
{
    /// <summary>
    /// Represents Fallout 1's DAT file format.
    /// </summary>
    public class Fallout1DatFormat : FileFormat
    {
        // https://falloutmods.fandom.com/wiki/DAT_file_format

        // Use BinaryReader since we're reading binary data
        private readonly BinaryReader br;

        // Info
        private readonly int dirCount;
        private readonly Fallout1DatDirectoryEntry[] dirEntries;

        // Constants
        private const int FLAG_TEXT = 0x20;
        private const int FLAG_LZSS = 0x40;

        public Fallout1DatFormat(string formatName, string fileName)
            : base(formatName, fileName)
        {
            br = new BinaryReader(inFileStream);

            // Directories
            dirCount = BigEndian.ConvertToLeInt(br.ReadInt32());
            dirEntries = new Fallout1DatDirectoryEntry[dirCount];

            // Unknown bytes
            br.BaseStream.Seek(12, SeekOrigin.Current);

            // Dir name block
            for (int i = 0; i < dirCount; i++)
            {
                dirEntries[i] = new Fallout1DatDirectoryEntry();
                dirEntries[i].dirName = ReadString();
            }

            // Dir content block
            foreach (Fallout1DatDirectoryEntry dir in dirEntries)
            {
                // Files
                dir.fileCount = BigEndian.ConvertToLeInt(br.ReadInt32());
                dir.fileEntries = new Fallout1DatFileEntry[dir.fileCount];

                // Unknown bytes
                br.BaseStream.Seek(12, SeekOrigin.Current);
                
                // Read in files
                //foreach (Fallout1DatFileEntry fe in dir.fileEntries)
                for (int i = 0; i < dir.fileCount; i++)
                {
                    dir.fileEntries[i] = new Fallout1DatFileEntry();

                    dir.fileEntries[i].fileName = ReadString();

                    // Attribute & validation
                    int compFlags = BigEndian.ConvertToLeInt(br.ReadInt32());
                    if (compFlags == FLAG_LZSS)
                        dir.fileEntries[i].compressed = true;
                    else if (compFlags == FLAG_TEXT)
                        dir.fileEntries[i].compressed = false;
                    else
                        Program.Quit($"Invalid file attribute received for file '{dir.dirName}\\{dir.fileEntries[i].fileName}', quitting.");

                    dir.fileEntries[i].offset = BigEndian.ConvertToLeInt(br.ReadInt32());
                    dir.fileEntries[i].originalLength = BigEndian.ConvertToLeInt(br.ReadInt32());
                    dir.fileEntries[i].compressedLength = BigEndian.ConvertToLeInt(br.ReadInt32());
                }
            }

            // File data block
            foreach (Fallout1DatDirectoryEntry dir in dirEntries)
            {
                foreach (Fallout1DatFileEntry fe in dir.fileEntries)
                {
                    br.BaseStream.Seek(fe.offset, SeekOrigin.Begin);
                    if (fe.compressed)
                        fe.data = new Fallout1LzssStream(new MemoryStream(br.ReadBytes(fe.compressedLength)));
                    else
                        fe.data = new BinaryStream(new MemoryStream(br.ReadBytes(fe.originalLength)));
                }
            }
        }

        /// <summary>
        /// Read a length-prefixed string.
        /// </summary>
        private string ReadString()
        {
            int stringLength = br.ReadByte();
            string outStr = System.Text.Encoding.ASCII.GetString(br.ReadBytes(stringLength));
            return outStr;
        }

        public override void ExportData()
        {
            Logger.LogInfo(ToString());

            string outputPath = CreateOutputPath();
            foreach (Fallout1DatDirectoryEntry dir in dirEntries)
            {
                Logger.LogInfo(dir.ToString());

                string dirOutPath;
                if (dir.dirName.Equals("."))
                {
                    // Ensures valid path, "." represents the root dir
                    dirOutPath = outputPath;
                }
                else
                {
                    dirOutPath = outputPath + dir.dirName;
                }

                // Create output subdirs
                if (!Directory.Exists(dirOutPath))
                {
                    Directory.CreateDirectory(dirOutPath);
                }

                // Process files
                foreach (Fallout1DatFileEntry fe in dir.fileEntries)
                {
                    Logger.LogInfo(fe.ToString());

                    string dataOutPath = dirOutPath + "\\" + fe.fileName;

                    using (MemoryStream file = fe.data.Decode())
                    {
                        WriteFileFromStream(dataOutPath, file);
                    }
                }
            }

            br.Close();
        }

        public override string ToString()
        {
            return base.ToString() +
                   $"\tDirectory count: {dirCount}\n";
        }
    }

    /// <summary>
    /// Represents a directory entry within a Fallout 1 DAT file.
    /// </summary>
    internal class Fallout1DatDirectoryEntry
    {
        public string dirName;
        public int fileCount;
        public Fallout1DatFileEntry[] fileEntries;

        public override string ToString()
        {
            return $"\t\tDirectory name: {dirName}\n" +
                   $"\t\tFile count: {fileCount}\n";
        }
    }

    /// <summary>
    /// Represents a file entry within a Fallout 1 DAT file.
    /// </summary>
    internal class Fallout1DatFileEntry
    {
        public string fileName;
        public bool compressed;
        public int offset;
        public int originalLength;
        public int compressedLength;
        public EncodingStream data;

        public override string ToString()
        {
            return $"\t\t\tFile name: {fileName}\n" +
                   $"\t\t\tCompressed: {compressed}\n" +
                   $"\t\t\tOffset: 0x{Convert.ToString(offset, 16).ToUpper()}\n" +
                   $"\t\t\tLength: 0x{Convert.ToString(originalLength, 16).ToUpper()}\n" +
                   $"\t\t\tCompressed length: 0x{Convert.ToString(compressedLength, 16).ToUpper()}\n";
        }
    }
}
