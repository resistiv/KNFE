using System.IO;
using KNFE.Core.Format.Transport;
using KNFE.Helper;

namespace KNFE.Test
{
    /// <summary>
    /// Handles all tests related to the BinHex 4.0 format.
    /// </summary>
    internal static class BinHex4Tester
    {
        private const string LogSig = "BinHex 4.0";

        /// <summary>
        /// Runs all tests related to the BinHex 4.0 format.
        /// </summary>
        internal static void RunAllTests()
        {
            CybergifTest();
            EarthTest();
        }

        /// <summary>
        /// Tests the 'cybergif.hqx' file and its output validity.
        /// </summary>
        private static void CybergifTest()
        {
            string filename = "../../BinHex/cybergif.hqx";
            Log.Info($"[{LogSig}] Running '{Path.GetFileName(filename)}' test...");

            // Instantiate
            BinHex4Format cybergif = new BinHex4Format(filename);

            // Baseline assertions
            Program.Assert(cybergif.FormatName == "BinHex 4.0", $"[{LogSig}] Wrong FormatName, check constructor!");
            Program.Assert(cybergif.Version == 0, $"[{LogSig}] Wrong Version, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(cybergif.FileType == "APPL", $"[{LogSig}] Wrong FileType, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(cybergif.Creator == "EXTR", $"[{LogSig}] Wrong Creator, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(cybergif.FinderFlags == 0x20, $"[{LogSig}] Wrong FinderFlags, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(cybergif.HeaderCrc == (ushort)0xFE1B, $"[{LogSig}] Wrong HeaderCrc, check constructor, BinHexStream, and Rle90Stream!");

            // Child assertions
            BinHex4FormatEntry data = (BinHex4FormatEntry)cybergif.Root.Children[0];
            Program.Assert(data.ItemPath == "CyberGif 1.1.sea", $"[{LogSig}] Incorrect data fork name, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.IsDataFork, $"[{LogSig}] Data fork marked as resource fork, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.ForkLength == 25562, $"[{LogSig}] Incorrect data fork length, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.Crc == (ushort)0x8955, $"[{LogSig}] Incorrect data fork CRC, check constructor, BinHexStream, and Rle90Stream!");

            BinHex4FormatEntry rsrc = (BinHex4FormatEntry)cybergif.Root.Children[1];
            Program.Assert(rsrc.ItemPath == "._CyberGif 1.1.sea", $"[{LogSig}] Incorrect resource fork name, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(!rsrc.IsDataFork, $"[{LogSig}] Resource fork marked as data fork, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(rsrc.ForkLength == 13273, $"[{LogSig}] Incorrect resource fork length, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(rsrc.Crc == (ushort)0xEFE8, $"[{LogSig}] Incorrect resource fork CRC, check constructor, BinHexStream, and Rle90Stream!");

            // Output file assertions
            ExtractHandler.ExtractDirectory($"{filename}_out", cybergif.Root);

            Program.Assert(File.Exists($"{filename}_out/CyberGif 1.1.sea"), $"[{LogSig}] Data fork not written to disk, check Extract()!");
            Program.Assert(File.Exists($"{filename}_out/._CyberGif 1.1.sea"), $"[{LogSig}] Resource fork not written to disk, check Extract()!");

            Program.Assert(Program.CalculateSHA256($"{filename}_out/CyberGif 1.1.sea") == "343C16D546F9B399BA24EEF9DF70570F6E90B91DC0FFE6C6FAFF581A02BAFA63", $"[{LogSig}] Data fork SHA256 does not match, check Extract()!");
            Program.Assert(Program.CalculateSHA256($"{filename}_out/._CyberGif 1.1.sea") == "DD2BBA61EEE98B8FB7A5B5AF70EE1F1B186EFB32EAFAAD918BD911853AEDCE9E", $"[{LogSig}] Resource fork SHA256 does not match, check Extract()!");

            // Clear rubble
            cybergif.Close();
            Directory.Delete($"{filename}_out", true);

            Log.Info($"[{LogSig}] '{Path.GetFileName(filename)}' test complete.\n");
        }

        /// <summary>
        /// Tests the 'earth.hqx' file and its output validity.
        /// </summary>
        private static void EarthTest()
        {
            string filename = "../../BinHex/earth.hqx";
            Log.Info($"[{LogSig}] Running '{Path.GetFileName(filename)}' test...");

            // Instantiate
            BinHex4Format earth = new BinHex4Format(filename);

            // Baseline assertions
            Program.Assert(earth.FormatName == "BinHex 4.0", $"[{LogSig}] Wrong FormatName, check constructor!");
            Program.Assert(earth.Version == 0, $"[{LogSig}] Wrong Version, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(earth.FileType == "GIFf", $"[{LogSig}] Wrong FileType, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(earth.Creator == "QGif", $"[{LogSig}] Wrong Creator, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(earth.FinderFlags == 0x0, $"[{LogSig}] Wrong FinderFlags, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(earth.HeaderCrc == (ushort)0x5D1A, $"[{LogSig}] Wrong HeaderCrc, check constructor, BinHexStream, and Rle90Stream!");

            // Child assertions
            BinHex4FormatEntry data = (BinHex4FormatEntry)earth.Root.Children[0];
            Program.Assert(data.ItemPath == "earth.gif", $"[{LogSig}] Incorrect data fork name, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.IsDataFork, $"[{LogSig}] Data fork marked as resource fork, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.ForkLength == 404505, $"[{LogSig}] Incorrect data fork length, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.Crc == (ushort)0x8E70, $"[{LogSig}] Incorrect data fork CRC, check constructor, BinHexStream, and Rle90Stream!");

            BinHex4FormatEntry rsrc = (BinHex4FormatEntry)earth.Root.Children[1];
            Program.Assert(rsrc.ItemPath == "._earth.gif", $"[{LogSig}] Incorrect resource fork name, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(!rsrc.IsDataFork, $"[{LogSig}] Resource fork marked as data fork, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(rsrc.ForkLength == 0, $"[{LogSig}] Incorrect resource fork length, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(rsrc.Crc == (ushort)0x0, $"[{LogSig}] Incorrect resource fork CRC, check constructor, BinHexStream, and Rle90Stream!");

            // Output file assertions
            ExtractHandler.ExtractDirectory($"{filename}_out", earth.Root);

            Program.Assert(File.Exists($"{filename}_out/earth.gif"), $"[{LogSig}] Data fork not written to disk, check Extract()!");
            Program.Assert(File.Exists($"{filename}_out/._earth.gif"), $"[{LogSig}] Resource fork not written to disk, check Extract()!");

            Program.Assert(Program.CalculateSHA256($"{filename}_out/earth.gif") == "2D67E7DAD50FFC9D3BD73817BE2C89C62B4307DF379D84DD53490C53C95C682D", $"[{LogSig}] Data fork SHA256 does not match, check Extract()!");
            Program.Assert(Program.CalculateSHA256($"{filename}_out/._earth.gif") == "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855", $"[{LogSig}] Resource fork SHA256 does not match, check Extract()!");

            // Clear rubble
            earth.Close();
            Directory.Delete($"{filename}_out", true);

            Log.Info($"[{LogSig}] '{Path.GetFileName(filename)}' test complete.\n");
        }
    }
}
