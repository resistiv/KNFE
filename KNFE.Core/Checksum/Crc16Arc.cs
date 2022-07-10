namespace KNFE.Core.Checksum
{
    /// <summary>
    /// Stores and manages an ARC implementation of a 16-bit CRC.
    /// </summary>
    public class Crc16Arc : CyclicRedundancyCheck
    {
        // Properties
        public new ushort Crc { get { return (ushort)(CalcCrc & ushort.MaxValue); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc16Arc"/> class.
        /// </summary>
        public Crc16Arc() : base(0xA001) { }

        public override void UpdateCrc(byte b)
        {
            bool xorFlag;
            // Immediately XOR by entire byte
            CalcCrc ^= b;
            // Cycle through bits
            for (int i = 0; i < 8; i++)
            {
                // The XOR flag is represented by a set LSB
                xorFlag = (CalcCrc & 0x0001) != 0;
                // Shift bit out
                CalcCrc >>= 1;
                if (xorFlag)
                {
                    // If the XOR flag is set, XOR!!
                    CalcCrc ^= (ushort)Polynomial;
                }
            }
        }
    }
}
