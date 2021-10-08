using KNFE.Util;
using System.IO;

namespace KNFE.Encoding.Compression
{
    /// <summary>
    /// Represents a LZSS-variant stream utilized by Fallout 1's DAT files.
    /// </summary>
    public class Fallout1LzssStream : EncodingStream
    {
        private readonly BinaryReader br;

        // Constants
        private const int DICT_SIZE = 4096;
        private const int MIN_MATCH = 3;
        private const int MAX_MATCH = 18;

        // Dictionary for matches
        private byte[] dict;
        // Number of bytes to be read
        private short numBytes;
        // Number of bytes currently read
        private short bytesRead;
        // Offset within dictionary for reading
        private short dOfs;
        // Offset within dictionary for writing
        private short dIndex;
        // Current length of encoded match
        private short matchLen;

        public Fallout1LzssStream(MemoryStream stream)
            : base(stream)
        {
            br = new BinaryReader(stream);
        }

        public override MemoryStream Decode()
        {
            // Adapted from the pseudocode implementation of Shadowbird's algorithm
            // https://falloutmods.fandom.com/wiki/DAT_file_format#Fallout_1_LZSS_uncompression_algorithm

            // Referenced Mr.Stalin's DAT Explorer II for bug-fixing
            // GitHub repo is outdated, used dotPeek on updated DLL
            // https://www.nma-fallout.com/resources/fallout-dat-explorer-ii.121/

            // While I did not reference this code, here's an extra implementation utilized by the FIFE engine
            // https://github.com/fifengine/fifengine/blob/master/engine/core/vfs/dat/lzssdecoder.cpp

            // Of importantance is that this stream is big endian, and thus needs conversion to little endian for number values larger than a byte
            // Literal runs of bytes do not need to be converted, as they are literal

            MemoryStream decoded = new MemoryStream();
            dict = new byte[DICT_SIZE];

            while (!IsLastByte())
            {
                // Number of bytes we'll read from the next block of data
                numBytes = BigEndian.ToLeShort(br.ReadInt16());

                // LZSS end of stream
                if (numBytes == 0)
                    break;
                else
                {
                    bytesRead = 0;

                    // If the numBytes is negative, we take the absolute value of numBytes and read that many literal bytes
                    if (numBytes < 0)
                    {
                        numBytes *= -1;
                        byte[] data = br.ReadBytes(numBytes);
                        decoded.Write(data, 0, data.Length);
                        continue;
                    }
                    // Else, our read will be LZSS encoded

                    // For every new block of compressed data, we flush the dictionary in order to read in new values
                    ClearDictionary();
                    while (bytesRead < numBytes && !IsLastByte())
                    {
                        // Get a flag byte
                        byte flags = br.ReadByte();
                        bytesRead++;

                        if (bytesRead >= numBytes || IsLastByte()) break;
                        
                        // Iterate through each bit of the flag byte
                        for (int i = 0; i < 8; i++)
                        {
                            // If bit is set, we have a literal byte; write to output and to dictionary
                            if (flags % 2 != 0)
                            {
                                byte b = br.ReadByte();
                                bytesRead++;
                                decoded.WriteByte(b);
                                WriteToDictionary(b);
                                if (bytesRead >= numBytes) break;
                            }
                            // If bit is NOT set, we have an encoded run, reference dictionary
                            else
                            {
                                if (bytesRead >= numBytes) break;
                                // Read the offset of our encoded data in the dictionary
                                dOfs = br.ReadByte();
                                bytesRead++;
                                if (bytesRead >= numBytes) break;
                                // Read how long the match is in bytes
                                matchLen = br.ReadByte();
                                bytesRead++;

                                // Prepend the 4 MSBs of matchLen to dOfs to get the full dictionary offset; 8 bits can only hold up to 255, while 12 bits can hold up to 4095, or our dictionary length
                                dOfs |= (short)((matchLen & 0xF0) << 4);
                                // Discard the 4 MSBs of matchLen since we added them to dOfs; matchLen can be up to 15 bytes
                                matchLen &= 0x0F;
                                // Then, read from the dictionary to output
                                for (int j = 0; j < matchLen + MIN_MATCH; j++)
                                {
                                    byte b = ReadDictionary();
                                    decoded.WriteByte(b);
                                    WriteToDictionary(b);
                                }
                            }

                            // Get next flag
                            flags >>= 1;

                            if (IsLastByte())
                                break;
                        }
                    }
                }
            }

            // Don't forget to release our file handle
            br.Close();

            decoded.Seek(0, SeekOrigin.Begin);
            return decoded;
        }

        /// <summary>
        /// Flushes the dictionary and resets the dictionary index.
        /// </summary>
        private void ClearDictionary()
        {
            // Fills the dictionary with ' ' chars to overwrite residual data
            for (int i = 0; i < DICT_SIZE; i++)
            {
                dict[i] = 0x20;
            }
            // Reset our dictionary index
            // Why DICT_SIZE - MAX_MATCH? I have no clue if I'm honest
            dIndex = DICT_SIZE - MAX_MATCH;
        }

        /// <summary>
        /// Reads from the LZSS dictionary and advances the current dictionary offset.
        /// </summary>
        private byte ReadDictionary()
        {
            byte b = dict[dOfs++ % DICT_SIZE];
            // If we overrun the dictionary, loop to beginning
            dOfs %= DICT_SIZE;
            return b;
        }

        /// <summary>
        /// Writes to the LZSS dictionary and advances the dictionary write index.
        /// </summary>
        private void WriteToDictionary(byte b)
        {
            dict[dIndex++ % DICT_SIZE] = b;
            // If we overrun the dictionary, loop to beginning
            dIndex %= DICT_SIZE;
        }

        /// <summary>
        /// Checks if the end of the base stream has been reached.
        /// </summary>
        private bool IsLastByte()
        {
            return br.BaseStream.Position == br.BaseStream.Length;
        }
    }
}
