using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace KNFE.UI
{
    // https://stackoverflow.com/questions/42910628#answer-59129804

    /// <summary>
    /// Handles system icons for use in the main TreeView.
    /// </summary>
    internal static class DefaultIcons
    {
        // Properties
        public static Icon File => _file ?? (_file = GetStockIcon(SHSIID_DOCNOASSOC, SHGSI_SMALLICON));
        public static Icon Folder => _folder ?? (_folder = GetStockIcon(SHSIID_FOLDER, SHGSI_SMALLICON));

        // Private members
        private static Icon _file;
        private static Icon _folder;

        // Constant members
        private const uint SHSIID_DOCNOASSOC = 0x0;
        private const uint SHSIID_FOLDER = 0x3;
        private const uint SHGSI_ICON = 0x100;
        private const uint SHGSI_LARGEICON = 0x0;
        private const uint SHGSI_SMALLICON = 0x1;

        private static Icon GetStockIcon(uint type, uint size)
        {
            var info = new SHSTOCKICONINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);

            SHGetStockIconInfo(type, SHGSI_ICON | size, ref info);

            var icon = (Icon)Icon.FromHandle(info.hIcon).Clone(); // Get a copy that doesn't use the original handle
            DestroyIcon(info.hIcon); // Clean up native icon to prevent resource leak

            return icon;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysIconIndex;
            public int iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;
        }

        [DllImport("shell32.dll")]
        public static extern int SHGetStockIconInfo(uint siid, uint uFlags, ref SHSTOCKICONINFO psii);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr handle);
    }
}
