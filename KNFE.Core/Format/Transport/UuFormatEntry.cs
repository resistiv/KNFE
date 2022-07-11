using System;
using System.Collections.Generic;
using System.IO;
using KNFE.Core.Encoding;

namespace KNFE.Core.Format.Transport
{
    /// <summary>
    /// Provides storage for a uuencode item entry and its information.
    /// </summary>
    public class UuFormatEntry : FormatEntry
    {
        // Internal members
        internal string _perms = "";
        internal long _offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="UuFormatEntry"/> class as a directory.
        /// </summary>
        /// <param name="path">A relative path to the virtual directory from the root of the <see cref="UuFormat"/>.</param>
        public UuFormatEntry(string path) : base(path) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UuFormatEntry"/> class as a file.
        /// </summary>
        /// <param name="path">A relative path to the virtual file from the root of the <see cref="UuFormat"/>.</param>
        public UuFormatEntry(string path, Stream source) : base(path, source) { }

        public override EntryProperties Extract(Stream outStream)
        {
            // Check for invalid call
            if (IsDirectory)
                throw new InvalidOperationException("Attempted to extract data from a directory UuFormatEntry.");

            _source.Seek(_offset, SeekOrigin.Begin);

            // Decoder
            UuStream stream = new UuStream(_source);
            stream.Decode(outStream);

            // No info for EntryProperties, so null
            return null;
        }

        public override Dictionary<string, string> ToFields()
        {
            if (_fields != null)
                return _fields;

            _fields = new Dictionary<string, string>();

            _fields.Add("File Name", ItemPath);
            _fields.Add("Permissions", _perms);

            return _fields;
        }

        /// <summary>
        /// Returns the Unix-style permissions of this <see cref="UuFormatEntry"/>.
        /// </summary>
        public string Permissions { get { return _perms; } }
    }
}
