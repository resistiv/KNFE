namespace KNFE.Core.Checksum
{
    /// <summary>
    /// Stores and manages a cyclic redundancy check.
    /// </summary>
    public abstract class CyclicRedundancyCheck
    {
        // Properties
        public virtual ulong Crc { get { return CalcCrc; } }

        // Protected members
        protected readonly ulong Polynomial;
        protected ulong CalcCrc;
        protected ulong BaseCrc;

        /// <summary>
        /// Initializes a new instance of the <see cref="CyclicRedundancyCheck"/> class with a specified polynomial & base CRC value.
        /// </summary>
        /// <param name="polynomial">The polynomial to be utilized when calculating this CRC.</param>
        /// <param name="baseCrc">The base value to be utilized when starting calculation or resetting the CRC, defaults to <c>0</c>.</param>
        public CyclicRedundancyCheck(ulong polynomial, ulong baseCrc = 0)
        {
            Polynomial = polynomial;
            BaseCrc = baseCrc;
            ResetCrc();
        }

        /// <summary>
        /// Resets a CRC to its base value.
        /// </summary>
        public void ResetCrc() => CalcCrc = BaseCrc;

        /// <summary>
        /// Updates a CRC with 1 byte of data.
        /// </summary>
        public abstract void UpdateCrc(byte b);
    }
}
