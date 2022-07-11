using System.IO;
using KNFE.Core.Format.Archive;
using KNFE.Helper;

namespace KNFE.Test
{
    /// <summary>
    /// Handles all tests related to the Vib-Ribbon PAK format.
    /// </summary>
    internal class VibRibbonPakTester
    {
        private const string LogSig = "Vib-Ribbon PAK";

        /// <summary>
        /// Runs all tests related to the Vib-Ribbon PAK format.
        /// </summary>
        internal static void RunAllTests()
        {
            Files01Test();
            Files20Test();
        }

        /// <summary>
        /// Tests the '01_FILES.PAK' file and its output validity.
        /// </summary>
        private static void Files01Test()
        {
            string filename = "../../VibRibbonPak/01_FILES.PAK";
            Log.Info($"[{LogSig}] Running '{Path.GetFileName(filename)}' test...");

            // Instantiate
            VibRibbonPakFormat files01 = new VibRibbonPakFormat(filename);

            // Baseline
            Program.Assert(files01.FormatName == "Vib-Ribbon PAK", $"[{LogSig}] Wrong FormatName, check constructor!");
            Program.Assert(files01.FileCount == 242, $"[{LogSig}] Wrong FileCount, check constructor!");

            // Child
            VibRibbonPakFormatEntry makepak = (VibRibbonPakFormatEntry)files01.Root.Children[2];
            Program.Assert(makepak.ItemPath == "MAKEPAK.PIF", $"[{LogSig}] Wrong ItemPath in MAKEPAK.PIF, check constructor!");
            Program.Assert(makepak.Length == 967, $"[{LogSig}] Wrong Length in MAKEPAK.PIF, check constructor!");
            Program.Assert(makepak.Offset == 0x534B4, $"[{LogSig}] Wrong Offset in MAKEPAK.PIF, check constructor!");

            // Output
            ExtractHandler.ExtractFile($"{filename}_out", makepak);
            Program.Assert(File.Exists($"{filename}_out/MAKEPAK.PIF"), $"[{LogSig}] MAKEPAK.PIF not written to disk, check Extract()!");
            Program.Assert(Program.CalculateSHA256($"{filename}_out/MAKEPAK.PIF") == "C16BE4F7706A469B260DA086A465B3C29674D774A84F326D34EACB6BC5EB39C9", $"[{LogSig}] MAKEPAK.PIF SHA256 does not match, check Extract()!");

            // Cleanup
            files01.Close();
            Directory.Delete($"{filename}_out", true);

            Log.Info($"[{LogSig}] '{Path.GetFileName(filename)}' test complete.\n");
        }

        /// <summary>
        /// Tests the '20_FILES.PAK' file and its output validity.
        /// </summary>
        private static void Files20Test()
        {
            string filename = "../../VibRibbonPak/20_FILES.PAK";
            Log.Info($"[{LogSig}] Running '{Path.GetFileName(filename)}' test...");

            // Instantiate
            VibRibbonPakFormat files20 = new VibRibbonPakFormat(filename);

            // Baseline
            Program.Assert(files20.FormatName == "Vib-Ribbon PAK", $"[{LogSig}] Wrong FormatName, check constructor!");
            Program.Assert(files20.FileCount == 242, $"[{LogSig}] Wrong FileCount, check constructor!");

            // Child
            VibRibbonPakFormatEntry n04wf = (VibRibbonPakFormatEntry)files20.Root.Children[0].Children[2].Children[9];
            Program.Assert(n04wf.ItemPath == "N04_W_F.ANM", $"[{LogSig}] Wrong ItemPath in N04_W_F.ANM, check constructor!");
            Program.Assert(n04wf.Length == 5168, $"[{LogSig}] Wrong Length in N04_W_F.ANM, check constructor!");
            Program.Assert(n04wf.Offset == 0x33660, $"[{LogSig}] Wrong Offset in N04_W_F.ANM, check constructor!");

            // Output
            ExtractHandler.ExtractFile($"{filename}_out", n04wf);
            Program.Assert(File.Exists($"{filename}_out/N04_W_F.ANM"), $"[{LogSig}] N04_W_F.ANM not written to disk, check Extract()!");
            Program.Assert(Program.CalculateSHA256($"{filename}_out/N04_W_F.ANM") == "01857823310072C7D15DC3F94C38C77E58280D2E9A51D11DC1C23EDA0EBEB682", $"[{LogSig}] N04_W_F.ANM SHA256 does not match, check Extract()!");

            // Cleanup
            files20.Close();
            Directory.Delete($"{filename}_out", true);

            Log.Info($"[{LogSig}] '{Path.GetFileName(filename)}' test complete.\n");
        }
    }
}
