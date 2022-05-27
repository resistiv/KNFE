using System.Collections.Generic;
using System.IO;

namespace KNFE.Core.Format
{
    /// <summary>
    /// Represents a generic file format. This is an abstract class.
    /// </summary>
    public abstract class FileFormat
    {
        // Public members
        public readonly Stream InFileStream;
        public readonly string FileName;

        // Properties
        public FormatEntry Root { get { return _root; } }

        // Protected members
        protected FormatEntry _root = null;

        // Internal members
        internal readonly string FormatName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileFormat"/> class from a file name.
        /// </summary>
        /// <param name="fileName">The name of the file to load a <see cref="FileFormat"/> from.</param>
        public FileFormat(string formatName, string fileName)
        {
            // Check if our file exists
            if (!File.Exists(fileName))
                throw new FileNotFoundException();

            FileName = fileName;
            FormatName = formatName;

            // Try to open our file
            InFileStream = File.Open(fileName, FileMode.Open);
        }

        /// <summary>
        /// Constructs a <see cref="Dictionary{TKey, TValue}"/> of this <see cref="FileFormat"/>'s information in a human-readable format.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> containing a human-readable set of file properties and information.</returns>
        public virtual Dictionary<string, string> ToFields()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("File Path", Path.GetFileName(FileName));
            dict.Add("Format Name", FormatName);

            return dict;
        }

        /// <summary>
        /// Closes this <see cref="FileFormat"/> and all associated <see cref="Stream"/>s and Readers.
        /// </summary>
        public virtual void Close()
        {
            InFileStream.Close();
            return;
        }
    }
}
