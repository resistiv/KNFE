using KNFE.Checksum;
using KNFE.Checksum.Reader;
using KNFE.Encoding;
using KNFE.Encoding.Compression;
using KNFE.Util;
using System;
using System.IO;

namespace KNFE.Format
{
    /// <summary>
    /// Represents a BinHex 4.0 file.
    /// </summary>
    public class BinHex4Format : FileFormat
    {
        // Start indicator
        private const string HEADER = "(This file must be converted with BinHex";
        private const char STREAM_MARKER = ':';

        private readonly string newLineConfig;

        // Streams
        private readonly StreamReader sr;
        private CrcBinaryReader br;
        private BinHex4Stream bhStream;
        private Rle90Stream rleStream;

        // Info
        private BinHex4Header header;

        public BinHex4Format(string formatName, string fileName)
            : base(formatName, fileName)
        {
            // It's much easier to manipulate a string rather than a StreamReader, and the size of most BinHex'd files is negligible in terms of modern computing, so we read in the entire file at once to more easily handle it.
            sr = new StreamReader(inFileStream);
            string fullBhFile = sr.ReadToEnd();
            sr.Close();

            // Identify newline pattern in order to properly create a single stream
            newLineConfig = Tools.FindNewLine(fullBhFile);

            if (newLineConfig.Equals(string.Empty))
            {
                Program.Quit("Could not identify newline configuration, quitting.");
            }

            // Find start of BinHex section
            int fileStart = fullBhFile.IndexOf(HEADER);
            
            if (fileStart == -1)
            {
                Program.Quit($"Could not find {base.formatName} header, quitting.");
            }

            // Trim beginning
            fullBhFile = fullBhFile.Substring(fileStart);

            // Find our stream start; validation
            int streamStart = fullBhFile.IndexOf($"{STREAM_MARKER}");

            if (streamStart == -1)
            {
                Program.Quit($"Could not identify start of {base.formatName} stream, quitting.");
            }

            // Trim to stream start
            fullBhFile = fullBhFile.Substring(streamStart);

            // Find stream end; we could use LastIndexOf(), but we aren't sure if there will be data after the stream that contains ":", such as in the case of Usenet messages, so to be safe we find the next occurence of it after the stream start
            int streamEnd = fullBhFile.Substring(1).IndexOf($"{STREAM_MARKER}") + 2;

            if (streamEnd == -1)
            {
                Program.Quit($"Could not identify end of {base.formatName} stream, quitting.");
            }

            // Trim down the final stream string and delete our newlines
            string streamString = fullBhFile.Substring(0, streamEnd).Replace(newLineConfig, string.Empty);

            // Create our stream
            bhStream = new BinHex4Stream(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(streamString)));
        }

