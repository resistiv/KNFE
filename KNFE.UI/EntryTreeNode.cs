using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using KNFE.Core.Format;

namespace KNFE.UI
{
    internal class EntryTreeNode : TreeNode
    {
        // Properties
        public Dictionary<string, string> Fields { get { return (_format == null) ? _entry.ToFields() : _format.ToFields(); } }
        public FormatEntry Entry { get { return _entry; } }
        public FileFormat Format { get { return _format; } }

        // Public members
        public readonly bool IsDirectory = true;

        // Private members
        private readonly FormatEntry _entry = null;
        private readonly FileFormat _format = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryTreeNode"/> class with a specified name, key, parent <see cref="TreeNode"/>, and child <see cref="FormatEntry"/>.
        /// </summary>
        /// <param name="key">A specified name & key.</param>
        /// <param name="parent">A parent <see cref="TreeNode"/>.</param>
        /// <param name="entry">A child <see cref="FormatEntry"/>.</param>
        public EntryTreeNode(string key, FormatEntry entry)
            : base(key)
        {
            Name = Text;

            IsDirectory = entry.IsDirectory;
            _entry = entry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryTreeNode"/> class with a specified parent <see cref="FileFormat"/>.
        /// </summary>
        /// <param name="format">A parent <see cref="FileFormat"/>.</param>
        public EntryTreeNode(FileFormat format)
            : base(Path.GetFileName(format.FileName))
        {
            Name = Text;

            _format = format;
        }

        /// <summary>
        /// Generates a set of fields for a <see cref="FormatEntry"/>-less or <see cref="FileFormat"/>-less <see cref="EntryTreeNode"/>.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> containing this <see cref="EntryTreeNode"/>'s fields.</returns>
        private Dictionary<string, string> GenerateFields()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string path = FullPath.Substring(FullPath.IndexOf("\\") + 1);
            string pathType = IsDirectory ? "Directory" : "File";

            dict.Add($"{pathType} Path", path);

            return dict;
        }
    }
}
