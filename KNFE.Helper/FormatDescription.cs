using System;

namespace KNFE.Helper
{
    /// <summary>
    /// Represents a resolvable file format.
    /// </summary>
    public class FormatDescription
    {
        public string Name { get; set; }
        public string[] Extensions { get; set; }
        public Type AssemblyType { get; set; }
    }
}