        public override void ExportData()
        {
            string outputPath = CreateOutputPath();

            // Decode all our data
            MemoryStream bhDecoded = bhStream.Decode();

            rleStream = new Rle90Stream(bhDecoded);
            MemoryStream rleDecoded = rleStream.Decode();

            // Read through our final decoded data to validate
            br = new CrcBinaryReader(rleDecoded, new Crc16Ccitt(false));
            header = new BinHex4Header();

            // Read header information
            header.originalFileName = Tools.ReadByteString(br);
            header.version = br.ReadByte();
            header.fileType = System.Text.Encoding.UTF8.GetString(br.ReadBytes(4));
            header.creator = System.Text.Encoding.UTF8.GetString(br.ReadBytes(4));
            header.finderFlags = br.ReadInt16();
            header.dataForkLength = BigEndian.ToLeInt(br.ReadInt32());
            header.rsrcForkLength = BigEndian.ToLeInt(br.ReadInt32());

            // Header CRC calculation
            // We include these two bytes of 0x00 to supplement where the CRC would be, as specified in Peter N. Lewis' BinHex 4.0 Definition
            br.Crc.UpdateCrc(0);
            br.Crc.UpdateCrc(0);
            ushort calcHeaderCrc = (ushort)br.Crc.GetCrc();
            header.headerCrc = (ushort)BigEndian.ToLeShort(br.ReadInt16());

            // Validate header
            if (header.headerCrc != calcHeaderCrc)
            {
                Program.Quit($"Calculated CRC did not match CRC in header\n" +
                             $"\tCalculated CRC: 0x{Convert.ToString(br.Crc.GetCrc(), 16).ToUpper()}\n" +
                             $"\tHeader CRC: 0x{Convert.ToString(header.headerCrc, 16).ToUpper()}");
            }
            br.Crc.ResetCrc();

            // Read data fork
            MemoryStream dataFork = new MemoryStream(br.ReadBytes(header.dataForkLength));

            // Data CRC
            br.Crc.UpdateCrc(0);
            br.Crc.UpdateCrc(0);
            ushort calcDataCrc = (ushort)br.Crc.GetCrc();
            header.dataForkCrc = (ushort)BigEndian.ToLeShort(br.ReadInt16());

            // Validate
            if (header.dataForkCrc != calcDataCrc)
            {
                Program.Quit($"Calculated CRC did not match CRC in data fork\n" +
                             $"\tCalculated CRC: 0x{Convert.ToString(br.Crc.GetCrc(), 16).ToUpper()}\n" +
                             $"\tData Fork CRC: 0x{Convert.ToString(header.headerCrc, 16).ToUpper()}");
            }
            br.Crc.ResetCrc();

            // Read resource fork
            MemoryStream rsrcFork = new MemoryStream(br.ReadBytes(header.rsrcForkLength));

            // Resource CRC
            br.Crc.UpdateCrc(0);
            br.Crc.UpdateCrc(0);
            ushort calcRsrcCrc = (ushort)br.Crc.GetCrc();
            header.rsrcForkCrc = (ushort)BigEndian.ToLeShort(br.ReadInt16());
            
            // Validate
            if (header.rsrcForkCrc != calcRsrcCrc)
            {
                Program.Quit($"Calculated CRC did not match CRC in resource fork\n" +
                             $"\tCalculated CRC: 0x{Convert.ToString(br.Crc.GetCrc(), 16).ToUpper()}\n" +
                             $"\tResource Fork CRC: 0x{Convert.ToString(header.headerCrc, 16).ToUpper()}");
            }
            br.Crc.ResetCrc();

            string dataForkOut = $"{outputPath}{header.originalFileName}";
            string rsrcForkOut = $"{outputPath}._{header.originalFileName}";

            Logger.LogInfo(ToString());

            WriteFileFromStream(dataForkOut, dataFork);
            WriteFileFromStream(rsrcForkOut, rsrcFork);

            sr.Close();
            br.Close();
        }

        public override string ToString()
        {
            return base.ToString() +
                   $"\tOutput file name: {header.originalFileName}\n" +
                   $"\tFile version: {header.version}\n" +
                   $"\tFile type: {header.fileType}\n" +
                   $"\tFile creator: {header.creator}\n" +
                   $"\tFinder flags: 0x{Convert.ToString(header.finderFlags, 16).ToUpper()}\n" +
                   $"\tHeader CRC: 0x{Convert.ToString(header.headerCrc, 16).ToUpper()}\n" +
                   $"\tData fork length: 0x{Convert.ToString(header.dataForkLength, 16).ToUpper()}\n" +
                   $"\tData fork CRC: 0x{Convert.ToString(header.dataForkCrc, 16).ToUpper()}\n" +
                   $"\tResource fork length: 0x{Convert.ToString(header.rsrcForkLength, 16).ToUpper()}\n" +
                   $"\tResource fork CRC: 0x{Convert.ToString(header.rsrcForkCrc, 16).ToUpper()}\n";
        }
    }

    /// <summary>
    /// Represents the header of a decoded BinHex 4.0 file.
    /// </summary>
    internal class BinHex4Header
    {
        public string originalFileName;
        public byte version;
        public string fileType;
        public string creator;
        public short finderFlags;
        public int dataForkLength;
        public int rsrcForkLength;
        public ushort headerCrc;
        public ushort dataForkCrc;
        public ushort rsrcForkCrc;
    }
}
