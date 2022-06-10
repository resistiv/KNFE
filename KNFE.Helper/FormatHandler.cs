﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KNFE.Core.Format;

namespace KNFE.Helper
{
    /// <summary>
    /// Provides utilities for resolving <see cref="FileFormat"/>s.
    /// </summary>
    public static class FormatHandler
    {
        /// <summary>
        /// A list of all available, resolvable <see cref="FileFormat"/>s.
        /// </summary>
        public static readonly FormatDescription[] Formats =
        {
            new FormatDescription()
            {
                Name = "Fallout 1 DAT",
                Extensions = new string[] {"dat"},
                Identifier = "fallout",
                AssemblyType = typeof(KNFE.Core.Format.Archive.Fallout1DatFormat)
            }
        };

        /// <summary>
        /// Resolves an array of possible matching <see cref="FormatDescription"/>s from a file name's extension.
        /// </summary>
        /// <param name="fileName">A file name to resolve.</param>
        /// <returns>An array of possible <see cref="FormatDescription"/>s that match the extension pattern of the given file name.</returns>
        public static FormatDescription[] ResolveFromFileName(string fileName)
        {
            // FIXME: This method and ResolveFromFormatCode will be deprecated upon the implementation of ResolveFile(), in which file characteristics will be weighed to produce the most likely match

            if (!File.Exists(fileName))
                throw new FileNotFoundException();

            // Gets the file extension
            string ext = Path.GetExtension(fileName).ToLower().Substring(1);

            // Find formats with matching extension
            IEnumerable<FormatDescription> fds = Formats.Where(fd => fd.Extensions.Contains(ext));

            return fds.ToArray();
        }

        /// <summary>
        /// Resolves a single <see cref="FormatDescription"/> from a provided format code.
        /// </summary>
        /// <param name="formatCode">A format code to resolve.</param>
        /// <returns>A <see cref="FormatDescription"/> if a passed a matching format code, <c>null</c> otherwise.</returns>
        public static FormatDescription ResolveFromFormatCode(string formatCode)
        {
            // FIXME: This method and ResolveFromFileName will be deprecated upon the implementation of ResolveFile(), in which file characteristics will be weighed to produce the most likely match

            // Because all identifiers are unique, only find First, default for no result is null
            return Formats.FirstOrDefault(fd => fd.Identifier == formatCode);
        }

        /// <summary>
        /// Creates an instance of a <see cref="FileFormat"/> from a <see cref="FormatDescription"/>.
        /// </summary>
        /// <param name="format">The <see cref="FormatDescription"/> to use when instantiating the new <see cref="FileFormat"/>.</param>
        /// <param name="fileName">The input file path of the new <see cref="FileFormat"/>.</param>
        /// <returns></returns>
        public static FileFormat InstantiateFormat(FormatDescription format, string fileName)
        {
            FileFormat outFormat = null;

            outFormat = (FileFormat)Activator.CreateInstance(format.AssemblyType, new string[] { fileName });

            return outFormat;
        }
    }
}