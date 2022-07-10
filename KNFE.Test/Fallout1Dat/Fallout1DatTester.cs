using System.IO;
using KNFE.Core.Format.Archive;
using KNFE.Helper;

namespace KNFE.Test
{
    /// <summary>
    /// Handles all tests related to the Fallout 1 DAT format.
    /// </summary>
    internal static class Fallout1DatTester
    {
        /// <summary>
        /// Runs all tests related to the Fallout 1 DAT format.
        /// </summary>
        internal static void RunAllTests()
        {
            InstallTest();
            MasterTest();
        }

        /// <summary>
        /// Tests the 'INSTALL.DAT' file and its output validity.
        /// </summary>
        private static void InstallTest()
        {
            Log.Info("[Fallout 1 DAT] Running 'INSTALL.DAT' test...");

            // Instantiate
            Fallout1DatFormat install = new Fallout1DatFormat("../../Fallout1Dat/INSTALL.DAT");

            // Baseline assertions
            Program.Assert(install.FormatName == "Fallout 1 DAT", "[Fallout 1 DAT] Wrong FormatName, check constructor!");
            Program.Assert(install.DirectoryCount == 1, "[Fallout 1 DAT] Wrong DirectoryCount, check constructor!");

            // Child assertions
            Fallout1DatFormatEntry huge = (Fallout1DatFormatEntry)install.Root.Children[15];
            Program.Assert(huge.ItemPath == "HUGE.WIN", "[Fallout 1 DAT] Wrong ItemPath in HUGE.WIN, check constructor and normalizer!");
            Program.Assert(huge.IsCompressed == true, "[Fallout 1 DAT] Wrong IsCompressed in HUGE.WIN, check constructor!");
            Program.Assert(huge.Offset == 0x35B7D, "[Fallout 1 DAT] Wrong Offset in HUGE.WIN, check constructor!");
            Program.Assert(huge.OriginalLength == 2444, "[Fallout 1 DAT] Wrong OriginalLength in HUGE.WIN, check constructor!");
            Program.Assert(huge.CompressedLength == 889, "[Fallout 1 DAT] Wrong CompressedLength in HUGE.WIN, check constructor!");

            // Output file assertions
            ExtractHandler.ExtractFile("../../Fallout1Dat/INSTALL.DAT_out", huge);
            Program.Assert(File.Exists("../../Fallout1Dat/INSTALL.DAT_out/HUGE.WIN"), "[Fallout 1 DAT] HUGE.WIN not written to disk, check Extract()!");
            Program.Assert(Program.CalculateSHA256("../../Fallout1Dat/INSTALL.DAT_out/HUGE.WIN") == "9FB6625831F9040BC2900B37030ED262C06E0491A5A64031F402E6A6680F8329", "[Fallout 1 DAT] HUGE.WIN SHA256 does not match, check Extract()!");

            // Cleanup
            install.Close();
            Directory.Delete("../../Fallout1Dat/INSTALL.DAT_out/", true);

            Log.Info("[Fallout 1 DAT] 'INSTALL.DAT' test complete.\n");
        }

        /// <summary>
        /// Tests the 'MASTER.DAT' file and its output validity.
        /// </summary>
        private static void MasterTest()
        {
            Log.Info("[Fallout 1 DAT] Running 'MASTER.DAT' test...");

            // Instantiate
            Fallout1DatFormat master = new Fallout1DatFormat("../../Fallout1Dat/MASTER.DAT");

            // Baseline assertions
            Program.Assert(master.FormatName == "Fallout 1 DAT", "[Fallout 1 DAT] Wrong FormatName, check constructor!");
            Program.Assert(master.DirectoryCount == 51, "[Fallout 1 DAT] Wrong DirectoryCount, check constructor!");

            // Child assertions
            Fallout1DatFormatEntry boil = (Fallout1DatFormatEntry)master.Root.Children[7].Children[0].Children[2].Children[0];
            Program.Assert(boil.ItemPath == "BOIL1.SVE", "[Fallout 1 DAT] Wrong ItemPath in BOIL1.SVE, check constructor and normalizer!");
            Program.Assert(boil.IsCompressed == false, "[Fallout 1 DAT] Wrong IsCompressed in BOIL1.SVE, check constructor!");
            Program.Assert(boil.Offset == 0x13D43E08, "[Fallout 1 DAT] Wrong Offset in BOIL1.SVE, check constructor!");
            Program.Assert(boil.OriginalLength == 94, "[Fallout 1 DAT] Wrong OriginalLength in BOIL1.SVE, check constructor!");

            // Output file assertions
            ExtractHandler.ExtractFile("../../Fallout1Dat/MASTER.DAT_out", boil);
            Program.Assert(File.Exists("../../Fallout1Dat/MASTER.DAT_out/BOIL1.SVE"), "[Fallout 1 DAT] BOIL1.SVE not written to disk, check Extract()!");
            Program.Assert(Program.CalculateSHA256("../../Fallout1Dat/MASTER.DAT_out/BOIL1.SVE") == "246F0358BB20BC3EC2F39BFBCA8A48D0229D2E6973AFCFD756482F648E82909B", "[Fallout 1 DAT] BOIL1.SVE SHA256 does not match, check Extract()!");

            // Cleanup
            master.Close();
            Directory.Delete("../../Fallout1Dat/MASTER.DAT_out/", true);

            Log.Info("[Fallout 1 DAT] 'MASTER.DAT' test complete.\n");
        }
    }
}
