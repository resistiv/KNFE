﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KNFE.Core.Format
{
    /// <summary>
    /// Provides storage for a format item entry and its information.
    /// </summary>
    public class FormatEntry
    {
        // Public members
        public readonly string ItemPath;
        public readonly bool IsDirectory;

        // Protected members
        protected FormatEntry _parent;
        protected readonly List<FormatEntry> _children;
        protected Stream _source;
        protected Dictionary<string, string> _fields = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatEntry"/> class as a directory.
        /// </summary>
        /// <param name="path">A relative path to the virtual directory from the root of the corresponding <see cref="Format"/>.</param>
        public FormatEntry(string path)
        {
            ItemPath = path;
            _children = new List<FormatEntry>();
            IsDirectory = true;
            _parent = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatEntry"/> class as a file.
        /// </summary>
        /// <param name="path">A relative path to the virtual file from the root of the corresponding <see cref="Format"/>.</param>
        /// <param name="source">The corresponding <see cref="Stream"/> where the virtual file is located.</param>
        public FormatEntry(string path, Stream source)
        {
            ItemPath = path;
            _children = new List<FormatEntry>();
            IsDirectory = false;
            _parent = null;
            _source = source;
        }

        /// <summary>
        /// Adds a <see cref="FormatEntry"/> to this <see cref="FormatEntry"/>'s children, while setting this <see cref="FormatEntry"/> as its parent.
        /// </summary>
        /// <param name="entry">A <see cref="FormatEntry"/>to add to the children of this <see cref="FormatEntry"/>.</param>
        public void AddChild(FormatEntry entry)
        {
            // We can't add a child entry to a file
            if (!IsDirectory)
                throw new InvalidOperationException("Attempted to add a child FormatEntry to a file FormatEntry.");

            _children.Add(entry);
            entry._parent = this;
        }

        /// <summary>
        /// Gets a child <see cref="FormatEntry"/> from its path.
        /// </summary>
        /// <param name="childPath">The desired <see cref="FormatEntry"/>'s path.</param>
        /// <returns>The corresponding <see cref="FormatEntry"/>; Returns <c>null</c> if the path cannot be found.</returns>
        public FormatEntry GetChild(string childPath)
        {
            // We can't get a child entry from a file
            if (!IsDirectory)
                throw new InvalidOperationException();

            return _children.FirstOrDefault(fe => fe.ItemPath == childPath);
        }

        /// <summary>
        /// Gets the full path of this <see cref="FormatEntry"/>.
        /// </summary>
        /// <returns>A full path of the current <see cref="FormatEntry"/>.</returns>
        public virtual string GetFullPath()
        {
            string outPath = ItemPath;
            FormatEntry temp = _parent;
            while (temp != null)
            {
                if (temp.ItemPath != "")
                    outPath = $"{temp.ItemPath}\\{outPath}";
                temp = temp._parent;
            }
            return outPath;
        }

        /// <summary>
        /// Constructs a <see cref="Dictionary{TKey, TValue}"/> of this <see cref="FormatEntry"/>'s information in a human-readable format.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> containing a human-readable set of file properties and information.</returns>
        public virtual Dictionary<string, string> ToFields()
        {
            if (_fields != null)
                return _fields;

            _fields = new Dictionary<string, string>();

            // Construct path with parental pathing
            string outPath = GetFullPath();

            _fields.Add((IsDirectory ? "Directory" : "File") + " Name", outPath);

            return _fields;
        }

        /// <summary>
        /// Processes and extracts the data from this <see cref="FormatEntry"/>.
        /// </summary>
        /// <returns>A set of <see cref="EntryProperties"/> for this <see cref="FormatEntry"/>.</returns>
        /// <param name="outStream">The <see cref="Stream"/> to write output data to.</param>
        public virtual EntryProperties Extract(Stream outStream) => null;

        /// <summary>
        /// Returns all the child <see cref="FormatEntry"/>s of this <see cref="FormatEntry"/>.
        /// </summary>
        public FormatEntry[] Children { get { return _children.ToArray(); } }
    }
}
