namespace KNFE.Checksum
{
    /// <summary>
    /// Represents a generic cyclic redundancy check.
    /// </summary>
    public abstract class Crc
    {
        protected readonly int polynomial;
        
        public Crc(int polynomial)
        {
            this.polynomial = polynomial;
        }

        /// <summary>
        /// Returns the current CRC.
        /// </summary>
        public abstract int GetCrc();

        /// <summary>
        /// Resets a CRC to its base value.
        /// </summary>
        public abstract void ResetCrc();

        /// <summary>
        /// Updates a CRC with byte of data.
        /// </summary>
        public abstract void UpdateCrc(byte b);
    }
}
