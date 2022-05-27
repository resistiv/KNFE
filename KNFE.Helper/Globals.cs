namespace KNFE.Helper
{
    /// <summary>
    /// Provides a set of global variables and constants.
    /// </summary>
    public static class Globals
    {
        // Versioning
        public const int MajorVersion = 2;
        public const int MinorVersion = 0;
        public const int PatchVersion = 0;

        // Program info
        public const string ProgramName = "KNFE";
        public const string ProgramAuthor = "Kaitlyn NeSmith";
        public const string ProgramCopyright = "Copyright (C) 2021-2022";

        // Properties
        public static string Version { get { return $"{MajorVersion}.{MinorVersion}.{PatchVersion}"; } }
    }
}
