using KNFE.Util;
using System;
using System.Linq;

namespace KNFE.Format
{
    /// <summary>
    /// Represents all available file formats.
    /// </summary>
    public enum FormatType
    {
        None,
        Invalid,
        Uuencode,
        Fallout1Dat,
        BinHex4
    }

    /// <summary>
    /// Provides utilities for resolving file formats for processing.
    /// </summary>
    public static class FormatHandler
    {
        // List of all available formats
        public static FormatDescription[] Formats =
        {
            new FormatDescription()
            {
                Format = FormatType.BinHex4,
                FormatName = "BinHex 4.0",
                FormatShortCode = "bh4",
                FormatLongCode = "binhex4.0",
                AssemblyName = $"{Globals.ProgramName}.Format.BinHex4Format, {Globals.ProgramName}"
            },
            new FormatDescription()
            {
                Format = FormatType.Fallout1Dat,
                FormatName = "Fallout 1 DAT",
                FormatShortCode = "f1d",
                FormatLongCode = "fallout1dat",
                AssemblyName = $"{Globals.ProgramName}.Format.Archive.Fallout1DatFormat, {Globals.ProgramName}"
            },
            new FormatDescription()
            {
                Format = FormatType.Uuencode,
                FormatName = "uuencode",
                FormatShortCode = "uue",
                FormatLongCode = "uuencode",
                AssemblyName = $"{Globals.ProgramName}.Format.UuencodeFormat, {Globals.ProgramName}"
            }
        };

        /// <summary>
        /// Resolves a FormatType from a format code string.
        /// </summary>
        public static FormatType ResolveFormat(string code)
        {
            // Check if provided string matches any known ShortCode or LongCode
            foreach (FormatDescription fd in Formats)
            {
                if (fd.FormatShortCode.Equals(code))
                    return fd.Format;
                else if (fd.FormatLongCode.Equals(code))
                    return fd.Format;
            }

            return FormatType.Invalid;
        }

        /// <summary>
        /// Resolves and instantiates a FileFormat object from a FormatType.
        /// </summary>
        //public static FileFormat ResolveFormat(FormatType type, string fileName)
        public static FileFormat ResolveFormat(RunProperties properties)
        {
            switch (properties.Format)
            {
                case FormatType.None:
                    Program.Quit("Format not determined, quitting.");
                    break;
                case FormatType.Invalid:
                    Program.Quit("Invalid format type, quitting.");
                    break;
                default:
                    // First, find the FormatDescription in Formats that has a matching FormatType, then save its AssemblyName
                    FormatDescription fd = Formats.Where(f => f.Format == properties.Format).First();
                    // Translate that AssemblyName into a Type
                    Type outType = Type.GetType(fd.AssemblyName);
                    // Create an instance of that assembly; casting to FileFormat from object is required to make this work
                    return (FileFormat)Activator.CreateInstance(outType, new string[] { fd.FormatName, properties.FileName });
            }

            return null;
        }
    }

    /// <summary>
    /// Represents a file format in resolvable form.
    /// </summary>
    public struct FormatDescription
    {
        public FormatType Format;
        public string FormatName;
        // Must be 3 characters
        public string FormatShortCode;
        // Must be at most 12 characters
        public string FormatLongCode;
        public string AssemblyName;
    }
}
