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
            dirCount = BigEndian.ToLeInt(br.ReadInt32());
            dirEntries = new Fallout1DatDirectoryEntry[dirCount];

            // Unknown bytes
            br.BaseStream.Seek(12, SeekOrigin.Current);

            // Dir name block
            for (int i = 0; i < dirCount; i++)
            {
                dirEntries[i] = new Fallout1DatDirectoryEntry();
                dirEntries[i].dirName = Tools.ReadByteString(br);
            }

            // Dir content block
            foreach (Fallout1DatDirectoryEntry dir in dirEntries)
            {
                // Files
                dir.fileCount = BigEndian.ToLeInt(br.ReadInt32());
                dir.fileEntries = new Fallout1DatFileEntry[dir.fileCount];

                // Unknown bytes
                br.BaseStream.Seek(12, SeekOrigin.Current);
                
                // Read in files
                //foreach (Fallout1DatFileEntry fe in dir.fileEntries)
                for (int i = 0; i < dir.fileCount; i++)
                {
                    dir.fileEntries[i] = new Fallout1DatFileEntry();

                    dir.fileEntries[i].fileName = Tools.ReadByteString(br);

                    // Attribute & validation
                    int compFlags = BigEndian.ToLeInt(br.ReadInt32());
                    if (compFlags == FLAG_LZSS)
                        dir.fileEntries[i].compressed = true;
                    else if (compFlags == FLAG_TEXT)
                        dir.fileEntries[i].compressed = false;
                    else
                        Program.Quit($"Invalid file attribute received for file '{dir.dirName}\\{dir.fileEntries[i].fileName}', quitting.");

                    // Info
                    dir.fileEntries[i].offset = BigEndian.ToLeInt(br.ReadInt32());
                    dir.fileEntries[i].originalLength = BigEndian.ToLeInt(br.ReadInt32());
                    dir.fileEntries[i].compressedLength = BigEndian.ToLeInt(br.ReadInt32());
                }
            }
        }

        public override void ExportData()
        {
            Logger.LogInfo(ToString());

            string outputPath = CreateOutputPath();

            // Iterate through directories
            for (int i = 0; i < dirCount; i++)
            {
                Logger.LogInfo(dirEntries[i].ToString());

                string dirOutPath;
                if (dirEntries[i].dirName.Equals("."))
                {
                    // Ensures valid path, "." represents the root dir
                    dirOutPath = outputPath;
                }
                else
                {
                    dirOutPath = outputPath + dirEntries[i].dirName;
                }

                // Create output subdirs
                if (!Directory.Exists(dirOutPath))
                {
                    Directory.CreateDirectory(dirOutPath);
                }

                // Process files
                for (int j = 0; j < dirEntries[i].fileCount; j++)
                {
                    Logger.LogInfo(dirEntries[i].fileEntries[j].ToString());

                    // Path for output file
                    string dataOutputPath = dirOutPath + "\\" + dirEntries[i].fileEntries[j].fileName;

                    // Initialize our output FileStream
                    FileStream outFile = null;
                    try
                    {
                        outFile = new FileStream(dataOutputPath, FileMode.OpenOrCreate, FileAccess.Write);
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(e);
                        Program.Quit("Quitting.");
                    }

                    // Seek to data and create a MemoryStream
                    br.BaseStream.Seek(dirEntries[i].fileEntries[j].offset, SeekOrigin.Begin);
                    if (dirEntries[i].fileEntries[j].compressed)
                    {
                        // If compressed, we initialize an LZSS stream
                        dirEntries[i].fileEntries[j].data = new Fallout1LzssStream(br.BaseStream, dirEntries[i].fileEntries[j].compressedLength);
                    }
                    else
                    {
                        // If binary data, initialize binary data stream
                        dirEntries[i].fileEntries[j].data = new BinaryStream(br.BaseStream, dirEntries[i].fileEntries[j].originalLength);
                    }

                    // Write file to output
                    dirEntries[i].fileEntries[j].data.Decode(outFile);

                    // If we have an incorrect output length, the file is likely corrupt; quit to avoid outputting even more corrupt files
                    if ((int)outFile.Length != dirEntries[i].fileEntries[j].originalLength)
                    {
                        Program.Quit($"Resulting length did not match file length, quitting.\n" +
                             $"\tResulting length: 0x{Convert.ToString(outFile.Length, 16).ToUpper()}\n" +
                             $"\tCorrect length: 0x{Convert.ToString(dirEntries[i].fileEntries[j].originalLength, 16).ToUpper()}");
                    }
                    outFile.Close();

                    // Dispose of resources to keep memory usage low, since big directories can consume memory like nobody's business
                    // Cuts down memory usage, albeit is a bit hacky
                    dirEntries[i].fileEntries[j].data.Stream = null;
                    dirEntries[i].fileEntries[j] = null;
                }

                // Dispose of resources
                dirEntries[i] = null;
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
