using System;
using System.Collections.Generic;
using System.IO;
using KNFE.Core.Encoding;

namespace KNFE.Core.Format
{
    public class UuFormatEntry : FormatEntry
    {
        // Internal members
        internal string _perms = "";

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

            // Decoder
            UuStream stream = new UuStream(_source);
            stream.Decode(outStream);

            // No info for EntryProperties, so null
            return null;
        }

        public override Dictionary<string, string> ToFields()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("File Name", ItemPath);
            dict.Add("Permissions", _perms);

            return dict;
        }

        /// <summary>
        /// Returns the Unix-style permissions of this <see cref="UuFormatEntry"/>.
        /// </summary>
        public string Permissions { get { return _perms; } }
    }
}
