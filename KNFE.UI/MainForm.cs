using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using KNFE.Core.Format;
using KNFE.Helper;

namespace KNFE.UI
{
    public partial class MainForm : Form
    {
        private Format fileFormat = null;
        private EntryTreeNode selNode = null;

        public MainForm()
        {
            InitializeComponent();

            // Create our DataGrid columns, size rightmost to fill
            // FIXME: Is there any way we can do this within the Designer?
            dataGrid.ColumnCount = 2;
            dataGrid.Columns[1].Width = dataGrid.Width - dataGrid.Columns[0].Width - dataGrid.Margin.Right;

            // Adds system icons
            iconList.Images.Add("File", DefaultIcons.File);
            iconList.Images.Add("Folder", DefaultIcons.Folder);
        }

        /// <summary>
        /// Attempts to open a file for processing through KNFE.
        /// </summary>
        private void openTsmi_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { RestoreDirectory = true, ShowHelp = false, Multiselect = false, CheckFileExists = true };
            // FIXME: Perhaps Multiselect can be changed later to handle multiple files loaded at once ?

            // Set our filters from available file formats
            string filter = "All files (*.*)|*.*|";
            foreach (FormatDescription fd in FormatHandler.Formats)
            {
                // Concat all extensions together, and trim final semicolon
                string ext = "";
                foreach (string s in fd.Extensions)
                    ext += $"*.{s};";
                ext = ext.Remove(ext.Length - 1, 1);

                filter += $"{fd.Name} ({ext})|{ext}|";
            }
            ofd.Filter = filter.Remove(filter.Length - 1, 1);

            // Did we select a file?
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FormatDescription fd = null;

                // User selected a specific file filter
                // FIXME: Resolve from data signature in order to confirm filter in case of erroneous user selection (add in FormatResolver.cs)
                if (ofd.FilterIndex != 1)
                    fd = FormatHandler.Formats[ofd.FilterIndex - 2];
                // Else, try to identify
                else
                {
                    FormatDescription[] fds = FormatHandler.ResolveFromFileName(ofd.FileName);
                    // No results
                    if (fds.Length == 0)
                        MessageBox.Show("Could not identify any available file format(s) matching that of the selected file's extension.", "Format Unresolvable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // One result
                    else if (fds.Length == 1)
                        fd = fds[0];
                    // Multiple results
                    else
                    {
                        FormatSelectorForm fsf = new FormatSelectorForm();
                        fsf.formatComboBox.DataSource = fds;
                        fsf.formatComboBox.DisplayMember = "Name";

                        if(fsf.ShowDialog() == DialogResult.OK)
                            fd = fds[fsf.formatComboBox.SelectedIndex];
                    }
                }

                // Do we have a FormatDescription to instantiate?
                if (fd != null)
                {
                    // Close the current format if opening a new one
                    if (fileFormat != null)
                        CloseFormat();

                    // Attempt to create FileFormat instance, catch open error
                    try
                    {
                        fileFormat = FormatHandler.InstantiateFormat(fd, ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Encountered an error when opening the specified file format:\n{ex.Message}", "Could Not Open Format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fileFormat = null;
                        return;
                    }

                    // If we've passed the above block, the FileFormat is assumed to be valid; constructors should throw detectable header data exceptions
                    closeToolStripMenuItem.Enabled = true;
                    PopulateTree(fileFormat.Root);
                    PopulateTable(((EntryTreeNode)treeViewPanel.Nodes[0]).Fields);
                }
            }
        }

