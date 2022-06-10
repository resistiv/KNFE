using System;
using KNFE.Core.Format;

namespace KNFE.Helper
{
    /// <summary>
    /// Represents a set of identifiers by which a <see cref="FileFormat"/> can be resolved.
    /// </summary>
    public class FormatDescription
    {
        /// <summary>
        /// The name of the <see cref="FileFormat"/> this <see cref="FormatDescription"/> represents.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An array of known file extensions relating to this <see cref="FormatDescription"/>.
        /// </summary>
        public string[] Extensions { get; set; }

        /// <summary>
        /// The identifier by which users can force this <see cref="FormatDescription"/> to be resolved.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// The <see cref="Type"/> that this <see cref="FormatDescription"/> resolves to.
        /// </summary>
        public Type AssemblyType { get; set; }
    }
}
