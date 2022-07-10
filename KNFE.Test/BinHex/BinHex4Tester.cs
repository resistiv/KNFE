using System.IO;
using KNFE.Core.Format;
using KNFE.Helper;

namespace KNFE.Test
{
    /// <summary>
    /// Handles all tests related to the BinHex 4.0 format.
    /// </summary>
    internal static class BinHex4Tester
    {
        /// <summary>
        /// Runs all tests related to the BinHex 4.0 format.
        /// </summary>
        internal static void RunAllTests()
        {
            CybergifTest();
            EarthTest();
            MacBinary3Test();
        }

        /// <summary>
        /// Tests the 'cybergif.hqx' file and its output validity.
        /// </summary>
        private static void CybergifTest()
        {
            Log.Info("[BinHex 4.0] Running 'cybergif.hqx' test...");

            // Instantiate
            BinHex4Format cybergif = new BinHex4Format("../../BinHex/cybergif.hqx");

            // Baseline assertions
            Program.Assert(cybergif.FormatName == "BinHex 4.0", "[BinHex 4.0] Wrong FormatName, check constructor!");
            Program.Assert(cybergif.Version == 0, "[BinHex 4.0] Wrong Version, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(cybergif.FileType == "APPL", "[BinHex 4.0] Wrong FileType, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(cybergif.Creator == "EXTR", "[BinHex 4.0] Wrong Creator, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(cybergif.FinderFlags == 0x20, "[BinHex 4.0] Wrong FinderFlags, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(cybergif.HeaderCrc == (ushort)0xFE1B, "[BinHex 4.0] Wrong HeaderCrc, check constructor, BinHexStream, and Rle90Stream!");

            // Child assertions
            BinHex4FormatEntry data = (BinHex4FormatEntry)cybergif.Root.Children[0];
            Program.Assert(data.ItemPath == "CyberGif 1.1.sea", "[BinHex 4.0] Incorrect data fork name, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.IsDataFork, "[BinHex 4.0] Data fork marked as resource fork, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.ForkLength == 25562, "[BinHex 4.0] Incorrect data fork length, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.Crc == (ushort)0x8955, "[BinHex 4.0] Incorrect data fork CRC, check constructor, BinHexStream, and Rle90Stream!");

            BinHex4FormatEntry rsrc = (BinHex4FormatEntry)cybergif.Root.Children[1];
            Program.Assert(rsrc.ItemPath == "._CyberGif 1.1.sea", "[BinHex 4.0] Incorrect resource fork name, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(!rsrc.IsDataFork, "[BinHex 4.0] Resource fork marked as data fork, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(rsrc.ForkLength == 13273, "[BinHex 4.0] Incorrect resource fork length, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(rsrc.Crc == (ushort)0xEFE8, "[BinHex 4.0] Incorrect resource fork CRC, check constructor, BinHexStream, and Rle90Stream!");

            // Output file assertions
            ExtractHandler.ExtractDirectory("../../BinHex/cybergif.hqx_out", cybergif.Root);

            Program.Assert(File.Exists("../../BinHex/cybergif.hqx_out/CyberGif 1.1.sea"), "[BinHex 4.0] Data fork not written to disk, check Extract()!");
            Program.Assert(File.Exists("../../BinHex/cybergif.hqx_out/._CyberGif 1.1.sea"), "[BinHex 4.0] Resource fork not written to disk, check Extract()!");

            Program.Assert(Program.CalculateSHA256("../../BinHex/cybergif.hqx_out/CyberGif 1.1.sea") == "343C16D546F9B399BA24EEF9DF70570F6E90B91DC0FFE6C6FAFF581A02BAFA63", "[BinHex 4.0] Data fork SHA256 does not match, check Extract()!");
            Program.Assert(Program.CalculateSHA256("../../BinHex/cybergif.hqx_out/._CyberGif 1.1.sea") == "DD2BBA61EEE98B8FB7A5B5AF70EE1F1B186EFB32EAFAAD918BD911853AEDCE9E", "[BinHex 4.0] Resource fork SHA256 does not match, check Extract()!");

            // Clear rubble
            cybergif.Close();
            Directory.Delete("../../BinHex/cybergif.hqx_out/", true);

            Log.Info("[BinHex 4.0] 'cybergif.hqx' test complete.\n");
        }

        /// <summary>
        /// Tests the 'earth.hqx' file and its output validity.
        /// </summary>
        private static void EarthTest()
        {
            Log.Info("[BinHex 4.0] Running 'earth.hqx' test...");

            // Instantiate
            BinHex4Format earth = new BinHex4Format("../../BinHex/earth.hqx");

            // Baseline assertions
            Program.Assert(earth.FormatName == "BinHex 4.0", "[BinHex 4.0] Wrong FormatName, check constructor!");
            Program.Assert(earth.Version == 0, "[BinHex 4.0] Wrong Version, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(earth.FileType == "GIFf", "[BinHex 4.0] Wrong FileType, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(earth.Creator == "QGif", "[BinHex 4.0] Wrong Creator, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(earth.FinderFlags == 0x0, "[BinHex 4.0] Wrong FinderFlags, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(earth.HeaderCrc == (ushort)0x5D1A, "[BinHex 4.0] Wrong HeaderCrc, check constructor, BinHexStream, and Rle90Stream!");

            // Child assertions
            BinHex4FormatEntry data = (BinHex4FormatEntry)earth.Root.Children[0];
            Program.Assert(data.ItemPath == "earth.gif", "[BinHex 4.0] Incorrect data fork name, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.IsDataFork, "[BinHex 4.0] Data fork marked as resource fork, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.ForkLength == 404505, "[BinHex 4.0] Incorrect data fork length, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.Crc == (ushort)0x8E70, "[BinHex 4.0] Incorrect data fork CRC, check constructor, BinHexStream, and Rle90Stream!");

            BinHex4FormatEntry rsrc = (BinHex4FormatEntry)earth.Root.Children[1];
            Program.Assert(rsrc.ItemPath == "._earth.gif", "[BinHex 4.0] Incorrect resource fork name, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(!rsrc.IsDataFork, "[BinHex 4.0] Resource fork marked as data fork, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(rsrc.ForkLength == 0, "[BinHex 4.0] Incorrect resource fork length, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(rsrc.Crc == (ushort)0x0, "[BinHex 4.0] Incorrect resource fork CRC, check constructor, BinHexStream, and Rle90Stream!");

            // Output file assertions
            ExtractHandler.ExtractDirectory("../../BinHex/earth.hqx_out", earth.Root);

            Program.Assert(File.Exists("../../BinHex/earth.hqx_out/earth.gif"), "[BinHex 4.0] Data fork not written to disk, check Extract()!");
            Program.Assert(File.Exists("../../BinHex/earth.hqx_out/._earth.gif"), "[BinHex 4.0] Resource fork not written to disk, check Extract()!");

            Program.Assert(Program.CalculateSHA256("../../BinHex/earth.hqx_out/earth.gif") == "2D67E7DAD50FFC9D3BD73817BE2C89C62B4307DF379D84DD53490C53C95C682D", "[BinHex 4.0] Data fork SHA256 does not match, check Extract()!");
            Program.Assert(Program.CalculateSHA256("../../BinHex/earth.hqx_out/._earth.gif") == "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855", "[BinHex 4.0] Resource fork SHA256 does not match, check Extract()!");

            // Clear rubble
            earth.Close();
            Directory.Delete("../../BinHex/earth.hqx_out/", true);

            Log.Info("[BinHex 4.0] 'earth.hqx' test complete.\n");
        }

        /// <summary>
        /// Tests the 'macbinary-3.sit.hqx' file and its output validity.
        /// </summary>
        private static void MacBinary3Test()
        {
            Log.Info("[BinHex 4.0] Running 'macbinary-3.sit.hqx' test...");

            // Instantiate
            BinHex4Format macbin = new BinHex4Format("../../BinHex/macbinary-3.sit.hqx");

            // Baseline assertions
            Program.Assert(macbin.FormatName == "BinHex 4.0", "[BinHex 4.0] Wrong FormatName, check constructor!");
            Program.Assert(macbin.Version == 0, "[BinHex 4.0] Wrong Version, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(macbin.FileType == "SITD", "[BinHex 4.0] Wrong FileType, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(macbin.Creator == "SIT!", "[BinHex 4.0] Wrong Creator, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(macbin.FinderFlags == 0x0, "[BinHex 4.0] Wrong FinderFlags, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(macbin.HeaderCrc == (ushort)0x9C31, "[BinHex 4.0] Wrong HeaderCrc, check constructor, BinHexStream, and Rle90Stream!");

            // Child assertions
            BinHex4FormatEntry data = (BinHex4FormatEntry)macbin.Root.Children[0];
            Program.Assert(data.ItemPath == "MacBinary_III.sit", "[BinHex 4.0] Incorrect data fork name, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.IsDataFork, "[BinHex 4.0] Data fork marked as resource fork, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.ForkLength == 9655, "[BinHex 4.0] Incorrect data fork length, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(data.Crc == (ushort)0xFE9A, "[BinHex 4.0] Incorrect data fork CRC, check constructor, BinHexStream, and Rle90Stream!");

            BinHex4FormatEntry rsrc = (BinHex4FormatEntry)macbin.Root.Children[1];
            Program.Assert(rsrc.ItemPath == "._MacBinary_III.sit", "[BinHex 4.0] Incorrect resource fork name, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(!rsrc.IsDataFork, "[BinHex 4.0] Resource fork marked as data fork, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(rsrc.ForkLength == 0, "[BinHex 4.0] Incorrect resource fork length, check constructor, BinHexStream, and Rle90Stream!");
            Program.Assert(rsrc.Crc == (ushort)0x0, "[BinHex 4.0] Incorrect resource fork CRC, check constructor, BinHexStream, and Rle90Stream!");

            // Output file assertions
            ExtractHandler.ExtractDirectory("../../BinHex/macbinary-3.sit.hqx_out", macbin.Root);

            Program.Assert(File.Exists("../../BinHex/macbinary-3.sit.hqx_out/MacBinary_III.sit"), "[BinHex 4.0] Data fork not written to disk, check Extract()!");
            Program.Assert(File.Exists("../../BinHex/macbinary-3.sit.hqx_out/._MacBinary_III.sit"), "[BinHex 4.0] Resource fork not written to disk, check Extract()!");

            Program.Assert(Program.CalculateSHA256("../../BinHex/macbinary-3.sit.hqx_out/MacBinary_III.sit") == "661672F09C05D464CE271F7290375EE36BC202DD92974AF95241431025AA6163", "[BinHex 4.0] Data fork SHA256 does not match, check Extract()!");
            Program.Assert(Program.CalculateSHA256("../../BinHex/macbinary-3.sit.hqx_out/._MacBinary_III.sit") == "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855", "[BinHex 4.0] Resource fork SHA256 does not match, check Extract()!");

            // Clear rubble
            macbin.Close();
            Directory.Delete("../../BinHex/macbinary-3.sit.hqx_out/", true);

            Log.Info("[BinHex 4.0] 'macbinary-3.sit.hqx' test complete.\n");
        }
    }
}
