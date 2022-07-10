using System.Collections.Generic;
using System.IO;

namespace KNFE.Core.Format
{
    /// <summary>
    /// Represents a generic file format. This is an abstract class.
    /// </summary>
    public abstract class Format
    {
        // Public members
        public readonly Stream InFileStream;
        public readonly string FileName;
        public readonly string FormatName;

        // Properties
        public FormatEntry Root { get { return _root; } }

        // Protected members
        protected FormatEntry _root = null;
        protected Dictionary<string, string> _fields = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Format"/> class from a file name.
        /// </summary>
        /// <param name="fileName">The name of the file to load a <see cref="Format"/> from.</param>
        public Format(string formatName, string fileName)
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
        /// Constructs a <see cref="Dictionary{TKey, TValue}"/> of this <see cref="Format"/>'s information in a human-readable format.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> containing a human-readable set of file properties and information.</returns>
        public virtual Dictionary<string, string> ToFields()
        {
            if (_fields != null)
                return _fields;

            _fields = new Dictionary<string, string>();

            _fields.Add("File Name", Path.GetFileName(FileName));
            _fields.Add("Format Name", FormatName);

            return _fields;
        }

        /// <summary>
        /// Closes this <see cref="Format"/> and all associated <see cref="Stream"/>s and Readers.
        /// </summary>
        public virtual void Close()
        {
            InFileStream.Close();
            return;
        }
    }
}
