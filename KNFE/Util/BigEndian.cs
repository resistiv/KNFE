using System;

namespace KNFE.Util
{
    /// <summary>
    /// Provides utilities for converting big endian data types into little endian data types.
    /// </summary>
    public static class BigEndian
    {
        /// <summary>
        /// Convert a big endian Int16 into a little endian Int16.
        /// </summary>
        public static Int16 ConvertToLeShort(short sShort)
        {
            ushort s = (ushort)sShort;
            return (Int16)
                (
                    ((s << 8) & 0xFF00) |
                    ((s >> 8) & 0x00FF)
                );
        }

        /// <summary>
        /// Convert a big endian Int32 into a little endian Int32.
        /// </summary>
        public static Int32 ConvertToLeInt(int nInt)
        {
            uint n = (uint)nInt;
            return (Int32)
                (
                    ((n >> 24) & 0x000000FF) |
                    ((n >> 8)  & 0x0000FF00) |
                    ((n << 8)  & 0x00FF0000) |
                    ((n << 24) & 0xFF000000)
                );
        }

        /// <summary>
        /// Convert a big endian Int64 into a little endian Int64.
        /// </summary>
        public static Int64 ConvertToLeLong(long lLong)
        {
            ulong l = (ulong)lLong;
            return (Int64)
                (
                    ((l >> 56) & 0x00000000000000FF) |
                    ((l >> 40) & 0x000000000000FF00) |
                    ((l >> 24) & 0x0000000000FF0000) |
                    ((l >> 8)  & 0x00000000FF000000) |
                    ((l << 8)  & 0x000000FF00000000) |
                    ((l << 24) & 0x0000FF0000000000) |
                    ((l << 40) & 0x00FF000000000000) |
                    ((l << 56) & 0xFF00000000000000)
                );
        }
    }
}
