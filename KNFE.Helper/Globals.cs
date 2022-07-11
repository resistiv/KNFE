using System;

namespace KNFE.Helper
{
    /// <summary>
    /// Provides a set of global variables and constants.
    /// </summary>
    public static class Globals
    {
        // Versioning
        public const int MajorVersion = 2;
        public const int MinorVersion = 1;
        public const int PatchVersion = 0;

        // Program info
        public const string ProgramName = "KNFE";
        public const string ProgramAuthor = "Kai NeSmith";
        public const string ProgramCopyright = "Copyright (C) 2021-2022";
        public const string ProgramRepo = "https://github.com/resistiv/KNFE";
        public static readonly string ProgramLicense = $"{ProgramName} - A general-purpose file extractor for obscure and proprietary file formats.{Environment.NewLine}{ProgramCopyright} {ProgramAuthor}{Environment.NewLine}{Environment.NewLine}This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.{Environment.NewLine}{Environment.NewLine}This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.{Environment.NewLine}{Environment.NewLine}You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.";

        // Properties
        public static string Version { get { return $"{MajorVersion}.{MinorVersion}.{PatchVersion}"; } }
    }
}
