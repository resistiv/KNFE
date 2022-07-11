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
        private const string LogSig = "Fallout 1 DAT";

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
            string filename = "../../Fallout1Dat/INSTALL.DAT";
            Log.Info($"[{LogSig}] Running '{Path.GetFileName(filename)}' test...");

            // Instantiate
            Fallout1DatFormat install = new Fallout1DatFormat(filename);

            // Baseline assertions
            Program.Assert(install.FormatName == "Fallout 1 DAT", $"[{LogSig}] Wrong FormatName, check constructor!");
            Program.Assert(install.DirectoryCount == 1, $"[{LogSig}] Wrong DirectoryCount, check constructor!");

            // Child assertions
            Fallout1DatFormatEntry huge = (Fallout1DatFormatEntry)install.Root.Children[15];
            Program.Assert(huge.ItemPath == "HUGE.WIN", $"[{LogSig}] Wrong ItemPath in HUGE.WIN, check constructor and normalizer!");
            Program.Assert(huge.IsCompressed == true, $"[{LogSig}] Wrong IsCompressed in HUGE.WIN, check constructor!");
            Program.Assert(huge.Offset == 0x35B7D, $"[{LogSig}] Wrong Offset in HUGE.WIN, check constructor!");
            Program.Assert(huge.OriginalLength == 2444, $"[{LogSig}] Wrong OriginalLength in HUGE.WIN, check constructor!");
            Program.Assert(huge.CompressedLength == 889, $"[{LogSig}] Wrong CompressedLength in HUGE.WIN, check constructor!");

            // Output file assertions
            ExtractHandler.ExtractFile($"{filename}_out", huge);
            Program.Assert(File.Exists($"{filename}_out/HUGE.WIN"), $"[{LogSig}] HUGE.WIN not written to disk, check Extract()!");
            Program.Assert(Program.CalculateSHA256($"{filename}_out/HUGE.WIN") == "9FB6625831F9040BC2900B37030ED262C06E0491A5A64031F402E6A6680F8329", $"[{LogSig}] HUGE.WIN SHA256 does not match, check Extract()!");

            // Cleanup
            install.Close();
            Directory.Delete($"{filename}_out/", true);

            Log.Info($"[{LogSig}] '{Path.GetFileName(filename)}' test complete.\n");
        }

        /// <summary>
        /// Tests the 'MASTER.DAT' file and its output validity.
        /// </summary>
        private static void MasterTest()
        {
            string filename = "../../Fallout1Dat/MASTER.DAT";
            Log.Info($"[{LogSig}] Running '{Path.GetFileName(filename)}' test...");

            // Instantiate
            Fallout1DatFormat master = new Fallout1DatFormat(filename);

            // Baseline assertions
            Program.Assert(master.FormatName == "Fallout 1 DAT", $"[{LogSig}] Wrong FormatName, check constructor!");
            Program.Assert(master.DirectoryCount == 51, $"[{LogSig}] Wrong DirectoryCount, check constructor!");

            // Child assertions
            Fallout1DatFormatEntry boil = (Fallout1DatFormatEntry)master.Root.Children[7].Children[0].Children[2].Children[0];
            Program.Assert(boil.ItemPath == "BOIL1.SVE", $"[{LogSig}] Wrong ItemPath in BOIL1.SVE, check constructor and normalizer!");
            Program.Assert(boil.IsCompressed == false, $"[{LogSig}] Wrong IsCompressed in BOIL1.SVE, check constructor!");
            Program.Assert(boil.Offset == 0x13D43E08, $"[{LogSig}] Wrong Offset in BOIL1.SVE, check constructor!");
            Program.Assert(boil.OriginalLength == 94, $"[{LogSig}] Wrong OriginalLength in BOIL1.SVE, check constructor!");

            // Output file assertions
            ExtractHandler.ExtractFile($"{filename}_out", boil);
            Program.Assert(File.Exists($"{filename}_out/BOIL1.SVE"), $"[{LogSig}] BOIL1.SVE not written to disk, check Extract()!");
            Program.Assert(Program.CalculateSHA256($"{filename}_out/BOIL1.SVE") == "246F0358BB20BC3EC2F39BFBCA8A48D0229D2E6973AFCFD756482F648E82909B", $"[{LogSig}] BOIL1.SVE SHA256 does not match, check Extract()!");

            // Cleanup
            master.Close();
            Directory.Delete($"{filename}_out/", true);

            Log.Info($"[{LogSig}] '{Path.GetFileName(filename)}' test complete.\n");
        }
    }
}
