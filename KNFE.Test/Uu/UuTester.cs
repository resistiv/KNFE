using System.IO;
using KNFE.Core.Format;
using KNFE.Core.Format.Transport;
using KNFE.Helper;

namespace KNFE.Test
{
    /// <summary>
    /// Handles all tests related to the uuencode format.
    /// </summary>
    internal static class UuTester
    {
        private const string LogSig = "Uu";

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
            string filename = "../../Uu/chess88.uu";
            Log.Info($"[{LogSig}] Running '{Path.GetFileName(filename)}' test...");

            // Instantiate
            UuFormat chess88 = new UuFormat(filename);

            // Baseline
            Program.Assert(chess88.FormatName == "uuencode", $"[{LogSig}] Wrong FormatName, check constructor!");

            // Child
            UuFormatEntry chessExe = (UuFormatEntry)chess88.Root.Children[0];
            Program.Assert(chessExe.ItemPath == "chess88.exe", $"[{LogSig}] Wrong ItemPath in chess88.exe, check constructor!");
            Program.Assert(chessExe.Permissions == "-rwxrwxr-x", $"[{LogSig}] Wrong Permissions in chess88.exe, check constructor!");

            // Output
            ExtractHandler.ExtractFile($"{filename}_out", chessExe);
            Program.Assert(File.Exists($"{filename}_out/chess88.exe"), $"[{LogSig}] chess88.exe not written to disk, check Extract()!");
            Program.Assert(Program.CalculateSHA256($"{filename}_out/chess88.exe") == "288202E42BBFD3183F0861748CABF730BD71A7817F48C3E9976DDB096C5DDE4B", $"[{LogSig}] chess88.exe SHA256 does not match, check Extract()!");

            // Cleanup
            chess88.Close();
            Directory.Delete($"{filename}_out", true);

            Log.Info($"[{LogSig}] '{Path.GetFileName(filename)}' test complete.\n");
        }

        /// <summary>
        /// Tests the 'uupc-ibm.uu' file and its output validity.
        /// </summary>
        private static void UupcTest()
        {
            string filename = "../../Uu/uupc-ibm.uu";
            Log.Info($"[{LogSig}] Running '{Path.GetFileName(filename)}' test...");

            // Instantiate
            UuFormat uupc = new UuFormat(filename);

            // Baseline
            Program.Assert(uupc.FormatName == "uuencode", $"[{LogSig}] Wrong FormatName, check constructor!");

            // Child
            UuFormatEntry uupcArc = (UuFormatEntry)uupc.Root.Children[0];
            Program.Assert(uupcArc.ItemPath == "uupc-ibm.arc", $"[{LogSig}] Wrong ItemPath in uupc-ibm.arc, check constructor!");
            Program.Assert(uupcArc.Permissions == "-rw-------", $"[{LogSig}] Wrong Permissions in uupc-ibm.arc, check constructor!");

            // Output
            ExtractHandler.ExtractFile($"{filename}_out", uupcArc);
            Program.Assert(File.Exists($"{filename}_out/uupc-ibm.arc"), $"[{LogSig}] uupc-ibm.arc not written to disk, check Extract()!");
            Program.Assert(Program.CalculateSHA256($"{filename}_out/uupc-ibm.arc") == "87D39BE66D314935F20C02F13875C028A313DD99C28B83B38DF15506251C196A", $"[{LogSig}] uupc-ibm.arc SHA256 does not match, check Extract()!");

            // Cleanup
            uupc.Close();
            Directory.Delete($"{filename}_out", true);

            Log.Info($"[{LogSig}] '{Path.GetFileName(filename)}' test complete.\n");
        }
    }
}
