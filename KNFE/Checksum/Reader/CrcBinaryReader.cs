﻿using System;
using System.IO;

namespace KNFE.Checksum.Reader
{
    /// <summary>
    /// Represents a BinaryReader that simultaneously calculates a CRC.
    /// </summary>
    public class CrcBinaryReader : BinaryReader
    {
        public readonly Crc Crc;

        public CrcBinaryReader(Stream stream, Crc Crc)
            : base(stream)
        {
            this.Crc = Crc;
        }

        /// <summary>
        /// Reads and returns a byte while calculating it into the current CRC.
        /// </summary>
        public override byte ReadByte()
        {
            byte b = base.ReadByte();
            Crc.UpdateCrc(b);
            return b;
        }

        /// <summary>
        /// Reads and returns an array of bytes of a given length while calculating it into the current CRC.
        /// </summary>
        public override byte[] ReadBytes(int count)
        {
            byte[] bs = base.ReadBytes(count);
            foreach (byte b in bs)
            {
                Crc.UpdateCrc(b);
            }
            return bs;
        }

        /// <summary>
        /// Reads and returns a 16-bit integer while calculating it into the current CRC.
        /// </summary>
        public override short ReadInt16()
        {
            short s = base.ReadInt16();
            byte[] bs = BitConverter.GetBytes(s);
            foreach (byte b in bs)
            {
                Crc.UpdateCrc(b);
            }
            return s;
        }

        /// <summary>
        /// Reads and returns a 32-bit integer while calculating it into the current CRC.
        /// </summary>
        public override int ReadInt32()
        {
            int i = base.ReadInt32();
            byte[] bs = BitConverter.GetBytes(i);
            foreach (byte b in bs)
            {
                Crc.UpdateCrc(b);
            }
            return i;
        }
    }
}
