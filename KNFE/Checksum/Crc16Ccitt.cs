namespace KNFE.Checksum
{
    /// <summary>
    /// Represents the CCITT implementation of a 16-bit CRC.
    /// </summary>
    public class Crc16Ccitt : Crc
    {
        // Calculated CRC
        private short calcCrc;

        // Consts
        private short emptyCrc = 0x0000;

        public Crc16Ccitt(bool fillInitialBits)
            : base(0x1021)
        {
            // Some implementations use a 0xFFFF starting value, while others use 0x0000.
            // Default is 0x0000, otherwise flip all bits to 0xFFFF
            if (fillInitialBits)
            {
                emptyCrc = (short)~emptyCrc;
            }
            ResetCrc();
        }

        public override int GetCrc()
        {
            return calcCrc & 0xFFFF;
        }

        public override void ResetCrc()
        {
            calcCrc = emptyCrc;
        }

        public override void UpdateCrc(byte b)
        {
            bool xorFlag;
            // Cycle through bits
            for (int i = 0; i < 8; i++)
            {
                // The XOR flag is represented by a set MSB
                xorFlag = (calcCrc & 0x8000) != 0;
                // Add MSB of byte to CRC
                calcCrc = (short)((calcCrc << 1) | (b >> 7));
                if (xorFlag)
                {
                    // If the XOR flag is set, XOR!!
                    calcCrc ^= (short)polynomial;
                }
                // Push a bit out of the byte
                b <<= 1;
            }
        }
    }
}
