using System;
using System.IO;

namespace KNFE.Core.Format
{
    /// <summary>
    /// Represents a set of file properties related to a <see cref="FormatEntry"/>.
    /// </summary>
    public class EntryProperties
    {
        public FileAttributes Attributes { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// Writes all of the properties present in this <see cref="EntryProperties"/> object to a file.
        /// </summary>
        /// <param name="filePath">The path of a file to write this <see cref="EntryProperties"/>' properties to.</param>
        public void SetAttributes(string filePath)
        {
            if (Attributes != 0)
                File.SetAttributes(filePath, Attributes);
            if (CreationTime != null)
                File.SetCreationTime(filePath, CreationTime);
            if (LastAccessTime != null)
                File.SetLastAccessTime(filePath, LastAccessTime);
            if (LastWriteTime != null)
                File.SetLastWriteTime(filePath, LastWriteTime);
        }
    }
}
