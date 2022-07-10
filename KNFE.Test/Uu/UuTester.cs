using KNFE.Core.Format;
using KNFE.Helper;
using System.IO;

namespace KNFE.Test
{
    /// <summary>
    /// Handles all tests related to the uuencode format.
    /// </summary>
    internal static class UuTester
    {
        /// <summary>
        /// Runs all tests related to the uuencode format.
        /// </summary>
        internal static void RunAllTests()
        {
            ChessTest();
            UupcTest();
        }

        /// <summary>
        /// Tests the 'chess88.uu' file and its output validity.
        /// </summary>
        private static void ChessTest()
        {
            Log.Info("[Uu] Running 'chess88.uu' test...");

            // Instantiate
            UuFormat chess88 = new UuFormat("../../Uu/chess88.uu");

            // Baseline
            Program.Assert(chess88.FormatName == "uuencode", "[Uu] Wrong FormatName, check constructor!");

            // Child
            UuFormatEntry chessExe = (UuFormatEntry)chess88.Root.Children[0];
            Program.Assert(chessExe.ItemPath == "chess88.exe", "[Uu] Wrong ItemPath in chess88.exe, check constructor!");
            Program.Assert(chessExe.Permissions == "-rwxrwxr-x", "[Uu] Wrong Permissions in chess88.exe, check constructor!");

            // Output
            ExtractHandler.ExtractFile("../../Uu/chess88.uu_out", chessExe);
            Program.Assert(File.Exists("../../Uu/chess88.uu_out/chess88.exe"), "[Uu] chess88.exe not written to disk, check Extract()!");
            Program.Assert(Program.CalculateSHA256("../../Uu/chess88.uu_out/chess88.exe") == "288202E42BBFD3183F0861748CABF730BD71A7817F48C3E9976DDB096C5DDE4B", "[Uu] chess88.exe SHA256 does not match, check Extract()!");

            // Cleanup
            chess88.Close();
            Directory.Delete("../../Uu/chess88.uu_out", true);

            Log.Info("[Uu] 'chess88.uu' test complete.\n");
        }

        /// <summary>
        /// Tests the 'uupc-ibm.uu' file and its output validity.
        /// </summary>
        private static void UupcTest()
        {
            Log.Info("[Uu] Running 'uupc-ibm.uu' test...");

            // Instantiate
            UuFormat uupc = new UuFormat("../../Uu/uupc-ibm.uu");

            // Baseline
            Program.Assert(uupc.FormatName == "uuencode", "[Uu] Wrong FormatName, check constructor!");

            // Child
            UuFormatEntry uupcArc = (UuFormatEntry)uupc.Root.Children[0];
            Program.Assert(uupcArc.ItemPath == "uupc-ibm.arc", "[Uu] Wrong ItemPath in uupc-ibm.arc, check constructor!");
            Program.Assert(uupcArc.Permissions == "-rw-------", "[Uu] Wrong Permissions in uupc-ibm.arc, check constructor!");

            // Output
            ExtractHandler.ExtractFile("../../Uu/uupc-ibm.uu_out", uupcArc);
            Program.Assert(File.Exists("../../Uu/uupc-ibm.uu_out/uupc-ibm.arc"), "[Uu] uupc-ibm.arc not written to disk, check Extract()!");
            Program.Assert(Program.CalculateSHA256("../../Uu/uupc-ibm.uu_out/uupc-ibm.arc") == "87D39BE66D314935F20C02F13875C028A313DD99C28B83B38DF15506251C196A", "[Uu] uupc-ibm.arc SHA256 does not match, check Extract()!");

            // Cleanup
            uupc.Close();
            Directory.Delete("../../Uu/uupc-ibm.uu_out", true);

            Log.Info("[Uu] 'uupc-ibm.uu' test complete.\n");
        }
    }
}