        /// <summary>
        /// Populates the information table with a <see cref="Dictionary{TKey, TValue}"/> of fields.
        /// </summary>
        /// <param name="fields">The <see cref="Dictionary{TKey, TValue}"/> of fields to fill the table.</param>
        internal void PopulateTable(Dictionary<string, string> fields)
        {
            // Depopulates the table first to get rid of residual data
            if (dataGrid.Rows.Count != 0) DepopulateTable();

            // Populate from dictionary
            foreach (KeyValuePair<string, string> kvp in fields)
            {
                dataGrid.Rows.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Depopulates the information table.
        /// </summary>
        private void DepopulateTable()
        {
            dataGrid.Rows.Clear();
        }

        /// <summary>
        /// Populates the directory tree from a root <see cref="FormatEntry"/>.
        /// </summary>
        /// <param name="root">A root <see cref="FormatEntry"/> to populate from.</param>
        private void PopulateTree(FormatEntry root, TreeNode topNode = null)
        {
            // Remove residual tree if loading new
            if (treeViewPanel.Nodes.Count != 0 && topNode == null) DepopulateTree();

            // Flag update
            treeViewPanel.BeginUpdate();

            // No node, assume top level as overall file
            if (topNode == null)
            {
                topNode = new EntryTreeNode(fileFormat) { ContextMenuStrip = extractContext };
                treeViewPanel.Nodes.Add(topNode);
                treeViewPanel.SelectedNode = topNode;
            }

            // Build new tree
            GenerateTreeNodes(topNode, root);

            // Complete update
            treeViewPanel.EndUpdate();
        }

        /// <summary>
        /// Encapsulates a tree of <see cref="FormatEntry"/>s in <see cref="EntryTreeNode"/>s.
        /// </summary>
        /// <param name="topNode">The topmost <see cref="TreeNode"/> to add new <see cref="EntryTreeNode"/>s to.</param>
        /// <param name="topEntry">The topmost <see cref="FormatEntry"/> to read from.</param>
        private void GenerateTreeNodes(TreeNode topNode, FormatEntry topEntry)
        {
            foreach (FormatEntry fe in topEntry.Children)
            {
                EntryTreeNode temp = new EntryTreeNode(fe.ItemPath, fe) { ContextMenuStrip = extractContext };
                topNode.Nodes.Add(temp);

                if (fe.IsDirectory) GenerateTreeNodes(temp, fe);
            }
        }

        /// <summary>
        /// Depopulates the directory tree.
        /// </summary>
        private void DepopulateTree()
        {
            treeViewPanel.BeginUpdate();
            treeViewPanel.Nodes.Clear();
            treeViewPanel.EndUpdate();
        }

        /// <summary>
        /// Closes the current <see cref="Format"/>.
        /// </summary>
        private void CloseFormat()
        {
            // Clean UI
            DepopulateTable();
            DepopulateTree();

            // Close format
            if (fileFormat != null)
            {
                fileFormat.Close();
                fileFormat = null;
            }

            // Garbage collection for all disowned nodes, formats, and entries
            GC.Collect();
        }
        
        /// <summary>
        /// Extracts the data/files from a given <see cref="EntryTreeNode"/>.
        /// </summary>
        private void extractTsmi_Click(object sender, EventArgs e)
        {
            // Attempt to get our path
            FolderBrowserDialog fbd = null;
            string path = null;
            fbd = new FolderBrowserDialog() { ShowNewFolderButton = true, Description = "Select an output directory." };
            if (fbd.ShowDialog() == DialogResult.OK)
                path = fbd.SelectedPath;

            if (path != null)
            {
                // Entire format
                if (selNode.Format != null)
                    ExtractHandler.ExtractDirectory($"{path}\\{Path.GetFileName(selNode.Format.FileName)}_out", selNode.Format.Root);
                // Directory
                else if (selNode.IsDirectory)
                    ExtractHandler.ExtractDirectory(path, selNode.Entry);
                // Single file
                else
                    ExtractHandler.ExtractFile(path, selNode.Entry);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFormat();
            closeToolStripMenuItem.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFormat();
            Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseFormat();
        }

        private void treeViewPanel_AfterSelect(object sender, TreeViewEventArgs e)
        {
            EntryTreeNode curNode = (EntryTreeNode)e.Node;
            PopulateTable(curNode.Fields);
        }

        private void treeViewPanel_MouseUp(object sender, MouseEventArgs e)
        {
            // Check for right click
            if (e.Button == MouseButtons.Right)
            {
                // Grab node where clicked
                TreeNode curNode = treeViewPanel.GetNodeAt(e.X, e.Y);
                // No bitches?
                if (curNode == null)
                    return;

                // Set selected node so we know what node we're dealing with from the ContextMenuStrip
                selNode = (EntryTreeNode)curNode;

                // Show our proper ContextMenu
                curNode.ContextMenuStrip.Show(treeViewPanel, e.Location);

                // Clear selected node
                // selNode = null;
            }
        }

        private void aboutTsmi_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }
    }
}

// "You can't just slaughter the younglings, can you?"
// - Maclane May, discussing BST removal