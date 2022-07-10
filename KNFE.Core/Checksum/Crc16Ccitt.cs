namespace KNFE.Core.Checksum
{
    /// <summary>
    /// Stores and manages a CCITT implementation of a 16-bit CRC.
    /// </summary>
    public class Crc16Ccitt : CyclicRedundancyCheck
    {
        // Properties
        public new ushort Crc { get { return (ushort)(CalcCrc & ushort.MaxValue); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc16Ccitt"/> class.
        /// </summary>
        public Crc16Ccitt() : base(0x1021) { }

        public override void UpdateCrc(byte b)
        {
            bool xorFlag;
            // Cycle through bits
            for (int i = 0; i < 8; i++)
            {
                // The XOR flag is represented by a set MSB
                xorFlag = (CalcCrc & 0x8000) != 0;
                // Add MSB of byte to CRC
                CalcCrc = (ushort)((CalcCrc << 1) | (ushort)(b >> 7));
                if (xorFlag)
                {
                    // If the XOR flag is set, XOR!!
                    CalcCrc ^= (ushort)Polynomial;
                }
                // Push a bit out of the byte
                b <<= 1;
            }
        }
    }
}
