using System;
using System.IO;
using System.Security.Cryptography;
using KNFE.Helper;

namespace KNFE.Test
{
    public static class Program
    {
        internal static int ErrorNum = 0;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            RunAllTests();
        }

        /// <summary>
        /// Provides a wrapper for running all needed KNFE tests.
        /// </summary>
        private static void RunAllTests()
        {
            BinHex4Tester.RunAllTests();
            Fallout1DatTester.RunAllTests();
            UuTester.RunAllTests();

            if (ErrorNum == 0)
                Log.Info("All tests completed successfully.");
            else
                Log.Info($"All tests completed, {ErrorNum} assertions failed.");
        }

        /// <summary>
        /// Tests an assertion. If false, prints a specified message to stdout.
        /// </summary>
        /// <param name="condition">The assertion to test.</param>
        /// <param name="message">The failure message to print.</param>
        internal static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                Log.Error(message);
                ErrorNum++;
            }
        }

        /// <summary>
        /// Calculates the SHA256 hash of a given file.
        /// </summary>
        /// <param name="fileName">The file to calculate a hash from.</param>
        /// <returns>A string representation of a SHA256 hash.</returns>
        internal static string CalculateSHA256(string fileName)
        {
            // Invalid file
            if (!File.Exists(fileName))
                return string.Empty;

            using (SHA256 sha = SHA256.Create())
            {
                using (FileStream stream = File.OpenRead(fileName))
                {
                    byte[] hash = sha.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", string.Empty);
                }
            }
        }
    }
}
