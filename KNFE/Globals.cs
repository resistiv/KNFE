namespace KNFE
{
    /// <summary>
    /// Provides a set of Global variables and constants.
    /// </summary>
    public static class Globals
    {
        public const string ProgramName = "KNFE";
        public const int MajorVersion = 1;
        public const int MinorVersion = 1;
        public const int PatchVersion = 0;
        public const string Copyright = "Copyright (C) 2021";
        public const string AuthorName = "Kaitlyn NeSmith";

        /// <summary>
        /// Provides a concatenated program version string.
        /// </summary>
        public static string Version()
        {
            return $"v{MajorVersion}.{MinorVersion}.{PatchVersion}";
        }
    }
}
