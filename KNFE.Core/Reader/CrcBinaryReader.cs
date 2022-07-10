using System;
using System.IO;
using KNFE.Core.Checksum;

namespace KNFE.Core.Reader
{
    /// <summary>
    /// Reads primitive data types as binary values while calculating the CRC of the read data.
    /// </summary>
    public class CrcBinaryReader : BinaryReader
    {
        /// <summary>
        /// The current CRC of the read data.
        /// </summary>
        public readonly CyclicRedundancyCheck Crc;

        public CrcBinaryReader(Stream stream, CyclicRedundancyCheck crc)
            : base(stream)
        {
            this.Crc = crc;
        }
        
        /// <summary>
        /// Reads and returns a <see cref="bool"/> while calculating it into the current CRC.
        /// </summary>
        public override bool ReadBoolean()
        {
            byte b = base.ReadByte();

            Crc.UpdateCrc(b);

            return b == 0 ? false : true;
        }

        /// <summary>
        /// Reads and returns a <see cref="byte"/> while calculating it into the current CRC.
        /// </summary>
        public override byte ReadByte()
        {
            byte b = base.ReadByte();

            Crc.UpdateCrc(b);

            return b;
        }

        /// <summary>
        /// Reads and returns an array of <see cref="byte"/>s of a given length while calculating it into the current CRC.
        /// </summary>
        public override byte[] ReadBytes(int count)
        {
            byte[] bs = base.ReadBytes(count);

            foreach (byte b in bs)
                Crc.UpdateCrc(b);

            return bs;
        }

        /// <summary>
        /// Reads and returns a character while calculating it into the current CRC.
        /// </summary>
        public override char ReadChar()
        {
            char c = base.ReadChar();

            Crc.UpdateCrc((byte)c);

            return c;
        }

        /// <summary>
        /// Reads and returns an array of <see cref="char"/>s of a given length while calculating it into the current CRC.
        /// </summary>
        public override char[] ReadChars(int count)
        {
            char[] cs = base.ReadChars(count);

            foreach (char c in cs)
                Crc.UpdateCrc((byte)c);

            return cs;
        }

        /// <summary>
        /// Reads and returns a <see cref="decimal"/> while calculating it into the current CRC.
        /// </summary>
        public override decimal ReadDecimal()
        {
            decimal d = base.ReadDecimal();
            int[] ds = decimal.GetBits(d);
            for (int i = 0; i < ds.Length; i++)
            {
                byte[] bs = BitConverter.GetBytes(ds[i]);
                foreach (byte b in bs)
                    Crc.UpdateCrc(b);
            }

            return d;
        }

        /// <summary>
        /// Reads and returns a <see cref="double"/> while calculating it into the current CRC.
        /// </summary>
        public override double ReadDouble()
        {
            double d = base.ReadDouble();
            byte[] bs = BitConverter.GetBytes(d);

            foreach (byte b in bs)
                Crc.UpdateCrc(b);

            return d;
        }

        /// <summary>
        /// Reads and returns a <see cref="short"/> while calculating it into the current CRC.
        /// </summary>
        public override short ReadInt16()
        {
            short s = base.ReadInt16();
            byte[] bs = BitConverter.GetBytes(s);

            foreach (byte b in bs)
                Crc.UpdateCrc(b);

            return s;
        }

        /// <summary>
        /// Reads and returns an <see cref="int"/> while calculating it into the current CRC.
        /// </summary>
        public override int ReadInt32()
        {
            int i = base.ReadInt32();
            byte[] bs = BitConverter.GetBytes(i);

            foreach (byte b in bs)
                Crc.UpdateCrc(b);

            return i;
        }

        /// <summary>
        /// Reads and returns a <see cref="long"/> while calculating it into the current CRC.
        /// </summary>
        public override long ReadInt64()
        {
            long l = base.ReadInt64();
            byte[] bs = BitConverter.GetBytes(l);

            foreach (byte b in bs)
                Crc.UpdateCrc(b);

            return l;
        }

        /// <summary>
        /// Reads and returns an <see cref="sbyte"/> while calculating it into the current CRC.
        /// </summary>
        public override sbyte ReadSByte()
        {
            sbyte s = base.ReadSByte();

            Crc.UpdateCrc((byte)s);

            return s;
        }

        /// <summary>
        /// Reads and returns a <see cref="float"/> while calculating it into the current CRC.
        /// </summary>
        public override float ReadSingle()
        {
            float f = base.ReadSingle();
            byte[] bs = BitConverter.GetBytes(f);

            foreach (byte b in bs)
                Crc.UpdateCrc(b);

            return f;
        }

        /// <summary>
        /// Reads and returns a <see cref="ushort"/> while calculating it into the current CRC.
        /// </summary>
        public override ushort ReadUInt16()
        {
            ushort s = base.ReadUInt16();
            byte[] bs = BitConverter.GetBytes(s);

            foreach (byte b in bs)
                Crc.UpdateCrc(b);

            return s;
        }

        /// <summary>
        /// Reads and returns a <see cref="uint"/> while calculating it into the current CRC.
        /// </summary>
        public override uint ReadUInt32()
        {
            uint i = base.ReadUInt32();
            byte[] bs = BitConverter.GetBytes(i);

            foreach (byte b in bs)
                Crc.UpdateCrc(b);

            return i;
        }

        /// <summary>
        /// Reads and returns a <see cref="ulong"/> while calculating it into the current CRC.
        /// </summary>
        public override ulong ReadUInt64()
        {
            ulong l = base.ReadUInt64();
            byte[] bs = BitConverter.GetBytes(l);

            foreach (byte b in bs)
                Crc.UpdateCrc(b);

            return l;
        }
    }
}
