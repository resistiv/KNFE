using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using KNFE.Core.Format;

namespace KNFE.UI
{
    internal class EntryTreeNode : TreeNode
    {
        // Public members
        public readonly bool IsDirectory = true;

        // Private members
        private readonly FormatEntry _entry = null;
        private readonly Format _format = null;

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
        /// Initializes a new instance of the <see cref="EntryTreeNode"/> class with a specified parent <see cref="Core.Format.Format"/>.
        /// </summary>
        /// <param name="format">A parent <see cref="Core.Format.Format"/>.</param>
        public EntryTreeNode(Format format)
            : base(Path.GetFileName(format.FileName))
        {
            Name = Text;

            _format = format;
        }

        /// <summary>
        /// Returns a <see cref="Dictionary{TKey, TValue}"/> containing a human-readable set of this <see cref="EntryTreeNode"/>'s properties and information.
        /// </summary>
        public Dictionary<string, string> Fields { get { return (_format == null) ? _entry.ToFields() : _format.ToFields(); } }

        /// <summary>
        /// Returns the <see cref="FormatEntry"/> that this <see cref="EntryTreeNode"/> represents, if applicable.
        /// </summary>
        public FormatEntry Entry { get { return _entry; } }

        /// <summary>
        /// Returns the <see cref="KNFE.Core.Format"/> that this <see cref="EntryTreeNode"/> represents, if applicable.
        /// </summary>
        public Format Format { get { return _format; } }
    }
}
